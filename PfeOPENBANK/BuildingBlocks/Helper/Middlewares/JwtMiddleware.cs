using Helper.Utils.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace Helper.Middlewares
{
    public class JwtMiddleware : IMiddleware
    {
        private readonly IJwtUtils jwtUtils;

        public JwtMiddleware(IJwtUtils jwtUtils)
        {
            this.jwtUtils = jwtUtils ?? throw new ArgumentNullException(nameof(jwtUtils));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Get the token from the Authorization header
            var bearer = context.Request.Headers["Authorization"].ToString();
            var token = bearer.Replace("Bearer ", string.Empty);

            if (!string.IsNullOrEmpty(token))
            {
                // Verify the token using the IJwtBuilder
                var loggedUser = jwtUtils.GetLoggedUser(token);

                if (loggedUser != null)
                {
                    // Store the userId in the HttpContext items for later use
                    context.Items["userId"] = loggedUser.userId;
                    context.Items["userRoles"] = loggedUser.userRoles;
                }
                else
                {
                    // If token or userId are invalid, send 401 Unauthorized status
                    context.Response.StatusCode = 401;
                }
            }

            // Continue processing the request
            await next(context);
        }
    }
}
