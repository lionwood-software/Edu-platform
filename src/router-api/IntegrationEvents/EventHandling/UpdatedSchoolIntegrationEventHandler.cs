using LionwoodSoftware.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using RouterApi.IntegrationEvents.Events;
using System;
using System.Threading.Tasks;

namespace RouterApi.IntegrationEvents.EventHandling
{
    public class UpdatedSchoolIntegrationEventHandler : IIntegrationEventHandler<UpdatedSchoolIntegrationEvent>
    {
        private readonly ILogger<UpdatedSchoolIntegrationEvent> _logger;

        public UpdatedSchoolIntegrationEventHandler(ILogger<UpdatedSchoolIntegrationEvent> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UpdatedSchoolIntegrationEvent @event)
        {
            // business logic here...
        }
    }
}
