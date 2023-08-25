using Amazon.Runtime.Internal;
using DPMGallery.Entities;
using DPMGallery.Identity;
using DPMGallery.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using NuGet.Protocol.Plugins;
using Org.BouncyCastle.Utilities;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace DPMGallery.Controllers.UI
{
    public class RemoveLoginModel
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }

    public class ChangeEmailModel
    {
        public string NewEmail { get; set; }
    }

    public class ConfirmEmailChangeModel
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string UserId { get; set; }
    }

    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class SetPasswordModel
    {
        public string NewPassword { get; set; }
    }

    [ApiController]
    [Route("/ui/account")]
    [Authorize]
    public class AccountController : Controller
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly UrlEncoder _urlEncoder;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserStore<User> _userStore;
        private readonly IEmailSender _emailSender;
        private readonly ServerConfig _serverConfig;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, UrlEncoder urlEncoder, IUserStore<User> userStore, IEmailSender emailSender, ServerConfig serverConfig)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _urlEncoder = urlEncoder;
            _userStore = userStore;
            _emailSender = emailSender;
            _serverConfig = serverConfig;
        }

        [HttpPost]
        [Route("2fa-generatecodes")]
        public async Task<IActionResult> GenerateRecoveryCodes()
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
                return NotFound($"Unable to load user with ID '{user.Id}'.");
            }
            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException($"Cannot generate recovery codes for user as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var recoveryCodesString = String.Join(",", recoveryCodes.ToArray());
            return Ok(new
            {
                codes = recoveryCodesString,
            });

        }


        [HttpPost]
        [Route("2fa-verify")]
        public async Task<IActionResult> Verify2faCode([FromBody] Verify2faCodeModel code)
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
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (string.IsNullOrEmpty(code.Code))
            {
                return BadRequest("Code cannot be empty");
            }

            // Strip spaces and hyphens
            var verificationCode = code.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                return BadRequest("Verification code is invalid.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

                var recoveryCodesString = String.Join(",", recoveryCodes.ToArray());

                return Ok(new
                {
                    codes = recoveryCodesString,
                });

            }
            else
            {
                return Ok();
            }
        }

        [HttpPost]
        [Route("2fa-forget")]
        public async Task<IActionResult> Forget2faClient()
        {
            string userName = HttpContext.User.Identity?.Name;
            if (userName == null)
            {
                //just return nothing
                return Unauthorized();
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) //shouldn't happne
            {
                return BadRequest();
            }
            await _signInManager.ForgetTwoFactorClientAsync();
            user = await _userManager.FindByNameAsync(userName); //need to refresh the user for the values below to be correct
            var model = new TwoFactorConfigModel()
            {
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                IsMachineRemembered = false, // this seems to lag behind await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };
            return Ok(model);
        }

        [HttpPost]
        [Route("2fa-disable")]
        public async Task<IActionResult> DisableAuthenticator()
        {
            string userName = HttpContext.User.Identity?.Name;
            if (userName == null)
            {
                //just return nothing
                return Unauthorized();
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) //shouldn't happne
            {
                return BadRequest();
            }
            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost]
        [Route("2fa-reset")]
        public async Task<IActionResult> ResetAuthenticator()
        {
            string userName = HttpContext.User.Identity?.Name;
            if (userName == null)
            {
                //just return nothing
                return Unauthorized();
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) //shouldn't happne
            {
                return BadRequest();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);

            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }


        [HttpGet]
        [Route("2fa-config")]
        public async Task<IActionResult> GetTwoFactorConfig()
        {
            string userName = HttpContext.User.Identity?.Name;
            if (userName == null)
            {
                //just return nothing
                return Unauthorized();
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) //shouldn't happen
            {
                return BadRequest();
            }

            var model = new TwoFactorConfigModel()
            {
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };
            return Ok(model);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                AuthenticatorUriFormat,
                _urlEncoder.Encode("DPM Gallery"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        [HttpGet]
        [Route("2fa-keyinfo")]
        public async Task<IActionResult> GetTwoFactorKeyInfo()
        {
            string userName = HttpContext.User.Identity?.Name;
            if (userName == null)
            {
                //just return nothing
                return Unauthorized();
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) //shouldn't happne
            {
                return BadRequest();
            }

            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var SharedKey = FormatKey(unformattedKey);

            var email = await _userManager.GetEmailAsync(user);
            var AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
            var model = new AuthenticatorDetailsModel()
            {
                AuthenticatorUri = AuthenticatorUri,
                SharedKey = SharedKey
            };
            return Ok(model);
        }

        [HttpGet]
        [Route("details")]
        public async Task<IActionResult> GetAccountDetails()
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
                return BadRequest();
            }

            var logins = await _userManager.GetLoginsAsync(user);


            return Ok(new
            {
                Username = user.UserName,
                user.Email,
                user.EmailConfirmed,
                ExternalLogins = logins.Select(login => login.ProviderDisplayName).ToList(),
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                TwoFactorClientRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            });
        }

        [HttpGet]
        [Route("external-logins")]
        public async Task<IActionResult> GetExternalLoginInfo()
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
                return BadRequest();
            }

            var logins = await GetLogins(user);

            return Ok(logins);
        }

        private async Task<dynamic> GetLogins(User user)
        {
            var currentLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();


            string passwordHash = user.PasswordHash;

            //only show remove buttons if the user has set a password on the account or there are > 1 external logins
            var showRemoveButton = passwordHash != null || currentLogins.Count > 1;

            return new
            {
                currentLogins = currentLogins.Select(x => new ExternalLoginModel(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToArray(),
                otherLogins = otherLogins.Select(x => new AuthenticationSchemeModel(x.Name, x.DisplayName)).ToArray(),
                showRemoveButton
            };
        }


        [HttpPost]
        [Route("remove-login")]
        public async Task<IActionResult> RemoveLoginAsync([FromBody] RemoveLoginModel model)
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
                return NotFound();
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                return Ok();
            }

            await _signInManager.RefreshSignInAsync(user);
            var logins = await GetLogins(user);
            return Ok(logins);

        }

        [HttpPost]
        [Route("link-login")]
        public async Task<IActionResult> LinkLoginAsync([FromForm] string provider)
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
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = $"/ui/auth/external-callback?returnUrl=/account/externallogins";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, user.Id.ToString());
            properties.AllowRefresh = true;
            return new ChallengeResult(provider, properties);
        }

        [HttpPost]
        [Route("send-verify-email")]
        public async Task<IActionResult> SendVerificationEmailAsync()
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

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var qparams = HttpUtility.ParseQueryString(string.Empty);
            qparams["userId"] = userId;
            qparams["code"] = code;
            var callbackUrl = _serverConfig.SiteBaseUrl + "/confirmemail?" + qparams.ToString();

            try
            {

                await _emailSender.SendEmailAsync(
                    email,
                    "DPM Gallery - Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }
            catch (Exception ex)
            {
                return StatusCode(503, "Unabled to send email : " + ex.Message);
            }
            return Ok("Verification email sent. Please check your email.");
        }

        [HttpPost]
        [Route("change-email")]
        public async Task<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailModel model)
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

            if (model.NewEmail != user.Email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var qparams = HttpUtility.ParseQueryString(string.Empty);
                qparams["userId"] = userId;
                qparams["email"] = model.NewEmail;
                qparams["code"] = code;
                var callbackUrl = _serverConfig.SiteBaseUrl + "/account/confirmemailchange?" + qparams.ToString();
                try
                {
                    await _emailSender.SendEmailAsync(
                        model.NewEmail,
                        "DPM Gallery - Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
                return Ok("Confirmation link to change email sent. Please check your email.");
            }


            return Ok("Your email is unchanged.");
        }

        [HttpPost]
        [Route("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChangeAsync([FromBody] ConfirmEmailChangeModel model)
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
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ChangeEmailAsync(user, model.Email, code);
            if (!result.Succeeded)
            {
                BadRequest("Error changing email.");
            }
            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        [HttpGet]
        [Route("haspassword")]
        public async Task<IActionResult> GetHasPasswordAsync()
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
                return NotFound($"Unable to load user '{userName}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordModel model)
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
                return NotFound($"Unable to load user '{userName}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                string error = changePasswordResult.Errors.FirstOrDefault()?.Description;
                return BadRequest(error);
            }
            await _signInManager.RefreshSignInAsync(user);

            return Ok("Your password has been changed.");

        }


        [HttpPost]
        [Route("set-password")]
        public async Task<IActionResult> SetPasswordAsync([FromBody] SetPasswordModel model)
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
                return NotFound($"Unable to load user '{userName}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                string error = addPasswordResult.Errors.FirstOrDefault()?.Description;
                return BadRequest(error);
            }

            await _signInManager.RefreshSignInAsync(user);
            return Ok("Your password has been set.");


        }
    }
}
