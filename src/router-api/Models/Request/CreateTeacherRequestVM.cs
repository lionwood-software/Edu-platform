using LionwoodSoftware.Attributes;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FileExtensionsAttribute = LionwoodSoftware.Attributes.FileExtensionsAttribute;

namespace RouterApi.Models.Request
{
    public class CreateTeacherRequestVM
    {
        [Required(ErrorMessage = "Поле є обов'язкове")]
        [StringAsObjectId(ErrorMessage = "Не правильний формат даних")]
        public string SchoolId { get; set; }

        [Required(ErrorMessage = "Поле є обов'язкове")]
        [DataType(DataType.Upload)]
        [FileSize(10 * 1024 * 1024, "Допустимий розмір файлу 10mb")]
        [FileExtensions("jpg, pdf, jpeg, png", "Допустимі формати файлу jpg, pdf, jpeg, png")]
        [NonEmptyList(ErrorMessage = "Необхідно додати вкладення")]
        public List<IFormFile> Attachments { get; set; }

        [Required(ErrorMessage = "Поле є обов'язкове")]
        [NonEmptyList(ErrorMessage = "Необхідно вибрати предмети")]
        public List<string> Subjects { get; set; }
    }
}
