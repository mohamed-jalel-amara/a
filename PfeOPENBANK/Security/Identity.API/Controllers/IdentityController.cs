using EventBus.Message.Events;
using Identity.API.Applications.Data;
using Identity.API.Applications.Dtos;
using Identity.API.Applications.Models;
using Identity.API.Applications.Models.Entities;
using Identity.API.Applications.Networks;
using Identity.API.Models;
using Identity.API.Services.Interfaces;
using Identity.API.Utils;
using Identity.API.Utils.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Twilio.Http;
using Twilio.TwiML.Voice;

namespace Identity.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<Entitlement> _roleManager;
        private readonly ILogger<IdentityController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IJwtUtils _jwtUtils;
        private readonly IPublishEndpoint _publish;
        private readonly ITokenManager _tokenManager;

        public IdentityController(UserManager<UserModel> userManager, IJwtUtils jwtUtils,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            RoleManager<Entitlement> roleManager,
            ILogger<IdentityController> logger, SignInManager<UserModel> signInManager,
            IPublishEndpoint publish, ITokenManager tokenManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = webHostEnvironment ?? throw new ArgumentException(nameof(webHostEnvironment));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _jwtUtils = jwtUtils ?? throw new ArgumentNullException(nameof(jwtUtils));
            _publish = publish ?? throw new ArgumentNullException(nameof(publish));
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
        }

        #region ADD USer
        [Route("add-user")]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest authDto)
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
                UserModel user = new UserModel();
                user.Email = authDto.Email;
                user.Last_name = authDto.Last_name;
                user.First_name = authDto.First_name;
                user.UserName = authDto.Username;
                user = UserProviderDetails.GetUriProviderDetails(Request, user);

                IdentityUser existedUser = await _userManager.FindByEmailAsync(user.Email);
                if (existedUser != null)
                {
                    return this.StatusCode(StatusCodes.Status409Conflict, new MessageResponse() { Code = 409, Message = "Email address is already in used!" });
                }

                user.NormalizedEmail = user.Email;
                user.TwoFactorEnabled = true;

                var success = await this._userManager.CreateAsync(user, authDto.Password);

                if (success.Succeeded)
                {
                    Entitlement adminRole = new Entitlement { Bank_id = "", Name = _configuration.GetValue<string>("AppRoles:AdminRole") };
                    adminRole.NormalizedName = adminRole.Name;

                    if (!await _roleManager.RoleExistsAsync(adminRole.Name))
                    {
                        await _roleManager.CreateAsync(adminRole);
                    }

                    IEnumerable<string> roles = new List<string>() { adminRole.Name ?? "ADMIN" };
                    user = await _userManager.FindByNameAsync(user.UserName);

                    if (user != null)
                    {
                        await _userManager.AddToRolesAsync(user, roles);
                    }
                    return await SendEmailTokenToConfirm(user, false, true, authDto.Password);
                }
                else
                {
                    return BadRequest(success.Errors);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
        #endregion 

        #region Register
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest authDto)
        {
            try
            {
                UserModel user = new UserModel();
                user.Email = authDto.Email;

                IdentityUser existedUser = await _userManager.FindByEmailAsync(user.Email);
                if (existedUser != null)
                {
                    return this.StatusCode(StatusCodes.Status409Conflict, new MessageResponse() { Code = 409, Message = "Email address is already in used!" });
                }

                user.NormalizedEmail = user.Email;
                user.UserName = authDto.UserName;
                user.First_name = authDto.First_name;
                user.Last_name = authDto.Last_name;
                user.TwoFactorEnabled = true;
                user = UserProviderDetails.GetUriProviderDetails(Request, user);

                var success = await this._userManager.CreateAsync(user, authDto.Password);

                if (success.Succeeded)
                {
                    Entitlement customerRole = new Entitlement { Bank_id = "", Name = _configuration.GetValue<string>("AppRoles:CustomerRole") };
                    customerRole.NormalizedName = customerRole.Name;

                    if (!await _roleManager.RoleExistsAsync(customerRole.Name))
                    {
                        await _roleManager.CreateAsync(customerRole);
                    }

                    IEnumerable<string> roles = new List<string>() { customerRole.Name ?? "CUSTOMER" };
                    user = await _userManager.FindByNameAsync(user.UserName);

                    if (user != null)
                    {
                        await _userManager.AddToRolesAsync(user, roles);
                    }
                    return await SendEmailTokenToConfirm(user, true);
                }
                else
                {
                    return BadRequest(success.Errors);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
        #endregion 

        #region Authentication
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest authDto)
        {
            try
            {
                UserModel user = await _userManager.FindByNameAsync(authDto.Login);

                user = user == null ? await _userManager.FindByEmailAsync(authDto.Login) : user;

                if (user != null && await _userManager.CheckPasswordAsync(user, authDto.Password))
                {
                    if (!user.EmailConfirmed)
                    {
                        return await SendEmailTokenToConfirm(user, true);
                    }
                    try
                    {
                        await SetTwoFactorAuthentication(user);
                        return Ok(new AuthResponse() { Message = "Please check your phone number, a code has been sent to you by sms! or email!", Token = await _jwtUtils.GetNotAuthenticatedToken(user) });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Two factor authentication: sending sms failed! " + ex.Message);
                        return this.Ok(await _jwtUtils.GetToken(user));
                    }
                }
                else
                {
                    return this.StatusCode(StatusCodes.Status401Unauthorized, new MessageResponse() { Code = 401, Message = "Unauthorized: Bad Credentials!" });
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
        #endregion

        #region Phone Number manager
        [HttpPost]
        [Authorize]
        [Route("add-phone-number")]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, request.Number);
                SendSmsEvent message = new SendSmsEvent
                {
                    Subject = "Please check your phone number",
                    Destination = request.Number,
                    Body = "Your security code is: " + code,
                    ToEmail = user.Email
                };
                // Send token
                await _publish.Publish(message);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
        [HttpPut]
        [Authorize]
        [Route("verify-phone-number")]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var result = await _userManager.ChangePhoneNumberAsync(user, request.PhoneNumber, request.Code);

                if (result.Succeeded)
                {
                    return Ok(await _jwtUtils.GetToken(user));
                }
                _logger.LogError($"Phone verification failed Number: {request.PhoneNumber}");
                return Problem();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
        #endregion

        #region Forgot Password
        [HttpPost]
        [Route("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPassword)
        {
            try
            {
                UserModel user = await _userManager.FindByEmailAsync(forgotPassword.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    string body = "<p>Hey " + user.UserName + ",</p>" +
                          "</br>" +
                          "<p>A password reset request attempt requires further verification. To reset your password, use the verification token.</p> " +
                          "</br> " +
                          "<ul> " +
                          "<li>Token : " + token + "</li>" +
                          "</ul> " +
                          "</br>" +
                          "<p>Thanks,</p>";

                    SendEmailEvent email = new SendEmailEvent() { Body = body, Subject = "Please verify your Email", To = user.Email };
                    await _publish.Publish(email);
                    return this.StatusCode(StatusCodes.Status200OK, new MessageResponse() { Code = 200, Message = "Please verify your Email address!" });
                }
                else
                {
                    return this.StatusCode(StatusCodes.Status404NotFound, new MessageResponse() { Code = 404, Message = "OBP-20005: User not found. Please specify a valid value for USER_ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }
        #endregion

        #region Reset Password
        [HttpPut]
        [Route("Reset-Password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest passwordReset)
        {
            try
            {
                UserModel user = await _userManager.FindByEmailAsync(passwordReset.Email);
                if (user != null)
                {
                    var isPasswordReset = await _userManager.ResetPasswordAsync(user, passwordReset.Token, passwordReset.Password);

                    if (isPasswordReset.Succeeded)
                    {
                        return Ok(new AuthenticationResponse() { Email = user.Email, Login = user.Email, UserName = user.UserName, Token = null });
                    }
                    else
                    {
                        return Problem("Cannot reset your password, Please retry!");
                    }
                }
                return this.StatusCode(StatusCodes.Status404NotFound, passwordReset.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
        #endregion

        #region Email Confirmation
        [HttpPut]
        [Route("Confirm-Email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            try
            {
                if (request.Email == null || request.Token == null)
                {
                    return BadRequest();
                }

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return NotFound(user);
                }

                var result = await _userManager.ConfirmEmailAsync(user, request.Token);
                if (result.Succeeded)
                {
                    AuthResponse authResponse = await _jwtUtils.GetToken(user);
                    return Ok(new AuthenticationResponse() { Email = user.Email, Login = user.Email, UserName = user.UserName, Token = authResponse.Token });
                }

                return this.StatusCode(StatusCodes.Status400BadRequest, new MessageResponse() { Code = 400, Message = "Email not confirmed successfully!"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
        #endregion

        #region Logout
        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _tokenManager.DeactivateCurrentAsync();
                return this.StatusCode(StatusCodes.Status200OK, new MessageResponse() { Code = 200, Message = "You are logged out!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
        #endregion

        #region Private methods
        private async Task<IActionResult> SendEmailTokenToConfirm(UserModel user, bool isRegistration, bool? isAdminUser =false, string? password="")
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _logger.LogInformation("Sending email to address "+ user.Email+" with token :" + token);
            string body = "<p>Hey " + user.UserName + ",</p>" +
                          "</br>" +
                          "<p>A sign in attempt requires further verification. To complete the sign in, use the verification token.</p> " +
                          "</br> " +
                          "<ul> " +
                          "<li>Username :" + user.UserName + "</li>";
                                    
            if (isAdminUser == true)
            {
                body += "<li>Password : " + password + "</li>";
            }
            body += "<li>Token : " + token + "</li>"+
                     "</ul> " +
                     "</br>" +
                   "<p>Thanks,</p>";

            SendEmailEvent email = new SendEmailEvent() { Body = body, Subject = "Please verify your Email", To = user.Email };
            await _publish.Publish(email);
            return Ok(new MessageResponse() { Code = 200, Message = isRegistration == true? "Please verify your Email!" : "User added successfully!" });
        }

        private async Task<bool> SetTwoFactorAuthentication(UserModel user)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            SendSmsEvent message = new SendSmsEvent
            {
                Subject = "Please check your phone number",
                Destination = user.PhoneNumber,
                Body = "Your security code is: " + code,
                ToEmail = user.Email
            };
            await _publish.Publish(message);
            return true;
        }

        private string GetRandomString(int number)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[number];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
        
        #endregion
    }
}
