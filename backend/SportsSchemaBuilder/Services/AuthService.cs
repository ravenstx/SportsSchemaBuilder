using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SportsSchemaBuilder.Services
{
    public class AuthService : IAuthService
    {

        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetPayloadName(string authorization)
        {
            string tokenstring = authorization;
            tokenstring = tokenstring.Remove(0, 7);

            var jwt = new JwtSecurityToken(jwtEncodedString: tokenstring);
            string name = jwt.Claims.First(c => c.Type == "name").Value;

            return name;
        }

        public string createToken(List<Claim> claims)
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

        public string createRefreshToken()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return token;
        }

        private const int SaltByteSize = 24;
        private const int HashByteSize = 24;
        private const int HasingIterationsCount = 10101;


        public string HashPassword(string password)
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

        public bool VerifyHashedPassword(string hashedPassword, string password)
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

        
    }
}
