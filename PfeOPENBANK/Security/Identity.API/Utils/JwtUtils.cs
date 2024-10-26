using Identity.API.Applications.Dtos;
using Identity.API.Applications.Models.Entities;
using Identity.API.Utils.Interfaces;
using Identity.API.Utils.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.API.Utils.Models;

namespace Identity.API.Utils
{
    public class JwtUtils : IJwtUtils
    {
        private readonly JwtOptions _options;
        private readonly UserManager<UserModel> _userManager;

        public JwtUtils(IOptions<JwtOptions> options, UserManager<UserModel> userManager)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

        }
        public async Task<AuthResponse> GetToken(UserModel user)
        {
            var userRoles = await this._userManager.GetRolesAsync(user);
            var signingKey = new SymmetricSecurityKey
                             (Encoding.UTF8.GetBytes(_options.Secret));
            var signingCredentials = new SigningCredentials
                                     (signingKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
               new Claim("userId", user.Id),
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("Roles", userRole));
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var expirationDate = DateTime.Now.AddMinutes(_options.ExpiryMinutes);
            var jwt = new JwtSecurityToken(claims: claims,
                      signingCredentials: signingCredentials, expires: expirationDate);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new AuthResponse() { Message = "Connected!", Token = encodedJwt};
        }
        public async Task<string> GetNotAuthenticatedToken(UserModel user)
        {
            var signingKey = new SymmetricSecurityKey
                             (Encoding.UTF8.GetBytes(_options.Secret));
            var signingCredentials = new SigningCredentials
                                     (signingKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),

            };
            var expirationDate = DateTime.Now.AddMinutes(_options.ExpiryMinutes);
            var jwt = new JwtSecurityToken(claims: claims,
                      signingCredentials: signingCredentials, expires: expirationDate);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        public LoggedUser GetLoggedUser(string token)
        {
            var principal = GetPrincipal(token);
            if (principal == null)
            {
                return null;
            }

            ClaimsIdentity identity;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            var userIdClaim = identity?.FindFirst("userId");
            var userRoleClaim = identity?.FindAll("Roles");
            if (userIdClaim == null)
            {
                return null;
            }
            var userId = userIdClaim.Value;
            if (userRoleClaim == null)
            {
                return new LoggedUser() { userId = userId };
            }
            IList<string> allRoles = new List<string>();
            foreach (var item in userRoleClaim)
            {
                allRoles.Add(item.Value);
            }

            return new LoggedUser() { userId = userId, userRoles = String.Join(",", allRoles) };
        }
        public string ValidateToken(string token)
        {
            var principal = GetPrincipal(token);
            if (principal == null)
            {
                return string.Empty;
            }

            ClaimsIdentity identity;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return string.Empty;
            }
            var userIdClaim = identity?.FindFirst("userId");
            if (userIdClaim == null)
            {
                return string.Empty;
            }
            var userId = userIdClaim.Value;
            return userId;
        }

        private ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                {
                    return null;
                }
                var key = Encoding.UTF8.GetBytes(_options.Secret);
                var parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                IdentityModelEventSource.ShowPII = true;
                ClaimsPrincipal principal =
                      tokenHandler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
