using Microsoft.IdentityModel.Tokens;
using NetflixUserbaseDatasetAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Razor.Parser.SyntaxTree;

namespace NetflixDatasetAPI.Authentication
{
    public static class JwtProvider
    {
        public static async Task<string> GenerateJwtAsync(User user, IConfiguration configuration)
        {
            Claim[] claims = new Claim[] 
            { 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Email,user.Email)
            };
            SigningCredentials signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!)),
                SecurityAlgorithms.HmacSha256
            );

            JwtSecurityToken token = new JwtSecurityToken(
                configuration["JwtSettings:Issuer"],
                configuration["JwtSettings:Audiance"],
                claims,
                null,
                DateTime.Now.Add(TimeSpan.Parse(configuration["JwtSettings:Lifetime"]!)),
                signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
