using using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(config => config.AddConsole());
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer("Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True"));
        services.AddScoped<IEmailService, EmailService>();

        var serviceProvider = services.BuildServiceProvider();

        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        await emailService.SendBirthdayMailsAsync();

        Console.WriteLine("Mail job tamamlandı, program kapanıyor.");
    }
}