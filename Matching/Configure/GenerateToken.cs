using Matching.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Matching.Configure
{
    public class GenerateToken
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public GenerateToken(ApplicationDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }
        public string GenerateApiToken(int id)
        {
            var currentUser = _context.Users.Find(id);
            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetValue<string>("Authentication:SecretKey")!));
            var SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new();
            claims.Add(new(JwtRegisteredClaimNames.Sub, currentUser!.Id.ToString()));
            claims.Add(new(JwtRegisteredClaimNames.GivenName, currentUser!.Name!));
            var token = new JwtSecurityToken(
                _config.GetValue<string>("Authentication:Issuer"),
                _config.GetValue<string>("Authentication:Audience"),
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(5),
                SigningCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
