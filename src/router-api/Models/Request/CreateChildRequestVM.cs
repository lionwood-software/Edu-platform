using System.ComponentModel.DataAnnotations;

namespace RouterApi.Models.Request
{
    public class CreateChildRequestVM
    {
        [Required(ErrorMessage = "Поле є обов'язкове")]
        public string Code { get; set; }
    }
}
