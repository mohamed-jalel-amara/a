using User.grpc.Protos;

namespace Transaction.API.Services.Grpc
{
    public class UserService 
    {
        private readonly UserProtoService.UserProtoServiceClient _grpcClient;

        public UserService(UserProtoService.UserProtoServiceClient grpcClient)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
        }

        public async Task<UserResponse> GetUserDataAsync(string userId)
        {
            var userRequest = new GetUserRequest() { UserId = userId };
            return await _grpcClient.GetUserAsync(userRequest);
        }
    }
}
