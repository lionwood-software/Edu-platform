using LionwoodSoftware.EventBus.Abstractions;
using LionwoodSoftware.EventBus.Events;
using Microsoft.Extensions.Logging;
using RouterApi.Domain.Enums;
using RouterApi.Interfaces.Repositories;
using System;

namespace RouterApi.Observers.Request
{
    public class RequestObserver
    {
        private readonly IEventBus _eventBus;
        private readonly IRequestRepository _requestRepository;
        private readonly ILogger<RequestObserver> _logger;

        public RequestObserver(
            IEventBus eventBus,
            IRequestRepository requestRepository,
            ILogger<RequestObserver> logger)
        {
            _eventBus = eventBus;
            _requestRepository = requestRepository;
            _logger = logger;
        }

        public async void SendNotificationAsync(object sender, RequestEventArgs e)
        {
            _logger.LogInformation("Executing SendNotification for event: {@event}", e);

            try
            {
                // send notification to user
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process UploadRequestAttachmentsAsync event");
            }
        }

        public async void AfterRejectActionsAsync(object sender, RequestEventArgs e)
        {
            _logger.LogInformation("Executing AfterRejectActions for event: {@event}", e);

            try
            {
                var request = await _requestRepository.GetByIdAsync(e.RequestId);

                if (request.Type == RequestType.Material)
                {
                    // send notification to other microservices
                    _eventBus.Publish(new IntegrationEvent());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process AfterRejectActions event");
            }
        }
    }
}
