using Invento.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [ApiController]
    [Route("api/email-test")]
    public class EmailTestController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailTestController(
            IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendTestEmail(
            string email)
        {
            await _emailService.SendEmailAsync(
                email,
                "Invento SMTP Test",
                @"
                <h2>SMTP is working successfully.</h2>

                <p>
                    This email was sent from the
                    Invento ERP System.
                </p>

                <p>
                    Congratulations!
                </p>
                ");

            return Ok("Email sent successfully.");
        }
    }
}