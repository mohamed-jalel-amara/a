using Account.API.Models;
using Account.API.Models.Requests;
using Account.API.Models.Responses;
using Account.API.Services.Grpc;
using Account.API.Services.Interfaces;
using AutoMapper;
using EventBus.Message.Events;
using Grpc.Core;
using Helper.Models;
using Helper.Utils;
using Helper.Utils.Models;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Account.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _service;
        private readonly ILogger<AccountsController> _logger;
        private readonly IConfiguration _cfg;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        private readonly BankService _bankService;
        private readonly IPublishEndpoint _publish;

        public AccountsController(IAccountService service, UserService userService, ILogger<AccountsController> logger,
            IConfiguration cfg, IMapper mapper, BankService bankService, IPublishEndpoint publish)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _bankService = bankService ?? throw new ArgumentNullException(nameof(bankService));
            _publish = publish ?? throw new ArgumentNullException(nameof(publish));
        }

        [HttpGet]
        [ProducesResponseType(typeof(AccountMinimalListResponse), 200)]
        public async Task<IActionResult> GetMyAccounts()
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                string ownerId = HttpContext.Items["userId"].ToString() ?? "";

                return Ok(await _service.GetAllAccountsByUser(ownerId));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("{bank_id}", Name = "GetAllAccounts")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountListResponse), 200)]
        public async Task<IActionResult> GetAllAccounts(string bank_id)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanQueryOtherUser" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }
                if (HttpContext.Request.Query.Count > 0 && HttpContext.Request.Query["q"].ToString() != null)
                {
                    return Ok(await _service.GetAllFilteringAccounts(HttpContext.Request.Query["q"].ToString()));
                }
                return Ok(await _service.GetAllAccounts(bank_id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }


        [Route("[action]/{bank_id}/{account_id}", Name = "GetAccountById")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountResponse), 200)]
        [ProducesResponseType(typeof(AccountResponse), 404)]
        public async Task<IActionResult> GetAccountById(string bank_id,string account_id)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanQueryOtherUser", "CanCreateAccount" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }

                IList<string> noAdminRoles = new List<string> { "SUPERADMIN", "CanQueryOtherUser" };

                AccountResponse resp = new AccountResponse();
                if (!noAdminRoles.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    resp = await _service.GetAccountById(bank_id, account_id, HttpContext.Items["userId"].ToString());
                }
                else
                {
                    resp = await _service.GetAccountById(bank_id, account_id, null);
                }

                if (resp.Code > 0)
                {
                    return this.StatusCode(resp.Code, new MessageResponse() { Code = resp.Code, Message = resp.ErrorMessage });
                }

                return Ok(resp);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("[action]/{bank_id}/{account_number}", Name = "GetAccountByNumber")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountResponse), 200)]
        [ProducesResponseType(typeof(AccountResponse), 404)]
        public async Task<IActionResult> GetAccountByNumber(string bank_id, Int64 account_number)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanQueryOtherUser", "CanCreateAccount" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }

                IList<string> noAdminRoles = new List<string> { "SUPERADMIN", "CanQueryOtherUser" };

                AccountResponse resp = new AccountResponse();
                if (!noAdminRoles.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    resp = await _service.GetAccount(bank_id, account_number, HttpContext.Items["userId"].ToString());
                }
                else
                {
                    resp = await _service.GetAccount(bank_id, account_number, null);
                }

                if (resp.Code > 0)
                {
                    return this.StatusCode(resp.Code, new MessageResponse() { Code = resp.Code, Message = resp.ErrorMessage });
                }

                return Ok(resp);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("{account_id}/{bank_id}")]
        [HttpPost]
        [ProducesResponseType(typeof(AccountCreated), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(AccountCreated), (int)HttpStatusCode.BadRequest)]
        
        public async Task<IActionResult> AddAccount(string account_id, string bank_id, [FromBody] AccountRequest account)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                string ownerId = account.User_id == null ? HttpContext.Items["userId"].ToString() ?? "" : account.User_id;

                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanQueryOtherUser", "CanCreateAccount" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }

                AccountResponse model = await _service.GetAccountById(bank_id, account_id, null);
                if (model.Id != null)
                {
                    return this.StatusCode(409, new MessageResponse() { Code = 409, Message = "OBP-30208: Account_ID already exists at the Bank." });
                }

                MessageResponse res = await ValidateData(account, ownerId, account_id, bank_id);
                if (res.Code == 200)
                {
                    AccountRequest request = new AccountRequest() { Balance = account.Balance, Label = account.Label, Type = account.Type };
                    AccountCreated response = await _service.AddAccount(account_id, bank_id, request, ownerId);
                    if (response.Code > 0)
                    {
                        return this.StatusCode(response.Code, new MessageResponse() { Code = response.Code, Message = response.ErrorMessage });
                    }
                    //Notify Client after account creation through RabbitMQ
                    try
                    {
                        await _publish.Publish(new CreationAccountEvent()
                        {
                            AccountId = account_id,
                            BankId = bank_id,
                            OwnerId = ownerId
                        });
                    }catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                    return this.StatusCode(201, response);
                }
                else
                {
                    return this.StatusCode(res.Code, res);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("{account_id}/{bank_id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateAccountLabel(string account_id, string bank_id, UpdateLabelRequest request)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanQueryOtherUser", "CanCreateAccount" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }
                AccountResponse account = new AccountResponse();
                IList<string> noAdminRoles = new List<string> { "SUPERADMIN", "CanQueryOtherUser" };
                if (!noAdminRoles.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    account = await _service.GetAccountById(bank_id, account_id, HttpContext.Items["userId"].ToString());
                }
                else
                {
                    account = await _service.GetAccountById(bank_id, account_id, null);
                }

                if (account.Id == null)
                {
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = "OBP-30018: Bank Account not found. Please specify valid values for BANK_ID and ACCOUNT_ID." });
                }
                else if (account.BankId != bank_id)
                {
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = "OBP-30018: Bank Account not found. Please specify valid values for BANK_ID and ACCOUNT_ID." });
                }
                else
                {
                    request.Id = account_id;
                    return Ok(await _service.UpdateAccount(request));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        private async Task<MessageResponse> ValidateData(AccountRequest account, string userId, string account_id, string bank_id)
        {
            int statusCode = 200;
            string message = "Ok";

            var bankIdMatch = Regex.Match(bank_id, @"^[0-9/a-z/A-Z/'-'/'.'/'_']{5,255}$");
            if (!bankIdMatch.Success || bank_id.Length > 255)
            {
                message = "OBP-30111: Invalid Bank Id. The BANK_ID should only contain 0-9/a-z/A-Z/'-'/'.'/'_', the length should be smaller than 255.";
                return new MessageResponse() { Code = 400, Message = message };
            }
            var bank = await _bankService.GetBankObjectAsync(bank_id);

            if (bank.Id.Length == 0)
            {
                message = "OBP-30001: Bank not found. Please specify a valid value for BANK_ID.";
                return new MessageResponse() { Code = 404, Message = message };
            }
            var isValidAccount = Guid.TryParse(account_id, out _);
            
            if (!isValidAccount)
            {
                message = "OBP-30111: Invalid Account Id. The ACCOUNT_ID should only contain 0-9/a-z/A-Z/'-'/'.'/'_', the length should be smaller than 255.";
                return new MessageResponse() { Code = 400, Message = message };
            }

            var isValid = Guid.TryParse(userId, out _);
            if (!isValid)
            {
                return new MessageResponse()
                {
                    Code = 400,
                    Message = "OBP-30107: Invalid User Id."
                };
            }

            var user = await _userService.GetUserAsync(userId);
            if (user.UserId.Length == 0)
            {
                message = "OBP-20005: User not found. Please specify a valid value for USER_ID.";
                return new MessageResponse() { Code = 404, Message = message };
            }

            
            if (!AvailablesCurrencies.Currencies.Contains(account.Balance.Currency.ToUpper()))
            {
                message = "OBP-30105: Invalid Balance Currency.";
                return new MessageResponse() { Code = 400, Message = message };
            }

            return new MessageResponse() {  Code = statusCode, Message = message  };
        }
    }
}
