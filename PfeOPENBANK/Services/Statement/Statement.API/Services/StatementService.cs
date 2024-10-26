using AutoMapper;
using Statement.API.Models;
using Statement.API.Models.Responses;
using Statement.API.Services.Grpc;
using Statement.API.Services.Interfaces;
using System.Collections.Generic;

namespace Statement.API.Services
{
    public class StatementService : IStatementService
    {
        private readonly ILogger<StatementService> _logger;
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        private string endPointUrl = "";
        private string endPointChallengeUrl = "";
        private string endPoinRequesttUrl = "";
        private IList<string> transactionAllowedTypes = new List<string>();

        private readonly AccountService _accountService;
        private readonly BranchService _branchService;

        public StatementService(ILogger<StatementService> logger, BranchService branchService, AccountService accountService,
            IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _branchService = branchService ?? throw new ArgumentNullException(nameof(branchService));
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

        public async Task<StatementModel> GetStatementAsync(string userId)
        {
            var userAccounts = await _accountService.GetAccountObjectListAsync(userId);
            IList<AccountMinimal> accountMinimals = new List<AccountMinimal>();
            if (userAccounts.Items.Count() > 0)
            {
                foreach (var item in userAccounts.Items)
                {
                    AccountMinimal account = new AccountMinimal() { Account_number = item.AccNumber, Bank_id = item.Bankid};
                    account.Transactions = await GetTransactionsMinimalFromRequest(item.Id, item.Bankid);
                    accountMinimals.Add(account);
                }
                return new StatementModel() { Accounts = accountMinimals.Where( am => am.Transactions.Count() > 0).ToList() };
            }
            return new StatementModel();
        }

        private async Task<IList<TransactionMinimal>> GetTransactionsMinimalFromRequest(string account_id, string bank_id)
        {
            IList < TransactionMinimal > transactions = new List< TransactionMinimal >();

            string filter = "?q={ \"$or\" : [  {\"$and\" : [  {\"from_bank_id\": { \"$eq\" : \"" + bank_id + "\" } }, { \"from_account_id\": { \"$eq\" : \"" + account_id + "\" } } ] }, { \"$and\" : [ {\"to_bank_id\": { \"$eq\" : \"" + bank_id + "\" } },  {\"to_account_id\": { \"$eq\" : \"" + account_id + "\" } } ] } ] }";
            var response = await _client.GetAsync(endPoinRequesttUrl + filter);
            try
            {
                var list = await response.Content.ReadAsAsync<TransactionRequestList>();

                if (list.Items.Count() > 0)
                {
                    foreach (var item in list.Items)
                    {
                        var transaction = await _client.GetAsync(endPointUrl + item.Transaction_ids).Result.Content.ReadAsAsync<TransactionModel>();
                        if (transaction != null)
                        {
                            TransactionMinimal transactionMinimal = new TransactionMinimal() { Date = transaction.Completed, Description = item.Description,
                                Transaction_Id = transaction.Id, Type = item.Transfert_type, Value = new TransactionValue() { Amount = Math.Abs(item.Amount), Currency = item.Currency } };
                            transactions.Add(transactionMinimal);
                        }
                    }
                }
                return transactions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return transactions;
            }
        }
    }
}
