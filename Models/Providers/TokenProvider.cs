using System.Text;
using Microsoft.Extensions.Configuration;
using Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace Models.Providers
{
    public class TokenProvider
    {
        private readonly IConfiguration _configuration;
        public TokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetAccessToken(TokenData tokenData)
        {
           var key=_configuration["Jwt:Key"];
           var issuer=_configuration["Jwt:Issuer"];
           var audience=_configuration["Jwt:Audience"];
           var signingCredentials=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)); 
           var cred=new SigningCredentials(signingCredentials, SecurityAlgorithms.HmacSha256);
           var claims=new List<Claim>
           {
               new Claim(ClaimTypes.NameIdentifier,tokenData.UserId),
               new Claim(ClaimTypes.Email,tokenData.Email),
           };
           foreach(var role in tokenData.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }
           var token=new JwtSecurityToken(
            issuer:issuer,
            audience:audience,
            expires:DateTime.Now.AddMinutes(30),
            signingCredentials:cred,
            claims:claims
           );
           return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}