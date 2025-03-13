using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) { }

        public DbSet<Employees> Employees { get; set; }
        public DbSet<SmtpSettings> SmtpSettings { get; set; }
        public DbSet<ApiSettings> ApiSettings { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
    }
}
