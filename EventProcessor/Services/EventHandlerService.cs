using EventShared.Models;
using EventProcessor.Data;
using Microsoft.EntityFrameworkCore;

namespace EventProcessor.Services
{
    public class EventHandlerService : IEventHandlerService
    {
        private readonly AppDbContext _db;

        public EventHandlerService(AppDbContext db)
        {
            _db = db;
        }

        public async Task HandleEventAsync(Event evt)
        {
            // Сохраняем событие
            _db.Events.Add(evt);
            await _db.SaveChangesAsync();

            // Проверяем шаблоны
            switch (evt.Type)
{
    case EventType.Type1:
        // Простой шаблон: сразу создаем инцидент Type1
        _db.Incidents.Add(new Incident
        {
            Id = Guid.NewGuid(),
            Type = IncidentType.Type1,
            Time = DateTime.UtcNow,
            EventIds = new List<Guid> { evt.Id }
        });
        break;

    case EventType.Type2:
        // Составной шаблон: ищем Type1 в течение 20 сек
        var startTime = evt.Time;
        var endTime = startTime.AddSeconds(20);

        var related = await _db.Events
            .Where(e => e.Type == EventType.Type1 && e.Time >= startTime && e.Time <= endTime)
            .ToListAsync();

        if (related.Any())
        {
            _db.Incidents.Add(new Incident
            {
                Id = Guid.NewGuid(),
                Type = IncidentType.Type2,
                Time = DateTime.UtcNow,
                EventIds = new List<Guid> { evt.Id }.Concat(related.Select(e => e.Id)).ToList()
            });
        }
        else
        {
            _db.Incidents.Add(new Incident
            {
                Id = Guid.NewGuid(),
                Type = IncidentType.Type1,
                Time = DateTime.UtcNow,
                EventIds = new List<Guid> { evt.Id }
            });
        }
        break;

    case EventType.Type3:
        // Промежуточно: сохраняем Type3, позже проверим если Type2 появился
        // В следующих версиях можно запустить фоновую проверку
        _db.Incidents.Add(new Incident
        {
            Id = Guid.NewGuid(),
            Type = IncidentType.Type1,
            Time = DateTime.UtcNow,
            EventIds = new List<Guid> { evt.Id }
        });
        break;
}


            await _db.SaveChangesAsync();
        }
    }
}
