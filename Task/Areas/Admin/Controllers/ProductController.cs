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
            //var products = await _context.Products.Where(p => !p.IsDeleted).Include(p => p.Category).ToListAsync();

            // En son modifies olunanın ən üstdə görünməsi üçün OrderByDescending İstifadə edirik
            var products = await _context.Products.Where(p => !p.IsDeleted).Include(p => p.Category).OrderByDescending(p => p.ModifiedAt).ToListAsync();


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


        public async Task<IActionResult> Update(int id) {

            var product = await _context.Products.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (product == null)
            {
                return BadRequest();
            }

            ProductViewModel productViewModel = new() { 
                Id = product.Id, Name = product.Name, Price = product.Price,Rating= product.Rating,Image = product.Image, CategoryId = product.CategoryId,
            };

            ViewBag.categories= _context.Categories.Where(c=>!c.IsDeleted).AsEnumerable();

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductViewModel productViewModel , int id)
        {
            ViewBag.categories = _context.Categories.Where(c => !c.IsDeleted).AsEnumerable();

            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!_context.Categories.Any(c => c.Id == productViewModel.CategoryId)) // Bu Id`də category yoxdursa BadRequest return edir
            {
                return BadRequest();
            }


            Product? product = await _context.Products.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (product is null)
            {
                return NotFound();
            }

            product.Name= productViewModel.Name;
            product.Price= productViewModel.Price;
            product.Rating= productViewModel.Rating;
            product.Image= productViewModel.Image;
            product.CategoryId= productViewModel.CategoryId;

            await _context.SaveChangesAsync();

            return View();
        }


        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (product is null)
            {
                return BadRequest();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public async Task<IActionResult> DeleteService(int id)
        {
            Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (product is null)
            {
                return NotFound();
            }
            product.IsDeleted = true;
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
