using DPMGallery.Extensions;
using DPMGallery.Services;
using Microsoft.AspNetCore.Authorization;
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
        public PackagePublishController(ILogger logger, IPackageIndexService packageService)
        {
            _logger = logger;
            _packageIndexService = packageService;
        }


        private async Task<Stream> GetRequestStream(CancellationToken cancellationToken)
        {
            Stream rawUploadStream = null;
            try
            {
                if (Request.HasFormContentType && Request.Form.Files.Count > 0)
                {
                    rawUploadStream = Request.Form.Files[0].OpenReadStream();
                }
                else
                {
                    rawUploadStream = Request.Body;
                }

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
                return Request.Form.Files[0].FileName;
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
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            if (claimsPrincipal == null) //shouldn't be possible to get here in this state!
            {
                return Unauthorized();
            }

            //we added the apikeyid as a claim in the apikey auth middleware
            var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == Constants.Claims.ApiKeyId);
            if (claim == null)
            {
                return Unauthorized();
            }
            string packageFileName = GetUploadFileName();
            _logger.Information($"Processing file : {packageFileName}");
            try
            {
                int apiKeyId = int.Parse(claim.Value);

                using (Stream uploadStream = await GetRequestStream(cancellationToken))
                {
                    if (uploadStream == null)
                    {
                        return BadRequest();
                    }

                    var result = await _packageIndexService.IndexAsync(uploadStream, apiKeyId, cancellationToken);
                    switch (result.Status)
                    {
                        case PackageIndexingStatus.InvalidPackage:
                            return BadRequest(result.Message);
                        case PackageIndexingStatus.Forbidden:
                            return Forbid(result.Message);
                        case PackageIndexingStatus.PackageAlreadyExists:
                            return StatusCode(409, "Package already exists");
                        case PackageIndexingStatus.Error:
                            return StatusCode(500, result.Message);
                        case PackageIndexingStatus.FailedAVScan:
                            return StatusCode(400, "Package failed Antivirus Scan");
                        case PackageIndexingStatus.Success:
                            return StatusCode(201);
                        default:
                            return StatusCode(500, "Unknown error"); 
                    }

                }
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