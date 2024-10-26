using Identity.API.Applications.Dtos;
using Identity.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                return Ok(await _userService.GetUserAsync(User.Identity.Name));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [Route("[action]/{email}", Name = "GetUserByEmail")]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanGetAnyUser" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }
                return Ok(await _userService.GetUserByEmailAsync(email));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                if (HttpContext.Items["userId"] == null)
                {
                    return this.StatusCode(401, new MessageResponse() { Code = 401, Message = "OBP-20001: User not logged in. Authentication is required!" });
                }
                IList<string> requiredRole = new List<string> { "SUPERADMIN", "CanGetAnyUser" };
                string userAuthorisations = (string)(HttpContext.Items["userRoles"] ?? "");

                if (!requiredRole.Intersect(userAuthorisations.Split(",").ToList()).Any())
                {
                    return this.StatusCode(403, new MessageResponse() { Message = "OBP-20006: User is missing one or more roles:", Code = 403 });
                }
                return Ok(await _userService.GetAllUsers());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
    }
}
