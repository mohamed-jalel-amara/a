using Branch.GRPC.Protos;

namespace Statement.API.Services.Grpc
{
    public class BranchService
    {
        private readonly BranchProtoService.BranchProtoServiceClient _grpcClient;

        public BranchService(BranchProtoService.BranchProtoServiceClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<BranchResponse> GetBranchAsync(int branchCode)
        {
            var branchRequest = new GetBranchRequest { BranchCode = branchCode };
            return await _grpcClient.GetBranchAsync(branchRequest);
        }
    }
}
