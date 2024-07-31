using AngleSharp.Html;
using DPMGallery.Data;
using DPMGallery.Entities;
using DPMGallery.Extensions.Mapping;
using DPMGallery.Models;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Controllers.UI
{
    [ApiController]
    [DisableRateLimiting]
    [Route("/ui/account/organisations")]
    public class OrganisationController : ApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly OrganisationRepository _organisationRepository;
        public OrganisationController(UserManager<User> userManager, OrganisationRepository organisationRepository, ILogger log) : base(log)
        {
            _userManager = userManager;
            _organisationRepository = organisationRepository;
        }

        [HttpGet]
        [Authorize]
        [Route("/ui/account/organisations")]
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

            PagedList<UserOrganisationModel> result = userOrgs.ToPagedModel();

            if (result == null)
            {
                var additionalInformation = new Dictionary<string, object> { { "userName", userName } };
                return GetProblemResponse("Failed to get list of Organisatgions", detail: "Null list of Organisation models returned", additionalInformation: additionalInformation);
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [Route("/ui/account/check-member/{userName}")]
        //used when adding members
        public async Task<IActionResult> CheckUserOrOrExists([FromRoute] string userName, CancellationToken cancellationToken = default)
        {
            bool exists = await _organisationRepository.CheckUserExists(userName, cancellationToken);
            return Ok(new
            {
                exists
            });
        }

        [Authorize]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganisationModel model, CancellationToken cancellationToken = default)
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

                bool exists = await _organisationRepository.CheckUserExists(model.Name, cancellationToken);

                if (exists)
                    return Conflict($"An Organisation or user with the name {model.Name} already exists.");

                //organisation is just a usee

                User organisation = new User()
                {
                    UserName = model.Name,
                    NormalizedUserName = userName.ToUpper(),
                    Email = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    IsOrganisation = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return GetIdentityResultAsResponse(_userManager, nameof(CreateOrganisation), result);
                }

                OrganisationMember member = new OrganisationMember()
                {
                    MemberId = user.Id,
                    OrgId = organisation.Id,
                    Role = MemberRole.Administrator
                };

                await _organisationRepository.AddMemberToOrganisation(member, cancellationToken);

                UserOrganisation newOrg = await _organisationRepository.GetOrganisationAsync(organisation.Id, cancellationToken);

                return Ok(newOrg.ToModel());
            }
            catch (Exception ex)
            {

                var additionalInformation = new Dictionary<string, object> { { "OrganisationName", model.Name }, { "OrganisationEmail", model.Email } };
                return GetProblemResponse($"Error creating Organisation", exception: ex, additionalInformation: additionalInformation);
            }
        }
    }
}
