using View.grpc.Models;
using View.grpc.Models.Response;
using View.grpc.Services.Interfaces;

namespace View.grpc.Services
{
    public class ViewService : IViewService
    {
        private readonly ILogger<ViewService> _logger;
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        private string endPointUrl = "";
        private string endPointUserAccessUrl = "";

        public ViewService(ILogger<ViewService> logger,
            IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            //Setting endPoint
            endPointUrl = _config.GetValue<string>("OrdsSettings:Uri");
            endPointUserAccessUrl = _config.GetValue<string>("OrdsSettings:UserAccessUri");
        }

        public Task<bool> GetIfUserHasCanAddTransactionRequestToAnyAccount(int view_id)
        {
            throw new NotImplementedException();
        }

        public async Task<ViewAccessModel> GetUserViewAsync(string provider, string provider_id, int view_id)
        {
            string filter = "?q={ \"view_id\": { \"$eq\":\""+view_id+ "\" }, \"provider\": { \"$eq\":\""+provider+ "\" }, \"provider_id\": { \"$eq\":\""+provider_id+"\" } }";
            var response = await _client.GetAsync(endPointUserAccessUrl + filter);
            try
            {
                var list =  await response.Content.ReadAsAsync<ViewAccessList>();
                if(list.Items.Count >0)
                {
                    return list.Items.First();
                }
                return new ViewAccessModel() { Id = 0 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ViewAccessModel() { Id = 0 };
            }
        }

        public async Task<ViewModel> GetViewByIdAsync(int view_id)
        {
            var response = await _client.GetAsync(endPointUrl + view_id);
            try
            {
                return await response.Content.ReadAsAsync<ViewModel>();
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ViewModel() { Id = 0 };
            }
        }

        public async Task<ViewModelList> GetViewsForAccount(string accountId)
        {
            string filter = "?q={ \"account_id\":{ \"$eq\" : \"" + accountId+"\"} }";
            
            try
            {
                var response = await _client.GetAsync(endPointUrl + filter);
                return await response.Content.ReadAsAsync<ViewModelList>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ViewModelList();
            }
        }
    }
}
