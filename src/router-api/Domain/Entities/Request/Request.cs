using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using RouterApi.Domain.Enums;

namespace RouterApi.Domain.Entities.Request
{
    [BsonIgnoreExtraElements]
    public class Request : BaseEntity
    {
        public Request()
        {
            WorkerClaims = new List<WorkerClaims>();
        }

        public RequestChild Child { get; set; }

        public RequestSender Sender { get; set; }

        public RequestSchool School { get; set; }

        public List<string> Receivers { get; set; }

        public RequestStatus Status { get; set; }

        public string Class { get; set; }

        public RequestType Type { get; set; }

        public string RejectDecision { get; set; }

        public ClassMaterial ClassMaterial { get; set; }

        public List<WorkerClaims> WorkerClaims { get; set; }

        public RequestMark Mark { get; set; }

        public string ChangedByUserId { get; set; }
    }
}
