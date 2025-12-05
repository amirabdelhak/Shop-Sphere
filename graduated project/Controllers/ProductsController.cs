using graduated_project.Models;
using graduated_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;

namespace graduated_project.Controllers
{
    
    public class ProductsController : Controller
    {

        IWebHostEnvironment _webHostEnvironment;
        private readonly IProductRepository productRepository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IWebHostEnvironment webHostEnvironment, IProductRepository productRepository, ILogger<ProductsController> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            this.productRepository = productRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewBag.categorys = productRepository.getCategorys();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            ViewBag.categorys = productRepository.getCategorys();
            if (ModelState.IsValid)
            {
                if (product.Image != null)
                {
                    try
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string imagesFolder = Path.Combine(wwwRootPath, "image");

                        // Ensure the images folder exists (helps avoid DirectoryNotFoundException)
                        if (!Directory.Exists(imagesFolder))
                        {
                            Directory.CreateDirectory(imagesFolder);
                        }

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                        string path = Path.Combine(imagesFolder, fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await product.Image.CopyToAsync(fileStream);
                        }

                        product.ImageName = fileName;
                    }
                    catch (Exception ex)
                    {
                        // Log the error and show friendly message to the user
                        _logger.LogError(ex, "Error saving uploaded image for product {ProductName}", product.Name);
                        ModelState.AddModelError(string.Empty, "Unable to save the uploaded image. Please check folder permissions on the server.");
                        return View(product);
                    }
                }

                try
                {
                    productRepository.Addproduct(product);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding product {ProductName}", product.Name);
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the product. Please try again later.");
                    return View(product);
                }

                return RedirectToAction(nameof(Getproducts));
            }

            return View(product);
        }
        public ActionResult Getproducts(int? categoryId)
        {
            var products=productRepository.getproducts();
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            return View(products);
        }
        [Authorize]

        public ActionResult getproduct(int productid)
        {
            var product = productRepository.Getbyid(productid);
            return View(product);
        }
        public ActionResult Deleteproduct(int id)
        {
            productRepository.Delete(id);
            return RedirectToAction(nameof(Getproducts));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.categorys = productRepository.getCategorys();

            var product = productRepository.Getbyid(id);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int id,Product product)
        {
            if (ModelState.IsValid)
            {
                productRepository.Update(id, product);
                return RedirectToAction(nameof(Getproducts));

            }
            else
            {
                return View(product);
            }
        }

        public IActionResult Search(string query)
        {
            List<Product> products;

            if (string.IsNullOrEmpty(query))
            {
                products = productRepository.getproducts(); 
            }
            else
            {
                products = productRepository.search(query);
            }

            return View("getproducts", products); 
        }

    }
}

