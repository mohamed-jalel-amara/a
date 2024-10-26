using Newtonsoft.Json;
using System.Text;
using Branch.GRPC.Models.Response;
using Branch.GRPC.Protos;
using Branch.GRPC.Models;
using System.Linq;
using Branch.GRPC.Services.Interfaces;
using Grpc.Core;
using AutoMapper;

namespace Branch.GRPC.Services
{
    public class BranchServiceProvider : BranchProtoService.BranchProtoServiceBase
    {
        private readonly IBranchService _branchService;
        private readonly IMapper _mapper;
        private readonly ILogger<BranchServiceProvider> _logger;

        public BranchServiceProvider(IBranchService branchService, IMapper mapper, ILogger<BranchServiceProvider> logger)
        {
            _branchService = branchService ?? throw new ArgumentNullException(nameof(branchService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<BranchResponse> GetBranch(GetBranchRequest request, ServerCallContext context)
        {
            var branch = await _branchService.GetBranch(request.BranchCode);
            if (branch == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Branch with code={request.BranchCode} is not found!"));
            }
            var branchResponse = _mapper.Map<BranchResponse>(branch);
            _logger.LogInformation($"Branch is retrieved by Code {request.BranchCode}");
            return branchResponse;
        }
        public override async Task<BranchResponse> GetOneBranchByName(GetOneBranchByNameRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Retrieved Branch by Name {request.Name} ...");
            var branch = await _branchService.GetOneBranchByName(request.Name);
            if (branch == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Branch with name={request.Name} is not found!"));
            }
            var branchResponse = _mapper.Map<BranchResponse>(branch);
            _logger.LogInformation($"Branch is retrieved by Name {request.Name}");
            return branchResponse;
        }
        private async Task<BranchList> GetBranchesByFilter(string filter)
        {
            return await _branchService.GetBranchesByFilter(filter);
        }
    }
}
