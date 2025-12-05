using graduated_project.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace graduated_project.Services
{
    public class AppUserProductRepository: IAppUserProductRepository
    {
        private readonly ShopSpheredbcontext context;

        public AppUserProductRepository(ShopSpheredbcontext context)
        {
            this.context = context;
        }
        public List<AppUserProduct> GetAll(int prodductid)
        {
        return context.AppUserProducts
        .Include(a => a.AppUser)   
        .Include(a => a.Product)   
        .Where(a => a.ProductId == prodductid)
        .ToList();
        }
        public AppUserProduct? GetByIds(string appUserId, int productId)
        {
            return context.AppUserProducts.Include(ap => ap.AppUser).Include(ap => ap.Product).FirstOrDefault(ap => ap.AppUserId == appUserId && ap.ProductId == productId);
        }
        public void Add(AppUserProduct appUserProduct)
        {
            context.AppUserProducts.Add(appUserProduct);
            context.SaveChanges();
        }
        public void Update(string appUserId, int productId, AppUserProduct appUserProduct)
        {
            var AUP = GetByIds(appUserId, productId);

            AUP.Rate = appUserProduct.Rate;
            AUP.Comment = appUserProduct.Comment;
            AUP.Date = appUserProduct.Date;

            context.AppUserProducts.Update(AUP);
            context.SaveChanges();

        }
        public void Delete(string appUserId, int productId)
        {
            var AUP = GetByIds(appUserId, productId);
            context.AppUserProducts.Remove(AUP);
            context.SaveChanges();
        }
    }
}
