using DPMGallery.Data;
using DPMGallery.Entities;
using DPMGallery.Models;
using DPMGallery.Repositories;
using DPMGallery.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Controllers.UI
{
    [ApiController]
    public class ApiKeyController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApiKeyRepository _apiKeyRepository;

        public ApiKeyController(UserManager<User> userManager, ApiKeyRepository apiKeyRepository)
        {
            _userManager = userManager;
            _apiKeyRepository = apiKeyRepository;
        }

        [Authorize]
        [HttpGet]
        [Route("/ui/account/apikeys")]
        public async Task<IActionResult> GetApiKeys(CancellationToken cancellationToken = default)
        {
            string userName = HttpContext.User.Identity?.Name;
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
            
            var apiKeys = await _apiKeyRepository.GetApiKeysForUser(user.Id, cancellationToken);

            var result = Mapping<ApiKey, ApiKeyModel>.Map(apiKeys);

            return Ok(result);
        }
    }
}
