using LionwoodSoftware.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using SchoolApi.IntegrationEvents.Events;
using SchoolApi.Services.Interfaces;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace SchoolApi.IntegrationEvents.EventHandling
{
    public class CreatedDefaultSchoolChatsIntegrationEventHandler : IIntegrationEventHandler<CreatedDefaultSchoolChatsIntegrationEvent>
    {
        private readonly ILogger<CreatedDefaultSchoolChatsIntegrationEventHandler> _logger;
        private readonly IChatService _chatService;

        public CreatedDefaultSchoolChatsIntegrationEventHandler(
            ILogger<CreatedDefaultSchoolChatsIntegrationEventHandler> logger,
            IChatService chatService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _chatService = chatService;
        }

        public async Task Handle(CreatedDefaultSchoolChatsIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                // business logic here...
            }
        }
    }
}
