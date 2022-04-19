using System.ComponentModel.DataAnnotations;

namespace RouterApi.Models.Request
{
    public class RejectRequestVM
    {
        [Required(ErrorMessage = "Поле є обов'язкове")]
        public string Decision { get; set; }
    }
}
