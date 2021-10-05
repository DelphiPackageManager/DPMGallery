using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static FluentMigrator.Runner.ConsoleUtilities;

namespace DPMGallery.DBMigration
{
    public class MigrationConsoleLogger : FluentMigratorRunnerLogger, IDisposable
    {
        private const string category = "Database Migration";
        private readonly Stream _stream;
        private StreamWriter _writer;
        private static readonly Regex classNameMatcher = new Regex(@"^(?<version>\d+)\:\s*(?<name>[^\s]+)\s*(?<action>migrating|migrated|reverting)$");
        private MigrationAttribute _currentMigrationAttribute = null;
        //private List<MigrationNoteAttribute> _currentMigrationNoteAttributes = null;
        private bool _upgrade = true;
        private readonly ILogger _logger;

        public MigrationConsoleLogger(string fileName, ILogger logger, FluentMigratorLoggerOptions options) : base(Console.Out, Console.Error, options)
        {
            _logger = logger;
            if (!string.IsNullOrEmpty(fileName))
            {
                _stream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
            }
        }

        /// <inheritdoc />
        protected override void WriteHeading(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            base.WriteHeading(message);
            Console.ResetColor();
            if (!_upgrade)
            {
                _logger.Debug($"[{category}] " + message);
                return;
            }
            ExtractMigrationDescriptionAndNotes(ref message);
            if (!string.IsNullOrEmpty(message))
            {
                _logger.Information($"[{category}] " + message);
            }
        }

        /// <inheritdoc />
        protected override void WriteEmphasize(string message)
        {
            AsEmphasize(() => base.WriteEmphasize(message));
        }

        /// <inheritdoc />
        protected override void WriteSql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                WriteEmptySql();
            }
            else
            {
                Console.WriteLine(sql);
            }
            _logger.Debug($"[{category}] " + sql);
        }

        /// <inheritdoc />
        protected override void WriteEmptySql()
        {
            Console.WriteLine(@"No SQL statement executed.");
        }

        /// <inheritdoc />
        protected override void WriteElapsedTime(TimeSpan timeSpan)
        {
            Console.ResetColor();
            string message = $"Elapsed time was {timeSpan.TotalSeconds} seconds.";
            _logger.Debug($"[{category}] " + message);
            base.WriteElapsedTime(timeSpan);
        }

        /// <inheritdoc />
        protected override void WriteSay(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            base.WriteSay(message);
            Console.ResetColor();
            if (!_upgrade)
            {
                _logger.Debug($"[{category}] " + message);
                return;
            }
            string lcMessage = message.ToLower();
            if (lcMessage.StartsWith("using database"))
                return;

            switch (lcMessage)
            {
                case "committing transaction":
                    message = $"Committing transaction";
                    break;
                case "rolling back transaction":
                    message = $"Rolling back transaction";
                    break;
                case "performing db operation":
                    message = $"Performing database operation";
                    break;
                case "beginning transaction":
                case "task completed.":
                    return;
                default:
                    if (ExtractMigrationDescriptionAndNotes(ref message))
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            _logger.Information($"[{category}] " + message);
                        }

                        return;
                    }
                    break;

            }
            _logger.Information($"[{category}] {message}");

        }

        /// <inheritdoc />
        protected override void WriteError(Exception exception)
        {
            AsError(() => base.WriteError(exception));
        }

        /// <inheritdoc />
        protected override void WriteError(string message)
        {
            AsError(() => base.WriteError(message));
            _logger.Error($"[{category}] " + message);
        }

        //this changes migration messages from fluentmigrator - adding description and notes from migration class attributes where required
        private bool ExtractMigrationDescriptionAndNotes(ref string message)
        {
            try
            {

                if (_upgrade && message.Equals("VersionMigration migrating", StringComparison.OrdinalIgnoreCase))
                {
                    _upgrade = false;
                    message = "Creating full database structure.";
                    _logger.Debug($"[{category}] " + message);
                    return false;
                }

                Match match = classNameMatcher.Match(message);
                if (match.Success)
                {

                    string className = match.Groups["name"]?.Value;
                    string action = match.Groups["action"]?.Value;
                    string version = match.Groups["version"]?.Value;
                    if (_upgrade && version == "0")
                    {
                        _upgrade = false;
                        _logger.Debug($"[{category}] " + message);
                        message = "Creating full database structure.";
                        return false;
                    }
                    if (!string.IsNullOrWhiteSpace(className))
                    {
                        string description = null;
                        if (action == "migrating" || _currentMigrationAttribute == null)
                        {
                            Type type = Type.GetType($"VSoft.DataMigrator.{className}");
                            if (type != null)
                            {
                                object[] attributes = type.GetCustomAttributes(typeof(MigrationAttribute), false);
                                if (attributes.Length > 0)
                                {
                                    _currentMigrationAttribute = attributes[0] as MigrationAttribute;
                                    description = _currentMigrationAttribute.Description;
                                }

                                //attributes = type.GetCustomAttributes(typeof(MigrationNoteAttribute), false);
                                //_currentMigrationNoteAttributes = attributes.Cast<MigrationNoteAttribute>().ToList();
                            }
                        }
                        else if (_currentMigrationAttribute != null)
                        {
                            description = _currentMigrationAttribute.Description;
                        }

                        if (string.IsNullOrWhiteSpace(description))
                        {
                            description = version;
                        }

                        if (string.IsNullOrWhiteSpace(description))
                        {
                            description = className;
                        }

                        switch (action)
                        {
                            case "migrating":
                                message = $"Starting migration: {description}";
                                /*
                                if (_currentMigrationNoteAttributes != null)
                                {
                                    MigrationNoteAttribute migrationNote = _currentMigrationNoteAttributes.FirstOrDefault(m => string.IsNullOrEmpty(m.Pattern));
                                    if (migrationNote != null)
                                        message = migrationNote.ReplaceMessage ? $"<b>Note</b>: {migrationNote.Description}" : message + $"<br /><b>Note</b>: {migrationNote.Description}";
                                }
                                */
                                return true;
                            case "migrated":
                                message = $"Completed migration: {description}";
                                return true;
                            case "reverting":
                                message = $"Reverting migration: {description}";
                                return true;
                        }
                    }
                }
                /*
                if (_currentMigrationNoteAttributes != null && _currentMigrationNoteAttributes.Count > 0)
                {
                    foreach (MigrationNoteAttribute migrationNote in _currentMigrationNoteAttributes.Where(m => !string.IsNullOrEmpty(m.Pattern)))
                    {
                        Match noteMatch = Regex.Match(message, migrationNote.Pattern);
                        if (noteMatch.Success)
                        {
                            message = migrationNote.ReplaceMessage ? $"<b>Note</b>: {migrationNote.Description}" : message + $"<br /><b>Note</b>: {migrationNote.Description}";
                            return migrationNote.ReplaceMessage;
                        }
                    }
                }*/
            }
            catch (Exception ex)
            {
                _logger.Error($"[{category}] " + $"An error occurred while parsing migration message '{message}': {ex.Message}");
                message = string.Empty;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Flush();
                _writer.Dispose();
                _writer = null;
            }
            _stream?.Dispose();
        }
    }

}
