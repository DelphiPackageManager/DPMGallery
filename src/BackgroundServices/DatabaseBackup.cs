using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.BackgroundServices
{
    //not registered yet.
    public class DatabaseBackup : BackgroundService
    {
        ServerConfig _serverConfig;
        ILogger _logger;
        public DatabaseBackup(ServerConfig serverConfig, ILogger logger)
        {
            _serverConfig = serverConfig;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Todo : backup db on a schedule. somthing like a cron job might be better?

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Information("[{category] Checking schedule..", "DB Back Task");
                Task.Delay(60000, stoppingToken);
            }
            return Task.CompletedTask;
        }
    }
}
