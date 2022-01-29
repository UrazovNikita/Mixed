using System.ComponentModel.DataAnnotations;

namespace Mixed.ViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Name { get; set; }
        public string ReturnUrl { get; set; }
    }
}
