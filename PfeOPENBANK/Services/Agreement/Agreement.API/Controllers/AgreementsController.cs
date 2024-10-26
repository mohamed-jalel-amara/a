using Agreement.API.Models;
using Agreement.API.Models.Requests;
using Agreement.API.Models.Responses;
using Agreement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agreements.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AgreementsController : ControllerBase
    {
        private readonly IAgreementService _service;

        public AgreementsController(IAgreementService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountAccessList), 200)]
        public async Task<ActionResult<AccountAccessList>> GetAllAccessListAsync()
        {
            return await _service.GetAllAccessListAsync();
        }

        [Authorize (Roles = "CUSTOMER")]
        [Route("custom", Name = "GetAllAccessCustomListAsync")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountAccessList), 200)]
        public async Task<ActionResult<AccountAccessList>> GetAllAccessCustomListAsync()
        {
            return await _service.GetAllAccessListAsync(HttpContext.Items["userId"].ToString());
        }

        [Authorize(Roles = "CUSTOMER")]
        [Route("custom/by-account/{account_number}", Name = "GetAccessListByAccountAsync")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountAccessList), 200)]
        public async Task<ActionResult<AccountAccessList>> GetAccessListByAccountAsync(long account_number)
        {
            return await _service.GetAccessListByAccountAsync(HttpContext.Items["userId"].ToString(), account_number);
        }

        [Authorize(Roles = "CUSTOMER")]
        [Route("custom/by-provider/{providerId}", Name = "GetAccessListByProviderAsync")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountAccessList), 200)]
        public async Task<ActionResult<AccountAccessList>> GetAccessListByProviderAsync(string providerId)
        {
            return await _service.GetAccessListByProviderAsync(HttpContext.Items["userId"].ToString(), providerId);
        }

        [Authorize]
        [Route("{accessId}", Name = "GetOneAccessAsync")]
        [HttpGet]
        [ProducesResponseType(typeof(AccountAccess), 200)]
        public async Task<ActionResult<AccountAccess>> GetOneAccessAsync(int accessId)
        {
            return await _service.GetOneAccessAsync(accessId);
        }

        [Authorize(Roles = "CUSTOMER")]
        [HttpPut]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(bool), 404)]
        public async Task<ActionResult<bool>> EditAccessAsync(AccessRequest request)
        {
            return await _service.EditAccessAsync(request, HttpContext.Items["userId"].ToString());
        }
    }
}
