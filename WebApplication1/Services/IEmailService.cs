using Models;

namespace Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(Employees employee,string type);
    }
}