using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

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
            await _daprClient.PublishEventAsync("pubsub", "stock-updates", data);
            Console.WriteLine($"Published: {data.ToString()}");
            return Ok(new { data });
        }
    }

    public class StockUpdateRequest {
        public required string Symbol { get; set; }
        public double Price { get; set; }
    }
}
