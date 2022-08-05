using SchoolAPI.Models;
using System.Security.Claims;

namespace SchoolAPI.Repository
{
    public interface IJWTManagerRepository
    {
        Tokens Authenticate(Users users);
        Tokens Refresh(Users users);
        ClaimsPrincipal GetPrincipalFromExpiredToken(Tokens tokens);
    }
}
