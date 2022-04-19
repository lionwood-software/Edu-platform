using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace RouterApi.Observers.Request
{
    public class RequestEventArgs : EventArgs
    {
        public string RequestId { get; set; }

        public string ChangedByUserId { get; set; }

        public string OperationType { get; set; }

        public List<IFormFile> Attachments { get; set; }
    }
}
