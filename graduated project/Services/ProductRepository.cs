using graduated_project.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace graduated_project.Services
{
    public class ProductRepository : IProductRepository
    {
        ShopSpheredbcontext context;
        IWebHostEnvironment _webHostEnvironment;

        public ProductRepository(ShopSpheredbcontext context , IWebHostEnvironment _webHostEnvironment)
        {
            this._webHostEnvironment = _webHostEnvironment;
            this.context = context;
        }

        public Product Getbyid(int productid)
        {
            return context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == productid);
        }


        public List<Product> search(string name)
        {
            return context.Products.Include(p => p.Category).Where(p => p.Name.Contains(name)).ToList();
        }
        public List<Product> getproducts ()
        { 
            return context.Products.Include(p => p.Category).ToList(); 
        }
        public List<Category> getCategorys()
        {
            return context.Categorys.ToList();
        }
        public void Addproduct(Product product) 
        {
            context.Products.Add(product);
            context.SaveChanges();
        }
        public void Update(int id, Product product)
        {
            var pdc = context.Products.Include(p => p.Category).SingleOrDefault(p => p.Id == id);

            pdc.Name = product.Name;
            pdc.Description = product.Description;
            pdc.Quantity = product.Quantity;
            pdc.Pricebeforediscount = product.Pricebeforediscount;
            pdc.Discount = product.Discount;
            pdc.CategoryId = product.CategoryId;
                    
            context.SaveChanges();       
        }

        public void Delete(int id)
        {
            var p = context.Products.SingleOrDefault(p => p.Id == id);
            context.Products.Remove(p);
            context.SaveChanges();
        }


    }
    
}
