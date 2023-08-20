using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using DPMGallery.Repositories;
using DPMGallery.Statistics;
using DPMGallery.Data;
using DPMGallery.Entities;

namespace DPMGallery.BackgroundServices
{
    public class DownloadsCountUpdaterBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServerConfig _serverConfig;
        private readonly DownloadsRecordQueue _downloadsRecordQueue;

        public DownloadsCountUpdaterBackgroundService(ILogger logger, ServerConfig serverConfig, IServiceProvider serviceProvider, DownloadsRecordQueue downloadsRecordQueue)
        {
            _logger = logger;
            _serverConfig = serverConfig;
            _serviceProvider = serviceProvider;
            _downloadsRecordQueue = downloadsRecordQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //wait 5 seconds on startup before doing anything!
            await Task.Delay(5000, stoppingToken);
            _logger.Information("[{category}] Started.", "DownloadsCountUpdaterBackgroundService");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var packageVersionRepository = scope.ServiceProvider.GetRequiredService<PackageVersionRepository>();
                        var targetPlatformRepository = scope.ServiceProvider.GetRequiredService<TargetPlatformRepository>();
                        var packageRepository = scope.ServiceProvider.GetRequiredService<PackageRepository>();
                        
                        var item = _downloadsRecordQueue.Dequeue();
                        bool workDone = false;
                        while (item != null && !stoppingToken.IsCancellationRequested)
                        {
                            var package = await packageRepository.GetPackageByPackageIdAsync(item.packageId, stoppingToken);
                            if (package == null) 
                                continue;
                            var targetPlatform = await targetPlatformRepository.GetByIdCompilerPlatformAsync(package.Id, item.compilerVersion, item.platform, stoppingToken);
                            if (targetPlatform == null)
                                continue;

                            var packageVersion = await packageVersionRepository.GetByIdAndVersionAsync(targetPlatform.Id, item.packageVersion, stoppingToken);

                            if (packageVersion != null)
                            {
                                await packageVersionRepository.IncrementDownloads(packageVersion, item.downloads, stoppingToken);
                            }
                            await packageRepository.IncrementDownloads(package, item.downloads, stoppingToken);
                            workDone = true;
                            item = _downloadsRecordQueue.Dequeue();
                        }
                        if (workDone) {
                            unitOfWork.Commit();
                        }
                        
                    }
                } catch (Exception ex)
                {
                    _logger.Error(ex, "[{category}] Error occurred during downloads processing.", "DownloadsCountUpdaterBackgroundService");
                }

                //waiting before checking again. 
                //TODO : make this configurable
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
