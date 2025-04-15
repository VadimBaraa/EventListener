using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using EventGenerator.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpClient("ProcessorClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5002");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();

// Добавляем поддержку Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "EventGenerator API", 
        Version = "v1",
        Description = "API для генерации событий"

        
    });
    
    c.UseInlineDefinitionsForEnums();
    // Добавляем фильтр для группировки эндпоинтов
    c.TagActionsBy(api => new[] { "EventGenerator" });
});

builder.Services.AddHostedService<EventGeneratorService>();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventGenerator API V1");
    c.SwaggerEndpoint("http://localhost:5002/swagger/v1/swagger.json", "EventProcessor API V1");
});


app.MapControllers();

app.Run();
