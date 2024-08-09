using AngleSharp.Dom;
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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
        private readonly PackageOwnerRepository _packageOwnerRepository;
        private readonly IUnitOfWork _unitOfWork;
        public OrganisationController(UserManager<User> userManager, OrganisationRepository organisationRepository, PackageOwnerRepository packageOwnerRepository, IUnitOfWork unitOfWork, ILogger log) : base(log)
        {
            _userManager = userManager;
            _organisationRepository = organisationRepository;
            _packageOwnerRepository = packageOwnerRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize]
        [Route("/ui/account/organisation-names")]
        public async Task<IActionResult> GetUserOrganisationNamesAsync(CancellationToken cancellationToken = default)
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

            try { 

                var userOrgs = await _organisationRepository.GetOrganisationNamesForMember(user.Id, cancellationToken);
                return Ok(new
                {
                    Succeeded = true,
                    data = userOrgs.ToModels()
                });
            }
            catch (Exception ex)
            {
                var additionalInformation = new Dictionary<string, object> { { "UserId", user.Id } };
                return GetProblemResponse($"Error getting user organisation names", exception: ex, additionalInformation: additionalInformation);
            }

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

        [HttpGet]
        [AllowAnonymous]
        [Route("/ui/account/check-unique/{userName}")]
        public async Task<IActionResult> CheckUserOrOrgExists([FromRoute] string userName, CancellationToken cancellationToken = default)
        {
            bool exists = await _organisationRepository.CheckUserOrOrgExists(userName, cancellationToken);
            return Ok(new
            {
                succeeded = true,
                data = exists
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/ui/account/check-user-exists/{userName}")]
        public async Task<IActionResult> CheckUserExists([FromRoute] string userName, CancellationToken cancellationToken = default)
        {
            bool exists = await _organisationRepository.CheckUserExists(userName, cancellationToken);
            return Ok(new
            {
                succeeded = true,
                data = exists
            });
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("/ui/account/check-email-unique/{email}")]
        public async Task<IActionResult> CheckEmailInUse([FromRoute] string email, CancellationToken cancellationToken = default)
        {
            bool exists = await _organisationRepository.CheckEmailInUse(email, cancellationToken);
            return Ok(new
            {
                succeeded = true,
                data = exists
            });
        }

        private static string NewSecurityStamp()
        {
#if NETSTANDARD2_0 || NETFRAMEWORK
        byte[] bytes = new byte[20];
        _rng.GetBytes(bytes);
        return Base32.ToBase32(bytes);
#else
            return Base32.GenerateBase32();
#endif
        }

        [Authorize]
        [HttpPost]
        [Route("/ui/account/organisation/update-email")]
        public async Task<IActionResult> UpdateOrganisationEmail([FromBody] UpdateOrganisationEmailModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _organisationRepository.UpdateOrgEmailAsync(model.Id, model.Email, cancellationToken);
                _unitOfWork.Commit();
                //TODO : Send verification email - see SendVerificationEmailAsync (need to test verification works for orgs.

                var resultModel = new { Succeeded = true, model.Email };
                return Ok(resultModel);

            }
            catch (Exception ex)
            {
                var additionalInformation = new Dictionary<string, object> { { "OrganisationId", model.Id }, { "Email", model.Email } };
                return GetProblemResponse($"Error updating Organisation email", exception: ex, additionalInformation: additionalInformation);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("/ui/account/organisation/update-settings")]
        public async Task<IActionResult> UpdateOrganisationSettings([FromBody] UpdateOrganisationNotifyModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _organisationRepository.UpdateOrgSettingsAsync(model.id, model.allowContact, model.notifyOnPublish, cancellationToken);
                _unitOfWork.Commit();
                var resultModel = new { Succeeded = true };
                return Ok(resultModel);

            }
            catch (Exception ex)
            {
                var additionalInformation = new Dictionary<string, object> { { "OrganisationId", model.id } };
                return GetProblemResponse($"Error updating Organisation settings", exception: ex, additionalInformation: additionalInformation);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/ui/account/organisation/update-member")]
        public async Task<IActionResult> UpdateMemberRole([FromBody] AddUpdateOrganisationMemberModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var members = await _organisationRepository.GetMembersAsync(model.orgId, cancellationToken);
                var existing = members.FirstOrDefault(x => x.UserName == model.userName);
                if (existing == null)
                    throw new Exception($"User {model.userName} is not a member.");
                var result = await _organisationRepository.AddOrUpdateMember(existing.OrgId,existing.MemberId, model.role , cancellationToken);
                _unitOfWork.Commit();
                var resultModel = new { Succeeded = result };
                return Ok(resultModel);

            }
            catch (Exception ex)
            {
                var additionalInformation = new Dictionary<string, object> { { "OrganisationId", model.orgId } };
                return GetProblemResponse($"Error updating Organisation settings", exception: ex, additionalInformation: additionalInformation);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("/ui/account/organisation/delete-member/{orgId}/{userName}")]
        public async Task<IActionResult> DeleteOrganisationMember([FromRoute] int orgId, [FromRoute] string userName, CancellationToken cancellationToken = default)
        {
            string currentUserName = HttpContext.User.Identity?.Name;
            if (currentUserName == null)
            {
                //just return nothing
                return Unauthorized();
            }
            var currentUser = await _userManager.FindByNameAsync(currentUserName);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                var members = await _organisationRepository.GetMembersAsync(orgId, cancellationToken);
                //check current user is admin of org
                var currentMember = members.FirstOrDefault(x => x.MemberId == currentUser.Id);
                if (currentMember == null || currentMember.Role != MemberRole.Administrator)
                {
                    return Unauthorized();
                }

                var userToDelete = await _userManager.FindByNameAsync(userName);
                if (userToDelete == null)
                {
                    throw new Exception($"User {userName} is not a member.");
                }

                var existing = members.FirstOrDefault(x => x.MemberId == userToDelete.Id);
                if (existing == null)
                    throw new Exception($"User {userName} is not a member.");

                //if they are an admin then make sure there will still be other admins
                if (existing.Role == MemberRole.Administrator)
                {
                    if (!members.Any(x => x.MemberId != userToDelete.Id && x.Role == MemberRole.Administrator))
                    {
                        throw new Exception("Cannot remove last admin from organisation");
                    }
                }

                var result = await _organisationRepository.RemoveMemberFromOrganisation(orgId, userToDelete.Id, cancellationToken);
                _unitOfWork.Commit();

                var resultModel = new { Succeeded = result };
                return Ok(resultModel);
            }
            catch (Exception ex)
            {
                var additionalInformation = new Dictionary<string, object> { { "OrganisationId",   orgId }, { "MemberName", userName } };
                return GetProblemResponse($"Error removing member from organisation", exception: ex, additionalInformation: additionalInformation);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/ui/account/organisation/add-member")]
        public async Task<IActionResult> AddOrganisationMember([FromBody] AddUpdateOrganisationMemberModel model, CancellationToken cancellationToken = default)
        {
            string userName = HttpContext.User.Identity?.Name;
            if (userName == null)
            {
                //just return nothing
                return Unauthorized();
            }
            var currentUser = await _userManager.FindByNameAsync(userName);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                var members = await _organisationRepository.GetMembersAsync(model.orgId, cancellationToken);
                //check current user is admin of org
                var currentMember = members.FirstOrDefault(x => x.MemberId == currentUser.Id);
                if (currentMember == null || currentMember.Role != MemberRole.Administrator)
                {
                    return Unauthorized(); 
                }

                var existing = members.FirstOrDefault(x => x.UserName == model.userName);
                if (existing != null)
                    throw new Exception($"User {model.userName} is already a member.");

                var user = await _userManager.FindByNameAsync(model.userName);
                if (user == null)
                    throw new Exception($"User {model.userName} does not exist.");

                var result = await _organisationRepository.AddMemberToOrganisation(model.orgId, user.Id, model.role, cancellationToken);
                _unitOfWork.Commit();

                if (!result) {
                    return Ok(
                        new
                        {
                            Succeeded = false
                        });
                }

                var member = await _organisationRepository.GetMemberAsync(model.orgId, user.Id, cancellationToken);

                var memberModel = member.ToModel();


                var resultModel = new { 
                    Succeeded = result,
                    Data = memberModel
                };
                return Ok(resultModel);
            }
            catch (Exception ex)
            {
                var additionalInformation = new Dictionary<string, object> { { "OrganisationId", model.orgId } };
                return GetProblemResponse($"Error updating Organisation settings", exception: ex, additionalInformation: additionalInformation);
            }
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

                bool exists = await _organisationRepository.CheckUserOrOrgExists(model.Name, cancellationToken);

                if (exists)
                    return Conflict($"An Organisation or user with the name {model.Name} already exists.");

                //organisation is just a usee

                User organisation = new User() //orgs are users in the db
                {
                    UserName = model.Name,
                    NormalizedUserName = model.Name.ToUpper(),
                    Email = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    IsOrganisation = true,
                    SecurityStamp = NewSecurityStamp()
                };

                //this commits - which is a problem as we can end up with an orphaned org if the next step fails.
                //var result = await _userManager.CreateAsync(organisation);
                bool created = await _organisationRepository.CreateOrganisatonAsync(organisation, cancellationToken);
                if (!created)
                {
                    return BadRequest();
                }

                await _organisationRepository.AddMemberToOrganisation(organisation.Id,user.Id, MemberRole.Administrator, cancellationToken);
                _unitOfWork.Commit();

                UserOrganisation newOrg = await _organisationRepository.GetOrganisationByIdAsync(organisation.Id, cancellationToken);

                //TODO Change this server and client side to use apiResult shape
                return Ok(newOrg.ToModel());
            }
            catch (Exception ex)
            {

                var additionalInformation = new Dictionary<string, object> { { "OrganisationName", model.Name }, { "OrganisationEmail", model.Email } };
                return GetProblemResponse($"Error creating Organisation", exception: ex, additionalInformation: additionalInformation);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("/ui/account/organisation/{orgName}")]
        public async Task<IActionResult> GetOrganisationByNameAsync([FromRoute] string orgName, CancellationToken cancellationToken = default)
        {
            try
            {
                UserOrganisation Org = await _organisationRepository.GetOrganisationByNameAsync(orgName, cancellationToken);
                if (Org == null)
                    throw new Exception($"Organisation {orgName} does not exist");
                var orgModel = Org.ToModel();
                var resultModel = new { Succeeded = true, orgModel };
                return Ok(resultModel);
            }
            catch (Exception ex)
            {
                var additionalInformation = new Dictionary<string, object> { { "OrganisationName", orgName} };
                return GetProblemResponse($"Error getting Organisation", exception: ex, additionalInformation: additionalInformation);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("/ui/account/organisation/{id}")]
        public async Task<IActionResult> DeleteOrganisation([FromRoute] int orgId, CancellationToken cancellationToken = default)
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

            var isAdmin = await _organisationRepository.GetIsOrgAdminAsync(user.Id, orgId, cancellationToken);

            if (!isAdmin)
                return Unauthorized();

            try
            {
                //get all the packages the org is listed as an owner on
                var packageOwnersForOrg = await _packageOwnerRepository.GetPackageOwnersForOrgAsync(orgId);
                if (packageOwnersForOrg.Any()) //none if there are no packages owned by or.
                {
                    foreach (var packageOwner in packageOwnersForOrg)
                    {
                        //get all the owners for the package.
                        var owners = await _packageOwnerRepository.GetPackageOwnersAsync(packageOwner.PackageId);
                        var selfOwner = owners.FirstOrDefault(x => x.OwnerId == user.Id); //the user doing the delete.
                        var otherOwners = owners.Where(x => x.OwnerId != user.Id && x.OwnerId != orgId);
                        var orgOwner = owners.FirstOrDefault(x => x.OwnerId == orgId); //the organsation ownership we want to delete
                        if (orgOwner == null) //should never happen?
                            continue;
                       
                        bool result = await _packageOwnerRepository.DeletePackageOwnerAsync(orgOwner, cancellationToken);
                        if (!result) {
                            throw new InvalidOperationException($"failed to remove owner from package id: {orgOwner.PackageId}.");
                        }
                        //if there are no other owners - then add the deleting user (who is already a member of the org). 
                        if (selfOwner == null && !otherOwners.Any())
                        {
                            selfOwner = new PackageOwner()
                            {
                                PackageId = packageOwner.PackageId,
                                OwnerId = user.Id,
                            };

                            await _packageOwnerRepository.InsertAsync(selfOwner);
                        }
                    }
                }

                var deleted = await _organisationRepository.DeleteOrganisationAsync(orgId, cancellationToken);
                _unitOfWork.Commit();
                var resultModel = new { Succeeded = true, Deleted = deleted };
                return Ok(resultModel);
            }
            catch (Exception ex)
            {
                var additionalInformation = new Dictionary<string, object> { { "OrgId", orgId } };
                return GetProblemResponse($"Error deleting Organisation", exception: ex, additionalInformation: additionalInformation);
            }
        }
    }

}
        
