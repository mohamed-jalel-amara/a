using User.grpc.Models;

namespace User.grpc.Services.Interfaces
{
    public interface IUserService
    {
        public UserModel? GetUserModelByIdAsync(string userId);
        public UserModel? GetUserModelByEmailAsync(string email);
    }
}
