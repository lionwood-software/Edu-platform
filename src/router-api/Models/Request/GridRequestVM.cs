using RouterApi.Domain.Enums;
using System;

namespace RouterApi.Models.Request
{
    public class GridRequestVM
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public RequestSenderVM Sender { get; set; }

        public RequestType Type { get; set; }

        public RequestStatus Status { get; set; }

        public RequestChildVM Child { get; set; }

        public RequestMarkVM Mark { get; set; }
    }
}
