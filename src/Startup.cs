using DPMGallery.Entities;
using DPMGallery.Extensions;
using DPMGallery.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using DPMGallery.Services;
using Serilog;
using DPMGallery.Models;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;

namespace DPMGallery
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        private IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serverConfig = ServerConfig.Current;
            services.AddSingleton(serverConfig);
            //_configuration.Bind(serverConfig);

            services.AddSingleton<Serilog.ILogger>(provider =>
            {
                return Serilog.Log.Logger;
            });


            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            ////load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(_configuration.GetSection("IpRateLimitOptions"));

            ////load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(_configuration.GetSection("IpRateLimitPolicies"));

            //_configuration.Bind("IpRateLimitOptions", serverConfig.IpRateLimitOptions);

            //_configuration.Bind("ipRateLimitPolicies", serverConfig.IpRateLimitPolicies);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // inject counter and rules stores
            services.AddInMemoryRateLimiting();

            services.AddDPMServices(serverConfig);

            //our asp.net identity implementation using dapper instead of EF
            services.AddIdentity<User, Role>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;


            }).AddRoles<Role>()
            .AddDefaultTokenProviders()
            .AddDapperStores();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                // requires using Microsoft.AspNetCore.Http;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = _env.IsDevelopment();
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                }); ;

            var mvcBuilder = services.AddRazorPages();
            if (_env.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            services.AddAuthentication()
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = serverConfig.Authentication.Microsoft.ClientId;// Configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = serverConfig.Authentication.Microsoft.ClientSecret;// Configuration["Authentication:Microsoft:ClientSecret"];
            })
            .AddGoogle(options =>
            {
                //IConfigurationSection googleAuthNSection =
                //Configuration.GetSection("Authentication:Google");

                options.ClientId = serverConfig.Authentication.Google.ClientId;// .googleAuthNSection["ClientId"];
                options.ClientSecret = serverConfig.Authentication.Google.ClientSecret;//  googleAuthNSection["ClientSecret"];
            }).AddGitHub(options =>
            {
                options.ClientId = serverConfig.Authentication.GitHub.ClientId;
                options.ClientSecret = serverConfig.Authentication.GitHub.ClientSecret;
                options.CallbackPath = new PathString("/github-oauth");
                options.Scope.Add("user:email");
                options.Scope.Add("user:login");
                options.ClaimActions.MapJsonKey("urn:github:login", "login");
                options.ClaimActions.MapJsonKey("urn:github:login", "email");
                options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
            });
            services.AddCors();

            //services.AddHttpLogging(logging =>
            //{
            //    // Customize HTTP logging here.
            //    logging.LoggingFields = HttpLoggingFields.All;
            //    //logging.ResponseHeaders.Add("My-Response-Header");
            //    //logging.MediaTypeOptions.AddText("application/javascript");
            //    logging.RequestBodyLogLimit = 4096;
            //    logging.ResponseBodyLogLimit = 4096;
            //});

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/identity/account/login");
                options.LogoutPath = new PathString("/identity/account/logout");

                //stop redirecting to login for api routes.
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == StatusCodes.Status200OK)
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };

            });
            DTOMappings.Configure();
            ModelMappings.Configure();
            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }
            app.UseForwardedHeaders();
            app.UseIpRateLimiting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseApiKeyAuthMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOperationCancelledMiddleware();
            //app.UseHttpLogging();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapApiRoutes();
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
