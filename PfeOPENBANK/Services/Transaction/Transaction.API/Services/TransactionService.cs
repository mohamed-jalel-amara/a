using Newtonsoft.Json;
using System.Text;
using Transaction.API.Models;
using Transaction.API.Models.Requests;
using Transaction.API.Models.Responses;
using Transaction.API.Models.Responses.Accounts;
using Transaction.API.Services.Grpc;
using Transaction.API.Services.Interfaces;
using Account.Grpc.Protos;
using MassTransit;
using Transaction.API.Utils.Models;

namespace Transaction.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        private readonly AccountService _accountService;
        private readonly ILogger<TransactionService> _logger;

        private string endPointUrl = "";
        private string endPointChallengeUrl = "";
        private string endPoinRequesttUrl = "";
        private IList<string> transactionAllowedTypes = new List<string>();

        HttpResponseMessage response = new HttpResponseMessage();

        public TransactionService(ILogger<TransactionService> logger, AccountService accountService,
            IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));

            //Setting endPoint
            endPointUrl = $"/ords/{_config.GetValue<string>("OracleSettings:DatabaseUser")}" +
                $"/{_config.GetValue<string>("OracleSettings:DatabaseTableName")}/";

            endPointChallengeUrl = $"/ords/{_config.GetValue<string>("OracleSettings:DatabaseUser")}" +
                $"/{_config.GetValue<string>("OracleSettings:DatabaseChallengeTableName")}/";

            endPoinRequesttUrl = $"/ords/{_config.GetValue<string>("OracleSettings:DatabaseUser")}" +
                $"/{_config.GetValue<string>("OracleSettings:DatabaseRequestTableName")}/";

            transactionAllowedTypes = _config.GetValue<List<string>>("Transaction:AllowedTypes") ?? new List<string>();
        }
        public async Task<TransactionModel> AddTransactionAsync(TransactionModel transaction)
        {
            var transactionPost = JsonConvert.SerializeObject(transaction);

            var response = await _client.PostAsync(endPointUrl, new StringContent(transactionPost, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Adding transaction success!");
            return await response.Content.ReadAsAsync<TransactionModel>();
        }

        async Task<TransactionResponse> ITransactionService.CreateTransactionRequest(string account_id, string bank_id, string type, TransactionRequestReq req)
        {

            string transactionIds = Guid.NewGuid().ToString();

            TransactionRequest request = new TransactionRequest()
            {
                Id = Guid.NewGuid().ToString(),
                Amount = req.Value.Amount,
                Currency = req.Value.Currency,
                Description = req.Description,
                Start_date = DateTime.Now,
                From_account_id = account_id,
                From_bank_id = bank_id,
                To_account_id = req.To.Account_id,
                To_bank_id = req.To.Bank_id,
                Transaction_ids = transactionIds
            };
            request.Type = TransactionRequestType.Types.Where(t => t == type).FirstOrDefault() ?? "SANDBOX_TAN";

            double meanAmount = _config.GetValue<double>("Transaction:MeanAmount");
            request.Status = request.Amount <= meanAmount ? "COMPLETED" : "INITIATED";
            request.Transfert_type = request.Amount < 0 ? "WITHDRAWAL" : "DEPOSIT";

            if(request.Status == "INITIATED")
            {
                await CreateTransactionChallenge(request, transactionIds);
            }
            else
            {
                await CreateTransaction(new TransactionModel() { Completed = DateTime.Now, Posted = request.Start_date, Id = request.Transaction_ids });
            }

            var requestPost = JsonConvert.SerializeObject(request);
            try
            {
                response = await _client.PostAsync(endPoinRequesttUrl, new StringContent(requestPost, Encoding.UTF8, "application/json"));
                return new TransactionResponse() { Code = 201 };
            }catch(Exception ex)
            {
                return new TransactionResponse() { Code = (int)response.StatusCode, ErrorMessage = response.ReasonPhrase.ToString() };
            }
            
        }

        private async Task<TransactionResponse> CreateTransactionChallenge(TransactionRequest request, string transactionIds)
        {
            TransactionChallenge challenge = new TransactionChallenge()
            {
                Id = Guid.NewGuid().ToString(),
                Challenge_type = request.Type,
                Transaction_ids = transactionIds,
                Allowed_attemps = _config.GetValue<int>("Transaction: ChallengeAllowedAttemps")
            };
            var challengePost = JsonConvert.SerializeObject(challenge);
            try
            {
                response = await _client.PostAsync(endPointChallengeUrl, new StringContent(challengePost, Encoding.UTF8, "application/json"));
                return new TransactionResponse() { Code = 201 };
            }
            catch (Exception ex)
            {
                return new TransactionResponse() { Code = (int)response.StatusCode, ErrorMessage = response.ReasonPhrase.ToString() };
            }
        }

        Task<TransactionResponse> ITransactionService.AnswerTransactionRequest(AnswerTransactionReq answer)
        {
            throw new NotImplementedException();
        }

        async Task<TransactionRequestResponseList> ITransactionService.GetTransactionsRequest(string account_id, string bank_id)
        {
            string filter = "?q={ \"$or\" : [  {\"$and\" : [  {\"from_bank_id\": { \"$eq\" : \""+bank_id+"\" } }, { \"from_account_id\": { \"$eq\" : \""+account_id+"\" } } ] }, { \"$and\" : [ {\"to_bank_id\": { \"$eq\" : \""+bank_id+"\" } },  {\"to_account_id\": { \"$eq\" : \""+account_id+"\" } } ] } ] }";
            var response = await _client.GetAsync(endPoinRequesttUrl + filter);
            try
            {
                var list = await response.Content.ReadAsAsync<TransactionRequestList>();
                var responseList = new TransactionRequestResponseList() { Count = list.Count, HasMore = list.HasMore,
                Limit = list.Limit, Offset = list.Offset};
                if (list.Items.Count > 0)
                {
                    foreach (var item in list.Items)
                    {
                        responseList.Transaction_requests_with_charges.Add(await GetTransactionRequestFromModemAsync(item));
                    }
                }
                return responseList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new TransactionRequestResponseList();
            }
        }

        async Task<TransactionResponseList> ITransactionService.GetTransactionsForAccount(string account_id)
        {
            var result = await _client.GetAsync(endPointUrl);
            try
            {
                var transactions = await result.Content.ReadAsAsync<TransactionList>();
                if (transactions.Items.Count > 0)
                {
                    TransactionResponseList responseList = new TransactionResponseList() { Count = transactions.Count, 
                    HasMore = transactions.HasMore, Limit = transactions.Limit, Offset = transactions.Offset};
                    foreach (var item in transactions.Items)
                    {
                        var transactionResponse = await GetTransactionResponseFromModel(item);
                        responseList.transactions.Add(transactionResponse);
                    }
                }
                return new TransactionResponseList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new TransactionResponseList();
            }
        }

        async Task<TransactionResponse> ITransactionService.GetTransactionAsync(string transactionId, string view_id)
        {
            var result = await _client.GetAsync(endPointUrl + transactionId);
            try
            {
                var transaction = await result.Content.ReadAsAsync<TransactionModel>();
                return await GetTransactionResponseFromModel(transaction, view_id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new TransactionResponse() { Code = 500, ErrorMessage = "OBP-50000: Unknown Error." };
            }
        }
        #region Private methods
        private async Task<TransactionRequestResponse> GetTransactionRequestFromModemAsync(TransactionRequest model)
        {
            TransactionRequestResponse response = new TransactionRequestResponse();
            response.Id = model.Id;
            response.Type = model.Type;
            response.From = new TransactionRequestFromAccount() { Account_id = model.From_account_id, Bank_id = model.From_bank_id };

            TransactionRequestToAccount to_Transfert_To = new TransactionRequestToAccount()
            {
                Description = model.Description,
                Future_date = model.Furture_Date,
                Transfert_type = model.Transfert_type,
                Value = new TransactionValue() {Amount = model.Amount, Currency = model.Currency },
                To = new ToAccountDetail() { Bank_id = model.To_bank_id, Account_id = model.To_account_id },
            };

            response.Details = new TransactionRequestDetails() { To_transfert_to_account = to_Transfert_To };
            response.Transaction_ids.Add(model.Transaction_ids);
            response.Status = model.Status;
            response.Start_date = model.Start_date;
            response.End_date = model.End_Date;
            return response;
        }
        //Get transaction response from Model
        private async Task<TransactionResponse> GetTransactionResponseFromModel(TransactionModel model, string? view_id = null)
        {
            TransactionResponse response = new TransactionResponse();
            response.Details = await GetTransactionDetail(model);
            response.Other_account = await GetToAccountAsync(response.Details.New_balance.Account_id, view_id);
            response.This_account = await GetFromAccountAsync(response.Details.Value.Account_id, view_id);
            response.Id = model.Id;
            return response;
        }
        //return Account Object From Account.Grpc
        private async Task<AccountObject> GetAccountObject(string account_id)
        {
            return await _accountService.GetAccountDataAsync(account_id);
        }
        //Get From Account Detail Of transaction
        private async Task<FromAccount> GetFromAccountAsync(string account_id, string view_id)
        {
            return new FromAccount();
        }
        //Get To Account Detail Of transaction
        private async Task<ToAccount> GetToAccountAsync(string account_id, string view_id)
        {
            return new ToAccount();
        }
        // Get Transaction request for transaction object
        private async Task<TransactionRequest?> GetTransactionRequest(string transactionId, string? account_id = null)
        {
            string filter = "?q = { \"transaction_ids\": { \"$eq\" : \"" + transactionId + "\" } }";
            if (account_id != null)
            {
                filter = "?q = { \"$or\" : [ \"from_account_id\": { \"$eq\" : \"" + account_id + "\" }, \"to_account_id\": { \"$eq\":\""+account_id+"\" } ] }";
            }
            var result = await _client.GetAsync(endPoinRequesttUrl + filter);
            try
            {
                var transactionReqList = await result.Content.ReadAsAsync<TransactionRequestList>();
                return transactionReqList.Items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }
        // Get detail of a transaction
        private async Task<TransactionDetail> GetTransactionDetail(TransactionModel model)
        {
            var detail = new TransactionDetail();
            //Get Detail of the transaction
            var detailResponse = await GetTransactionRequest(model.Id);
            
            if (detailResponse == null)
            {
                return detail;
            }
            //Get From and To account 
            var toAccount = await GetAccountObject(detailResponse.To_account_id);
            var fromAccount = await GetAccountObject(detailResponse.From_account_id);


            detail.Type = detailResponse.Type;
            detail.Posted = model.Posted;
            detail.Completed = model.Completed;
            if (toAccount != null && fromAccount != null)
            {
                detail.New_balance = new TransactionValue() { Amount = toAccount.Amount, Currency = toAccount.Currency, Account_id = toAccount.Id };
                detail.Value = new TransactionValue() { Amount = fromAccount.Amount, Currency = fromAccount.Currency, Account_id = fromAccount.Id };
            }
            return detail;
        }

        private async Task CreateTransaction(TransactionModel transaction)
        {
            var requestPost = JsonConvert.SerializeObject(transaction);
            try
            {
                response = await _client.PostAsync(endPointUrl, new StringContent(requestPost, Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
        #endregion
    }
}
