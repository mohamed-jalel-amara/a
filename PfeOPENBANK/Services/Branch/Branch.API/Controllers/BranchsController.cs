using Branch.API.Models;
using Branch.API.Models.Requests;
using Branch.API.Models.Response;
using Branch.API.Services.Grpc;
using Branch.API.Services.Interfaces;
using Helper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Branch.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class BranchsController : ControllerBase
    {
        private IBranchService _service;
        private ILogger<BranchsController> _logger;
        private readonly BankService _bankService;

        public BranchsController(IBranchService service, ILogger<BranchsController> logger, BankService bankService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bankService = bankService ?? throw new ArgumentNullException(nameof(bankService));
        }

        [Route("{bank_id}", Name = "GetAllBranchs")]
        [HttpGet]
        [ProducesResponseType(typeof(BranchListResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BranchListResponse>> GetAllBranchs(string bank_id)
        {
            try
            {
                int page = 1;
                int size = 10;
                string filter = "";
                if (HttpContext.Request.Query.Count > 0)
                {
                    page = HttpContext.Request.Query["page"].ToString() != null ? Int32.Parse(HttpContext.Request.Query["page"].ToString()) : page;
                    size = HttpContext.Request.Query["size"].ToString() != null ? Int32.Parse(HttpContext.Request.Query["size"].ToString()) : size;
                    filter = HttpContext.Request.Query["q"].ToString() != null ? HttpContext.Request.Query["q"].ToString() : filter;
                }

                if (filter != null && filter.Length > 0)
                {
                    return await _service.GetBranchesByFilter(filter, bank_id, page, size);
                }
                return await _service.GetAllBranches(bank_id, page, size);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("[action]/{branch_id}")]
        [HttpGet]
        [ProducesResponseType(typeof(BranchResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BranchResponse), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<BranchResponse>> GetBranch(string branch_id)
        {
            try
            {
                BranchResponse response = await _service.GetBranch(branch_id);
                if (response.Code > 0)
                {
                    return this.StatusCode(response.Code, new MessageResponse() { Code = response.Code, Message = response.ErrorMessage });
                }
                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("{bank_id}")]
        [HttpPost]
        [ProducesResponseType(typeof(BranchResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BranchResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Addbranch( string bank_id,[FromBody] BranchRequest branch)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanCreateBranch" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-30209: Insufficient authorisation to Create Branch. You do not have the role CanCreateBranch.", Code = 403 });
                }
                var bank = await _bankService.GetBank(bank_id);

                if (bank.Id.Length == 0)
                {
                    string message = "OBP-30001: Bank not found. Please specify a valid value for BANK_ID.";
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = message });
                }
                branch.Bank_id = bank.Id;
                BranchResponse response = await _service.AddBranch(branch);
                if (response.Code > 0)
                {
                    return this.StatusCode(response.Code, new MessageResponse() { Code = response.Code, Message = response.ErrorMessage });
                }
                return this.StatusCode(201, response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("{bank_id}/{branch_id}", Name = "DeleteBranchById")]
        [HttpDelete]
        [ProducesResponseType(typeof(BranchModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BranchModel), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BranchModel), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteBranch(string bank_id,string branch_id)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanCreateBranch" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-30209: Insufficient authorisation to Create Branch. You do not have the role CanCreateBranch.", Code = 403 });
                }
                var bank = await _bankService.GetBank(bank_id);

                if (bank.Id.Length == 0)
                {
                    string message = "OBP-30001: Bank not found. Please specify a valid value for BANK_ID.";
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = message });
                }
                BranchResponse response = await _service.GetBranch(branch_id);

                if (response.Bank_id != bank.Id)
                {
                    string message = "OBP-300010: Branch not found. Please specify a valid value for BRANCH_ID. Or License may not be set. meta.license.id and meta.license.name can not be empty";
                    return this.StatusCode(404, new MessageResponse() { Code = 404, Message = message });
                }

                return Ok(await _service.DeleteBranch(branch_id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
    }
}
