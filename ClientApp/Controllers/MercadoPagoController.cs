using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MercadoPagoController(ILogger<MercadoPagoController> logger) : ControllerBase
    {
        [HttpGet("webhook")]
        [HttpPost("webhook")]
        public async Task<IActionResult> LegacyWebhook()
        {
            Request.EnableBuffering();

            string body;
            using (var reader = new StreamReader(Request.Body, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            logger.LogInformation(
                "MercadoPago legacy webhook recibido. Method: {Method}, Path: {Path}, Query: {Query}, Headers: {@Headers}, Body: {Body}",
                Request.Method,
                Request.Path,
                Request.QueryString.Value,
                Request.Headers.ToDictionary(header => header.Key, header => header.Value.ToString()),
                body);

            return Ok();
        }
    }
}
