# PocRedis
POC usando dotnet e Redis como cache.

# Criação de ambiente
Criar um Redis localmente com docker: 

```
docker run -p 6379:6379 --name redis -d redis
```


# Configuração do appsettings.json
Adicionar à lista de ConnectionStrings:
```
"ConnectionStrings": {
    "Redis": "localhost:6379,ssl=false,abortConnect=false",

    // Exemplo AWS
    // "host.da.aws.region.cache.amazonaws.com:6379,password=sua-senha-secreta,ssl=true,abortConnect=false
}
```

# Configuração da aplicação

1- Adicionar a lib Microsoft.Extensions.Caching.StackExchangeRedis.

2- Configurar o container de injeção de dependência para usar o Redis como provedor de IDistributedCache.
```csharp
var redisConnection = builder.Configuration.GetConnectionString("Redis");

if (redisConnection == null)
{
    // Se não houver connectionString de Redis, usa cache em memória.
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddStackExchangeRedisCache(s =>
    {
        s.Configuration = redisConnection;
        s.InstanceName = "poc-redis"; // Isso é um prefixo que será adicionado às keys (tanto Get quanto Set).
    });
}
```

3- Feito isso, basta usar a interface IDistributedCache nos serviços. 