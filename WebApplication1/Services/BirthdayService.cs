using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data;
using Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class BirthdayService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BirthdayService> _logger;

        public BirthdayService(
            IServiceScopeFactory scopeFactory,
            ILogger<BirthdayService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var nextRun = now.Date.AddHours(9); // Her gün saat 09:00'da çalışacak
                    
                    // Eğer şu an saat 09:00'dan sonraysa, bir sonraki günün 09:00'ını bekle
                    if (now > nextRun)
                    {
                        nextRun = nextRun.AddDays(1);
                    }

                    var delay = nextRun - now;
                    _logger.LogInformation("Bir sonraki çalışma zamanı: {NextRun}", nextRun);
                    await Task.Delay(delay, stoppingToken);

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        _logger.LogInformation("BirthdayService çalışmaya başladı - {Time}", DateTime.Now);
                        
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                        var employees = await context.Employees.ToListAsync();
                        _logger.LogInformation("Veritabanından {Count} çalışan yüklendi", employees.Count);

                        var today = DateTime.Today;
                        _logger.LogInformation("Bugünün tarihi: {Date}", today.ToString("dd/MM/yyyy"));

                        foreach (var employee in employees)
                        {
                            _logger.LogInformation(
                                "Çalışan kontrol ediliyor - Ad: {Name}, Doğum Günü: {BirthDate}, İşe Giriş: {HireDate}", 
                                employee.Name, 
                                employee.BirthDate.ToString("dd/MM/yyyy"), 
                                employee.HireDate.ToString("dd/MM/yyyy"));

                            if (employee.BirthDate.Month == today.Month && employee.BirthDate.Day == today.Day)
                            {
                                _logger.LogInformation("Doğum günü e-postası gönderiliyor: {Name}", employee.Name);
                                await emailService.SendEmailAsync(employee, "Birthday");
                            }

                            if (employee.HireDate.Month == today.Month && employee.HireDate.Day == today.Day)
                            {
                                _logger.LogInformation("İş yıldönümü e-postası gönderiliyor: {Name}", employee.Name);
                                await emailService.SendEmailAsync(employee, "WorkAnniversary");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BirthdayService'de hata oluştu");
                }
            }
        }
    }
}
