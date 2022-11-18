# PocRedis
POC usando dotnet e Redis como cache.

# Criação de ambiente
Criar um Redis localmente com docker: 

```bash
docker run -p 6379:6379 --name redis -d redis
```


# Configuração do appsettings.json
Adicionar à lista de ConnectionStrings:
```jsonc
"ConnectionStrings": {
    "Redis": "localhost:6379,ssl=false,abortConnect=false",

    // Exemplo AWS
    // "host.da.aws.region.cache.amazonaws.com:6379,password=sua-senha-secreta,ssl=true,abortConnect=false
}
```

# Configuração da aplicação

1- Adicionar a lib abaixo ao projeto:
```
Microsoft.Extensions.Caching.StackExchangeRedis
```

2- Configurar o container de injeção de dependência para usar o Redis como provedor de IDistributedCache.
```csharp
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

if (redisConnectionString == null)
{
    // Se não houver connectionString de Redis, usa cache em memória.
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddStackExchangeRedisCache(s =>
    {
        s.Configuration = redisConnectionString;
        
        // Isso é um prefixo que será adicionado às keys (tanto Get quanto Set).
        s.InstanceName = "poc-redis-"; 
    });
}
```

3- Feito isso, basta usar a interface IDistributedCache nos serviços. 
```csharp
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
```

4- Para visualizar os dados que estão sendo gravados no cache, pode ser usado o [Another Redis Desktop Manager](https://github.com/qishibo/AnotherRedisDesktopManager).