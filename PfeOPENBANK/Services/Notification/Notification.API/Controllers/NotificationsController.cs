using Helper.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.API.Features.Commands;
using Notification.API.Models;
using Notification.API.Services.Interfaces;

namespace Notification.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _sender;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(IMediator sender, ILogger<NotificationsController> logger)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendEmail(Email email)
        {
            try
            {
                SendEmailCmd cmd = new SendEmailCmd() { Emailrequest = email };
                return Ok(await _sender.Send(cmd));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendSms(IdentityMessage message)
        {
            try
            {
                SendSmsCmd cmd = new SendSmsCmd() { SmsMessage = message };
                return Ok(await _sender.Send(cmd));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return this.StatusCode(500, new MessageResponse() { Code = 500, Message = "OBP-50000: Unknown Error." });
            }
        }
    }
}
