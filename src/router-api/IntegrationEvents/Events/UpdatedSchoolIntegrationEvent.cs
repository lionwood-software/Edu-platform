using LionwoodSoftware.EventBus.Events;
using Newtonsoft.Json;

namespace RouterApi.IntegrationEvents.Events
{
    public class UpdatedSchoolIntegrationEvent : IntegrationEvent
    {
        public UpdatedSchoolIntegrationEvent(string schoolUId, string name, string fullName, string type, string form) : base()
        {
            SchoolUId = schoolUId;
            Name = name;
            FullName = fullName;
            Type = type;
            Form = form;
        }

        [JsonProperty]
        public string SchoolUId { get; private set; }

        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public string FullName { get; private set; }

        [JsonProperty]
        public string Type { get; private set; }

        [JsonProperty]
        public string Form { get; private set; }
    }
}
