using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Data;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace Services
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ApplicationDbContext context, ILogger<EmailService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SendEmailAsync(Employees employee, string templateType)
        {
            try
            {
                // E-posta şablonu ve SMTP ayarlarını al
                var emailTemplate = await _context.EmailTemplates
                    .FirstOrDefaultAsync(t => t.Type == templateType) 
                    ?? throw new InvalidOperationException($"{templateType} template bulunamadı.");

                var smtpSettings = await _context.SmtpSettings.FirstOrDefaultAsync()
                    ?? throw new Exception("SMTP ayarları bulunamadı.");

                // E-posta gövdesini hazırla
                var body = emailTemplate.Body?.Replace("{Name}", employee.Name);

                // E-posta mesajını oluştur
                using var message = new MailMessage
                {
                    From = new MailAddress("insan.kaynaklari@doqu.com.tr"),
                    Subject = emailTemplate.Subject,
                    IsBodyHtml = true
                    //deneme
                };

                message.ReplyToList.Add(new MailAddress("insan.kaynaklari@doqu.com.tr"));
                message.To.Add(new MailAddress(employee.Email));

                // BCC e-postaları ekle
                foreach (var bccEmail in employee.BccEmails.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimmedEmail = bccEmail.Trim();
                    if (!string.IsNullOrEmpty(trimmedEmail))
                    {
                        message.Bcc.Add(new MailAddress(trimmedEmail));
                    }
                }

                // Yıl hesaplama (sadece iş yıldönümü için)
                int years = 0;
                if (templateType.ToLower() == "workanniversary")
                {
                    years = CalculateWorkAnniversaryYears(employee.HireDate);
                    
                    // Kıdem yılı bilgisini ekle
                    body = body?.Replace("{Years}", years.ToString());
                }
                else
                {
                    // Doğum günü için Years placeholder'ını kaldır
                    body = body?.Replace("{Years}", "");
                }

                // Resim yolunu hazırla
                string imagePath = GetImagePath(emailTemplate.ImagePath, templateType, years);
                
                // Resmi e-postaya ekle
                if (!string.IsNullOrEmpty(imagePath))
                {
                    body = AddImageToEmailBody(message, body, imagePath);
                }
                else
                {
                    _logger.LogWarning($"Resim yolu belirtilmemiş: {templateType}");
                    body = body?.Replace("{Image}", "");
                }

                // Son düzenlenmiş body'yi mesaja ekle
                message.Body = body;

                // E-postayı gönder
                using var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password)
                };

                await smtpClient.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {employee.Email} for {templateType}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Email service error for {employee.Email}");
                throw;
            }
        }
        
        // Kıdem yılını hesapla
        private int CalculateWorkAnniversaryYears(DateTime hireDate)
        {
            var today = DateTime.Now;
            var years = today.Year - hireDate.Year;
            
            // Yıl içinde tarih kontrolü
            if (today.Month < hireDate.Month || (today.Month == hireDate.Month && today.Day < hireDate.Day))
            {
                years--;
            }
            
            // Minimum 1 yıl olarak ayarla
            return Math.Max(1, years);
        }
        
        // Resim yolunu belirle
        private string GetImagePath(string basePath, string templateType, int years)
        {
            if (string.IsNullOrEmpty(basePath))
                return string.Empty;
                
            bool isUrl = basePath.StartsWith("http://") || basePath.StartsWith("https://");
            string fileName = templateType.ToLower() == "workanniversary" 
                ? $"Kidem_{years}.jpg" 
                : "Dugum_Gunu.jpg";
                
            return isUrl 
                ? basePath.TrimEnd('/') + "/" + fileName
                : Path.Combine(basePath, fileName);
        }
        
        // Resmi e-posta gövdesine ekle
        private string? AddImageToEmailBody(MailMessage message, string body, string imagePath)
        {
            bool isUrl = imagePath.StartsWith("http://") || imagePath.StartsWith("https://");
            
            _logger.LogInformation($"Adding image: {imagePath}, Is URL: {isUrl}");
            
            try
            {
                if (isUrl)
                {
                    // URL resimleri için HTML img etiketi kullan
                   return body?.Replace("{Image}", imagePath);
                }
                else if (File.Exists(imagePath))
                {
                    // Yerel resim dosyaları için ContentID kullan
                    var contentId = Guid.NewGuid().ToString();
                    var htmlBody = body?.Replace("{Image}", $"<img src=\"cid:{contentId}\" alt=\"Doqu Email\" style=\"max-width:100%;\" />");
                    
                    var alternateView = AlternateView.CreateAlternateViewFromString(
                        htmlBody, null, MediaTypeNames.Text.Html);
                    
                    var linkedResource = new LinkedResource(imagePath)
                    {
                        ContentId = contentId,
                        ContentType = new ContentType("image/jpg")
                    };
                    
                    alternateView.LinkedResources.Add(linkedResource);
                    message.AlternateViews.Add(alternateView);
                    
                    return htmlBody;
                }
                else
                {
                    _logger.LogWarning($"Resim dosyası bulunamadı: {imagePath}");
                    return body?.Replace("{Image}", "");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Resim eklenirken hata oluştu: {imagePath}");
                return body?.Replace("{Image}", "");
            }
        }
    }
}