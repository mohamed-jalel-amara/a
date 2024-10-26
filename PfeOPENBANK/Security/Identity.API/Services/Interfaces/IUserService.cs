using Identity.API.Applications.Models.Responses;

namespace Identity.API.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserResponse> GetUserAsync(string username);
        public Task<UserResponse> GetUserByEmailAsync(string email);
        public Task<UserResponseList> GetAllUsers();
    }
}
