using DPMGallery.Data;
using DPMGallery.DBMigration;
using DPMGallery.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;

namespace DPMGallery
{
    public class Program
    {

        private static ServerConfig LoadServerConfig()
        {
            //need to get the datashare path from the server service config file.

            string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            string configFileName = Path.Combine(commonAppDataPath, "dpm\\dpmserver", ServerConfig.ConfigFileName + ".config.json");

            if (!File.Exists(configFileName))
                return ServerConfig.CreateDefaultConfig(configFileName);

            return ServerConfig.Load(configFileName);

        }


        public static void Main(string[] args)
        {

            ServerConfig serverConfig = LoadServerConfig();

            //TODO : Granular Serilog config and logging to file.

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .CreateLogger();


            var host = CreateHostBuilder(args).ConfigureAppConfiguration((ctx, builder) =>
            {
                if (ctx.HostingEnvironment.IsDevelopment())
                {
                    builder.AddUserSecrets<Program>();

                }
                TypeMapper.Initialize("DPMGallery.Entities");

            }).Build();


            try
            {
                //Run DB migrator here
                if (serverConfig.Database.AutoMigrate)
                {

                    Log.Information("[{category}] Migrating database", "Database");
                    try
                    {
                        if (!Migrator.Execute(serverConfig))
                        {
                            Log.Error("Error occurred during db migration");
                            throw new Exception("Error migrating DB");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "[{category}] Database migration failed", "Database");
                        throw;
                    }
                    Log.Information("[{category}] Database migration done.", "Database");
                }


                Log.Information("Starting up");
                using (var scope = host.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
                    if (!roleManager.RoleExistsAsync("Administrator").Result)
                    {
                        var role = new Role() { Name = "Administrator", NormalizedName = "administrator" };
                        roleManager.CreateAsync(role).Wait();
                    }

#if !DEBUGX
                    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                    if (userManager.FindByNameAsync("vincent").Result == null)

                    {
                        User user = new()
                        {
                            UserName = "vincent",
                            NormalizedUserName = "vincent",
                            Email = "vincent@parrett.id.au"
                        };

                        IdentityResult result = userManager.CreateAsync(user, "dJ6EHI8$1Gk375^iQMkJO$g*OEWe").Result;

                        if (result.Succeeded)
                        {
                            userManager.AddToRoleAsync(user, "Administrator").Wait();
                        }
                    }
#endif
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
#if DEBUG
                Console.ReadLine();
#endif
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog();
    }
}
