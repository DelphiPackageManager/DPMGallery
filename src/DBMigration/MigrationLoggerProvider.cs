using FluentMigrator.Runner;
using Microsoft.Extensions.Logging;

namespace DPMGallery.DBMigration
{
    public class MigrationLoggerProvider : ILoggerProvider
    {
        private readonly FluentMigratorLoggerOptions _options;

        private readonly Serilog.ILogger _logger;

        private readonly string _fileName;

        public MigrationLoggerProvider(string fileName, Serilog.ILogger log, FluentMigratorLoggerOptions options)
        {
            _fileName = fileName;
            _logger = log;
            _options = options;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MigrationConsoleLogger(_fileName, _logger, _options);
        }

        public void Dispose()
        {
            //nothing to dispose 
        }
    }
}
