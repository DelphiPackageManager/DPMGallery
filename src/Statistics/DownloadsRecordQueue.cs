using DPMGallery.Types;
using System.Collections.Generic;
using System.Linq;

namespace DPMGallery.Statistics
{
    public class Item
    {
        public string packageId;
        public Platform platform;
        public CompilerVersion compilerVersion;
        public string packageVersion;
        public int downloads;
    }

    /// <summary>
    /// not really a queue - since we want to group together downloads of the same package to avoid too many db updates 
    /// when we process the queue.
    /// The order these are done in doesn't matter much anyway.
    /// </summary>
    public class DownloadsRecordQueue
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, Item> _records;
        public DownloadsRecordQueue() 
        { 
            _records = new Dictionary<string, Item>();
        }

        public static DownloadsRecordQueue Instance { get; } = new DownloadsRecordQueue();

        public void RecordDownload(string packageId, Platform platform, CompilerVersion compilerVersion, string packageVersion) 
        {
            string key = (packageId + "-" +platform.ToString() + "-" + compilerVersion.ToString() + "-" + packageVersion).ToLowerInvariant();
            
            lock (_lock)
            {
                if (_records.ContainsKey(key))
                {
                    _records[key].downloads = _records[key].downloads + 1;
                    
                }
                else
                {
                    var item = new Item();
                    item.packageId = packageId;
                    item.platform = platform;
                    item.compilerVersion = compilerVersion;
                    item.packageVersion = packageVersion;
                    item.downloads = 1;
                    _records[key] = item;
                }
            }
        }

        public Item Dequeue()
        {
            lock (_lock)
            {
                if ( _records.Count == 0 )
                    return null;
                var key = _records.Keys.First();
                var result = _records[key];
                _records.Remove(key);
                return result;
            }
        }
    }
}
