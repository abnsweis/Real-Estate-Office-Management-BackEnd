using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Security;
using RealEstate.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace RealEstate.Infrastructure.Identity.jwt
{
    public class JwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtTokenGenerator(IOptions<JwtSettings> JwtSettings,UserManager<ApplicationUser> userManager)
        {
            _jwtSettings = JwtSettings.Value;
            this._userManager = userManager;
        }


        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),  
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
                  
            };


            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }



            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


         
    }
}
