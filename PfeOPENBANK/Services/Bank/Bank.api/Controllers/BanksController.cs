using Bank.api.Models.Requests;
using Bank.api.Services.Interfaces;
using Helper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Bank.api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly IBankServices _service;
        private readonly ILogger<BanksController> _logger;

        public BanksController(IBankServices service, ILogger<BanksController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetALL()
        {
            return Ok(await _service.GetAllBankAsync());
        }
        [HttpGet("{id}", Name = "GetBankById")]
        public async Task<IActionResult> GetBankById(string id)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }

                var bank = await _service.GetBankAsync(id);
                //if (bank.Code > 0)
                //{
                //    string message = bank.Code == 404 ? "OBP-30001: Bank not found. Please specify a valid value for BANK_ID." : bank.ErrorMessage;
                //    return this.StatusCode(bank.Code, new MessageResponse() { Code = bank.Code, Message = message });
                //}
                //else
                //{
                    return Ok(bank);
                //}
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankById(string id)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanCreateBank" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }

                var bank = await _service.GetBankAsync(id);
                if (bank.Code > 0)
                {
                    string message = bank.Code == 404 ? "OBP-30001: Bank not found. Please specify a valid value for BANK_ID." : bank.ErrorMessage;
                    return this.StatusCode(bank.Code, new MessageResponse() { Code = bank.Code, Message = message });
                }
                return Ok(await _service.DeleteBankAsync(id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateBank(BankRequest request)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    //return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanCreateBank" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    //return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }
                var bankIdMatch = Regex.Match(request.Id, @"^[0-9/a-z/A-Z/'-'/'.'/'_']{5,255}$");
                if (!bankIdMatch.Success || request.Id.Length > 255)
                {
                    string message = "OBP-30111: Invalid Bank Id. The BANK_ID should only contain 0-9/a-z/A-Z/'-'/'.'/'_', the length should be smaller than 255.";
                    return this.StatusCode(400, new MessageResponse() { Code = 400, Message = message });
                }

                //var bank = await _service.GetBankAsync(request.Id);
                //if (bank.Code == 0)
                //{
                //    return this.StatusCode(409, new MessageResponse() { Code = 409, Message = "OBP-50000: Unknown Error." });
                //}
                var bank = await _service.CreateBankAsync(request);
                if (bank.Code > 0)
                {
                    return this.StatusCode(bank.Code, new MessageResponse() { Code = bank.Code, Message = bank.ErrorMessage });
                }
                else
                {
                    return this.StatusCode(201, bank);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
    }
}
