using Dapr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StockProtos;

namespace StockModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        public static (string, double)[] Stocks = new[] {
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

        [HttpPost("UpdateStock")]
        public ActionResult<StockUpdateResponse> UpdateStock([FromBody] StockUpdateRequest request)
        {
            _logger.LogInformation($"Received stock update via HTTP: {request.Symbol} - {request.Price}");

            var stockIndex = -1;
            for (int i = 0; i < Stocks.Length; i++)
            {
                if (Stocks[i].Item1 == request.Symbol)
                {
                    stockIndex = i;
                    break;
                }
            }

            if (stockIndex == -1)
            {
                _logger.LogWarning($"Stock {request.Symbol} not found.");
                return Ok(new StockUpdateResponse { Success = false, Message = $"Stock {request.Symbol} not found." });
            }

            Stocks[stockIndex] = (request.Symbol, request.Price);
            _logger.LogInformation($"Updated stock {request.Symbol} price to {request.Price}");

            return Ok(new StockUpdateResponse { Success = true, Message = "Stock updated successfully." });
        }
    }
    public class StockDto
    {
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