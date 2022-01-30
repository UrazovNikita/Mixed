using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Mixed.Models;
using System;
using System.Linq;

namespace Mixed.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;
        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult ChangeTheme()
        {
            CookieOptions cookie = new CookieOptions();
            cookie.Expires = DateTime.Now.AddDays(1);

            if (Request.Cookies["theme"].Contains("light"))
            {
                Response.Cookies.Append("theme", "dark");
            }
           if(Request.Cookies["theme"].Contains("dark"))
                {
                Response.Cookies.Append("theme", "light");
            }

            return RedirectToAction("Index");
        }

            
        public ActionResult Index()
        {

            IQueryable<Collection> collections = _context.Collections;
            IQueryable<Item> items = _context.Items;

            collections = collections.OrderByDescending(s => s.CountItems);
            items = items.OrderByDescending(s => s.AddTime);

            HomeModel model = new HomeModel(items.ToList(), collections.ToList());

            return View(model);
        }


        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
