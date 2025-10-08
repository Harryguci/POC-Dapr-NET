using Grpc.Core;
using StockProtos;

namespace StockModule.Services
{
    public class StockGrpcService : StockService.StockServiceBase
    {
        private readonly ILogger<StockGrpcService> _logger;
        public StockGrpcService(ILogger<StockGrpcService> logger)
        {
            _logger = logger;
        }

        public override Task<StockUpdateResponse> UpdateStock(StockUpdateRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Received stock update via gRPC: {request.Symbol} - {request.Price}");

            var stockIndex = -1;
            for (int i = 0; i < Controllers.StocksController.Stocks.Length; i++)
            {
                if (Controllers.StocksController.Stocks[i].Item1 == request.Symbol)
                {
                    stockIndex = i;
                    break;
                }
            }

            if (stockIndex == -1)
            {
                _logger.LogWarning($"Stock {request.Symbol} not found.");
                return Task.FromResult(new StockUpdateResponse { Success = false, Message = $"Stock {request.Symbol} not found." });
            }

            Controllers.StocksController.Stocks[stockIndex] = (request.Symbol, request.Price);
            _logger.LogInformation($"Updated stock {request.Symbol} price to {request.Price}");

            return Task.FromResult(new StockUpdateResponse { Success = true, Message = "Stock updated successfully." });
        }
    }
}
