using Identity.API.Applications.Dtos;
using Identity.API.Applications.Models.Entities;
using Identity.API.Utils.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Utils.Interfaces
{
    public interface IJwtUtils
    {
        LoggedUser GetLoggedUser(string token);
        Task<AuthResponse> GetToken(UserModel user);
        Task<string> GetNotAuthenticatedToken(UserModel user);
        string ValidateToken(string token);
    }
}
