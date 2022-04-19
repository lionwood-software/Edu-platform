using RouterApi.Domain.Enums;
using System;
using System.Collections.Generic;

namespace RouterApi.Models.Request
{
    public class RequestVM
    {
        public RequestVM()
        {
            Child = new RequestChildVM();
            Sender = new RequestSenderVM();
            School = new RequestSchoolVM();
            Attachments = new List<RequestAttachmentVM>();
        }

        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public RequestSenderVM Sender { get; set; }

        public RequestSchoolVM School { get; set; }

        public RequestChildVM Child { get; set; }

        public RequestStatus Status { get; set; }

        public string Class { get; set; }

        public RequestType Type { get; set; }

        public string RejectDecision { get; set; }

        public List<RequestAttachmentVM> Attachments { get; set; }

        public bool CanConfirm { get; set; }

        public RequestMarkVM Mark { get; set; }

        public string ChangedByUserId { get; set; }
    }
}
