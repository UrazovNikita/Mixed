using Mixed.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Mixed.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
