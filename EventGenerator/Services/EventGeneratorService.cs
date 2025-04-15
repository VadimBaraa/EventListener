using EventShared.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace EventGenerator.Services;

public class EventGeneratorService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EventGeneratorService> _logger;
    private static readonly Random _random = new();

    public EventGeneratorService(IHttpClientFactory httpClientFactory, ILogger<EventGeneratorService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _httpClientFactory.CreateClient("ProcessorClient");

        while (!stoppingToken.IsCancellationRequested)
        {
            var generatedEvent = new Event
            {
                Type = (EventType)_random.Next(1, 4),
                Time = DateTime.UtcNow
            };

            try
            {
                var response = await client.PostAsJsonAsync("/event/receive", generatedEvent, stoppingToken);

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Событие отправлено: {Type} в {Time}", generatedEvent.Type, generatedEvent.Time);
                else
                    _logger.LogWarning("Не удалось отправить событие. Статус: {Status}", response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке события");
            }

            // Ждём 10 секунд перед следующим событием
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
