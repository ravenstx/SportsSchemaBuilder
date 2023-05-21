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

namespace SportsSchemaBuilder.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;
        public UserAuthController(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private const int refreshTokenDuration = 10;


        // GET: api/UserAuth
        [HttpGet]
        [Authorize]
        [ActionName("User")]
        public async Task<ActionResult> GetUsers()
        {
            string tokenstring = Request.Headers.Authorization;
            tokenstring = tokenstring.Remove(0, 7);

            var jwt = new JwtSecurityToken(jwtEncodedString: tokenstring);
            string name = jwt.Claims.First(c => c.Type == "name").Value;

            //Console.WriteLine(_configuration["Jwt:Key"]);
            var res =  _context.Users.SingleOrDefault(e => e.Name == name);

            return Ok(new { name = res.Name });
        }

       

        // LOGIN 

        [HttpPost]
        [ActionName("Login")]
        public async Task<ActionResult> Login(UserTemplate userTemplate)
        {

            User user = new User();
            user.Name = userTemplate.Name;
            user.HashedPassword = userTemplate.Password;


            var DbUser = _context.Users
                        .SingleOrDefault(e => e.Name == user.Name);
            
            if (DbUser != null)
            {

                if (VerifyHashedPassword(DbUser.HashedPassword, user.HashedPassword))
                {
                    var refreshToken = createRefreshToken();
                    var DbToken = _context.RefreshTokens
                        .SingleOrDefault(e => e.UserId == DbUser.Id);

                    if (DbToken != null)
                    {
                        DbToken.Token = refreshToken;
                        DbToken.TokenExpiration = DateTime.Now.AddMinutes(refreshTokenDuration);

                        _context.RefreshTokens.Update(DbToken);
                        await _context.SaveChangesAsync();

                    }
                    else
                    {

                        UserRefreshToken UserRefreshToken = new UserRefreshToken();
                        
                        UserRefreshToken.Token = refreshToken;
                        UserRefreshToken.TokenExpiration = DateTime.Now.AddMinutes(refreshTokenDuration);
                        UserRefreshToken.UserId = DbUser.Id;

                        _context.RefreshTokens.Add(UserRefreshToken);
                        await _context.SaveChangesAsync();

                    }

                        
                    Response.Cookies.Append("X-Refresh-Token", refreshToken, new CookieOptions() { HttpOnly = true, Expires= DateTime.Now.AddMinutes(refreshTokenDuration), SameSite=SameSiteMode.None, Secure=true});
                    List<Claim> claims = new List<Claim>{
                    new Claim("name", user.Name)
                    };
                    var token = createToken(claims);    
                    return Ok(new { Token = token });
                }
                
                return BadRequest(new { message = "password wrong" });

            }
            
            return NotFound(new { message = "user not found" });  
            
        }

        // POST: api/UserAuth
        [HttpPost]
        [ActionName("Register")]
        public async Task<ActionResult> Register(UserTemplate userTemplate)
        {
            
            User user = new User();
            user.Name = userTemplate.Name;
            user.HashedPassword = userTemplate.Password;
            if (_context.Users.Any(e => e.Name == user.Name))
            {
                return BadRequest(new { message = "Username not available" });
            }
                user.HashedPassword = HashPassword(user.HashedPassword);
                _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("Register", new { Message = "account created"  });
        }
        
        [HttpPost]
        [ActionName("Refresh")]
        public async Task<ActionResult> Refresh()
        {
            string refreshToken = Request.Cookies["X-Refresh-Token"];


            string tokenstring = Request.Headers.Authorization;
            tokenstring = tokenstring.Remove(0, 7);

            var jwt = new JwtSecurityToken(jwtEncodedString: tokenstring);
            string name = jwt.Claims.First(c => c.Type == "name").Value;


            var DbUserToken = _context.Users
                        .Where(u => u.Name == name).Include(u => u.refreshTokens).FirstOrDefault();


            if (DbUserToken != null)
            {

                if(DbUserToken.refreshTokens[0].Token == refreshToken)
                {

                DateTime tokendate = DbUserToken.refreshTokens[0].TokenExpiration;
                
                int res = DateTime.Compare(tokendate, DateTime.Now);
                if(res > 0)
                {

                        
                        List<Claim> claims = new List<Claim>{
                    new Claim("name", DbUserToken.Name)
                    };
                        var token = createToken(claims);
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

            string name = GetPayloadName(Request.Headers.Authorization);
            var DbUser = _context.Users
                        .SingleOrDefault(e => e.Name == name);

            if (DbUser != null)
            { 
                _context.Users.Remove(DbUser);
                await _context.SaveChangesAsync();
                return NoContent();

            }

            return BadRequest();    

        }


        private string GetPayloadName(string authorization)
        {
            string tokenstring = authorization;
            tokenstring = tokenstring.Remove(0, 7);

            var jwt = new JwtSecurityToken(jwtEncodedString: tokenstring);
            string name = jwt.Claims.First(c => c.Type == "name").Value;

            return name;
        }


        private string createToken(List<Claim> claims)
        {
            

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                //Expires = DateTime.UtcNow.AddMinutes(10),
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds,
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"]
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private string createRefreshToken()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return token;
        }

        




        private const int SaltByteSize = 24;
        private const int HashByteSize = 24;
        private const int HasingIterationsCount = 10101;


        public static string HashPassword(string password)
        {
            // credit for hashing algorithm http://stackoverflow.com/questions/19957176/asp-net-identity-password-hashing

            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, SaltByteSize, HasingIterationsCount))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(HashByteSize);
            }
            byte[] dst = new byte[(SaltByteSize + HashByteSize) + 1];
            Buffer.BlockCopy(salt, 0, dst, 1, SaltByteSize);
            Buffer.BlockCopy(buffer2, 0, dst, SaltByteSize + 1, HashByteSize);
            return Convert.ToBase64String(dst);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] _passwordHashBytes;

            int _arrayLen = (SaltByteSize + HashByteSize) + 1;

            if (hashedPassword == null)
            {
                return false;
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            byte[] src = Convert.FromBase64String(hashedPassword);

            if ((src.Length != _arrayLen) || (src[0] != 0))
            {
                return false;
            }

            byte[] _currentSaltBytes = new byte[SaltByteSize];
            Buffer.BlockCopy(src, 1, _currentSaltBytes, 0, SaltByteSize);

            byte[] _currentHashBytes = new byte[HashByteSize];
            Buffer.BlockCopy(src, SaltByteSize + 1, _currentHashBytes, 0, HashByteSize);

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, _currentSaltBytes, HasingIterationsCount))
            {
                _passwordHashBytes = bytes.GetBytes(SaltByteSize);
            }

            return AreHashesEqual(_currentHashBytes, _passwordHashBytes);

        }

        private static bool AreHashesEqual(byte[] firstHash, byte[] secondHash)
        {
            int _minHashLength = firstHash.Length <= secondHash.Length ? firstHash.Length : secondHash.Length;
            var xor = firstHash.Length ^ secondHash.Length;
            for (int i = 0; i < _minHashLength; i++)
                xor |= firstHash[i] ^ secondHash[i];
            return 0 == xor;
        }


    public class UserTemplate
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    }

}
