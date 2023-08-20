using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using DPMGallery.Data;
using DPMGallery.Repositories;
using DPMGallery.Statistics;
using DPMGallery.Extensions;

namespace DPMGallery.BackgroundServices
{
    public class StatisticsUpateBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServerConfig _serverConfig;

        public StatisticsUpateBackgroundService(ILogger logger, ServerConfig serverConfig, IServiceProvider serviceProvider)
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
                _logger.Information("[{category}] Updating Statistics.", "StatisticsUpateBackgroundService");
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var statsRepository = scope.ServiceProvider.GetRequiredService<StatsRepository>();
                        var statsData = scope.ServiceProvider.GetRequiredService<StatisticsData>();

                        statsData.TotalDownloads = await statsRepository.GetTotalDownloads(stoppingToken);
                        var packageDownloads = await statsRepository.GetTopDownloadedPackages(10, stoppingToken);
                        statsData.TopPackageDownloads.Clear();
                        statsData.TopPackageDownloads.AddRange(packageDownloads);

                        var versionDownloads = await statsRepository.GetTopDownloadedPackageVersions(10, stoppingToken);

                        statsData.TopVersionDownloads.Clear();
                        statsData.TopVersionDownloads.AddRange(versionDownloads);

                        statsData.UniquePackages = await statsRepository.GetUniquePackagesCount(stoppingToken);
                        statsData.PackageVersions = await statsRepository.GetTotalPackageVersions(stoppingToken);

                        statsData.LastUpdated = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "[{category}] Error occurred during processing.", "StatisticsUpateBackgroundService");
                }

                //waiting before checking again. 
                //TODO : make this configurable
#if DEBUG
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); 
#else
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); 
#endif
            }
        }
    }
}
