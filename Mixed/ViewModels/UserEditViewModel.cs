using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Mixed.ViewModels
{
    public class UserEditViewModel
    {     

        [Display(Name = "About")]
        public string About { get; set; }

        public IFormFile Image { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "OldPassword")]
        public string OldPassword { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string PasswordConfirm { get; set; }
    }
}
