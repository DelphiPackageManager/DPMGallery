using Amazon.Runtime.Internal.Util;
using DPMGallery.Data;
using DPMGallery.Entities;
using DPMGallery.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Claims = DPMGallery.Constants.Claims;
using T = DPMGallery.Constants.Database.TableNames;
using ILogger = Serilog.ILogger;

namespace DPMGallery.Middleware
{
    public class ApiKeyAuthMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ILogger _logger;

        public ApiKeyAuthMiddleware(RequestDelegate next, IDbContextFactory dbContextFactory, ILogger logger)// UserRepository userRepository)
        {
            _next = next;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task Invoke(HttpContext ctx, UserManager<User> userManager)
        {
            if (ctx.Request.Path.StartsWithSegments(new PathString("/api")))
            {
                // Let's check if this is an API Call
                if (ctx.Request.Headers["X-ApiKey"].Any())
                {
                    // validate the supplied API key
                    // Validate it
                    var headerKey = ctx.Request.Headers["X-ApiKey"].FirstOrDefault();
                    await ValidateApiKey(ctx, userManager, _next, headerKey);
                }
                else
                {
                    await _next(ctx);
                }
            }
            else
            {
                await _next(ctx);
            }
        }

        private async Task ValidateApiKey(HttpContext ctx, UserManager<User> userManager, RequestDelegate next, string key)
        {
            // validate it here
            ApiKey apiKey;
            string hashed = key.ToLower().GetHashSha256();
            _logger.Information("validating apikey [{key}]", key);
            //NOTE : Not using ApiKeyRepository here as we can't used scoped services here.
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                string q = $"select * from {T.ApiKeys} where key_hashed = @hashed";
                apiKey = await dbContext.QueryFirstOrDefaultAsync<ApiKey>(q, new { hashed });
            }

            if (apiKey == null)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await ctx.Response.WriteAsync("Invalid API Key");
                return;
            }


            User user = await userManager.FindByIdAsync(apiKey.UserId.ToString());
            if (user == null)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await ctx.Response.WriteAsync("User not found");
                return;
            }

            if (apiKey.ExpiresUTC < DateTime.UtcNow)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await ctx.Response.WriteAsync("API Key Expired");
                return;
            }

            //add api info to claims so we can use them in package upload 
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, user.UserName), //needed for the identiy
                new Claim(Claims.UserName, user.UserName),
                new Claim(Claims.UserId, user.Id.ToString()),
                new Claim(Claims.ApiKeyId, apiKey.Id.ToString()), //we need this in the packageindexservice
                new Claim(Claims.ApiKeyScopes, apiKey.Scopes.ToString()), //we need this in the packageindexservice
                new Claim(Claims.ApiKeyExpires, apiKey.ExpiresUTC.DateTime.ToFileTimeUtc().ToString()),
            };

            if (!string.IsNullOrEmpty(apiKey.GlobPattern))
            {
                claims.Add(new Claim(Claims.ApiKeyGlob, apiKey.GlobPattern));
            }

            if (!string.IsNullOrEmpty(apiKey.Packages))
            {
                claims.Add(new Claim(Claims.ApiKeyPackages, apiKey.Packages));
            }
            var identity = new ClaimsIdentity(claims, "ApiKey");
            var principal = new ClaimsPrincipal(identity);

            ctx.User = principal;
            await next(ctx);

        }
    }

}
