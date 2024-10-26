using Identity.API.Applications.Dtos;
using Identity.API.Applications.Models.Entities;
using Identity.API.Services.Grpc;
using Identity.API.Services.Interfaces;
using Identity.API.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Identity.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EntitlementsController : ControllerBase
    {
        private readonly IEntitlementService _service;
        private readonly BankService _bankService;
        private readonly UserManager<UserModel> _userService;
        private readonly RoleManager<Entitlement> _roleManager;
        private readonly UserManager<UserModel> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<EntitlementsController> _logger;

        public EntitlementsController(IEntitlementService service, UserManager<UserModel> userService, IConfiguration config, RoleManager<Entitlement> roleManager,
            UserManager<UserModel> userManager, BankService bankService, ILogger<EntitlementsController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _bankService = bankService ?? throw new ArgumentNullException(nameof(bankService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetAll()
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
                return Ok(_service.GetAllEntitlements());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("[action]/{user_id}", Name = "GetEntitlementsForUser")]
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetEntitlementsForUser(string user_id)
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

                var user = await _userService.FindByIdAsync(user_id);
                if (user == null)
                {
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = "OBP-20005: User not found. Please specify a valid value for USER_ID." });
                }
                return Ok(await _service.GetEntitlementForUserAsync(user_id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("[action]/{user_id}", Name = "AddEntitlement")]
        [HttpPost]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> AddEntitlement(string user_id, [FromBody] EntitlementRequest request)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }

                var bank = await _bankService.GetBankObjectAsync(request.Bank_Id);

                if (bank.Id.Length == 0)
                {
                    string message = "OBP-30001: Bank not found. Please specify a valid value for BANK_ID.";
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = message });
                }

                if (!BankInfos.Roles.Contains(request.Role_name))
                {
                    return this.StatusCode(400, new MessageResponse() { Message = "OOBP-30205: This entitlement is not a Bank Role", Code = 400 });
                }

                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanCreateEntitlementAtOneBank" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }
                var user = await _userService.FindByIdAsync(user_id);
                if (user == null)
                {
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = "OBP-20005: User not found. Please specify a valid value for USER_ID." });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains(request.Role_name))
                {
                    return this.StatusCode(409, new MessageResponse() { Code = 409, Message = "OBP-30216: Entitlement already exists for the user." });
                }
                return this.StatusCode(201, await _service.AddEntitlementForUserAsync(user_id, request));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("[action]/{userId}/{entitlement_id}", Name = "DeleteEntitlement")]
        [HttpDelete]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> DeleteEntitlement(string userId,string entitlement_id)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }

                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanCreateEntitlementAtOneBank" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }

                var role = await _roleManager.FindByIdAsync(entitlement_id);
                if (role == null)
                {
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = "OBP-30212: EntitlementId not found" });
                }
                return Ok(await _service.DeleteEntitlementAsync(userId, entitlement_id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
    }
}
