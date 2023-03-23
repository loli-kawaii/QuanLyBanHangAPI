﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuanLyBanHangAPI.Data.DTO;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QuanLyBanHangAPI.Data;
using QuanLyBanHangAPI.Services.TokenServices;

namespace QuanLyBanHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenServices _tokenServices;
        private readonly DB _dB;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration,DB dB,ITokenServices tokenServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dB = dB;
            _tokenServices = tokenServices;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            //var user = await _userManager.FindByNameAsync(loginDto.UserName);

            //if (user == null)
            //{
            //    return BadRequest("Invalid login attempt.");
            //}

            //var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, false);

            //if (!result.Succeeded)
            //{
            //    return BadRequest("Invalid login attempt.");
            //}

            //return Ok();
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {           
                var authClaims = new List<Claim>
                {
                    //new Claim(ClaimTypes.Name, user.UserName),
                    //new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.UtcNow.AddMinutes(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                // Lưu token vào db

                var tokendb = new Token
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    TokenKey = new JwtSecurityTokenHandler().WriteToken(token),
                    CreateAt = DateTime.UtcNow,
                    VaiLidTo = token.ValidTo,
                    TokenIsUsed = true,
                    TokenIsReVoked = false,
                    ReFreshToken = _tokenServices.GenerateRefreshToken(),
                    IsUsed = false,
                    IsRevoked = false
                };

                await _dB.AddAsync(tokendb);
                await _dB.SaveChangesAsync();
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult> GetUser()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
