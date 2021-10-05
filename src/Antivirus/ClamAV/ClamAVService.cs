using nClam;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Antivirus
{
    public class ClamAVService : IAntivirusService
    {
        private readonly ILogger _logger;
        private readonly ServerConfig _serverConfig;
        public ClamAVService(ILogger logger, ServerConfig serverConfig)
        {
            _logger = logger;
            _serverConfig = serverConfig;
        }

        public bool Enabled => _serverConfig.AntivirusConfig.ClamAVConfig.Enabled;

        public string ServiceName => "ClamAV";

        //run first = since it will run on the local machine
        public int Order => 0;

        public async Task<AVScanResult> Scan(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                var config = _serverConfig.AntivirusConfig.ClamAVConfig;
                var clam = new ClamClient(config.Host, config.Port);
                // or var clam = new ClamClient(IPAddress.Parse("127.0.0.1"), 3310);
                if (!await clam.PingAsync())
                    throw new Exception("Clam is not responding");

                var scanResult = await clam.SendAndScanFileAsync(filePath, cancellationToken);  //any file you would like!

                switch (scanResult.Result)
                {
                    case ClamScanResults.Clean:
                        return new AVScanResult()
                        {
                            Message = "The file is clean!",
                            Result = true
                        };

                    case ClamScanResults.VirusDetected:
                        return new AVScanResult()
                        {
                            Message = $"Virus Found : {scanResult.InfectedFiles.First().VirusName}",
                            Result = false
                        };

                    case ClamScanResults.Error:
                        return new AVScanResult()
                        {
                            Message = $"Woah an error occured! Error: {scanResult.RawResult}",
                            Result = false
                        };
                    default:
                        return new AVScanResult()
                        {
                            Message = $"ClamAv Result returned Unknown ",
                            Result = false
                        };

                };
            }
            catch (Exception ex)
            {
                return new AVScanResult()
                {
                    Message = $"Error calling ClamAV : {ex}",
                    Result = false
                };
            }
        }
    }
}
