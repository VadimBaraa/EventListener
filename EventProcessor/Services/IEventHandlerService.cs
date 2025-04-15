using EventShared.Models;

namespace EventProcessor.Services
{
    public interface IEventHandlerService
    {
        Task HandleEventAsync(Event evt);
    }
}
