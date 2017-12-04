using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Memberships.Areas.Admin.Models;
using System.Collections;
using Memberships.Entities;
using Memberships.Models;
using System.Data.Entity;

namespace Memberships.Areas.Admin.Extensions
{
    public static class ConversionExtensions
    {
        public static async Task<IEnumerable<ProductModel>> Convert(this IEnumerable<Product> products, ApplicationDbContext db)
        {
            if (products.Count().Equals(0))
            {
                return new List<ProductModel>();
            }

            var texts = await db.ProductLinkTexts.ToListAsync();
            var types = await db.ProductTypes.ToListAsync();

            return from p in products
                   select new ProductModel()
                   {
                       Id = p.Id,
                       Title = p.Title,
                       Description = p.Description,
                       ImageUrl = p.ImageUrl,
                       ProductLinkTextId = p.ProductLinkTextId,
                       ProductTypeId = p.ProductTypeId,
                       ProductLinkTexts = texts,
                       ProductTypes = types
                   };
        }

        public static async Task<ProductModel> Convert(this Product product, ApplicationDbContext db)
        {
            if (product == null)
            {
                return new ProductModel();
            }

            var text = await db.ProductLinkTexts.Where(t => t.Id == product.ProductLinkTextId).FirstAsync();
            var type = await db.ProductTypes.Where(t => t.Id == product.ProductTypeId).FirstAsync();

            return new ProductModel()
                   {
                       Id = product.Id,
                       Title = product.Title,
                       Description = product.Description,
                       ImageUrl = product.ImageUrl,
                       ProductLinkTextId = product.ProductLinkTextId,
                       ProductTypeId = product.ProductTypeId,
                       ProductLinkTexts = text,
                       ProductTypes = type,

            };
        }
    }
}