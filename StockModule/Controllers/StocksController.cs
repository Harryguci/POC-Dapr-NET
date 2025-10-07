using Dapr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace StockModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private static (string, double)[] Stocks = new[] {
            ("MSFT", 280.98),
            ("AAPL", 145.32),
            ("GOOGL", 2729.25),
            ("AMZN", 3342.88),
            ("TSLA", 688.99)
        };

        private readonly ILogger<StocksController> _logger;
        public StocksController(ILogger<StocksController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<StockDto>> Get()
        {
            var res = Enumerable.Range(1, 5)
                .Select(index => new StockDto()
                {
                    Symbol = Stocks[index - 1].Item1,
                    Price = Stocks[index - 1].Item2
                })
                .ToArray();

            return Ok(res);
        }

        [Topic("pubsub", "stock-updates")]
        [HttpPost("updates")]
        public IActionResult ReceiveStockUpdate([FromBody] StockUpdate stockUpdate)
        {
            _logger.LogInformation($"Received stock update: {stockUpdate}");
            
            // Find the index of the stock to update
            var stockIndex = -1;
            for (int i = 0; i < Stocks.Length; i++)
            {
                if (Stocks[i].Item1 == stockUpdate.Symbol)
                {
                    stockIndex = i;
                    break;
                }
            }

            if (stockIndex == -1)
            {
                _logger.LogWarning($"Stock {stockUpdate.Symbol} not found.");
                return NotFound();
            }

            // Update the stock price in the array
            Stocks[stockIndex] = (stockUpdate.Symbol, stockUpdate.Price);
            _logger.LogInformation($"Updated stock {stockUpdate.Symbol} price to {stockUpdate.Price}");

            return Ok();
        }
    }

    public class StockDto {
        public required string Symbol { get; set; }
        public double Price { get; set; }
    }

    public class StockUpdate
    {
        public required string Symbol { get; set; }
        public double Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
