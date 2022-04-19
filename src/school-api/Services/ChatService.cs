using LionwoodSoftware.EventBus.Abstractions;
using LionwoodSoftware.Repository.Interfaces;
using SchoolApi.IntegrationEvents.Events;
using SchoolApi.Services.Interfaces;
using System.Threading.Tasks;

namespace SchoolApi.Services
{
    public class ChatService : IChatService
    {
        private readonly IRepository _repository;
        private readonly IEventBus _eventBus;

        public ChatService(IRepository repository, IEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        public async Task CreateDefaultChatsAsync()
        {
            var @event = new CreatedDefaultSchoolChatsIntegrationEvent();
            _eventBus.Publish(@event, @event.GetType().Name + "-" + Program.AppSchoolId);
        }
    }
}