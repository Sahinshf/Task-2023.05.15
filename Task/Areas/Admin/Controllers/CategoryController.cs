using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;

namespace Task.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context; // Instance DbContext

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {

            var category  = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category is null)
            {
                return NotFound();
            }

            return View(category);
        }
    }
}