var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
