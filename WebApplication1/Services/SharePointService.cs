using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Services
{
    // public class SharePointService : ISharePointService
    // {
    //     private readonly HttpClient _httpClient;
    //     private readonly ApplicationDbContext _context;

    //     public SharePointService(HttpClient httpClient, ApplicationDbContext context)
    //     {
    //         _httpClient = httpClient;
    //         _context = context;
    //     }

        // public async Task<List<Employee>> GetEmployeesFromSharePointAsync()
        // {
        //     var apiSettings = await _context.ApiSettings
        //         .OrderBy(s => s.Id)
        //         .FirstOrDefaultAsync() ?? throw new InvalidOperationException("API ayarları bulunamadı.");

        //     _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync(apiSettings));
            
        //     // SharePoint URL'sini oluştur
        //     var sharePointUrl = $"{apiSettings.BaseUrl}/sites/{apiSettings.SiteName}";
        //     var response = await _httpClient.GetAsync($"{sharePointUrl}/_api/web/Lists/getbytitle('{apiSettings.ListName}')/items");
        //     response.EnsureSuccessStatusCode();
            
        //     var jsonResponse = await response.Content.ReadAsStringAsync();
        //     var responseData = JsonSerializer.Deserialize<SharePointResponse<SharePointEmployeeViewModel>>(
        //         jsonResponse,
        //         new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        //     );

        //     if (responseData?.Value == null || !responseData.Value.Any())
        //         throw new Exception("SharePoint'ten veri alınamadı.");

        //     return responseData.Value.Select(item => new Employee
        //     {
        //         Name = item.Title,
        //         Email = NormalizeEmails(item.Email),
        //         BccEmails = NormalizeEmails(item.BccEmails),
        //         BirthDate = item.BirthDate,
        //         HireDate = item.HireDate
        //     }).ToList();
        // }

        // public async Task SaveEmployeeAsync(Employee employee)
        // {
        //     // Email'leri normalize et
        //     employee.Email = NormalizeEmails(employee.Email);
        //     employee.BccEmails = NormalizeEmails(employee.BccEmails);

        //     // Veritabanına kaydet
        //     if (employee.Id == 0)
        //     {
        //         _context.Employees.Add(employee);
        //     }
        //     else
        //     {
        //         _context.Entry(employee).State = EntityState.Modified;
        //     }

        //     await _context.SaveChangesAsync();
        // }

        // private static string NormalizeEmails(string? emails)
        // {
        //     if (string.IsNullOrEmpty(emails)) return string.Empty;

        //     return string.Join(";", 
        //         emails.Split(';', StringSplitOptions.RemoveEmptyEntries)
        //             .Select(e => e.Trim().ToLower())
        //             .Where(IsValidEmail)
        //             .Distinct()
        //             .OrderBy(e => e));
        // }

        // private static bool IsValidEmail(string email)
        // {
        //     try
        //     {
        //         var addr = new System.Net.Mail.MailAddress(email);
        //         return addr.Address == email;
        //     }
        //     catch
        //     {
        //         return false;
        //     }
        // }

        // private async Task<string> GetAccessTokenAsync(ApiSettings apiSettings)
        // {
        //     var tokenEndpoint = $"https://login.microsoftonline.com/{apiSettings.TenantId}/oauth2/token";
            
        //     var content = new FormUrlEncodedContent(new Dictionary<string, string>
        //     {
        //         { "grant_type", "client_credentials" },
        //         { "client_id", apiSettings.ClientId },
        //         { "client_secret", apiSettings.ClientSecret },
        //         { "resource", "00000003-0000-0ff1-ce00-000000000000" },
        //         { "scope", "https://doqu365.sharepoint.com/.default" }
        //     });

        //     var response = await _httpClient.PostAsync(tokenEndpoint, content);
            
        //     if (!response.IsSuccessStatusCode)
        //     {
        //         var errorMessage = await response.Content.ReadAsStringAsync();
        //         throw new Exception($"Token alma başarısız oldu. StatusCode: {response.StatusCode}, Hata Mesajı: {errorMessage}");
        //     }

        //     var responseBody = await response.Content.ReadAsStringAsync();
        //     using var doc = JsonDocument.Parse(responseBody);
        //     return doc.RootElement.GetProperty("access_token").GetString() 
        //         ?? throw new InvalidOperationException("Access token alınamadı.");
        // }
    //}
}