// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using WebApplication1.Services;

// namespace WebApplication1.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class SharePointWebhookController : ControllerBase
//     {
//         private readonly ISharePointService _sharePointService;
//         private readonly ILogger<SharePointWebhookController> _logger;

//         public SharePointWebhookController(
//             ISharePointService sharePointService,
//             ILogger<SharePointWebhookController> logger)
//         {
//             _sharePointService = sharePointService;
//             _logger = logger;
//         }

//         [HttpPost("notification")]
//         public async Task<IActionResult> HandleNotification([FromBody] SharePointNotification notification)
//         {
//             try
//             {
//                 _logger.LogInformation($"SharePoint değişiklik bildirimi alındı: {notification.Resource}");

//                 // SharePoint'ten güncel verileri çek
//                 var employees = await _sharePointService.GetEmployeesFromSharePointAsync();
                
//                 // Veritabanını güncelle
//                 // foreach (var employee in employees)
//                 // {
//                 //     await _sharePointService.SaveEmployeeAsync(employee);
//                 // }

//                 _logger.LogInformation($"Çalışan verileri güncellendi. Toplam {employees.Count} çalışan işlendi.");
//                 return Ok();
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "SharePoint webhook işleme hatası");
//                 return StatusCode(500, "İşlem sırasında bir hata oluştu.");
//             }
//         }
//     }

//     public class SharePointNotification
//     {
//         public string Resource { get; set; } = string.Empty;
//         public List<ChangeToken> Changes { get; set; } = new();
//     }

//     public class ChangeToken
//     {
//         public string ChangeType { get; set; } = string.Empty;
//         public string ItemId { get; set; } = string.Empty;
//     }
// } 