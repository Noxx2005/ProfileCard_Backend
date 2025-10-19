using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ProfileApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public MeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            // Your details
            var user = new
            {
                email = "Tislohbot2022@gmail.com",
                name = "Bot Peter Tisloh",
                stack = ".NET Core 8 / ASP.NET Core Web API"
            };

            string catFact = "Could not fetch cat fact at the moment.";

            try
            {
                // Fetch random cat fact
                _httpClient.Timeout = TimeSpan.FromSeconds(5);
                var response = await _httpClient.GetFromJsonAsync<CatFactResponse>("https://catfact.ninja/fact");

                if (response != null && !string.IsNullOrWhiteSpace(response.Fact))
                {
                    catFact = response.Fact;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cat fact API error: {ex.Message}");
            }

            var result = new
            {
                status = "success",
                user,
                timestamp = DateTime.UtcNow.ToString("o"),
                fact = catFact
            };

            return Ok(result);
        }

        private class CatFactResponse
        {
            public string? Fact { get; set; }
        }
    }
}
