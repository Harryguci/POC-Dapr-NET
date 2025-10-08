using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using StockProtos;
using System.Net.Sockets;

namespace FintechDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceManagersController : ControllerBase
    {
        private readonly ILogger<FinanceManagersController> _logger;
        private readonly DaprClient _daprClient;

        public FinanceManagersController(ILogger<FinanceManagersController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }
        [HttpGet]
        public ActionResult Get()
        {
            return Content("Running...");
        }
        
        [HttpPost]
        public async Task<IActionResult> StockUpdate([FromBody] StockUpdateRequest data)
        {
            _logger.LogInformation($"Invoking stock service with: {data}");
            
            try
            {
                // Try HTTP endpoint first
                var response = await _daprClient.InvokeMethodAsync<StockUpdateRequest, StockUpdateResponse>("stockmodule", "api/stocks/UpdateStock", data);
                _logger.LogInformation($"Service responded: {response.Message}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error invoking service: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
