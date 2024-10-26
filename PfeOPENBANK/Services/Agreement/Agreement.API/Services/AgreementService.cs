using Agreement.API.Models;
using Agreement.API.Models.Requests;
using Agreement.API.Models.Responses;
using Agreement.API.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Agreement.API.Services
{
    public class AgreementService : IAgreementService
    {
        private readonly HttpClient _client;
        private readonly ILogger<AgreementService> _logger;
        private readonly IConfiguration _config;
        private string endPointUrl = "";

        public AgreementService(HttpClient client, ILogger<AgreementService> logger, IConfiguration config)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config;
            endPointUrl = $"/ords/{_config.GetValue<string>("OracleSettings:DatabaseUser")}" +
                $"/{_config.GetValue<string>("OracleSettings:DatabaseTableName")}/";
        }
        public async Task<bool> EditAccessAsync(AccessRequest request, string userId)
        {
            AccountAccess access = await GetOneAccessAsync(request.AccessId);
            if (access != null && access.CUSTOMERID.Equals(userId))
            {
                access.STATUS = request.Status.ToString();
                access.DURATION = request.Duration;
                access.UPDATEDAT = DateTime.UtcNow;
                var accessJson = JsonConvert.SerializeObject(access);
                var response = await _client.PutAsync(endPointUrl + access.ID, new StringContent(accessJson, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                _logger.LogInformation($"An account access has been up to date! customer : {userId}, provider : {access.PROVIDERID}");
                return true;
            }
            return false;
        }

        public async Task<AccountAccessList> GetAccessListByAccountAsync(string userId, long account_number)
        {
            string filter = "?q={ \"customerid\" : { \"$eq\": \"" + userId + "\" }, \"accnumber\": { \"$eq\": \"" + account_number + "\" } }";
            var response = await _client.GetAsync(endPointUrl + filter);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AccountAccessList>();
        }

        public async Task<AccountAccessList> GetAccessListByProviderAsync(string userId, string providerId)
        {
            string filter = "?q={ \"customerid\" : { \"$eq\": \"" + userId + "\" }, \"providerid\": { \"$eq\": \""+ providerId+"\" } }";
            var response = await _client.GetAsync(endPointUrl + filter);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AccountAccessList>();
        }

        public async Task<AccountAccessList> GetAllAccessListAsync(string userId)
        {
            string filter = "?q={ \"customerid\" : { \"$eq\": \"" + userId + "\" } }";
            var response = await _client.GetAsync(endPointUrl + filter);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AccountAccessList>();
        }

        public async Task<AccountAccessList> GetAllAccessListAsync()
        {
            var response = await _client.GetAsync(endPointUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AccountAccessList>();
        }

        public async Task<AccountAccess> GetOneAccessAsync(int accessId)
        {
            var response = await _client.GetAsync(endPointUrl + accessId);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AccountAccess>();
        }

        public async Task<bool> UpdateAccessAsync(AccountAccess access)
        {
                var accessJson = JsonConvert.SerializeObject(access);
                var response = await _client.PutAsync(endPointUrl + access.ID, new StringContent(accessJson, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                _logger.LogInformation($"An account access has been up to date! customer : {access.CUSTOMERID}, provider : {access.PROVIDERID}");
                return true;
        }
    }
}
