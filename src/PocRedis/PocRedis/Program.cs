var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

if (redisConnectionString == null)
{
    // Se n�o houver connectionString de Redis, usa cache em mem�ria.
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddStackExchangeRedisCache(s =>
    {
        s.Configuration = redisConnectionString;
        
        // Isso � um prefixo que ser� adicionado �s keys (tanto Get quanto Set).
        s.InstanceName = "poc-redis:"; 
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
