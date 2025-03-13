using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Data;
using System.Net.Mime;

namespace Services
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ApplicationDbContext context, ILogger<EmailService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SendEmailAsync(Employees employee, string templateType)
        {
            try
            {
                var emailTemplate = await _context.EmailTemplates
                    .FirstOrDefaultAsync(t => t.Type == templateType) 
                    ?? throw new InvalidOperationException($"{templateType} template bulunamadı.");

                var smtpSettings = await _context.SmtpSettings.FirstOrDefaultAsync();
                if (smtpSettings == null)
                    throw new Exception("SMTP ayarları bulunamadı.");   

                var body = emailTemplate.Body;

                // Yıl hesaplama ve body içine yerleştirme
                if (templateType == "WorkAnniversary")
                {
                    var years = (DateTime.Now.Year - employee.HireDate.Year);
                    body = body?.Replace("{Years}", years.ToString());
                }

                // Çalışan adını body içine yerleştirme
                body = body?.Replace("{Name}", employee.Name);

                // Image ID'yi ayarla
                var imageId = $"{templateType.ToLower()}_image";
                body = body?.Replace("{Image}", $"cid:{imageId}");

                Console.WriteLine($"Email Body: {body}"); // Debug için body'i logla

                try
                {
                    using var message = new MailMessage
                    {
                        From = new MailAddress(smtpSettings.Username),
                        Body = body,
                        IsBodyHtml = true,
                        Subject = emailTemplate.Subject
                    };

                    message.To.Add(new MailAddress(employee.Email));

                    // Add BCC addresses
                    foreach (var bccEmail in employee.BccEmails.Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var trimmedEmail = bccEmail.Trim();
                        if (!string.IsNullOrEmpty(trimmedEmail))
                        {
                            try
                            {
                                var mailAddress = new MailAddress(trimmedEmail);
                                message.Bcc.Add(mailAddress);
                            }
                            catch (FormatException)
                            {
                                _logger.LogWarning($"Invalid email format: {trimmedEmail}");
                            }
                        }
                    }

                    // Resim yolunu belirle
                    string imagePath;
                    if (!string.IsNullOrEmpty(emailTemplate.ImagePath))
                    {
                        imagePath = emailTemplate.ImagePath;
                    }
                    else
                    {
                        // Varsayılan resim yolu
                        imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", 
                            templateType == "Birthday" ? "birthday.png" : "workanniversary.png");
                    }

                    // Trim any extra slashes from the image path
                    imagePath = imagePath.TrimEnd('/', '\\');

                    Console.WriteLine($"Image Path: {imagePath}"); // Debug için image path'i logla

                    // Inline image ekleme
                    if (File.Exists(imagePath) && body !=null)
                    {
                        var linkedResource = new LinkedResource(imagePath)
                        {
                            ContentId = imageId,
                            TransferEncoding = TransferEncoding.Base64
                        };

                        var alternateView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                        alternateView.LinkedResources.Add(linkedResource);
                        message.AlternateViews.Add(alternateView);
                    }
                    else
                    {
                        _logger.LogWarning($"Image not found at path: {imagePath}");
                    }

                    using var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
                    {
                        EnableSsl = true,
                        Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password)
                    };

                    await smtpClient.SendMailAsync(message);
                }
                catch (FormatException ex)
                {
                    _logger.LogWarning($"Invalid email format in SMTP settings: {smtpSettings.Username}");
                    Console.WriteLine($"Email sending failed: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw;
            }
        }

        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
