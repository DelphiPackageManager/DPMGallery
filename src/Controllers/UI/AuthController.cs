using DPMGallery.DTO;
using DPMGallery.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DPMGallery.Extensions;
using System.Threading;
using Microsoft.AspNetCore.Http.HttpResults;
using DPMGallery.Models.Identity;

namespace DPMGallery.Controllers.UI
{

    //for now - will move
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class ResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class TokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    [Route("ui/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private const string EmailConfirmedClaim = "EmailConfirmedClaim";

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ServerConfig _serverConfig;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, IUserStore<User> userStore, IUserEmailStore<User> emailStore,  ServerConfig serverConfig)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _serverConfig = serverConfig;
            _userStore = userStore;
            _emailStore = emailStore;
        }

        private async Task<Tuple<object,List<Claim>>> GenerateProfileObject(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            userRoles.Add("RegisteredUser");
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            authClaims.Add(new Claim(ClaimTypes.Email, user.Email));
            authClaims.Add(new Claim(EmailConfirmedClaim, user.EmailConfirmed.ToString()));

            var hash = user.Email.ToLower().ToMd5();

            var userProfile = new
            {
                email = user.Email,
                emailConfirmed = user.EmailConfirmed,
                userName = user.UserName,
                avatarUrl = $"https://www.gravatar.com/avatar/{hash}",
                roles = userRoles.ToArray()
            }; 

            return new Tuple<object, List<Claim>>(userProfile, authClaims);
        }


        [HttpPost]
        [Route("identity")]
        //[Authorize]
        public async Task<IActionResult> Identity()
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

            var result = await GenerateProfileObject(user);

            return Ok(result.Item1);
        }

        private async Task<Tuple<object, List<Claim>>> GenerateJWT(User user, bool rememberMe)
        {
            var result = await GenerateProfileObject(user);

            var token = CreateToken(result.Item2);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            DateTimeOffset refreshTokenExpires = DateTimeOffset.UtcNow.AddDays(_serverConfig.Authentication.Jwt.RefreshTokenValidityInDays);

            user.RefreshTokenExpiryTime = refreshTokenExpires;

            await _userManager.UpdateAsync(user);

            string Token = new JwtSecurityTokenHandler().WriteToken(token);

            DateTimeOffset? accessTokenExpires = null;

            if (rememberMe)
            {
#if DEBUG
                accessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(1); //TESTING Remove
#else
                    accessTokenExpires = DateTimeOffset.UtcNow.AddDays(_serverConfig.Authentication.Jwt.RefreshTokenValidityInDays + 1);
#endif
                Response.Cookies.Append("X-Remember-Me", "true", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = accessTokenExpires?.AddMinutes(2) });
            }
            Response.Cookies.Append("X-Access-Token", Token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = accessTokenExpires });

            Response.Cookies.Append("X-Refresh-Token", user.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = refreshTokenExpires });
            return result;
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            //just delete the the token cookies.
            Response.Cookies.Delete("X-Access-Token");
            Response.Cookies.Delete("X-Refresh-Token");
            Response.Cookies.Delete("X-Remember-Me");
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO requestDTO, string returnUrl = null)
        {
            var user = await _userManager.FindByNameAsync(requestDTO.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            if (user.IsOrganisation)
            {
                return BadRequest("Organisation cannot login, login as an org administrator");
            }
            var result = await _signInManager.PasswordSignInAsync(user, requestDTO.Password, requestDTO.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded) {
                var jwt = await GenerateJWT(user, requestDTO.RememberMe);
                return Ok(jwt.Item1);
            }
            if (result.RequiresTwoFactor)
            {
                return Ok(
                    new
                    {
                        requires2fa = true,
                    });
                
            }
            if (result.IsLockedOut)
            {
                return Unauthorized(new
                {
                    lockedOut = true,
                });
            }
            
            return Unauthorized("Invalid username or password.");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login-2fa")]
        public async Task<IActionResult> LoginWith2fa([FromBody] LogonWith2faRequestModel model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest("Unable to load two-factor authentication user.");
            }

            var authenticatorCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberMachine);

          if (result.Succeeded)
            {
                var jwt = await GenerateJWT(user, model.RememberMe);
                return Ok(jwt.Item1);
            }
            else if (result.IsLockedOut)
            {
                return Ok(new
                {
                    lockedOut = true
                });
            }
            else
            {
                return BadRequest("Invalid authenticator code.");
            }
        }



        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Username already in use!" });

            User user = new()
            {
                Email = model.Email,
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            string oldAccessToken = Request.Cookies.FirstOrDefault(x => x.Key == "X-Access-Token").Value;
            string refreshToken = Request.Cookies.FirstOrDefault(x => x.Key == "X-Refresh-Token").Value;
            string rememberMe = Request.Cookies.FirstOrDefault(x => x.Key == "X-Remember-Me").Value;
            if (string.IsNullOrEmpty(oldAccessToken) || string.IsNullOrEmpty(refreshToken))
                   return Unauthorized();

            var principal = GetPrincipalFromExpiredToken(oldAccessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token");
            }

            string username = principal.Identity?.Name;
            if(string.IsNullOrEmpty(username))
            {
                return BadRequest("Invalid access token");
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow)
            {
                return Unauthorized();
            }

            await GenerateJWT(user,!string.IsNullOrEmpty(rememberMe));
            //should we return profile here?
            return Ok();
        }

        /// <summary>
        /// when an external auth button is clicked on the login form we do a post to here.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("external")]
        public IActionResult External([FromForm] string provider, string returnUrl)
        {
            var redirectUrl = $"/ui/auth/external-register?returnUrl={returnUrl}";

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.AllowRefresh = true;
            return new ChallengeResult(provider, properties);
        }


        [HttpGet]
        [Route("external-register")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            //no info from the oauth server - shouldn't happen
            if (info == null)
                return Unauthorized();

            string redirectTo = String.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            //this may return null
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                //see if the user already has an external login associated with an account here.
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (result.Succeeded)
                {
                    await GenerateJWT(user, false);
                    return LocalRedirect(redirectTo);
                }
            }
            //no existing external login - but perhaps there is an account.
            if (user != null)
            {
                await _userManager.AddLoginAsync(user, info);
                //mark the user's email as confirmed since the external provider has likely already confirmed it.
                user.EmailConfirmed = true;
                await _userStore.UpdateAsync(user, CancellationToken.None);

                await _signInManager.SignInAsync(user, isPersistent: false);
                await GenerateJWT(user, false);
                return LocalRedirect(redirectTo);
            }
            //no account or externalLogin so create one
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            string userName = name.Replace(' ', '-').ToLower(); //TODO Find best option for creating username
            var newUser = new User()
            {
                Email = email,
//                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userName,
                EmailConfirmed = true //since the external provider has already confirmed it
            };
            var createResult = await _userManager.CreateAsync(newUser);
            if (createResult.Succeeded)
            {
                createResult = await _userManager.AddLoginAsync(newUser, info);
                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    await GenerateJWT(user, false);
                    return LocalRedirect(redirectTo);
                }
            }

            return LocalRedirect("/login");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            return NoContent();
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            //var aud = authClaims.FirstOrDefault(x => x.Type == "aud");
            //if (aud != null)
            //    authClaims.Remove(aud);

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_serverConfig.Authentication.Jwt.Secret));

            var token = new JwtSecurityToken(
                issuer: _serverConfig.Authentication.Jwt.ValidIssuer,
                audience: _serverConfig.Authentication.Jwt.ValidAudience,
                expires: DateTime.Now.AddMinutes(_serverConfig.Authentication.Jwt.TokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_serverConfig.Authentication.Jwt.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
    }
}
