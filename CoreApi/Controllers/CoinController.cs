using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowedOrigins")]
    [ApiController]
    public class CoinController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        private static bool isCoinListCaching = false;
        private static bool isGlobalMetricsCaching = false;

        public CoinController(IMemoryCache memoryCache, IHttpClientFactory clientFactory)
        {
            _cache = memoryCache;
            _clientFactory = clientFactory;
        }

        // GET: api/Coin
        [HttpGet]
        public async Task<string> Get()
        {
            var cacheKey = "CoinList";

            // Look for cache key.
            if (!_cache.TryGetValue(cacheKey, out string cacheEntry) && !isCoinListCaching)
            {
                // Key not in cache, so get data.
                isCoinListCaching = true;
                var client = _clientFactory.CreateClient("CoinMarketCap"); // Startup.cs -> ConfigureServices
                HttpResponseMessage response = await client.GetAsync("v1/cryptocurrency/listings/latest");

                if (response.IsSuccessStatusCode)
                {
                    cacheEntry = await response.Content.ReadAsStringAsync();
                    CacheInMemory(cacheKey, cacheEntry, 5);
                }
                isCoinListCaching = false;
            }
            
            return cacheEntry;
        }

        [Route("~/api/[controller]/metrics")]
        [HttpGet]
        public async Task<string> Metrics()
        {
            var cacheKey = "GlobalMetrics";

            // Look for cache key.
            if (!_cache.TryGetValue(cacheKey, out string cacheEntry) && !isGlobalMetricsCaching)
            {
                // Key not in cache, so get data.
                isGlobalMetricsCaching = true;
                var client = _clientFactory.CreateClient("CoinMarketCap"); // Startup.cs -> ConfigureServices
                HttpResponseMessage response = await client.GetAsync("v1/global-metrics/quotes/latest");

                if (response.IsSuccessStatusCode)
                {
                    cacheEntry = await response.Content.ReadAsStringAsync();
                    CacheInMemory(cacheKey, cacheEntry, 5);
                }
                isGlobalMetricsCaching = false;
            }

            return cacheEntry;
        }

        [NonAction]
        private void CacheInMemory(string cacheKey, string cacheEntry, int expireMinute)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(expireMinute));
                        //.RegisterPostEvictionCallback(callback: EvictionCallback, state: this);
            _cache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            //System.Diagnostics.Debug.WriteLine($"Cached {cacheKey}: {DateTime.Now}");
        }

        [NonAction]
        private static void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            System.Diagnostics.Debug.WriteLine("EvictionCallback");
        }

        [Route("~/api/keepalive")]
        [HttpGet]
        public IActionResult KeepAlive()
        {
            return new OkObjectResult("Ok");
        }

        // GET: api/Coin/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Coin
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Coin/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
