using Microsoft.AspNetCore.Identity;

namespace Mixed.Models
{
    public class User : IdentityUser
    {
        public bool Permit { get; set; } = true;
        public string About { get; set; }
        public byte[] Image { get; set; }
    }
}
