using DPMGallery.Entities;
using DPMGallery.Extensions;
using DPMGallery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Controllers
{
    [Authorize]
    [ApiController]
    [DisableRateLimiting]
    public class PackagePublishController : Controller
    {
        private readonly IPackageIndexService _packageIndexService;
        private readonly ILogger _logger;
        private readonly UserManager<User> _userManager;
        public PackagePublishController(ILogger logger, IPackageIndexService packageService, UserManager<User> userManager)
        {
            _logger = logger;
            _packageIndexService = packageService;
            _userManager = userManager;
        }


        private async Task<Stream> GetRequestStream(int idx, CancellationToken cancellationToken)
        {
            Stream rawUploadStream = null;
            try
            {
                rawUploadStream = Request.Form.Files[idx].OpenReadStream();
                // Convert the upload stream into a temporary file stream to
                // minimize memory usage.
                return await rawUploadStream?.CopyToTemporaryFileStreamAsync(cancellationToken);
            }
            finally
            {
                rawUploadStream?.Dispose();
            }

        }

        private string GetUploadFileName()
        {
            if (Request.HasFormContentType && Request.Form.Files.Count > 0)
            {
                return Path.GetFileName(Request.Form.Files[0].FileName); // remove any path info from filename in case of malicious uploads.
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Push package file(s) to the server. Not sure if it's a good idea to allow pushing multiple files?
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Constants.RoutePatterns.PackagePublish)]
        public async Task<IActionResult> PushPackage(CancellationToken cancellationToken)
        {
            if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
            {
                return BadRequest("no files");
            }

            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            if (claimsPrincipal == null) //shouldn't be possible to get here in this state!
            {
                return Unauthorized();
            }

            string userName = HttpContext.User.Identity?.Name;
            if (userName == null)
            {
                //just return nothing
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound($"Unable to load user with name '{userName}'.");
            }

            //we add the apikeyid as a claim in the apikey auth middleware
            var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == Constants.Claims.ApiKeyId);
            // claim can be null if uploading using the website

            string packageFileName = GetUploadFileName();
            _logger.Information($"Processing file : {packageFileName}");
            try
            {
                int apiKeyId = claim != null ? int.Parse(claim.Value) : -1;

                for (int i = 0; i < Request.Form.Files.Count; i++)
                {
                    using (Stream uploadStream = await GetRequestStream(i, cancellationToken))
                    {
                        if (uploadStream == null)
                        {
                            return BadRequest();
                        }

                        Thread.Sleep(500);
                        //return StatusCode(409, "Package already exists");

#if !DEBUG  //turning off indexing while we debug/test uploads.
                        var result = await _packageIndexService.IndexAsync(uploadStream, user.Id, apiKeyId, cancellationToken);
                        switch (result.Status)
                        {
                            case PackageIndexingStatus.InvalidPackage:
                                return BadRequest(result.Message);
                            case PackageIndexingStatus.Forbidden:
                                return StatusCode(403, result.Message);
                            case PackageIndexingStatus.PackageAlreadyExists:
                                return StatusCode(409, "Package already exists");
                            case PackageIndexingStatus.Error:
                                return StatusCode(500, result.Message);
                            case PackageIndexingStatus.FailedAVScan:
                                return StatusCode(400, "Package failed Antivirus Scan");
                            case PackageIndexingStatus.Success:                               
                                break; //just continue
                            default:
                                return StatusCode(500, "Unknown error");
                        }
#endif
                    }
                }
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception thrown during package upload of file {packageFileName}");

                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete]
        [Route(Constants.RoutePatterns.PackageDelist)]
        public async Task DelistPackage([FromRoute] string id, [FromRoute] string compilerVersion, [FromRoute] string platform, [FromRoute] string version, CancellationToken cancellationToken)
        {

            //just for now.
            await Task.CompletedTask;
        }

    }
}