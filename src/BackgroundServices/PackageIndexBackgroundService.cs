using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DPMGallery.Types;
using DPMGallery.Data;
using DPMGallery.Repositories;
using DPMGallery.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using DPMGallery.Entities;
using DPMGallery.Antivirus;
using System.IO;
using DPMGallery.PackageExtraction;
using NuGet.Versioning;

namespace DPMGallery.BackgroundServices
{
    public class PackageIndexBackgroundService : BackgroundService
    {

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServerConfig _serverConfig;

        private enum CopyResult  {
            Ok,
            Retry,
            Failed
         };

        //not ideal, but we can't inject scoped services
        public PackageIndexBackgroundService(ILogger logger, ServerConfig serverConfig, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serverConfig = serverConfig;
            _serviceProvider = serviceProvider;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //wait 5 seconds on startup before doing anything!
            await Task.Delay(5000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Information("[{category}] Starting processing.", "PackageIndexBackgroundService");
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var packageVersionProcessRepository = scope.ServiceProvider.GetRequiredService<PackageVersionProcessRepository>();
                        
                        //get any packageversionprocess records not marked as completed.
                        var toBeProcessed = await packageVersionProcessRepository.GetNotCompleted(1000, stoppingToken);
                        if (stoppingToken.IsCancellationRequested) //might have been signalled while we were querying the db
                            return;

                        if (toBeProcessed.Any())
                        {
                            foreach (var item in toBeProcessed)
                            {
                                await DoProcess(scope, item, stoppingToken);
                            }
                        }
                        _logger.Information("[{category}] Done processing.", "PackageIndexBackgroundService");
                        //                    throw new Exception("foobar!!!!");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "[{category}] Error occurred during processing.", "PackageIndexBackgroundService");
                }

                //waiting 30s before checking again. 
                //TODO : make this configurable
                await Task.Delay(10000, stoppingToken);
            }

        }

        private const string PackageContentType = "binary/octet-stream";
        private const string DspecContentType = "application/json";
        private const string ReadmeContentType = "text/markdown";
        private const string IconContentType = "image/xyz";


        private async Task<CopyResult> DoCopyToFileSystem(IServiceScope scope, Package package,  PackageVersion packageVersion, PackageVersionProcess item, PackageTargetPlatform targetPlatform, CancellationToken cancellationToken)
        {
            var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();

            CopyResult result = CopyResult.Failed;
            string folderName = Path.Combine(targetPlatform.SantisedCompilerVersion, targetPlatform.Platform.ToString(), package.PackageId);

            string localFile = Path.Combine(_serverConfig.ProcessingFolder, item.PackageFileName); 
            //lowercase the directory always!
            string remotePath = Path.Combine(folderName.ToLower(),item.PackageFileName);
            _logger.Information("[{processing}] Copying to filesystem : {localFile} to : {remotePath}", "PackageIndexBackgroundService", localFile, remotePath);

            //this is messy! what happens with s3 if you try to overwrite a file? 
            //TODO : Check for memory leaks - are all streams disposed properly.
            try
            {
                using (var stream = File.OpenRead(localFile))
                {
                    var putResult = await storageService.PutAsync(remotePath, stream, PackageContentType, cancellationToken);
                    if (putResult != StoragePutResult.Success)
                       throw new Exception("Uploading package file failed");

                    stream.Seek(0, SeekOrigin.Begin); //not sure if this is needed
                    using (var reader = new PackageArchiveReader(stream))
                    {
                        using var dspecStream = reader.GetDspec();

                        remotePath = Path.ChangeExtension(remotePath, ".dspec");
                        putResult = await storageService.PutAsync(remotePath, dspecStream, DspecContentType, cancellationToken);
                        if (putResult != StoragePutResult.Success)
                            throw new Exception("Uploadin dspec failed");
                        if (packageVersion.HasIcon)
                        {
                            try { 
                                remotePath = Path.ChangeExtension(remotePath, Path.GetExtension(packageVersion.Icon));
                                _logger.Information("[{processing}] Copying icon to filesystem", "Copy to CDN - file : {remotePath}", remotePath);
                                using var iconStream = reader.GetStream(packageVersion.Icon);
                                putResult = await storageService.PutAsync(remotePath, iconStream, IconContentType, cancellationToken);
                                if (putResult != StoragePutResult.Success)
                                    throw new Exception("Uploadin icon failed");
                                }
                            catch(FileNotFoundException ex)
                            {
                                _logger.Warning(ex, $"Package {item.PackageFileName} metadata says it has an icon, but the icon wasn't found in the package!");
                            }
                        }

                        if (packageVersion.HasReadMe)
                        {
                            try
                            { 
                                remotePath = Path.ChangeExtension(remotePath, ".readme");
                                _logger.Information("[{processing}] Copying readme to filesystem", "Copy to CDN - file : {remotePath}", remotePath);
                                using var readmeStream = reader.GetStream(packageVersion.ReadMe);

                                putResult = await storageService.PutAsync(remotePath, readmeStream, ReadmeContentType, cancellationToken);
                                if (putResult != StoragePutResult.Success)
                                    throw new Exception("Uploadin readme failed");
                            }
                            catch (FileNotFoundException ex)
                            {
                                _logger.Warning(ex, $"Package {item.PackageFileName} metadata says it has a readme, but the readme wasn't found in the package!");
                            }
                        }
                        result = CopyResult.Ok;
                    }

                }
               
            }
            catch (Exception ex)
            {
                if (item.RetryCount < _serverConfig.Storage.MaxRetries)
                {
                    result = CopyResult.Retry;
                    packageVersion.StatusMessage = "Failed to copy to CDN - will retry.";
                    item.RetryCount = item.RetryCount + 1;
                    _logger.Warning(ex, "Could not copy to CDN");
                }
                else
                {
                    result = CopyResult.Failed;
                    packageVersion.StatusMessage = $"Failed to copy to CDN - out of retries {ex.Message}\nContact Support!.";
                    _logger.Error(ex, "Error copying to CD, no retries left");
                }
            }
            return result;
        }

        private async Task<AVScanResult> DoVirusScan(IServiceScope scope, PackageVersionProcess item, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(_serverConfig.ProcessingFolder, item.PackageFileName);

            var avServices = scope.ServiceProvider.GetServices<IAntivirusService>().Where(x => x.Enabled == true).OrderBy(x => x.Order).ToList();
            AVScanResult aVScanResult = new()
            {
                Message = "No Av enabled",
                Result = true
            };
            if (!avServices.Any())
                return aVScanResult;


            foreach (var service in avServices)
            {
                _logger.Information("[{category}] {service} Scanning file : {filePath}", "Antivirus", service.ServiceName, filePath);
                aVScanResult = await service.Scan(filePath, cancellationToken);
                if (!aVScanResult.Result)
                {
                    _logger.Warning("[{category}] {service} Scanning on : {filePath} failed.", "Antivirus", service.ServiceName, filePath);
                    return aVScanResult; //don't bother doing the next one.
                }
            }
            _logger.Information("[{category}] Scanning file : {filePath} completed : {aVScanResult.Message}", "Antivirus", filePath, aVScanResult.Message);

            //NOTe : if no av is enabled we'll just return true - should this ever happen (except in dev?)
            return aVScanResult;
        }

        private bool CheckLatestVersions(PackageTargetPlatform targetPlatform, PackageVersion packageVersion)
        {
            //update the targetplatform with the latest versions

            NuGetVersion latestVer = null;
            NuGetVersion latestStableVer = null;
            bool updateVersions = false;

            if (!NuGetVersion.TryParseStrict(packageVersion.Version, out NuGetVersion version))
            {
                _logger.Error("Package version is not a valid semantic version : {thePackageVersion.Version}", packageVersion.Version);
                return false;
            }

            if (!string.IsNullOrEmpty(targetPlatform.LatestStableVersion))
            {
                if (!NuGetVersion.TryParseStrict(targetPlatform.LatestStableVersion, out latestStableVer))
                {
                    _logger.Error("Package version is not a valid semantic version : {targetPlatform.LatestStableVersion}", targetPlatform.LatestStableVersion);
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(targetPlatform.LatestVersion))
            {
                if (!NuGetVersion.TryParseStrict(targetPlatform.LatestVersion, out latestVer))
                {
                    _logger.Error("Package version is not a valid semantic version : {targetPlatform.LatestVersion}", targetPlatform.LatestVersion);
                    return false;
                }
            }


            if (!version.IsPrerelease) //stable
            {

                if (latestStableVer != null)
                {
                    if (version > latestStableVer)
                    {
                        targetPlatform.LatestStableVersionId = packageVersion.Id;
                        targetPlatform.LatestStableVersion = packageVersion.Version;
                        updateVersions = true;
                    }
                }
                else
                {
                    targetPlatform.LatestStableVersionId = packageVersion.Id;
                    targetPlatform.LatestStableVersion = packageVersion.Version;
                    updateVersions = true;
                }
            }


            if (latestVer != null) 
            {
                if (version > latestVer)
                {
                    targetPlatform.LatestVersionId = packageVersion.Id;
                    targetPlatform.LatestVersion = packageVersion.Version;
                    updateVersions = true;
                }
            }
            else
            {
                targetPlatform.LatestVersionId = packageVersion.Id;
                targetPlatform.LatestVersion = packageVersion.Version;
                updateVersions = true;
            }

            return updateVersions;
       }

        private async Task DoProcess(IServiceScope scope, PackageVersionProcess item, CancellationToken cancellationToken)
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var packageVersionRepository = scope.ServiceProvider.GetRequiredService<PackageVersionRepository>();
            var targetPlatformRepository = scope.ServiceProvider.GetRequiredService<TargetPlatformRepository>();
            var packageRepository = scope.ServiceProvider.GetRequiredService<PackageRepository>();
            var packageVersionProcessRepository = scope.ServiceProvider.GetRequiredService<PackageVersionProcessRepository>();
            //var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();
            var packageVersion = await packageVersionRepository.GetById(item.PackageVersionId, cancellationToken);

            //this should never happen due to foreign key
            if (packageVersion == null)
            {
                _logger.Error("[{category}] While processing id {id} no matching package version was found", "PackageIndexBackgroundService", item.PackageVersionId);
                return;
            }

            var targetPlatform = await targetPlatformRepository.GetByIdAsync(packageVersion.TargetPlatformId, cancellationToken);
            //should never happen
            if (targetPlatform == null)
            {
                _logger.Error("[{category}] While processing id {id} no matching targetplatform was found", "PackageIndexBackgroundService", item.PackageVersionId);
                return;
            }

            var package = await packageRepository.GetPackageByIdAsync(targetPlatform.PackageId);
            //should never happen
            if (package == null)
            {
                _logger.Error("[{category}] While processing id {id} no matching package was found", "PackageIndexBackgroundService", item.PackageVersionId);
                return;
            }

            switch (packageVersion.Status)
            {
                case PackageStatus.Queued:
                    //update to ProcessingAV and scan.
                    packageVersion.Status = PackageStatus.ProcessingAV;
                    packageVersion.StatusMessage = "Scanning for virus...";
                    item.LastUpdatedUtc = DateTime.UtcNow;
                    await packageVersionRepository.UpdateAsyncStatus(packageVersion, cancellationToken);
                    unitOfWork.Commit();
                    goto case PackageStatus.ProcessingAV;
                case PackageStatus.ProcessingAV:
                    var aVScanResult = await DoVirusScan(scope, item, cancellationToken);
                    if (aVScanResult.Result)
                    {
                        packageVersion.Status = PackageStatus.CopyToFileSystem;
                        packageVersion.StatusMessage = "Copying to CDN ...";
                        item.LastUpdatedUtc = DateTime.UtcNow;
                    }
                    else
                    {
                        packageVersion.StatusMessage = aVScanResult.Message;
                        packageVersion.Status = PackageStatus.FailedAV;
                        item.Completed = true; //we don't want to see this one again.
                        item.LastUpdatedUtc = DateTime.UtcNow;
                    }
                    await packageVersionRepository.UpdateAsyncStatus(packageVersion, cancellationToken);
                    await packageVersionProcessRepository.UpdatePartialAsync(item, cancellationToken);
                    unitOfWork.Commit();
                    if (aVScanResult.Result)
                        goto case PackageStatus.CopyToFileSystem;
                    break; //if failed AV then we are done.

                case PackageStatus.CopyToFileSystem:
                    _logger.Information("[{processing}] Copying to filesystem", "PackageIndexBackgroundService");

                    CopyResult copyResult = await DoCopyToFileSystem(scope, package, packageVersion, item, targetPlatform, cancellationToken);

                    switch (copyResult)
                    {
                        case CopyResult.Failed:
                            item.Completed = true;
                            item.LastUpdatedUtc = DateTime.UtcNow;
                            break;

                        case CopyResult.Retry:
                            item.Completed = false;
                            item.LastUpdatedUtc = DateTime.UtcNow;
                            break;

                        case CopyResult.Ok:
                            item.Completed = true;
                            item.LastUpdatedUtc = DateTime.UtcNow;
                            packageVersion.Status = PackageStatus.Passed;
                            packageVersion.StatusMessage = "Ok";
                            packageVersion.Listed = true;
                            break;
                    }

                    if (packageVersion.Status == PackageStatus.Passed)
                    {
                        if (CheckLatestVersions(targetPlatform, packageVersion))
                        {
                            try
                            {
                                await targetPlatformRepository.UpdateAsync(targetPlatform, cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex, "[PackageIndexService] Error updating package versions");
                            }
                        }
                    }

                    await packageVersionRepository.UpdateAsyncStatus(packageVersion, cancellationToken);
                    await packageVersionProcessRepository.UpdatePartialAsync(item, cancellationToken);
                    unitOfWork.Commit();

                    if (packageVersion.Status == PackageStatus.Passed)
                    {
                        string localFile = Path.Combine(_serverConfig.ProcessingFolder, item.PackageFileName);
                        try
                        {
                            File.Delete(localFile);
                        }
                        catch(Exception ex)
                        {
                            _logger.Error(ex, "Error deleting processed file!");
                        }
                       
                    }
                    break;
                case PackageStatus.FailedAV:
                    //we should never get here, but if we do
                    item.Completed = true;
                    item.LastUpdatedUtc = DateTime.UtcNow;
                    await packageVersionProcessRepository.UpdatePartialAsync(item, cancellationToken);
                    unitOfWork.Commit();
                    break;

                default:
                    break;
            }
        }
    }
}
