using DPMGallery.Entities;
using DPMGallery.Extensions;
using DPMGallery.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

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
            _configuration.Bind(serverConfig);

            services.AddSingleton<ILogger>(provider =>
            {
                return Serilog.Log.Logger;
            });

            //allows running behind nginx or yarp
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });


            //TODO: Replace with built in rate limiting when we upgrade to netcore 7.0
            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();
            ////load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(_configuration.GetSection("IpRateLimitOptions"));
            ////load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(_configuration.GetSection("IpRateLimitPolicies"));

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

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential 
            //    // cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    // requires using Microsoft.AspNetCore.Http;
            //    options.MinimumSameSitePolicy = SameSiteMode.Strict;
            //});

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = serverConfig.Authentication.Jwt.ValidAudience,
                    ValidIssuer = serverConfig.Authentication.Jwt.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serverConfig.Authentication.Jwt.Secret))
                };
                options.Events = new JwtBearerEvents();
                options.Events.OnMessageReceived = context => {
                    if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                    {
                        context.Token = context.Request.Cookies["X-Access-Token"];
                    }

                    return Task.CompletedTask;
                };
                options.Events.OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                    }
                    return Task.CompletedTask;
                };
            }).AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.IsEssential = true;
            })

            //need to figure out how to use these with react
            
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
                
            });
            /*.AddGitHub(options =>
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
            }); */


            services.AddControllers().AddJsonOptions(j =>
            {
                j.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
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
                app.UseCors(config =>
                {
                    config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                }) ;
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseForwardedHeaders();
            app.UseIpRateLimiting(); //TODO : replace with built in rate limiting
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapWhen(x => !(x.Request.Path.Value.StartsWith("/api") | x.Request.Path.Value.StartsWith("/ui")), builder =>
            {
                builder.UseSpa(spa =>
                {
                    if (env.IsDevelopment())
                    {
                        // Make sure you have started the frontend with npm run dev on port 4000
                        spa.UseProxyToSpaDevelopmentServer("http://localhost:3175");
                    }
                });
            });




            //app.UseCookiePolicy();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseApiKeyAuthMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOperationCancelledMiddleware();
            app.UseHttpLogging();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapApiRoutes();
                endpoints.MapFallbackToFile("/index.html");
                
            });

        }
    }
}
