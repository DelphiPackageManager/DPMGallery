using DPMGallery.Data;
using DPMGallery.Entities;
using DPMGallery.Extensions.Mapping;
using DPMGallery.Models.Account;
using DPMGallery.Repositories;
using DPMGallery.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Controllers.UI
{
    [ApiController]
    [DisableRateLimiting]
    [Route("/ui/account/apikeys")]
    public class ApiKeyController : ApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly ApiKeyRepository _apiKeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ApiKeyController(UserManager<User> userManager, ApiKeyRepository apiKeyRepository, IUnitOfWork unitOfWork, ILogger logger) : base(logger)
        {
            _userManager = userManager;
            _apiKeyRepository = apiKeyRepository;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetApiKeys([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string sort = null, [FromQuery] SortDirection sortDirection = SortDirection.Default, [FromQuery] string filter = null, CancellationToken cancellationToken = default)
        {
            string userName = HttpContext.User.Identity?.Name;
            try
            {
                if (userName == null)
                {
                    //just return nothing
                    return Unauthorized();
                }
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return Unauthorized();
                }

                var paging = new Paging();
                paging.Page = page;
                paging.PageSize = pageSize;
                paging.Sort = sort;
                paging.Filter = filter;
                paging.SortDirection = sortDirection;

                PagedList<ApiKey> apiKeys = await _apiKeyRepository.GetApiKeysForUser(user.Id, paging, cancellationToken);

                PagedList<ApiKeyModel> apiKeyModels = apiKeys.ToPagedModel();

                if (apiKeyModels == null)
                {
                    var additionalInformation = new Dictionary<string, object> { { "userName", userName } };
                    return GetProblemResponse("Failed to get list of API keys", detail: "Null list of API key models returned", additionalInformation: additionalInformation);
                }

                return Ok(apiKeyModels);
            }
            catch (Exception ex)
            {
                var additionalInformation = new Dictionary<string, object> { { "Page", page }, { "PageSize", pageSize }, { "Sort", sort }, { "SortDirection", sortDirection }, { "Filter", filter }, { "UserName", userName } };
                return GetProblemResponse($"Error getting list of API keys", exception: ex, additionalInformation: additionalInformation);
            }
        }


        //TODO: move this to a manager class
        private const int _numberOfSecureBytesToGenerate = 32;
        private const int _lengthOfKey = 32;

        private static string GenerateApiKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(_numberOfSecureBytesToGenerate);

            string base64String = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_");


            return base64String;
        }

        [Authorize]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateApiKey([FromBody] CreateApiKeyModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                string userName = HttpContext.User.Identity?.Name;
                if (userName == null)
                {
                    //just return nothing
                    return Unauthorized();
                }
                User user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return Unauthorized();

                var apiKey = new ApiKey()
                {
                    UserId = user.Id,
                    Name = model.Name,
                    GlobPattern = model.GlobPattern,
                    Scopes = model.Scopes,
                    ExpiresUTC = DateTime.UtcNow.AddDays(model.ExpiresInDays),
                    Key = GenerateApiKey()
                };

                bool exists = await _apiKeyRepository.ApiKeyNameExists(model.Name);
                if (exists)
                    return Conflict("An API key with that name already exists");

                ApiKey result = await _apiKeyRepository.Insert(apiKey);
                _unitOfWork.Commit();
                ApiKeyModel apiKeyModel = ApiKeyMappings.ToModel(result);

                string url = $"/ui/admin/apikeys/{apiKeyModel.Id}";
                if (!Uri.TryCreate(url, UriKind.Relative, out Uri uri))
                    return BadRequest("Failed to create URL to created API key");

                return Created(uri, apiKeyModel);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error("{CurrentAction} failed: {Message}", nameof(CreateApiKey), ex.Message);
                return BadRequest(Constants.ErrorMessages.ServerError);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateApiKeyEnabled([FromRoute] string id, [FromBody] bool enabled, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("You must provide an API key id");

                if (!int.TryParse(id, out int keyId))
                    return BadRequest("Invalid API key id");

                bool revoked = !enabled;
                bool updated = await _apiKeyRepository.UpdateApiKeyRevoked(keyId, revoked, cancellationToken);
                _unitOfWork.Commit();
                var resultModel = new { Succeeded = true, Updated = updated };
                return Ok(resultModel);
            }
            catch (Exception ex)
            {

                Log.Error("{CurrentAction} failed: {Message}", nameof(UpdateApiKeyEnabled), ex.Message);
                _unitOfWork.Commit();
                //TODO: return ProblemDetails instead of BadRequest for all actions
                return BadRequest(Constants.ErrorMessages.ServerError);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("{id}/regenerate")]
        public async Task<IActionResult> RegenerateApiKey([FromRoute] string id, [FromBody] int? expiresInDays, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("You must provide an API key id");

                if (!int.TryParse(id, out int keyId))
                    return BadRequest("Invalid API key id");

                ApiKey existing = await _apiKeyRepository.GetApiKeyById(keyId, cancellationToken);
                if (existing == null)
                    return BadRequest($"An API key does not exist with id: {id}");

                string newKey = GenerateApiKey();
                DateTime? expiresUtc = null;
                if (expiresInDays.HasValue)
                    expiresUtc = DateTime.UtcNow.AddDays(expiresInDays.Value);

                bool updated = await _apiKeyRepository.UpdateApiKey(keyId, expiresUtc, newKey, cancellationToken);
                _unitOfWork.Commit();
                ApiKeyModel apiKeyModel = ApiKeyMappings.ToModel(existing);
                ApiKeyModel result;
                if (updated)
                {
                    DateTimeOffset expiresUtcOffset = apiKeyModel.ExpiresUTC;
                    if (expiresUtc.HasValue)
                        expiresUtcOffset = expiresUtc.Value;

                    result = apiKeyModel with { Key = newKey, ExpiresUTC = expiresUtcOffset };
                }
                else
                    result = apiKeyModel;

                var resultModel = new { Succeeded = updated, Data = result };
                return Ok(resultModel);

            }
            catch (Exception ex)
            {
                Log.Error("{CurrentAction} failed: {Message}", nameof(UpdateApiKeyEnabled), ex.Message);
                _unitOfWork.Rollback();
                //TODO: return ProblemDetails instead of BadRequest for all actions
                return BadRequest(Constants.ErrorMessages.ServerError);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("You must provide an api key id");

                if (!int.TryParse(id, out int keyId))
                    return BadRequest("Invalid api key id");

                bool deleted = await _apiKeyRepository.Delete(keyId, cancellationToken);
                _unitOfWork.Commit();
                var resultModel = new { Succeeded = true, Deleted = deleted };
                return Ok(resultModel);

            }
            catch (Exception ex)
            {
                Log.Error("{CurrentAction} failed: {Message}", nameof(Delete), ex.Message);
                _unitOfWork.Rollback();
                return BadRequest(Constants.ErrorMessages.ServerError);
            }

        }

    }
}
