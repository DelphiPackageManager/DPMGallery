using DPMGallery.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DPMGallery
{
    public class ServerConfig
    {

        public const string ConfigFileName = "dpm.server";

        private static JsonSerializerOptions serializerOptions;

        private static string _defaultProcessingFolder;

        static ServerConfig()
        {
            serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
                    
            };
            serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            _defaultProcessingFolder = System.IO.Path.Combine(commonAppDataPath, "dpm","dpmserver","processing");

        }

        //This is for a few places where we can't inject the config.
        public static ServerConfig Current { get; private set; } = new ServerConfig();


        [JsonIgnore]
        public string FileName { get; private set; }


        public void Save()
        {
            if (string.IsNullOrWhiteSpace(FileName))
                throw new InvalidOperationException("FileName has not been set");

            Save(FileName);
        }

        public void Save(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            //string contents = JsonConvert.SerializeObject(this, Formatting.Indented, SerializerSettings);
            string contents = JsonSerializer.Serialize(this, serializerOptions);
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            File.WriteAllText(fileName, contents, Encoding.UTF8);
            FileName = fileName;
        }

        public static ServerConfig Load(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            string fileContent = File.ReadAllText(fileName, Encoding.UTF8);
            Current = JsonSerializer.Deserialize<ServerConfig>(fileContent, serializerOptions);
            Current.FileName = fileName;
            return Current;
        }
        public static ServerConfig CreateDefaultConfig(string configFileName)
        {
            if (string.IsNullOrWhiteSpace(configFileName))
                throw new ArgumentNullException(nameof(configFileName));

            Current = new ServerConfig();
            //			Current.CreateDefault();
            Current.Save(configFileName);
            return Current;
        }

        public EmailConfig Email { get; set; } = new EmailConfig();

        public DatabaseConfig Database { get; set; } = new DatabaseConfig();

        public AuthConfig Authentication { get; set; } = new AuthConfig();

        public StorageConfig Storage { get; set; } = new StorageConfig();

        public AntivirusConfig AntivirusConfig { get; set; } = new AntivirusConfig();

        public string ProcessingFolder { get; set; } = _defaultProcessingFolder;
    }
}
