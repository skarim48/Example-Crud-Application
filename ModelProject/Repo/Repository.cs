using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelProject.Repo
{
    public class Repository
    {
        private Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;


        public void insertProduct(Product product)
        {
            using (var db = new EFContext())
            {
                db.Add(product);
                db.SaveChanges();
            }
            return;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            using (var db = new EFContext())
            {
                return await db.Products.ToListAsync();
                
            }
        }
        public List<Product> readProduct()
        {
            List<Product> result= new List<Product>();
            using (var db = new EFContext())
            {
                List<Product> products = db.Products.ToList();
                foreach (Product p in products)
                {
                    result.Add(p);
                }
            }
            return result;
        }

        static void updateProduct()
        {
            using (var db = new EFContext())
            {
                Product product = db.Products.Find(1);
                product.Name = "Better Pen Drive";
                db.SaveChanges();
            }
            return;
        }

        public void deleteProduct(string IdProduct)
        {
            using (var db = new EFContext())
            {
                Product product = db.Products.Find(int.Parse(IdProduct));
                db.Products.Remove(product);
                db.SaveChanges();
            }
            return;
        }

        public Product getProduct(string IdProduct)
        {
            using (var db = new EFContext())
            {
                Product product = db.Products.Find(int.Parse(IdProduct));
                return product;

            }
        }

        public void updateeProduct(Product updateProduct)
        {
            using (var db = new EFContext())
            {
                db.Products.Update(updateProduct);
                db.SaveChanges();
            }
            return;
        }

        public async void CreateUser(IdentityUser user)
        {

        }
    }
}
