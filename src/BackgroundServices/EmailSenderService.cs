using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace DPMGallery.BackgroundServices
{
    public class EmailSenderService : BackgroundService, IEmailSender
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServerConfig _serverConfig;

        public EmailSenderService(ILogger logger, ServerConfig serverConfig, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serverConfig = serverConfig;
            _serviceProvider = serviceProvider;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Task.CompletedTask;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //wait 1 second on startup before doing anything!
            await Task.Delay(1000, stoppingToken);
            _logger.Information("[{category}] Started.", "EmailSenderService");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "[{category}] Error occurred during processing.", "EmailSenderService");
                }
#if DEBUG
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
#else
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); 
#endif
            }
        }
    }
}
