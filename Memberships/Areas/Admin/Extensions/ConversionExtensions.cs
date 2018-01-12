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
using System.Transactions;

namespace Memberships.Areas.Admin.Extensions
{
    public static class ConversionExtensions
    {
        #region Product
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

            var model = new ProductModel()
                   {
                       Id = product.Id,
                       Title = product.Title,
                       Description = product.Description,
                       ImageUrl = product.ImageUrl,
                       ProductLinkTextId = product.ProductLinkTextId,
                       ProductTypeId = product.ProductTypeId,
                       ProductLinkTexts = new List<ProductLinkText>(),
                       ProductTypes = new List<ProductType>()
            };

            model.ProductTypes.Add(type);
            model.ProductLinkTexts.Add(text);
            return model;
        }
        #endregion

        #region ProductItem
        public static async Task<IEnumerable<ProductItemModel>> Convert(this IQueryable<ProductItem> productItems, ApplicationDbContext db)
        {
            if (productItems.Count().Equals(0))
            {
                return new List<ProductItemModel>();
            }

            return await (from pi in productItems
                   select new ProductItemModel()
                   {
                        ItemId = pi.ItemId,
                        ProductId = pi.ProductId,
                        ItemTitle = db.Items.FirstOrDefault(i => i.Id.Equals(pi.ItemId)).Title,
                        ProductTitle = db.Products.FirstOrDefault(i => i.Id.Equals(pi.ProductId)).Title
                   }).ToListAsync();
        }

        public static async Task<ProductItemModel> Convert(this ProductItem productItem, ApplicationDbContext db, bool addListData = true)
        {
            if (productItem == null)
            {
                return new ProductItemModel();
            }

            var model = new ProductItemModel()
            {
                ItemId = productItem.ItemId,
                ProductId = productItem.ProductId,
                Items = addListData ? await db.Items.ToListAsync() : null,
                Products = addListData ? await db.Products.ToListAsync() : null,
                ItemTitle = (await db.Items.FirstOrDefaultAsync(i => i.Id.Equals(productItem.ItemId))).Title,
                ProductTitle = (await db.Products.FirstOrDefaultAsync(p => p.Id.Equals(productItem.ProductId))).Title
            };

            return model;
        }

        public static async Task<bool> canChange(this ProductItem productItem, ApplicationDbContext db)
        {
            var oldPI = await db.ProductItems.CountAsync(pi => pi.ProductId.Equals(productItem.OldProductId) &&
                        pi.ItemId.Equals(productItem.OldItemId));

            var newPI = await db.ProductItems.CountAsync(pi => pi.ProductId.Equals(productItem.ProductId) &&
                        pi.ItemId.Equals(productItem.ItemId));

            return oldPI.Equals(1) && newPI.Equals(0);
        }

        public static async Task Change(this ProductItem productItem, ApplicationDbContext db)
        {
            var oldPI = await db.ProductItems.FirstOrDefaultAsync(pi => pi.ProductId.Equals(productItem.OldProductId) &&
                        pi.ItemId.Equals(productItem.OldItemId));

            var newPI = await db.ProductItems.FirstOrDefaultAsync(pi => pi.ProductId.Equals(productItem.ProductId) &&
                        pi.ItemId.Equals(productItem.ItemId));

            if(oldPI != null && newPI == null)
            {
                newPI = new ProductItem()
                {
                    ItemId = productItem.ItemId,
                    ProductId = productItem.ProductId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.ProductItems.Remove(oldPI);
                        db.ProductItems.Add(newPI);
                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch(Exception e)
                    {
                        transaction.Dispose();
                    }
                }
            }
        }
        #endregion

        #region SubscriptionProduct

        public static async Task<IEnumerable<SubscriptionProductModel>> Convert(this IQueryable<SubscriptionProduct> subscriptionProducts, ApplicationDbContext db)
        {
            if (subscriptionProducts.Count().Equals(0))
            {
                return new List<SubscriptionProductModel>();
            }

            return await (from sp in subscriptionProducts
                          select new SubscriptionProductModel()
                          {
                              SubscriptionId = sp.SubscriptionId,
                              ProductId = sp.ProductId,
                              SubscriptionTitle = db.Subscriptions.FirstOrDefault(i => i.Id.Equals(sp.SubscriptionId)).Title,
                              ProductTitle = db.Products.FirstOrDefault(i => i.Id.Equals(sp.ProductId)).Title
                          }).ToListAsync();
        }

        public static async Task<SubscriptionProductModel> Convert(this SubscriptionProduct subscriptionProduct, ApplicationDbContext db, bool addListData = true)
        {
            if (subscriptionProduct == null)
            {
                return new SubscriptionProductModel();
            }

            var model = new SubscriptionProductModel()
            {
                SubscriptionId = subscriptionProduct.SubscriptionId,
                ProductId = subscriptionProduct.ProductId,
                Subscriptions = addListData ? await db.Subscriptions.ToListAsync() : null,
                Products = addListData ? await db.Products.ToListAsync() : null,
                SubscriptionTitle = (await db.Subscriptions.FirstOrDefaultAsync(i => i.Id.Equals(subscriptionProduct.SubscriptionId))).Title,
                ProductTitle = (await db.Products.FirstOrDefaultAsync(p => p.Id.Equals(subscriptionProduct.ProductId))).Title
            };

            return model;
        }

        public static async Task<bool> canChange(this SubscriptionProduct subscriptionProduct, ApplicationDbContext db)
        {
            var oldSP = await db.SubscriptionProducts.CountAsync(sp => sp.ProductId.Equals(subscriptionProduct.OldProductId) &&
                        sp.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId));

            var newSP = await db.SubscriptionProducts.CountAsync(sp => sp.ProductId.Equals(subscriptionProduct.ProductId) &&
                        sp.SubscriptionId.Equals(subscriptionProduct.SubscriptionId));

            return oldSP.Equals(1) && newSP.Equals(0);
        }

        public static async Task Change(this SubscriptionProduct subscriptionProduct, ApplicationDbContext db)
        {
            var oldSP = await db.SubscriptionProducts.FirstOrDefaultAsync(sp => sp.ProductId.Equals(subscriptionProduct.OldProductId) &&
                        sp.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId));

            var newSP = await db.SubscriptionProducts.FirstOrDefaultAsync(sp => sp.ProductId.Equals(subscriptionProduct.ProductId) &&
                        sp.SubscriptionId.Equals(subscriptionProduct.SubscriptionId));

            if (oldSP != null && newSP == null)
            {
                newSP = new SubscriptionProduct()
                {
                    SubscriptionId = subscriptionProduct.SubscriptionId,
                    ProductId = subscriptionProduct.ProductId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.SubscriptionProducts.Remove(oldSP);
                        db.SubscriptionProducts.Add(newSP);
                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch (Exception e)
                    {
                        transaction.Dispose();
                    }
                }
            }
        }

        #endregion
    }
}