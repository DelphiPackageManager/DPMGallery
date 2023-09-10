using DPMGallery.Entities;
using DPMGallery.Models;
using DPMGallery.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace DPMGallery.Controllers.UI
{
    [Route("ui")]
    [ApiController]
    [DisableRateLimiting]
    public class UIProfileController : ControllerBase
    {

        private readonly UserManager<User> _userManager;

        public UIProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        [HttpGet]
        [Route("profile/{userName}")]
        public async Task<IActionResult> Profile([FromRoute] string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return BadRequest(new 
                {
                    message = $"Unknown user {userName}"
                });
            }

            var hash = user.Email.ToLower().ToMd5();

            var model = new ProfileModel()
            {
                UserName = userName,
                AvatarUrl = $"https://www.gravatar.com/avatar/{hash}"
            };

            return Ok(model);
        }

    }
}
