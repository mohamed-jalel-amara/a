using Helper.Utils.Interfaces;
using Helper.Utils.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Helper.Utils
{
    public class JwtUtils : IJwtUtils
    {
        private readonly JwtOptions _options;

        public JwtUtils(IOptions<JwtOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
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
