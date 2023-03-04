﻿using DPMGallery.DTO;
using DPMGallery.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FluentMigrator.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Markdig.Helpers;
using DPMGallery.Extensions;
using System.Security.Policy;
using NuGet.Common;

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
        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, ServerConfig serverConfig)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _serverConfig = serverConfig;
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
        [Route("profile")]
        //[Authorize]
        public async Task<IActionResult> Profile()
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



        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO requestDTO)
        {
            var user = await _userManager.FindByNameAsync(requestDTO.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, requestDTO.Password))
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

                if (requestDTO.RememberMe)
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
                return Ok(result.Item1);
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
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

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            var refreshTokenExpires = DateTimeOffset.UtcNow.AddDays(_serverConfig.Authentication.Jwt.RefreshTokenValidityInDays);
            user.RefreshTokenExpiryTime = refreshTokenExpires;

            await _userManager.UpdateAsync(user);



            DateTimeOffset? accessTokenExpires = null;
            if (!string.IsNullOrEmpty(rememberMe)) //hacky way to set rememberme
            {

#if DEBUG
                accessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(1); //TESTING Remove
#else
                accessTokenExpires = DateTimeOffset.UtcNow.AddDays(_serverConfig.Authentication.Jwt.RefreshTokenValidityInDays + 1);
#endif
                Response.Cookies.Append("X-Remember-Me", "true", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = accessTokenExpires?.AddMinutes(2) });
            }

            var token = new JwtSecurityTokenHandler().WriteToken(newAccessToken);
            Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = accessTokenExpires });
            Response.Cookies.Append("X-Refresh-Token", user.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = refreshTokenExpires });

            return Ok();
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