using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Antivirus
{
    public class AVScanResult
    {
        public bool Result { get; set; }

        public string Message { get; set; }

    }

    public interface IAntivirusService
    {
        Task<AVScanResult> Scan(string filePath, CancellationToken cancellationToken);

        bool Enabled { get; }

        public int Order { get; }

        public string ServiceName { get; }
    }
}
