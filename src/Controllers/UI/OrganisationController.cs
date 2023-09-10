using DPMGallery.Entities;
using DPMGallery.Models;
using DPMGallery.Repositories;
using DPMGallery.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Controllers.UI
{
    [ApiController]
    [DisableRateLimiting]
    public class OrganisationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly OrganisationRepository _organisationRepository;
        public OrganisationController(UserManager<User> userManager, OrganisationRepository organisationRepository)
        {
            _userManager = userManager;
            _organisationRepository = organisationRepository;
        }

        [HttpGet]
        [Authorize]
        [Route("/ui/account/user-organisations")]
        public async Task<IActionResult> GetUserOrganisationsAsync(CancellationToken cancellationToken = default)
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

            var userOrgs = await _organisationRepository.GetOrganisationsForMember(user.Id, cancellationToken);

            return Ok(Mapping<UserOrganisation, UserOrganisationModel>.Map(userOrgs));
        }

        [HttpPost]
        [Authorize]
        [Route("/ui/account/check-member/{userName}")]
        public async Task<IActionResult> GetUserOrganisationsAsync([FromRoute] string userName, CancellationToken cancellationToken = default)
        {
            bool exists = await _organisationRepository.CheckUserExists(userName, cancellationToken);
            if (!exists)
            {
                return NotFound();
            } else
            {
                return Ok();
            }
        }
    }
}
