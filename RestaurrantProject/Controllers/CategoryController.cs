using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurrantProject.Context;
using RestaurrantProject.Models;
using RestaurrantProject.ViewModels;
//using RestaurrantProject.Views.Shared;

namespace RestaurrantProject.Controllers
{
    //[Authorize(Roles ="Admin")]
    public class Category : Controller
    {
        MyContext _context;
        public Category(MyContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Include(c => c.items) 
                .Where(c => c.items.Any()) 
                .ToListAsync();

            return View(categories);
        }


        public IActionResult Create()
        {
            CrtCategoryVM crtCategoryVM = new CrtCategoryVM();
            return View(crtCategoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrtCategoryVM crtCategoryVM)
        {

            if (!ModelState.IsValid)
            {
                //var cats = await _context.Categories.ToListAsync();
                return View(crtCategoryVM);

            }
            else
            {
                RestaurrantProject.Models.Category NewCategory = new RestaurrantProject.Models.Category()
                {
                    Name = crtCategoryVM.Name,
                };
                await _context.Categories.AddAsync(NewCategory);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("GetAll");
        }


        public IActionResult Delete(int id)
        {
            var x = _context.Categories.FirstOrDefault(x => x.Id == id);
            x.IsDeleted = true;
            _context.SaveChangesAsync();
            return RedirectToAction("GetAll");
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var old = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (old == null)
            {
                return NotFound();
            }
            var ViewModel = new UpdateCategoryVM()
            {
                Id = old.Id,
                Name = old.Name,
               
            };
            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM updateCategoryVM)
        {

            if (!ModelState.IsValid)
            {
                return View(updateCategoryVM);
            }
            var RealCat = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (RealCat == null)
            {
                return NotFound();
            }

            RealCat.Name = updateCategoryVM.Name;
            
            _context.Update(RealCat);
            await _context.SaveChangesAsync();


            return RedirectToAction("GetAll");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}
