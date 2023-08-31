using DPMGallery.Entities;
using DPMGallery.Models;
using DPMGallery.Repositories;
using DPMGallery.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Controllers.UI
{
    [ApiController]
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
    }
}
