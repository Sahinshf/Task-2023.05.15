using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.Areas.Admin.ViewModels;
using Task.Utils;
using Task.Contexts;
using Task.Utils.Enums;
using NuGet.ProjectModel;
using System.Diagnostics.SymbolStore;

namespace Task.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // wwwroot`a qədər olan hissəni dinamikləşdirmək

        public SliderController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            //List<Slide> slides = _context.Slides.ToList();                
            //IQueryable<Slide> slides = _context.Slides.AsQueryable();  
            IEnumerable<Slide> slides = _context.Slides.AsEnumerable();

            ViewBag.Count = slides.Count(); 
            
            return View(slides);
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SlideViewModel slideViewModel ) // Async Actions
        {

            if (!ModelState.IsValid) // Check annotation is valid
            {
                return View();
            }

            if (slideViewModel.Offer > 100)
            {
                ModelState.AddModelError("Offer", "More than 100"); //Error message 
                return View();

            }
            #region The first version of Condition
            //if (slideViewModel.Image.Length / 1024 > 100) // Check Size of File (Must be <100)
            //{
            //    ModelState.AddModelError("Image", "Image size is not correct");
            //    return View();
            //}
            #endregion

            if (!slideViewModel.Image.CheckFileSize(100)) // Check Size of File (Must be <100)
            {
                ModelState.AddModelError("Image", "Image size is not correct");
                return View();
            }

            #region The first version of Condition
            //if (!slideViewModel.Image.ContentType.Contains("image/")) // Check Type of File (Must be image)
            //{
            //    ModelState.AddModelError("Image", "File type is not correct");
            //    return View();
            //}
            #endregion

            if (!slideViewModel.Image.CheckContentType(ContentType.image.ToString())) // Check Type of File (Must be image)
            {
                ModelState.AddModelError("Image", "File type is not correct");
                return View();
            }


            string filename = Guid.NewGuid().ToString()+ "-" + slideViewModel.Image.FileName; // Creating Unique image name 

            //string path = _webHostEnvironment.WebRootPath + @"\assets\images\website-images\" + filename; // Path for save images
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "website-images", filename); // Əməliyyat sistemi fərqliliyənə görə uyğunlaşdırmaq


            using (FileStream stream = new FileStream(path , FileMode.Create)) // File`n path`ni veririk, və Path yaratdığımız üçün create edirik
            {
                await slideViewModel.Image.CopyToAsync(stream); //
            }


            Slide slide = new Slide()
            {
                Image = filename,
                Offer = slideViewModel.Offer,
                Title= slideViewModel.Title,    
                Description= slideViewModel.Description,    
            };

            //return Content(slideViewModel.Image.FileName); // Create.cshtml`dƏ uöload olunan File`n Name`ni çıxardır
            //return Content(slideViewModel.Image.Length.ToString(); // File`n ölçüsünü verir
            //return Content(slideViewModel.Image.FileName); //Type `nı verir məsələn İmage Application Text

            await _context.Slides.AddAsync(slide); // Add slide`ı to table
            await _context.SaveChangesAsync(); // Save table to Database

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int id)
        {
            Slide? slider = _context.Slides.FirstOrDefault(s => s.Id == id);

            if (slider == null)
            {
                return NotFound();
            }


            return View(slider);
        }

        #region The first version of Delete action
        //public IActionResult Delete(int id)
        //{

        //    if (_context.Slides.Count() == 0)
        //    {
        //        return BadRequest();
        //    }

        //    Slide? slider = _context.Slides.FirstOrDefault(s => s.Id == id);

        //    if (slider == null)
        //    {
        //        return NotFound();
        //    }


        //    return View(slider);
        //}
        #endregion

        public async Task<IActionResult> Delete(int id)
        {

            if (_context.Slides.Count() == 0)
            {
                return BadRequest();
            }

            Slide? slider = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slider == null)
            {
                return NotFound();
            }

            return View(slider); 
        }

        #region The first version of Post Delete action

        //[HttpPost]
        //[ActionName("Delete")]
        //public IActionResult DeleteService(int id)
        //{

        //    Slide? slider = _context.Slides.FirstOrDefault(s => s.Id == id);

        //    if (slider == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Slides.Remove(slider);
        //    _context.SaveChanges();


        //    return RedirectToAction(nameof(Index));
        //}

        #endregion

        [HttpPost]
        [ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {

            Slide? slider = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slider == null)
            {
                return NotFound();
            }

            #region The first version of Post Delete process
            //string path = Path.Combine( _webHostEnvironment.WebRootPath , "assets" , "images" , "website-images" , slider.Image );

            //if (System.IO.File.Exists(path))
            //{
            //    System.IO.File.Delete(path);
            //}
            #endregion


            FileServices.DeleteFile(_webHostEnvironment.WebRootPath, "assets", "images", "website-images", slider.Image);

            _context.Slides.Remove(slider);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            Slide? slider = await _context.Slides.FirstOrDefaultAsync(_context => _context.Id == id);
            if (slider == null)
            {
                return NotFound();
            }

            SlideViewModel slideViewModel = new()
            {
                Id= slider.Id,
                Title = slider.Title,
                Description= slider.Description,
                Offer=slider.Offer,
            };

            return View(slideViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(SlideViewModel slideItem, int id)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // First Method
            Slide? slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null)
            {
                return NotFound();
            }

            if (slideItem.Image != null)
            {
                if (!slideItem.Image.CheckFileSize(100)) // Check Size of File (Must be <100)
                {
                    ModelState.AddModelError("Image", "Image size is not correct");
                    return View();
                }
                if (!slideItem.Image.CheckContentType(ContentType.image.ToString())) // Check Type of File (Must be image)
                {
                    ModelState.AddModelError("Image", "File type is not correct");
                    return View();
                }

                var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "website-images", slide.Image);
                FileServices.DeleteFile(path);


                string filename = Guid.NewGuid().ToString() + "-" + slideItem.Image.FileName; // Creating Unique image name 
                var newPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "website-images", filename);
                using (FileStream fs = new FileStream(newPath,FileMode.Create))
                {
                    await slideItem.Image.CopyToAsync(fs);
                }
                slide.Image = filename;
            }

            slide.Title = slideItem.Title;
            slide.Description = slideItem.Description;
            slide.Offer = slideItem.Offer;
             

            //Second way
            //Slide? slide = await _context.Slides.FirstOrDefault(s => s.Id == id);
            //if (slide is null)
            //{
            //    return NotFound();
            //}

            //Slide newSlide= new() { 
            //    Id= slideItem.Id,
            //    Description= slideItem.Description,
            //    Offer=slideItem.Offer,  
            //    Title= slideItem.Title,
            //};


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
