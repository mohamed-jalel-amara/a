using AutoMapper;
using Bank.grpc.Protos;
using Bank.grpc.Services.Interfaces;
using Grpc.Core;

namespace Bank.grpc.Services
{
    public class BankServiceProvider : BankProtoService.BankProtoServiceBase
    {
        private readonly IBankService _service;
        private readonly ILogger<BankServiceProvider> _logger;
        private readonly IMapper _mapper;

        public BankServiceProvider(IBankService service, ILogger<BankServiceProvider> logger, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<BankObject> GetBank(GetBankRequest request, ServerCallContext context)
        {
            var bank = await _service.GetBank(request.Id);
            if (bank.Code > 0)
            {
                _logger.LogInformation($"Failed to retrieve Bank by id {request.Id}");
                return new BankObject();
            }
            else
            {
                _logger.LogInformation($"Bank is retrieved by id {request.Id}");
                return _mapper.Map<BankObject>(bank);
            }

        }
    }
}
