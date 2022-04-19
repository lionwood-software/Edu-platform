using RouterApi.Domain.Enums;
using System;

namespace RouterApi.Models.Request
{
    public class RequestMarkVM
    {
        public string Class { get; set; }

        public string Id { get; set; }

        public int? NewMarkRating { get; set; }

        public bool NewMarkPresence { get; set; }

        public string ReasonOfChanging { get; set; }

        public MarkSubType SubType { get; set; }

        public string Subject { get; set; }

        public DateTime Day { get; set; }

        public int? OldMarkRating { get; set; }

        public bool OldMarkPresence { get; set; }
    }
}
