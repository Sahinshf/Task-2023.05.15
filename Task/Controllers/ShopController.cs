using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Contexts;

namespace Task.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            //IQueryable<Product> products = _context.Products.Where(p => !p.IsDeleted).AsQueryable();

            // AppDbContext`də OnModelCreating`i override etdiyimiz üçün .Where(p => !p.IsDeleted) şərtini silə bilərik.
            // Nə vaxtsa işə düşməsini istəməsək İgnoreQueryFilters() əlavə etməliyik IQueryable<Product> products = _context.Products.IgnoreQueryFilters().AsQueryable();


            IQueryable<Product> products = _context.Products.AsQueryable();


            ViewBag.productsCount = await _context.Products.Where(p => !p.IsDeleted).CountAsync();

            ShopViewModel shopViewModel = new()
            {
                Products = id != null ? await products.Where(p => p.CategoryId == id).ToListAsync() 
                : await products.ToListAsync(),
                Categories = await _context.Categories.Include(p=>p.Products.Where(p=> !p.IsDeleted)).Where(c => !c.IsDeleted).ToListAsync()
            };


            return View(shopViewModel);
        }
    }
}
