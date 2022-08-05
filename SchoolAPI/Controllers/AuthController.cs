using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Models;
using SchoolAPI.Repository;
using System.Security.Cryptography;

namespace SchoolAPI.Controllers
{
    [ApiController]
    public class AuthController : Controller
    {
        private readonly SchoolContext _schoolContext;
        private readonly IJWTManagerRepository _repository;
        public AuthController(IJWTManagerRepository repository, SchoolContext schoolContext)
        {
            _repository = repository;
            _schoolContext = schoolContext;
        }
        [Route("api/login")]
        [HttpPost]
        public IActionResult Login(Users users)
        {
            UserRefreshTokens tokens = _schoolContext.UserRefreshTokens.Where(x => x.UserName == users.Name && x.Password == MD5.Create(users.Password).ToString()).FirstOrDefault();
            if (tokens == null)
            {
                return Unauthorized("Incorrect username or password!");
            }
            var token = _repository.Authenticate(users);
            tokens.RefreshToken = token.RefreshToken;
            _schoolContext.Entry(tokens).State = EntityState.Modified;
            _schoolContext.SaveChangesAsync();
            return Ok(token);
        }

        [Route("api/refresh")]
        [HttpPost]
        public IActionResult Refresh(Tokens tokens)
        {
            var principal = _repository.GetPrincipalFromExpiredToken(tokens);
            var userName = principal.Identity.Name;
            var saveRefreshToken = _schoolContext.UserRefreshTokens.Where(x => x.UserName == userName).FirstOrDefault();
            if (saveRefreshToken == null || saveRefreshToken.RefreshToken != tokens.RefreshToken)
            {
                return Unauthorized("Invalid attempt!");
            }
            var token = _repository.Refresh(new Users { Name = userName });
            if (token == null)
            {
                return Unauthorized("Invalid attempt!");
            }
            saveRefreshToken.RefreshToken = token.RefreshToken;
            _schoolContext.Entry(tokens).State = EntityState.Modified;
            _schoolContext.SaveChangesAsync();
            return Ok(token);
        }
    }
}
