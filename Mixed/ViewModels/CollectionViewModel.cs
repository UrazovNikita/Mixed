using Mixed.СustomAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Mixed.ViewModels
{
    public class CollectionViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Theme")]
        public string Theme { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
