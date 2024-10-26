using Helper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Statement.API.Models;
using Statement.API.Services.Interfaces;

namespace Statement.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatementsController : ControllerBase
    {
        private readonly IStatementService _service;
        private readonly ILogger<StatementsController> _logger;

        public StatementsController(IStatementService service, ILogger<StatementsController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(StatementModel), 200)]
        public async Task<ActionResult<StatementModel>> GetStatements()
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                string ownerId = HttpContext.Items["userId"].ToString() ?? "";
                return Ok(await _service.GetStatementAsync(ownerId));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
    }
}
