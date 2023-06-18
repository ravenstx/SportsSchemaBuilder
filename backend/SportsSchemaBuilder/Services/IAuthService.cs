using System.Security.Claims;

namespace SportsSchemaBuilder.Services
{
    public interface IAuthService
    {


        string GetPayloadName(string authorization);

        string createToken(List<Claim> claims);
        string createRefreshToken();

        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string password);
    }
}
