using RouterApi.Domain.Enums;
using System;

namespace RouterApi.Domain.Entities.Request
{
    public class RequestMark
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

        public string GroupId { get; set; }

        public string Description { get; set; }

        public string ColumnId { get; set; }

        public string ClassId { get; set; }

        public bool IsDeleting { get; set; }

        public int? NewCustomMark { get; set; }

        public int? OldCustomMark { get; set; }
    }
}
