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

namespace DPMGallery.Middleware
{
    public class ApiKeyAuthMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IDbContextFactory _dbContextFactory;

        public ApiKeyAuthMiddleware(RequestDelegate next, IDbContextFactory dbContextFactory)// UserRepository userRepository)
        {
            _next = next;
            _dbContextFactory = dbContextFactory;
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
                //else if (ctx.Request.Query.ContainsKey("apikey"))
                //{
                //    if (ctx.Request.Query.TryGetValue("apikey", out var queryKey))
                //    {
                //        await ValidateApiKey(ctx, userManager, _next, queryKey);
                //    }
                //}
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
            var valid = true;
            ApiKey apiKey;
            //NOT : Not using ApiKeyRepository here as we can't used scoped services here.
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                string hashed = key.GetHashSha256();

                string q = $"select * from {T.ApiKey} where key_hashed = @hashed";
                apiKey = await dbContext.QueryFirstOrDefaultAsync<ApiKey>(q, new { hashed });

            }

            User user = null;
            if (apiKey != null)
            {
                user = await userManager.FindByIdAsync(apiKey.UserId.ToString());
                if (user == null || apiKey.ExpiresUTC < DateTime.UtcNow)
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }

            if (!valid)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await ctx.Response.WriteAsync("Invalid API Key");
            }
            else
            {
                //add api info to claims so we can use them in package upload 
                var claims = new List<Claim>() {
                    new Claim(Claims.UserName, user.UserName),
                    new Claim(Claims.UserId, user.Id.ToString()),
                    new Claim(Claims.ApiKeyId, apiKey.Id.ToString()), //we need this in the packageindexservice
                    new Claim(Claims.ApiKeyExpires, apiKey.ExpiresUTC.ToFileTimeUtc().ToString()),
                };

                if (!string.IsNullOrEmpty(apiKey.GlobPattern))
                {
                    claims.Add(new Claim(Claims.ApiKeyGlob, apiKey.GlobPattern));
                }

                if (!string.IsNullOrEmpty(apiKey.Packages))
                {
                    claims.Add(new Claim(Claims.ApiKeyPackages, apiKey.Packages));
                }

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims.ToArray(), "ApiKey"));

                ctx.User = principal;
                await next(ctx);
            }

        }
    }

}
