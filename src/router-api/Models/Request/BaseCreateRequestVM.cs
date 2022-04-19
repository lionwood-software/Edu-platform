using Microsoft.AspNetCore.Http;
using RouterApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RouterApi.Models.Request
{
    public class BaseCreateRequestVM
    {
        public BaseCreateRequestVM()
        {
            Attachments = new List<IFormFile>();
            Subjects = new List<string>();
        }

        [Required(ErrorMessage = "Поле є обов'язкове")]
        public RequestType Type { get; set; }

        public string Code { get; set; }

        public string SchoolId { get; set; }

        public string ChildId { get; set; }

        public string Class { get; set; }

        public List<IFormFile> Attachments { get; set; }

        public List<string> Subjects { get; set; }

        public string MarkId { get; set; }

        public int? NewMarkRating { get; set; }

        public bool NewMarkPresence { get; set; }

        public string ReasonOfChanging { get; set; }

        public string Subject { get; set; }

        public DateTime MarkDay { get; set; }

        public int? OldMarkRating { get; set; }

        public bool OldMarkPresence { get; set; }

        public string GroupId { get; set; }

        public string Description { get; set; }

        public string ColumnId { get; set; }

        public string ClassId { get; set; }

        public bool IsDeleting { get; set; }

        public int? CustomMark { get; set; }
    }
}
