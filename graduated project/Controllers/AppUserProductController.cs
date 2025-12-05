using graduated_project.Models;
using graduated_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace graduated_project.Controllers
{
    public class AppUserProductController : Controller
    {
        private readonly IAppUserProductRepository appUserProductRepository;
        public AppUserProductController(IAppUserProductRepository appUserProductRepository)
        {
            this.appUserProductRepository = appUserProductRepository;
        }
        public IActionResult GETALL(int productid)
        {
            var reviews = appUserProductRepository.GetAll(productid);
            return View(reviews);
        }
        public IActionResult GetByIds(string appUserId, int productId)
        {
            appUserProductRepository.GetByIds(appUserId, productId);
            return RedirectToAction(nameof(GETALL));
        }
        public IActionResult Add(string userid ,int productid)
        {
            var review = new AppUserProduct { AppUserId = userid ,ProductId = productid ,Date=DateTime.Now};
            
            return View(review);
        }
        [Authorize]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Add(AppUserProduct appUserProduct)
        {
            if (ModelState.IsValid)
            {
                appUserProductRepository.Add(appUserProduct);
                return RedirectToAction("getproduct", "Products", new { productid = appUserProduct.ProductId });
            }
            return View(appUserProduct);
        }
        public IActionResult Update(string appUserId, int productId)
        {
            var AUP = appUserProductRepository.GetByIds(appUserId,productId);
            return View(AUP);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Update(string appUserId, int productId, AppUserProduct appUserProduct)
        {
            if (ModelState.IsValid)
            {
                appUserProductRepository.Update(appUserId, productId, appUserProduct);
                return RedirectToAction(nameof(GETALL)); 
            }

            return View(appUserProduct);
        }
        [Authorize(Roles = "admin,super admin")]
        public IActionResult Delete(string appUserId, int productId)
        {

            appUserProductRepository.Delete(appUserId, productId);
            return RedirectToAction("getproduct", "Products", new { productid = productId });
        }

    }
}
