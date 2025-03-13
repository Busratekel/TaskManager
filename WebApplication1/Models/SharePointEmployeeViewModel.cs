using System.Text.Json.Serialization;

namespace Models
{
    public class SharePointEmployeeViewModel
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("Email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("BccEmails")]
       public List<string> BccEmails { get; set; } = new List<string>();

        [JsonPropertyName("BirthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("HireDate")]
        public DateTime HireDate { get; set; }
    }
} 