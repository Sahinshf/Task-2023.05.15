using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Areas.Admin.ViewModels;
using Task.Contexts;

namespace Task.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context; // Instance DbContext

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Where IsDelet statusuna görə filter etmək
            // İnclude Sql`dəki Join məntiqi kimidir
            var products = await _context.Products.Where(p => !p.IsDeleted).Include(p => p.Category).ToListAsync();
            ViewBag.count = products.Count();

            return View(products);
        }


        public IActionResult Create()
        {
            ViewBag.categories = _context.Categories.AsEnumerable();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            ViewBag.categories = _context.Categories.AsEnumerable();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!_context.Categories.Any(c => c.Id == productViewModel.CategoryId)) // Bu Id`də category yoxdursa BadRequest return edir
            {
                return BadRequest();
            }

            Product product = new()
            {
                Name = productViewModel.Name,
                Price = productViewModel.Price,
                Rating = productViewModel.Rating,
                Image = productViewModel.Image,
                CategoryId = productViewModel.CategoryId,
                IsDeleted = false
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return View();
        }
    }
}
