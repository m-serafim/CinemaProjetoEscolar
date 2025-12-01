using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CinemaGestao2223226.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Simulate email sending by logging to console
            _logger.LogInformation("===== EMAIL SIMULATION =====");
            _logger.LogInformation($"To: {email}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Message: {htmlMessage}");
            _logger.LogInformation("============================");
            
            // In a real application, you would send the email here using SendGrid, SMTP, etc.
            return Task.CompletedTask;
        }
    }
}