using Mixed.Models;
using Mixed.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mixed.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private ApplicationContext _context;
        private RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            #region AdminInitialization           
            
            if (await _roleManager.FindByNameAsync("admin") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await _roleManager.FindByNameAsync("user") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("user"));
            }
            if (await _userManager.FindByNameAsync("admin") == null)
            {
                User admin = new User { UserName = "admin", Permit = true, Image = null };
                ImageSetter imageSetter = new ImageSetter();
                imageSetter.SetImage(admin, ref admin);
                IdentityResult result = await _userManager.CreateAsync(admin, "admin");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "admin");
                }
            }

            #endregion

            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = model.UserName,
                    Permit = true
                };

                ImageSetter imageSetter = new ImageSetter();
                imageSetter.SetImage(model, ref user);

                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, "user");
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsync(string returnUrl = null)
        {
            var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = externalProviders
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user != null)
                {
                    if (!user.Permit)
                    {
                        ModelState.AddModelError("", "User is blocked");
                    }

                    else
                    {
                        var result = await _signInManager.PasswordSignInAsync(model.Name, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                            {
                                return Redirect(model.ReturnUrl);
                            }
                            else
                            {
                                await _userManager.UpdateAsync(user);
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Incorrect login and (or) password");
                        }
                    }
                }
            }
            model.ExternalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
            return View(model);
        }

        //[AllowAnonymous]
        //public IActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    var redirectUrl = Url.Action(nameof(ExternalLoginCallBack), "Account", new { returnUrl });
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //    return Challenge(properties, provider);
        //}

        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLoginCallBack(string returnUrl)
        //{
        //    var info = await _signInManager.GetExternalLoginInfoAsync();

        //    if (info == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return RedirectToAction("RegisterExternal", new ExternalLoginViewModel
        //    {
        //        ReturnUrl = returnUrl,
        //        Name = info.Principal.FindFirstValue(ClaimTypes.Name)
        //    }); ;
        //}

        //[AllowAnonymous]
        //public IActionResult RegisterExternal(ExternalLoginViewModel model)
        //{
        //    return View(model);
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> RegisterExternalConfirmed(ExternalLoginViewModel model)
        //{
        //    var info =await _signInManager.GetExternalLoginInfoAsync();
        //    if (info==null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    User user = new User
        //    {
        //        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
        //        UserName = model.Name,
        //        Status = "active",
        //    };
        //    var result = await _userManager.CreateAsync(user);
        //    await _userManager.AddToRoleAsync(user, "user");
        //    if (result.Succeeded)
        //    {
        //        var identityReult = await _userManager.AddLoginAsync(user, info);
        //        if (identityReult.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, false);
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    else
        //    {
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }

        //    return View(model);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string name)
        {
            User user = await _userManager.FindByNameAsync(name);
            ViewBag.User = user;

            var collections = _context.Collections.Where(x => x.User.Equals(name)).ToList();
            ViewBag.Collections = collections;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Edit(User model, string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            var name = user.UserName;
            if (model.About != null)
                user.About = model.About;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Profile", "Account", new { name });
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> ChangePermission(string[] selectedUsers)
        {
            foreach (var selectedUser in selectedUsers)
            {
                User user = await _userManager.FindByNameAsync(selectedUser);
                if (user == null)
                {
                    return NotFound();
                }

                if (user.Permit == true)
                {
                    user.Permit = false;
                }
                else
                {
                    user.Permit = true;
                }

                await _userManager.UpdateAsync(user);

                if (user.Permit == false && User.Identity.Name.Equals(user.UserName))
                {
                    await _signInManager.SignOutAsync();
                }
            }
            return RedirectToAction("AdminControl");
        }    


        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string[] selectedUsers)
        {
            foreach (var str in selectedUsers)
            {
                User user = await _userManager.FindByNameAsync(str);
                if (user != null)
                {
                    if (User.Identity.Name.Equals(user.UserName))
                    {
                        await _signInManager.SignOutAsync();
                    }
                    IdentityResult result = await _userManager.DeleteAsync(user);
                }
            }
            if (_userManager.FindByNameAsync(User.Identity.Name).Status.Equals("blocked"))
            {
                return Redirect("~/Account/Logout");
            }
            return RedirectToAction("AdminControl");
        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult AdminControl(SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<User> users = _context.Users;
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["PermissionSort"] = sortOrder == SortState.PermissionAsc ? SortState.PermissionDesc : SortState.PermissionAsc;
            switch (sortOrder)
            {
                case SortState.NameAsc:
                    users = users.OrderBy(s => s.UserName);
                    break;
                case SortState.NameDesc:
                    users = users.OrderByDescending(s => s.UserName);
                    break;
                case SortState.PermissionAsc:
                    users = users.OrderBy(s => s.Permit);
                    break;
                case SortState.PermissionDesc:
                    users = users.OrderByDescending(s => s.Permit);
                    break;
                default:
                    users = users.OrderBy(s => s.Email);
                    break;
            }
            return View(users.ToList());
        }
    }
}
