using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurrantProject.Context;
using RestaurrantProject.Models;
using RestaurrantProject.ViewModels;
using System.Threading.Tasks;

namespace RestaurrantProject.Controllers
{
    public class ItemsController : Controller
    {
        MyContext _context;

        public ItemsController(MyContext context)
        {
            _context = context;
        }
        [Route("AllItems")]
        public async Task<IActionResult> GetAll(string? category)
        {
            var query = _context.Items
                .Include(x => x.category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                query = query.Where(x => x.category.Name == category);
            }

            var items = await query.ToListAsync();

            
            var availableCategories = await _context.Categories
                .Where(c => c.items.Any(i => i.IsAvailable)) 
                .Select(c => c.Name)
                .ToListAsync();

            ViewBag.SelectedCategory = category ?? "All";
            ViewBag.Categories = availableCategories;

            return View(items);
        }


        public async Task<IActionResult> Create()
        {
            var cats = await _context.Categories.ToListAsync();
            CrtItemVM crtItemVM = new CrtItemVM()
            {
                categories = new SelectList(cats, "Id", "Name")
            };
            return View(crtItemVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrtItemVM crtItemVM)
        {

            if (!ModelState.IsValid)
            {
                var cats = await _context.Categories.ToListAsync();
                crtItemVM.categories = new SelectList(cats, "Id", "Name");
                return View(crtItemVM);

            }
            if(crtItemVM.CategoryID == 0)
            {
                ModelState.AddModelError("CategoryID", "Select Category");
                var cats = await _context.Categories.ToListAsync();
                crtItemVM.categories = new SelectList(cats, "Id", "Name");
                return View(crtItemVM);

            }
            else
            {
                Item NewItem = new Item()
                {
                    Name = crtItemVM.Name,
                    Description = crtItemVM.Description,
                    Price = crtItemVM.Price,
                    CategoryID = crtItemVM.CategoryID,

                };
                await _context.Items.AddAsync(NewItem);
                await _context.SaveChangesAsync();
            }
            
            
            return RedirectToAction("GetAll");
        }

        public IActionResult Delete(int id)
        {
            var x = _context.Items.FirstOrDefault(x => x.Id == id);
            x.IsDeleted = true;
            _context.SaveChangesAsync();
            return RedirectToAction("GetAll");
        }


        public async Task<IActionResult> Update(int id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var old = await _context.Items.FirstOrDefaultAsync( x => x.Id == id);
            if(old == null)
            {
                return NotFound();
            }
            
            var ViewModel = new UpdateItemVM()
            {
                Id = old.Id,
                Name = old.Name,
                Description = old.Description,
                Price = old.Price,
                CategoryID = old.CategoryID,
                categories = await GetCategoriesSelectList()
            };
            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateItemVM updateItemVM)
        {

            if(!ModelState.IsValid)
            {
                updateItemVM.categories = await GetCategoriesSelectList();
                return View(updateItemVM);
            }
            var RealItem = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (RealItem == null)
            {
                return NotFound();
            }
            if (RealItem.CategoryID == 0)
            { 

            RealItem.Name = updateItemVM.Name;
            RealItem.Description = updateItemVM.Description;
            RealItem.Price = updateItemVM.Price;
            RealItem.CategoryID = updateItemVM.CategoryID;
            _context.Update( RealItem );
            await _context.SaveChangesAsync();


            return RedirectToAction("GetAll");
            }
            else
            {
                ModelState.AddModelError("", "Select Department");
                updateItemVM.categories = await GetCategoriesSelectList();
                return View(updateItemVM);
            }
        }

        private async Task<SelectList> GetCategoriesSelectList()
        {
            var Cats = await _context.Categories.ToListAsync();
            return new SelectList(Cats, "Id", "Name");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Items
                .Include(x => x.category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return NotFound();

            return View(item);
        }
    }
}
