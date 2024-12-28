using Cebelica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Cebelica.Controllers
{
    public class ProductsController : Controller
    {

        private readonly CebelicaDatabase _context;

        public ProductsController(CebelicaDatabase context)
        {
            _context = context;
        }

        // GET: Products
        public ActionResult Index()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            var products = _context.Products.Where(product => product.IsActive);

            return View(products);
        }

        // GET: Products/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsModel product, IFormFile Picture)
        {
            if (ModelState.IsValid)
            {
                if(Picture != null && Picture.Length > 0)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", Picture.FileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Picture.CopyToAsync(stream);
                    }
                    product.PicturePath = "/images/" + Picture.FileName;
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int id)
        {
            ProductsModel product;

            product = _context.Products.FirstOrDefault(x => x.Id == id);

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductsModel product, IFormFile Picture)
        {
            ProductsModel productToEdit = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            //productToEdit.PicturePath = Picture;
            productToEdit.Price = product.Price;
            productToEdit.Description = product.Description;
            productToEdit.Name = product.Name;

            if (Picture != null && Picture.Length > 0)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", Picture.FileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await Picture.CopyToAsync(stream);
                }
                productToEdit.PicturePath = "/images/" + Picture.FileName;
            }

            

            try
            {
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
