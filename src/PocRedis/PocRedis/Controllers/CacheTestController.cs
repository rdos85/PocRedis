using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace PocRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheTestController : ControllerBase
    {

        private readonly ILogger<CacheTestController> _logger;
        private readonly IDistributedCache _distributedCache;

        private const string cacheKey = "key-teste";

        public CacheTestController(ILogger<CacheTestController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        [HttpGet, Route("ler")]
        public async Task<ActionResult> GetAsync()
        {
            var cacheValue = await _distributedCache.GetStringAsync(cacheKey);

            if (cacheValue == null)
                return NotFound($"Não foi encontrado valor no cache. Key: [{cacheKey}]");

            return Ok(cacheValue);
        }

        [HttpPost, Route("gravar")]
        public async Task<ActionResult> AddAsync(string valor, int duracaoSegundos)
        {
            await _distributedCache.SetStringAsync(cacheKey, valor, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(duracaoSegundos)
            });

            return Ok($"Valor gravado no cache. Key: [{cacheKey}]");
        }
    }
}