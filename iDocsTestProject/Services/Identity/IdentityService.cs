using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace iDocsTestProject.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        public string GenerateJwtToken(IdentityUser user, string secret)
        {
            var jwtTokenHanlder = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("userId", user.Id),
                    new Claim("userName", user.UserName)
                }),

                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHanlder.CreateToken(tokenDescriptor);
            return jwtTokenHanlder.WriteToken(token);
        }
    }
}
