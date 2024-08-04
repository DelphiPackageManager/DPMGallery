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
using DPMGallery.Services;
using Serilog;
using DPMGallery.Models;
using System;
using Microsoft.AspNetCore.HttpOverrides;
using System.Linq;
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

            //NOTE : we are using cloudfare for cors configuration
            //if running this elsewhere then uncomment and configure cors as required.
			//services.AddCors(options =>
			//{
			//	options.AddPolicy("AllowSpecified", policy =>
			//	{
			//		policy.WithOrigins("https://delphi.dev",
			//						   "https://localhost:5002",
			//							"https://*.delphi.dev")
			//		.SetIsOriginAllowedToAllowWildcardSubdomains()
			//		.AllowAnyMethod().AllowAnyHeader();
			//	});
			//});



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

                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;


            }).AddRoles<Role>()
            .AddDefaultTokenProviders()
            .AddDapperStores();

            services.ConfigureApplicationCookie(opt => {
                opt.Cookie.Name = "DPMGallery.Identity";
            });

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential 
            //    // cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    // requires using Microsoft.AspNetCore.Http;
            //    options.MinimumSameSitePolicy = SameSiteMode.Strict;
            //});

            //services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, option =>
            //{
            //	option.Cookie.Name = "Hello"; // change cookie name
            //	option.ExpireTimeSpan = TimeSpan.FromDays(7); // change cookie expire time span
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

            services.AddAuthentication()
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;

            //}).AddIdentityCookies();

            //.AddJwtBearer(options =>
            //{
            //    options.SaveToken = true;
            //    options.RequireHttpsMetadata = false;
            //    options.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ClockSkew = TimeSpan.Zero,
            //        ValidAudience = serverConfig.Authentication.Jwt.ValidAudience,
            //        ValidIssuer = serverConfig.Authentication.Jwt.ValidIssuer,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serverConfig.Authentication.Jwt.Secret))
            //    };
            //    options.Events = new JwtBearerEvents();
            //    options.Events.OnMessageReceived = context => {
            //        if (context.Request.Cookies.ContainsKey("X-Access-Token"))
            //        {
            //            context.Token = context.Request.Cookies["X-Access-Token"];
            //        }

            //        return Task.CompletedTask;
            //    };
            //    options.Events.OnAuthenticationFailed = context =>
            //    {
            //        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //        {
            //            context.Response.Headers.Append("IS-TOKEN-EXPIRED", "true");
            //        }
            //        return Task.CompletedTask;
            //    };
            //})
            .AddGoogle(options =>
            {
                options.ClientId = serverConfig.Authentication.Google.ClientId;
                options.ClientSecret = serverConfig.Authentication.Google.ClientSecret;
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
            });


			services.AddControllers().AddJsonOptions(j =>
            {
                j.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });


			DTOMappings.Configure();
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
				//app.UseCors(config =>
				//{
				//	config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
				//});
			}
			else
			{
				//app.UseCors(config =>
				//{
				//	config.WithOrigins("https://delphi.dev",
				//					   "https://localhost:5002",
				//						"https://*.delphi.dev")
				//	.SetIsOriginAllowedToAllowWildcardSubdomains()
				//	.AllowAnyMethod().AllowAnyHeader().AllowCredentials();
				//});

				//app.UseCors(builder => builder.AllowAnyOrigin()
				//				.AllowAnyMethod()
				//				.AllowAnyHeader());

				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				//app.UseHsts();
			}

			


			app.UseForwardedHeaders();
			//app.UseIpRateLimiting(); //TODO : replace with built in rate limiting
			app.UseHttpsRedirection();
			
            app.UseStaticFiles();
            //app.UseSpaStaticFiles();

            string[] nonSpaUrls = { "/api", "/ui", "/oauth-" };

			

			app.MapWhen(x => !nonSpaUrls.Any(y => x.Request.Path.Value.StartsWith(y)), builder =>
            {
            	builder.UseSpa(spa =>
            	{
            		//spa.ApplicationBuilder.UseCors("AllowSpecified");
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

			app.UseRouting();


			//app.UseCors("AllowSpecified");

			app.UseOperationCancelledMiddleware();
			app.UseOutputCache();

			app.UseApiKeyAuthMiddleware();
			app.UseAuthentication();
            app.UseAuthorization();
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
