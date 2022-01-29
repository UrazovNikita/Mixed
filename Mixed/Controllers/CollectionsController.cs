using Mixed.Models;
using Mixed.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Mixed.Controllers
{
 
    public class CollectionsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;
        private readonly SignInManager<User> _signInManager;
        public CollectionsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public ActionResult Index(Guid collectionId, SortState sortOrder = SortState.NameAsc)
        {
            Collection collection = _context.Collections.Find(collectionId);
            ViewBag.Collection = collection;
            IQueryable<Item> items = _context.Items.Where(p => p.CollectionId == collectionId.ToString());
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            switch (sortOrder)
            {
                case SortState.NameAsc:
                    items = items.OrderBy(s => s.Name);
                    break;
                default:
                    items = items.OrderByDescending(s => s.Name);
                    break;
            }
            return View(items);
        }
        [HttpGet]
        public ActionResult Collections(SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<Collection> collections = _context.Collections;
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["CountSort"] = sortOrder == SortState.CountAsc ? SortState.CountDesc : SortState.CountAsc;
            ViewData["ThemeSort"] = sortOrder == SortState.ThemeAsc ? SortState.ThemeDesc : SortState.ThemeAsc;
            switch (sortOrder)
            {
                case SortState.NameAsc:
                    collections = collections.OrderBy(s => s.Name);
                    break;
                case SortState.NameDesc:
                    collections = collections.OrderByDescending(s => s.Name);
                    break;
                case SortState.CountDesc:
                    collections = collections.OrderByDescending(s => s.CountItems);
                    break;
                case SortState.ThemeAsc:
                    collections = collections.OrderBy(s => s.Theme);
                    break;
                case SortState.ThemeDesc:
                    collections = collections.OrderByDescending(s => s.Theme);
                    break;
                default:
                    collections = collections.OrderBy(s => s.CountItems);
                    break;
            }
            return View(collections.ToList());
        }


        [HttpGet]
        public IActionResult Create(string username)
        {
            ViewBag.UserName = username;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CollectionViewModel model, string username)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(username);
                Collection collection = new Collection { Name = model.Name, Theme = model.Theme, Description = model.Description, User = user.UserName, UserId = user.Id, CountItems = 0};
               
                _context.Collections.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProfileCollections", "Collections", new { name = username });
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid[] selectedCollections)
        {
            string name = _context.Collections.Find(selectedCollections[0]).User;
            foreach (var id in selectedCollections)
            {
                Collection collection = _context.Collections.Find(id);
                var items = _context.Items.Where(p => p.CollectionId == id.ToString()).ToList();
                if (collection == null)
                {
                    return NotFound();
                }
                _context.Collections.Remove(collection);
                foreach (var item in items)
                {
                    _context.Items.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ProfileCollections", "Collections", new { name });
        }


        [HttpGet]
        public IActionResult Edit(Guid collectionId)
        {
            Collection collection = _context.Collections.Find(collectionId);
            ViewBag.Collection = collection;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Collection model, Guid collectionId)
        {
            Collection collection = _context.Collections.Find(collectionId);
            collection.Name = model.Name;
            collection.Theme = model.Theme;
            collection.Description= model.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Collections", new { collectionId });
        }
        [HttpGet]
        public async Task<ActionResult> ProfileCollections(string name, SortState sortOrder = SortState.NameAsc)
        {
            User user = await _userManager.FindByNameAsync(name);
            ViewBag.User = user;
            IQueryable<Collection> collections = _context.Collections.Where(p => p.UserId.Equals(user.Id));
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["CountSort"] = sortOrder == SortState.CountAsc? SortState.CountDesc : SortState.CountAsc;
            ViewData["ThemeSort"] = sortOrder == SortState.ThemeAsc? SortState.ThemeDesc : SortState.ThemeAsc;
            switch (sortOrder)
            {
                case SortState.NameAsc:
                    collections = collections.OrderBy(s => s.Name);
                    break;
                case SortState.NameDesc:
                    collections = collections.OrderByDescending(s => s.Name);
                    break;
                case SortState.CountDesc:
                    collections = collections.OrderByDescending(s => s.CountItems);
                    break;
                case SortState.ThemeAsc:
                    collections = collections.OrderBy(s => s.Theme);
                    break;
                case SortState.ThemeDesc:
                    collections = collections.OrderByDescending(s => s.Theme);
                    break;
                default:
                    collections = collections.OrderBy(s => s.CountItems);
                    break;
            }
            return View(collections.ToList());
        }     
       
    }
}
