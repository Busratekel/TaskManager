using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Employees
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string BccEmails { get; set; } = string.Empty;
        
        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }
    }
}