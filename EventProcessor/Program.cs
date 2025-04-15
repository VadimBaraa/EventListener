using EventProcessor.Data;
using EventProcessor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "EventProcessor API", 
        Version = "v1",
        Description = "API для обработки событий"
    });
    
    // Добавляем фильтр для группировки эндпоинтов
    c.TagActionsBy(api => new[] { "EventProcessor" });
});
builder.Services.AddScoped<IEventHandlerService, EventHandlerService>();


// Получаем строку подключения из конфигурации
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// EF Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Применяем миграции

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventProcessor API V1");
    c.SwaggerEndpoint("http://localhost:5000/swagger/v1/swagger.json", "EventGenerator API V1");
});


app.MapControllers();

app.Run();
