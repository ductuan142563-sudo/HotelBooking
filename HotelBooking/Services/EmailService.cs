namespace HotelBooking.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IWebHostEnvironment _env;

        public EmailService(ILogger<EmailService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {

            _logger.LogInformation("=== EMAIL DEMO ===");
            _logger.LogInformation("To: {Email}", toEmail);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {Body}", htmlMessage);

            // Lưu file email demo để dễ kiểm tra
            var emailsFolder = Path.Combine(_env.ContentRootPath, "Emails");
            if (!Directory.Exists(emailsFolder))
                Directory.CreateDirectory(emailsFolder);

            var fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{toEmail.Replace("@", "_at_")}.html";
            var filePath = Path.Combine(emailsFolder, fileName);

            var fullHtml = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'><title>{subject}</title></head>
<body>
    <h3>To: {toEmail}</h3>
    <h3>Subject: {subject}</h3>
    <hr/>
    {htmlMessage}
</body>
</html>";

            await File.WriteAllTextAsync(filePath, fullHtml);
            _logger.LogInformation("Email demo đã lưu tại: {Path}", filePath);

            await Task.CompletedTask;
        }
    }
}