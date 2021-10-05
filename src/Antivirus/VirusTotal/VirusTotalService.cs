using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Antivirus
{
    public class VirusTotalService : IAntivirusService
    {

        private readonly ILogger _logger;
        private readonly ServerConfig _serverConfig;
        public VirusTotalService(ILogger logger, ServerConfig serverConfig)
        {
            _logger = logger;
            _serverConfig = serverConfig;
        }

        public bool Enabled => _serverConfig.AntivirusConfig.VirusTotalConfig.Enabled;

        public string ServiceName => "VirusTotal";

        public async Task<AVScanResult> Scan(string filePath, CancellationToken cancellationToken)
        {
            var result = new AVScanResult()
            {
                Message = "Not Run",
                Result = false
            };

            return await Task.FromResult(result);
        }

        //run second.
        public int Order => 1;
    }
}
