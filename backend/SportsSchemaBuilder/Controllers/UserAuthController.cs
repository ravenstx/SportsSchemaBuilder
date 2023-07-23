using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using NuGet.Common;
using Newtonsoft.Json.Linq;
using Azure.Core;
using Microsoft.CodeAnalysis.Scripting;
using System.Diagnostics;
using SportsSchemaBuilder.Dto;
using SportsSchemaBuilder.Services;

namespace SportsSchemaBuilder.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        //private readonly UserContext _context;
        //private readonly IConfiguration _configuration;
        private readonly IUserRepository _UserRepository;
        private readonly IAuthService _authService;
        public UserAuthController(IUserRepository UserRepository, IAuthService authService)
        {
            //_context = context;
            //_configuration = configuration;
            _UserRepository = UserRepository;
            _authService = authService;
        }

        private const int refreshTokenDuration = 10;


        // GET: api/UserAuth
        [HttpGet]
        [Authorize]
        [ActionName("User")]
        public async Task<ActionResult> GetUsers()
        {
            string name = _authService.GetPayloadName(Request.Headers.Authorization);

            var res = await _UserRepository.GetUser(name);
            if(res != null) { 
            return Ok(new { name = res.Name });
            }

            return BadRequest();    
        }

       

        // LOGIN 

        [HttpPost]
        [ActionName("Login")]
        public async Task<ActionResult> Login(UserTemplate userTemplate)
        {

            var DbUser = await _UserRepository.GetUser(userTemplate.Name);

            if (DbUser == null)
            {
                return NotFound(new { message = "user not found" });
            }

                if (_authService.VerifyHashedPassword(DbUser.HashedPassword, userTemplate.Password))
                {
                    var refreshToken = _authService.createRefreshToken();
                    var DbToken = _UserRepository.GetRefreshTokens(DbUser.Id);

                    if (DbToken != null)
                    {
                        DbToken.Token = refreshToken;
                        DbToken.TokenExpiration = DateTime.Now.AddMinutes(refreshTokenDuration);

                        _UserRepository.UpdateRefreshToken(DbToken);
                        await _UserRepository.SaveChangesAsync();   

                    }
                    else
                    {

                        UserRefreshToken UserRefreshToken = new UserRefreshToken();
                        
                        UserRefreshToken.Token = refreshToken;
                        UserRefreshToken.TokenExpiration = DateTime.Now.AddMinutes(refreshTokenDuration);
                        UserRefreshToken.UserId = DbUser.Id;

                        _UserRepository.AddRefreshToken(UserRefreshToken);
                        await _UserRepository.SaveChangesAsync();

                    }

                        
                    Response.Cookies.Append("X-Refresh-Token", refreshToken, new CookieOptions() { HttpOnly = true, Expires= DateTime.Now.AddMinutes(refreshTokenDuration), SameSite=SameSiteMode.None, Secure=true});
                    List<Claim> claims = new List<Claim>{
                    new Claim("name", userTemplate.Name)
                    };
                    var token = _authService.createToken(claims);    
                    return Ok(new { Token = token });
                }
                
                return BadRequest(new { message = "password wrong" });

            
            
            
            
        }

        // POST: api/UserAuth
        [HttpPost]
        [ActionName("Register")]
        public async Task<ActionResult> Register(UserTemplate userTemplate)
        {
            
            User user = new User();
            user.Name = userTemplate.Name;
            user.HashedPassword = userTemplate.Password;
            if(await _UserRepository.GetUser(user.Name) != null)
            {
                return BadRequest(new { message = "Username not available" });
            }
                user.HashedPassword = _authService.HashPassword(user.HashedPassword);
                _UserRepository.Add(user);
            await _UserRepository.SaveChangesAsync();
            return CreatedAtAction("Register", new { Message = "account created"  });
        }
        
        [HttpPost]
        [ActionName("Refresh")]
        public async Task<ActionResult> Refresh()
        {
            string refreshToken = Request.Cookies["X-Refresh-Token"];


            string name = _authService.GetPayloadName(Request.Headers.Authorization);

            var DbUser = await _UserRepository.GetUser(name);

            var DbUserToken = _UserRepository.GetRefreshTokens(DbUser.Id);


            if (DbUserToken != null)
            {

                if(DbUserToken.Token == refreshToken)
                {

                DateTime tokendate = DbUserToken.TokenExpiration;
                
                int res = DateTime.Compare(tokendate, DateTime.Now);
                if(res > 0)
                {

                        
                        List<Claim> claims = new List<Claim>{
                    new Claim("name", DbUser.Name)
                    };
                        var token = _authService.createToken(claims);
                        return Ok(new { NewJWT = token });
                    }

               
                }
            }

            return BadRequest();   

        }


        [HttpDelete]
        [Authorize]
        [ActionName("Delete")]
        public async Task<ActionResult> Delete()
        {

            string name = _authService.GetPayloadName(Request.Headers.Authorization);
            var DbUser = await _UserRepository.GetUser(name);

            if (DbUser != null)
            { 
                _UserRepository.Remove(DbUser);
                await _UserRepository.SaveChangesAsync();
                return NoContent();

            }

            return BadRequest();    

        }



    }

}
