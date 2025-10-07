using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace FintechDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CounterController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private const string StoreName = "statestore";
        private const string Key = "counter";

        public CounterController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var value = await _daprClient.GetStateAsync<int>(StoreName, Key);
            return Ok(value);
        }

        [HttpPost("increment")]
        public async Task<IActionResult> Increment()
        {
            var value = await _daprClient.GetStateAsync<int>(StoreName, Key);
            value++;
            await _daprClient.SaveStateAsync(StoreName, Key, value);
            return Ok(value);
        }
    }
}
