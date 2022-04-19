using System;
using System.ComponentModel.DataAnnotations;

namespace RouterApi.Models.Request
{
    public class CreateMarkRequestVM
    {
        public string MarkId { get; set; }

        public int? NewMarkRating { get; set; }

        public bool NewMarkPresence { get; set; }

        [Required]
        public string ReasonOfChanging { get; set; }

        public string Class { get; set; }

        [Required]
        public string ChildId { get; set; }

        public string Subject { get; set; }

        public DateTime MarkDay { get; set; }

        public int? OldMarkRating { get; set; }

        public bool OldMarkPresence { get; set; }

        public string GroupId { get; set; }

        public string Description { get; set; }

        [Required]
        public string ColumnId { get; set; }

        [Required]
        public string ClassId { get; set; }

        public bool IsDeleting { get; set; }

        public int? CustomMark { get; set; }
    }
}
