using Serilog;
using System;
using System.Linq;
using System.Threading;

namespace DPMGallery.DBMigration
{
    public class Migrator
    {
        public static bool Execute(ServerConfig config)
        {
            var connectionString = config.Database.ConnectionString;
            if (connectionString == null)
            {
                // ServerStatusDetails.Record(ServerStatus.Errored, "Database configuration has not been completed in " + config.FileName + " - Current connection object not found.");
                Log.Error("[Database] Database configuration has not been completed in the configuration. ");
                return false;
            }

            try
            {
                int connectionTimeoutInSeconds = 30;
                DateTime? timeoutTime = null;
                bool connectionError = false;
                bool messageLogged = false;
                do
                {
                    try
                    {
                        int erroredConnectionResolveDelayInSeconds = 2;
                        DateTime? erroredConnectionResolveTime = DateTime.Now.AddSeconds(erroredConnectionResolveDelayInSeconds);

                        //migrationRunner
                        MigrationRunner.Execute(typeof(Migrator).Assembly, connectionString, null, Log.Logger);

                        //ensure delay after connection error as otherwise db still may not be ready when we try to use it.
                        while (connectionError && DateTime.Now < erroredConnectionResolveTime.Value)
                        {
                            Thread.Sleep(10);
                        }

                        connectionError = false;
                        break;
                    }
                    catch (Exception ex)
                    {
                        if ((!timeoutTime.HasValue || DateTime.Now < timeoutTime.Value) && ex.StackTrace != null && ex.StackTrace.Contains("EnsureConnectionIsOpen"))
                        {
                            if (!messageLogged)
                            {
                                string message = $"Could not connect to the database. Retrying for up to {connectionTimeoutInSeconds} seconds.";
                                Log.Warning("[Database] {message}", message);
                                //ServerStatusDetails.Record(ServerStatus.Configuring, message);
                                messageLogged = true;
                            }

                            if (!timeoutTime.HasValue)
                            {
                                timeoutTime = DateTime.Now.AddSeconds(connectionTimeoutInSeconds);
                            }

                            connectionError = true;
                            Thread.Sleep(200);
                            continue;
                        }
                        throw;
                    }

                } while (connectionError && DateTime.Now < timeoutTime);

                return true;


            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[Database] Could not initialise database:\r\n\r\n{0}\r\nExiting...");
#if DEBUG
                Console.ReadLine();
#endif
                return false;
            }

        }

    }
}
