using Bank.grpc.Models;
using Bank.grpc.Services.Interfaces;

namespace Bank.grpc.Services
{
    public class BankService : IBankService
    {
        private readonly ILogger<BankService> _logger;
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        private string endPointUrl = "";

        public BankService(ILogger<BankService> logger,
            IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            //Setting endPoint
            endPointUrl = _config.GetValue<string>("OrdsSettings:Uri");
        }
        public async Task<BankModel> GetBank(string bankId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await _client.GetAsync(endPointUrl + bankId);
                return await response.Content.ReadAsAsync<BankModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError("Getting Bank", ex.Message);
                return new BankModel() { Code = (int) response.StatusCode};
            }
        }
    }
}
