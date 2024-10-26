using Account.Access.API.Models;
using Account.Access.API.Models.Requests;
using Account.Access.API.Models.Responses;
using Account.Access.API.Services.Interfaces;
using AutoMapper;
using EventBus.Message.Events;
using MassTransit;
using Newtonsoft.Json;
using System.Text;

namespace Account.Access.API.Services
{
    public class AccountAccessService : IAccountAccessService
    {
        private readonly HttpClient _client;
        private readonly ILogger<AccountAccessService> _logger;
        private readonly IConfiguration _config;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;
        private IList<string> _allowedActions = new List<string>();

        private string endPointUrl = "";
        private string endPointUrlUserAccess = "";

        public AccountAccessService(HttpClient client, ILogger<AccountAccessService> logger, IMapper mapper,
         IConfiguration config, IPublishEndpoint publishEndpoint)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _config = config;
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            endPointUrl = $"/ords/{_config.GetValue<string>("OracleSettings:DatabaseUser")}" +
                $"/{_config.GetValue<string>("OracleSettings:DatabaseTableName")}/";
            endPointUrlUserAccess = $"/ords/{_config.GetValue<string>("OracleSettings:DatabaseUser")}" +
                $"/{_config.GetValue<string>("OracleSettings:DatabaseAccUSAccessTableName")}/";

            _allowedActions = _config.GetValue<List<string>>("AllowedViewActions");
        }

        public async Task<AccountModel> GetAccountAsync(string providerId, long accountNumber)
        {
            _logger.LogError($"Retrieving account failed providerid ={providerId} account number = {accountNumber}");
            return null;
        }

        public async Task<AccountAccessList> GetAllAccessAsync(string providerId)
        {
            return await getAllAccess(providerId);
        }

        public async Task<AccountAccessResponse> GetOneAccessAsync(int view_id)
        {
            var response = await _client.GetAsync(endPointUrl + view_id);

            try
            {
               return await response.Content.ReadAsAsync<AccountAccessResponse>();
            }catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        private async Task<AccountAccessList> getAllAccess(string providerId)
        {
            string filter = "{ \"providerid\": { \"$eq\": \""+ providerId +"\" } }";
            var response = await _client.GetAsync(endPointUrl + "?q=" + filter);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AccountAccessList>();
        }

        public async Task<AccountAccessResponse> CreateView(string Account_id, string bank_id, AddViewRequest request, string ownerId)
        {
            AccountAccess model = new AccountAccess();
            model.Account_id = Account_id;
            model.Bank_id = bank_id;
            model.Short_name = request.Name;
            model.Updated_at = DateTime.Now;
            model.Created_at = DateTime.Now;
            model.Alias = request.Alias.Length == 0 ? "NONE" : request.Alias.ToUpper();
            model.Which_alias_to_use = request.Which_alias_to_use;
            model.Description = request.Description;
            model.Is_Public = model.Alias == "PUBLIC"? request.Is_public : false;
            model.Hide_metadata_if_alias_used = request.Hide_metadata_if_alias_used;
            model.Owner_id = ownerId;

            model = GetAccountAccessWithAllowedActionsMetaData(model, request.Allowed_actions);

            var accessJson = JsonConvert.SerializeObject(model);
            var result = await _client.PostAsync(endPointUrl, new StringContent(accessJson, Encoding.UTF8, "application/json"));

            try
            {
                await _publishEndpoint.Publish(new GrantEntitlementEvent() { BankId = model.Bank_id, EntitlementName = "CanGrantAccessToViews",UserId=model.Owner_id });
                return await result.Content.ReadAsAsync<AccountAccessResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new AccountAccessResponse() { Code = (int)result.StatusCode, ErrorMessage = "OBP-50000: Unknown Error." };
            }
        }
        public async Task<AccountAccessResponse> UpdateView(string Account_id, string bank_id, int view_id, UpdateViewRequest request)
        {
            AccountAccess model = new AccountAccess();
            try
            {
                model = await _client.GetAsync(endPointUrl + view_id).Result.Content.ReadAsAsync<AccountAccess>();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
            model.Account_id = Account_id;
            model.Bank_id = bank_id;
            model.Updated_at = DateTime.Now;
            model.Created_at = DateTime.Now;
            model.Alias = request.Alias.Length == 0 ? "NONE" : request.Alias.ToUpper();
            model.Which_alias_to_use = request.Which_alias_to_use;
            model.Description = request.Description;
            model.Is_Public = request.Is_public;
            model.Hide_metadata_if_alias_used = request.Hide_metadata_if_alias_used;

            model = GetAccountAccessWithAllowedActionsMetaData(model, request.Allowed_actions);

            var accessJson = JsonConvert.SerializeObject(model);
            var result = await _client.PutAsync(endPointUrl + model.Id, new StringContent(accessJson, Encoding.UTF8, "application/json"));

            try
            {
                return await result.Content.ReadAsAsync<AccountAccessResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new AccountAccessResponse();
            }
        }
        public async Task DeleteViewAsync(string Account_id, string bank_id, int view_id)
        {
            try
            {
                AccountAccess access = await _client.GetAsync(endPointUrl + view_id).Result.Content.ReadAsAsync<AccountAccess>();
                if (access.Account_id == Account_id && access.Bank_id == bank_id)
                {
                    await _client.DeleteAsync(endPointUrl + access.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

            }
        }
        private AccountAccess GetAccountAccessWithAllowedActionsMetaData(AccountAccess model, IList<string> actions)
        {
            foreach (var item in actions)
            {
                switch (item.ToLower())
                {
                    case "can_add_more_info": model.Can_add_more_info = true;  break;
                    case "can_see_bank_account_balance": model.Can_see_bank_account_balance = true; break;
                    case "can_see_bank_account_bank_name": model.Can_see_bank_account_bank_name = true; break;
                    case "can_see_bank_account_currency": model.Can_see_bank_account_currency = true; break;
                    case "can_see_bank_account_iban": model.Can_see_bank_account_iban = true; break;
                    case "can_see_bank_account_label": model.Can_see_bank_account_label = true; break;
                    case "can_see_bank_account_number": model.Can_see_bank_account_number = true; break;
                    case "can_see_bank_account_owners": model.Can_see_bank_account_owners = true; break;
                    case "can_see_bank_account_swift_bic": model.Can_see_bank_account_swift_bic = true; break;
                    case "can_see_other_account_bank_name": model.Can_see_other_account_bank_name = true; break;
                    case "can_see_other_account_iban": model.Can_see_other_account_iban = true; break;
                    case "can_see_other_account_number": model.Can_see_other_account_number = true; break;
                    case "can_see_transaction_amount": model.Can_see_transaction_amount = true; break;
                    case "can_see_transaction_balance": model.Can_see_transaction_balance = true; break;
                    case "can_see_transaction_currency": model.Can_see_transaction_currency = true; break;
                    case "can_see_transaction_finish_date": model.Can_see_transaction_finish_date = true; break;
                    case "can_see_transaction_other_bank_account": model.Can_see_transaction_other_bank_account = true; break;
                    case "can_see_transaction_this_bank_account": model.Can_see_transaction_this_bank_account = true; break;
                    case "can_add_transaction_request_to_any_account": model.Can_add_trans_req_to_any_account = true ; break;
                }
            }
            return model;
        }

        public async Task<AccountAccessResponse> GrantUserAccessToView(string provider, string provider_id, int view_id)
        {
           AccountUserAccess userAccess = new AccountUserAccess() { Provider = provider.Replace("%", "").Trim(), Provider_Id = provider_id, View_id = view_id };
            try
            {
                var accessJson = JsonConvert.SerializeObject(userAccess);
                await _client.PostAsync(endPointUrlUserAccess, new StringContent(accessJson, Encoding.UTF8, "application/json"));
                return await this.GetOneAccessAsync(view_id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new AccountAccessResponse() { Code = 500, ErrorMessage = "Could not save the privilege" };
            }
        }

        public async Task<AccountAccessResponse> RevokeAccessToOneView(string provider, string provider_id, int view_id)
        {
            string filter = "?q={ \"view_id\": { \"$eq\":\""+view_id+ "\" }, \"provider\": { \"$eq\":\""+ provider.Replace("%", "").Trim()+ "\" }, \"provider_id\": { \"$eq\":\""+provider_id+"\" } }";
            try
            {
                await _client.DeleteAsync(endPointUrlUserAccess + filter);
                return await this.GetOneAccessAsync(view_id) ;
            }catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new AccountAccessResponse();
            }
        }

        public async Task<UserAccessList> GetAccountAccessForUser(string provider, string provider_id)
        {
            string filter = "?q={ \"provider\": { \"$eq\":\"" + provider + "\" }, \"provider_id\": { \"$eq\":\"" + provider_id + "\" } }";

            try
            {
                AccountUserAccessList list = await _client.GetAsync(endPointUrlUserAccess + filter).Result.Content.ReadAsAsync<AccountUserAccessList>();
                if (list.Items.Count > 0)
                {
                    UserAccessList accessList = new UserAccessList();
                    foreach (var item in list.Items)
                    {
                        accessList.Views.Add(await this.GetOneAccessAsync(item.View_id));
                    }
                    return accessList;
                }
                return new UserAccessList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new UserAccessList();
            }
        }

        public async Task<AccountAccessResponseList> GetAllViews()
        {
            AccountAccessResponseList respList = new AccountAccessResponseList();
            try
            {
                var response = await _client.GetAsync(endPointUrl);
                AccountAccessResponseDbList list = await response.Content.ReadAsAsync<AccountAccessResponseDbList>();
                respList.Count = list.Count;
                respList.HasMore = list.HasMore;
                respList.Offset = list.Offset;
                respList.Limit = list.Limit;
                respList.Views = list.Items;
                return respList;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return respList; 
            }
        }
    }
}
