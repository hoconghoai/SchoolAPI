using Microsoft.IdentityModel.Tokens;
using SchoolAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SchoolAPI.Repository
{
    public class JWTManagerRepository : IJWTManagerRepository
    {
		private readonly IConfiguration _configuration;
		public JWTManagerRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}

        public Tokens Authenticate(Users users)
        {
			return GenerateToken(users);
		}

        public Tokens Refresh(Users users)
        {
			return GenerateToken(users);
		}

		public ClaimsPrincipal GetPrincipalFromExpiredToken(Tokens tokens)
		{
			var Key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Key),
				ClockSkew = TimeSpan.Zero
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(tokens.Token, tokenValidationParameters, out SecurityToken securityToken);
			JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}

		private Tokens GenerateToken(Users users)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, users.Name)
				}),
				Expires = DateTime.UtcNow.AddMinutes(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return new Tokens { Token = tokenHandler.WriteToken(token), RefreshToken = GenerateRefreshToken() };
		}

		private string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{ 
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}
    }
}
