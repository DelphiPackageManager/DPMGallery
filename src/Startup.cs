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
using DPMGallery.Services;
using Serilog;
using DPMGallery.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.AspNetCore.HttpOverrides;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

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

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(policyName: "api-fixed", options =>
                {
                    options.PermitLimit = serverConfig.RateLimiting.PermitLimit;
                    options.Window = TimeSpan.FromSeconds(serverConfig.RateLimiting.Window);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    //options.QueueLimit = 200; //TODO :add to 
                    options.AutoReplenishment = true;
                });


                //options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                //{
                //    return RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Request.Headers.Host.ToString(), partition =>
                //        new FixedWindowRateLimiterOptions
                //        {
                //            PermitLimit = serverConfig.RateLimiting.PermitLimit,
                //            AutoReplenishment = true,
                //            Window = TimeSpan.FromSeconds(serverConfig.RateLimiting.Window)
                //        });
                //});
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        await context.HttpContext.Response.WriteAsync(
                            $"Too many requests. Please try again after {retryAfter.TotalMinutes} minute(s). " +
                            $"Read more about our rate limits at https://docs.delphi.dev/ratelimiting.", cancellationToken: token);
                    }
                    else
                    {
                        await context.HttpContext.Response.WriteAsync(
                            "Too many requests. Please try again later. " +
                            "Read more about our rate limits at https://docs.delphi.dev/ratelimiting.", cancellationToken: token);
                    }
                };

            });

            //used in uiservice
            services.AddMemoryCache();

            //TODO: Replace with built in rate limiting when we upgrade to netcore 7.0
            //////load general configuration from appsettings.json
            //services.Configure<IpRateLimitOptions>(_configuration.GetSection("IpRateLimitOptions"));
            //////load ip rules from appsettings.json
            //services.Configure<IpRateLimitPolicies>(_configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            //services.AddInMemoryRateLimiting();

            services.AddDPMServices(serverConfig);

            //our asp.net identity implementation using dapper instead of EF
            services.AddIdentity<User, Role>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                //options.Stores.

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

            services.AddOutputCache(x =>
            {
                x.AddPolicy("UIQuery", builder => {
                    builder.Cache()
                    .Expire(TimeSpan.FromSeconds(10))
                    .SetVaryByQuery(new string[] { "*" })
                    .With(c => c.HttpContext.Request.Path.StartsWithSegments("/ui"));
                }, excludeDefaultPolicy: true);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;

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
            })//.AddCookie(options =>
            //{
            //    options.Cookie.SameSite = SameSiteMode.Lax;
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            //    options.Cookie.IsEssential = true;
            //    options.Events.OnRedirectToLogin += context =>
            //    {
            //        context.Response.StatusCode = 401;
            //        return Task.CompletedTask;
            //    };
            //})

            //need to figure out how to use these with react
            .AddGoogle(options =>
            {
                //IConfigurationSection googleAuthNSection =
                //Configuration.GetSection("Authentication:Google");
                options.ClientId = serverConfig.Authentication.Google.ClientId;// .googleAuthNSection["ClientId"];
                options.ClientSecret = serverConfig.Authentication.Google.ClientSecret;//  googleAuthNSection["ClientSecret"];
                options.Scope.Add("profile");
                options.CallbackPath = new PathString("/oauth-google");
                options.SignInScheme = IdentityConstants.ExternalScheme;

            })
            .AddGitHub(options =>
            {
                options.ClientId = serverConfig.Authentication.GitHub.ClientId;
                options.ClientSecret = serverConfig.Authentication.GitHub.ClientSecret;
                options.CallbackPath = new PathString("/oauth-github");
                options.Scope.Add("user:email");
                options.Scope.Add("user:login");
                options.ClaimActions.MapJsonKey("urn:github:login", "login");
                options.ClaimActions.MapJsonKey("urn:github:login", "email");
                //options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                //options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
               // options.SignInScheme = IdentityConstants.ApplicationScheme;
            });


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
            //services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        }

        private Func<HttpContext, Func<Task>, Task> RemoveCacheControlHeadersForNon200s()
        {
            return async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    var headers = context.Response.GetTypedHeaders();
                    if (context.Response.StatusCode != StatusCodes.Status200OK &&
                        headers.CacheControl?.NoCache == false)
                    {
                        headers.CacheControl.NoCache = true;
                        //headers.CacheControl = new CacheControlHeaderValue
                        //{
                        //    NoCache = true
                        //};
                    }

                    return Task.FromResult(0);
                });
                await next();
            };
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
            //app.UseIpRateLimiting(); //TODO : replace with built in rate limiting
            app.UseHttpsRedirection();
            app.UseStaticFiles();


            string[] nonSpaUrls = { "/api", "/ui", "/oauth-" };

            app.MapWhen(x => !nonSpaUrls.Any(y => x.Request.Path.Value.StartsWith(y)), builder =>
            {
                builder.UseSpa(spa =>
                {
                    if (env.IsDevelopment())
                    {
                        // Make sure you have started the frontend with npm run dev
                        spa.UseProxyToSpaDevelopmentServer("http://localhost:3175");
                    }
                    else
                    {
                        spa.Options.SourcePath = "wwwroot";
                    }
                });
            });


            app.UseRateLimiter();

            //app.UseCookiePolicy();
            //app.UseSerilogRequestLogging();
            app.UseApiKeyAuthMiddleware();
            app.UseOperationCancelledMiddleware();
            app.UseRouting();
            app.UseOutputCache();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseHttpLogging();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapApiRoutes();
                endpoints.MapFallbackToFile("/index.html");
                
            });
            app.Use(RemoveCacheControlHeadersForNon200s());

        }
    }
}
