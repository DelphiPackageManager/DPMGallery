using DPMGallery.Entities;
using DPMGallery.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DPMGallery.Controllers.UI
{
    [ApiController]
    [Route("/ui/account")]
    [Authorize]
    public class AccountController : Controller
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly UrlEncoder _urlEncoder;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _urlEncoder = urlEncoder;
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

                //NOTE : Cannot send redirects as axios uses XMLHttpRequest which follows redirects automatically - which
                //doesn't work for us.

                return Ok(new
                {
                    codes = recoveryCodesString,
                });

                //TODO : find a cleaner way to return the codes.
                //return Redirect($"/account/showrecoverycodes?recoverycodes={recoveryCodesString}");
            }
            else
            {
                return Ok();
            }           
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
                return  Unauthorized();
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
                AuthenticatorUri =  AuthenticatorUri,
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
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                ExternalLogins = logins.Select(login => login.ProviderDisplayName).ToList(),
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                TwoFactorClientRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            });
        }

    }
}
