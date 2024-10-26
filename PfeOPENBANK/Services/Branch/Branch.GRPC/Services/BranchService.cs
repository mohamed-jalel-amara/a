using Branch.GRPC.Models;
using Branch.GRPC.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;
using Branch.GRPC.Models.Response;

namespace Branch.GRPC.Services
{
    public class BranchService : IBranchService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<BranchService> _logger;
        private readonly HttpClient _client;
        private string endPointUrl = "";

        public BranchService(IConfiguration configuration, ILogger<BranchService> logger, HttpClient httpClient) {

            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //Setting endPoint
            endPointUrl = _config.GetValue<string>("OrdsSettings:Uri");
        }

        public async Task<BranchModel> GetBranch(int branchCode)
        {
            var response = await _client.GetAsync(endPointUrl + branchCode);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<BranchModel>();
        }

        public async Task<BranchList> GetBranchesByFilter(string filter)
        {
            try
            {
                var filterJson = JsonConvert.DeserializeObject(filter);
                var result = await _client.GetAsync($"{endPointUrl}?q={filter}");

                return await result.Content.ReadAsAsync<BranchList>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BranchList();
            }
        }

        public async Task<BranchModel> GetOneBranchByName(string name)
        {
            string requestString = "{ \"name\":{ \"$like\":\"%" + name + "%\"} }";

            BranchList list = await GetBranchesByFilter(requestString);
            if (list != null && list.Items.Count > 0)
            {
                return list.Items.FirstOrDefault<BranchModel>();
            }
            else
            {
                return null;
            }
        }
    }
}
