using AutoMapper;
using EventBus.Message.Events;
using Helper.Models;
using Helper.Utils;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Transaction.API.Features.Commands;
using Transaction.API.Features.Queries;
using Transaction.API.Models;
using Transaction.API.Models.Requests;
using Transaction.API.Models.Responses;
using Transaction.API.Services.Grpc;
using Transaction.API.Utils.Models;

namespace Transaction.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly AccountService accountService;
        private readonly BankService _bankService;
        private readonly ViewService _viewService;
        private readonly UserService _userService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publisher;
        private readonly ILogger<TransactionsController> _logger;
        

        public TransactionsController(AccountService accountService, IMapper mapper, UserService userService,
            IMediator mediator, IPublishEndpoint publishEndpoint, BankService bankService,ViewService viewService, 
            ILogger<TransactionsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _publisher = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _bankService = bankService ?? throw new ArgumentNullException(nameof(bankService));
            _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("[action]/{account_id}/{bank_id}/{transaction_request_type}/{view_id}", Name = "AddTransactionRequest")]
        public async Task<IActionResult> AddTransactionRequest(string account_id,string bank_id, string transaction_request_type,
           int view_id, [FromBody] TransactionRequestReq request)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                string userId = HttpContext.Items["userId"].ToString();

                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanCreateAnyTransactionRequest" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles: CanCreateAnyTransactionRequest", Code = 403 });
                }

                if (!TransactionRequestType.Types.Contains(transaction_request_type))
                {
                    return this.StatusCode(400, new MessageResponse() { Message = "OBP-40001: Invalid value for TRANSACTION_REQUEST_TYPE", Code = 400 });
                }

                To toAccount = request.To;
                MessageResponse message = await ValidateData(request.Value, toAccount, account_id, bank_id, view_id, userId);
                if (message.Code == 200)
                {
                    var response = await _mediator.Send(new TransactionCmd()
                    {
                        Request = request,
                        AccountId = account_id,
                        BankId = bank_id,
                        Type = transaction_request_type
                    });
                    if (response.Code > 0)
                    {
                        return this.StatusCode(response.Code, new MessageResponse() { Message = response.ErrorMessage, Code = response.Code });
                    }
                    return Ok();
                }
                else
                {
                    return this.StatusCode(message.Code, message);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [HttpGet("[action]/{account_id}/{bank_id}/{view_id}", Name = "GetTransactionRequests")]
        public async Task<IActionResult> GetTransactionRequests(string account_id, string bank_id, int view_id)
        {
            try
            {
                string message = string.Empty;
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                string userId = HttpContext.Items["userId"].ToString();

                var user = await _userService.GetUserDataAsync(userId);
                if (user.UserId.Length == 0)
                {
                    message = "OBP-20005: User not found. Please specify a valid value for USER_ID.";
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = message });
                }

                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanCreateAnyTransactionRequest" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    var account = await accountService.GetAccountDataAsync(account_id);
                    if (account.Id.Length == 0)
                    {
                        return this.StatusCode(404, new MessageResponse() { Code = 404, Message = "OBP-30018: Bank Account not found. Please specify valid values for BANK_ID and ACCOUNT_ID." });
                    }

                    if (account.Ownerid != userId)
                    {
                        return this.StatusCode(403, new MessageResponse() { Message = "User does not have owner access", Code = 403 });
                    }
                    //return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles: CanCreateAnyTransactionRequest", Code = 403 });
                }

                var view = await _viewService.GetViewDataAsync(view_id);

                if (view.Id == 0)
                {
                    message = "OBP-30005: View not found for Account. Please specify a valid value for VIEW_ID.";
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = message });
                }

                var userAccess = await _viewService.GetViewAccessDataAsync(user.Provider, user.ProviderId, view.Id);
                if (userAccess.Id == 0)
                {
                    message = "OBP-20017: Current user does not have access to the view. Please specify a valid value for VIEW_ID.";
                    return this.StatusCode(403, new MessageResponse() { Code = 403, Message = message });
                }

                return Ok(await _mediator.Send(new GetTransactionRequestsCmd() { AccountId = account_id, BankId = bank_id }));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        private async Task<MessageResponse> ValidateData(AmountValue value, To toAccount, string account_id, string bank_id, 
            int view_id, string userId)
        {
            int statusCode = 200;
            string message = "Ok";

            var bankIdMatch = Regex.Match(bank_id, @"^[0-9/a-z/A-Z/'-'/'.'/'_']{5,255}$");
            if (!bankIdMatch.Success || bank_id.Length > 255)
            {
                message = "OBP-30111: Invalid Bank Id. The BANK_ID should only contain 0-9/a-z/A-Z/'-'/'.'/'_', the length should be smaller than 255.";
                return new MessageResponse() { Code = 400, Message = message };
            }
            var isValidAccount = Guid.TryParse(account_id, out _);

            if (!isValidAccount)
            {
                message = "OBP-30111: Invalid Account Id. The ACCOUNT_ID should only contain 0-9/a-z/A-Z/'-'/'.'/'_', the length should be smaller than 255.";
                return new MessageResponse() { Code = 400, Message = message };
            }

            var bank = await _bankService.GetBankDataAsync(bank_id);

            if (bank.Id.Length == 0)
            {
                message = "OBP-30001: Bank not found. Please specify a valid value for BANK_ID.";
                return new MessageResponse() { Code = 404, Message = message };
            }

            var FromAccount = await accountService.GetAccountDataAsync(account_id);

            if (FromAccount.Id.Length == 0)
            {
                message = "OBP-30003: Account not found. Please specify a valid value for ACCOUNT_ID.";
                return new MessageResponse() { Code = 404, Message = message };
            }
            if (bank.Id != FromAccount.Bankid)
            {
                message = "OBP-30018: Bank Account not found. Please specify valid values for BANK_ID and ACCOUNT_ID.";
                return new MessageResponse() { Code = 404, Message = message };
            }

            var toBank = await _bankService.GetBankDataAsync(toAccount.Bank_id);
            if (toBank.Id.Length == 0)
            {
                message = "OBP-30001: Bank not found. Please specify a valid value for BANK_ID.";
                return new MessageResponse() { Code = 404, Message = message };
            }
            
            var ToAccount = await accountService.GetAccountDataAsync(toAccount.Account_id);
            if (ToAccount.Id.Length == 0)
            {
                message = "OBP-30003: Account not found. Please specify a valid value for ACCOUNT_ID.";
                return new MessageResponse() { Code = 404, Message = message };
            }

            if (FromAccount.Currency != value.Currency)
            {
                message = "OBP-40003: Transaction Request Currency must be the same as From Account Currency.";
                return new MessageResponse() { Code = 400, Message = message };
            }
            if (toBank.Id != ToAccount.Bankid)
            {
                message = "OBP-30018: Bank Account not found. Please specify valid values for BANK_ID and ACCOUNT_ID.";
                return new MessageResponse() { Code = 404, Message = message };
            }

            var view = await _viewService.GetViewDataAsync(view_id);

            if (view.Id == 0)
            {
                message = "OBP-30005: View not found for Account. Please specify a valid value for VIEW_ID.";
                return new MessageResponse() { Code = 404, Message = message };
            }

            var user = await _userService.GetUserDataAsync(userId);
            if (user.UserId.Length ==0 )
            {
                message = "OBP-20005: User not found. Please specify a valid value for USER_ID.";
                return new MessageResponse() { Code = 401, Message = message };
            }

            var userAccess = await _viewService.GetViewAccessDataAsync(user.Provider, user.ProviderId, view.Id);
            if (userAccess.Id == 0)
            {
                message = "OBP-20017: Current user does not have access to the view. Please specify a valid value for VIEW_ID.";
                return new MessageResponse() { Code = 403, Message = message };
            }

            if(!view.CanAddTransReqToAnyAccount)
            {
                message = "OBP-40002: Insufficient authorisation to create TransactionRequest. " +
                    "The Transaction Request could not be created because the login user doesn't have access to the view " +
                    "of the from account or the consumer doesn't have the access to the view of the from account or the " +
                    "login user does not have the `CanCreateAnyTransactionRequest` role or the view does not have the permission " +
                    "canaddtransactionrequesttoanyaccount.";
                return new MessageResponse() { Code = 403, Message = message };
            }

            statusCode = 200;
            return new MessageResponse() { Code = statusCode, Message = message };
        }
    }
}
