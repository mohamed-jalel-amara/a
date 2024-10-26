using Bank.api.Models;
using Bank.api.Models.Requests;
using Bank.api.Models.Responses;
using Bank.api.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace Bank.api.Services
{
    public class BankServices : IBankServices
    {
        private readonly ILogger<BankServices> _logger;
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        private string endPointUrl = "";

        public BankServices(ILogger<BankServices> logger,
            IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            //Setting endPoint
            endPointUrl = $"/ords/{_config.GetValue<string>("OracleSettings:DatabaseUser")}" +
                $"/{_config.GetValue<string>("OracleSettings:DatabaseTableName")}/";
        }
        public async Task<BankResponse> CreateBankAsync(BankRequest request)
        {
            BankModel model = new BankModel() { address = request.Bank_routing.Address, scheme = request.Bank_routing.Scheme,
            Full_name = request.Full_name, Short_name = request.Short_name, Logo = request.Logo, Id = request.Id, Website = request.Website};

            model.Created = DateTime.Now;
            model.Updated = DateTime.Now;

            var bankPost = JsonConvert.SerializeObject(model);

            
            try
            {
                var response = await _client.PostAsync(endPointUrl, new StringContent(bankPost, Encoding.UTF8, "application/json"));
                return GetBankResponseFromModel(await response.Content.ReadAsAsync<BankModel>());
            }
            catch (Exception ex)
            {
                _logger.LogError("Creating bank", ex.Message);
                return new BankResponse()
                {
                    Id = request.Id,
                    Bank_routing = request.Bank_routing,
                    Full_name = request.Full_name,
                    Logo = request.Logo,
                    Short_name = request.Short_name,
                    Website = request.Website
                };
                return new BankResponse() { Code = 500, ErrorMessage = "OBP-50000: Unknown Error." };
            }
        }

        public async Task<bool> DeleteBankAsync(string id)
        {
            var response = await _client.DeleteAsync(endPointUrl + id);
            try
            {
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Deleting Bank", ex.Message);
                return false;
            }
        }

        public  List<BankResponse> GetTunisianBanks()
        {
            return new List<BankResponse>
        {
                new BankResponse
            {
                Id = "STB.1",
                Short_name = "STB",
                Full_name = "Société Tunisienne de Banque",
                Logo = "https://www.stb.com.tn/sites/default/files/styles/logo_stb/public/logo_stb.png",
                Website = "https://www.stb.com.tn/",
                Bank_routing = new BankRouting{
                Scheme = "STB",
                Address = "..." }
            },
            new BankResponse
            {
                Id = "BT.2",
                Short_name = "BT",
                Full_name = "Banque de Tunisie",
                Logo = "https://www.bt.com.tn/images/logos/logo_bt_horizontal.png",
                Website = "https://www.bt.com.tn/",
                Bank_routing = new BankRouting{
                Scheme = "BT",
                Address = "Avenue Habib Bourguiba, Tunis, Tunisia" }
            },
            
            new BankResponse
            {
                Id = "BIAT.3",
                Short_name = "BIAT",
                Full_name = "Banque Internationale Arabe de Tunisie",
                Logo = "https://www.biat.com.tn/images/logo.png",
                Website = "https://www.biat.com.tn/",
                Bank_routing = new BankRouting{
                Scheme = "BIAT",
                Address = "..." }
            },

        };
        }
        public async Task<BankResponseList> GetAllBankAsync()
        {
            
            try
            {
                var response = await _client.GetAsync(endPointUrl);
                BankList list = await response.Content.ReadAsAsync<BankList>();
                if (list.Items.Count > 0)
                {
                    BankResponseList responseList = new BankResponseList()
                    {
                        Count = list.Count,
                        HasMore = list.HasMore,
                        Limit = list.Limit,
                        Offset = list.Offset
                    };
                    foreach (var item in list.Items)
                    {
                        responseList.Banks.Add( GetBankResponseFromModel(item) );
                    }
                    return responseList;
                }
                return new BankResponseList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Getting all Banks", ex.Message);
                return new BankResponseList(
                    )
                {
                    Banks = GetTunisianBanks()
                };
            }
        }

        public async Task<BankResponse> GetBankAsync(string id)
        {
            
            try
            {
                var response = await _client.GetAsync(endPointUrl + id);
                return GetBankResponseFromModel(await response.Content.ReadAsAsync<BankModel>());
            }catch (Exception ex)
            {
                _logger.LogError("Getting Bank", ex.Message);
                var bk = GetTunisianBanks().FirstOrDefault(x => x.Id == id);
                if (bk != null) return bk;
                return new BankResponse() { Code =500 /*(int)response.StatusCode*/, ErrorMessage = "OBP-50000: Unknown Error." };
            }
        }
        #region private méthodes
        private BankResponse GetBankResponseFromModel(BankModel model)
        {
            if (model == null)
            {
                return new BankResponse() { Code = 404, ErrorMessage = "OBP-30001: Bank not found. Please specify a valid value for BANK_ID." };
            }
            BankResponse response = new BankResponse() { Id = model.Id, Full_name = model.Full_name, Logo = model.Logo,
            Short_name = model.Short_name, Website = model.Website };
            BankRouting routing = new BankRouting() { Address = model.address, Scheme = model.scheme };
            response.Bank_routing = routing;
            return response;
        }
        #endregion
    }
}
