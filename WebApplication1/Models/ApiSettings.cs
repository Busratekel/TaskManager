namespace Models
{
    public class ApiSettings
    {
        public int Id { get; set; }
        public required string BaseUrl { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string TenantId { get; set; }
        public required string ListName { get; set; }
        public required string SiteName { get; set; }
    }
}