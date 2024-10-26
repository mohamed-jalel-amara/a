using User.grpc.Protos;

namespace Account.Access.API.Services.Grpc
{
    public class UserService
    {
        private readonly UserProtoService.UserProtoServiceClient _grpcClient;

        public UserService(UserProtoService.UserProtoServiceClient grpcClient)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
        }

        public async Task<UserResponse> GetUserAsync(string userId)
        {
            var getUserRequest = new GetUserRequest() { UserId = userId };
            return await _grpcClient.GetUserAsync(getUserRequest);
        }
    }
}
