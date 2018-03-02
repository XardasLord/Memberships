﻿using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Memberships.Entities;
using Memberships.Models;
using Memberships.Areas.Admin.Models;
using Memberships.Areas.Admin.Extensions;

namespace Memberships.Areas.Admin.Controllers
{
    public class SubscriptionProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/SubscriptionProduct
        public async Task<ActionResult> Index()
        {
            return View(await db.SubscriptionProducts.ConvertAsync(db));
        }

        // GET: Admin/SubscriptionProduct/Details/5
        public async Task<ActionResult> Details(int? subscriptionId, int? productId)
        {
            if (subscriptionId == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionProduct subscriptionProduct = await GetSubscriptionProduct(subscriptionId, productId);
            if (subscriptionProduct == null)
            {
                return HttpNotFound();
            }
            return View(await subscriptionProduct.ConvertAsync(db));
        }

        // GET: Admin/SubscriptionProduct/Create
        public async Task<ActionResult> Create()
        {
            var model = new SubscriptionProductModel
            {
                Subscriptions = await db.Subscriptions.ToListAsync(),
                Products = await db.Products.ToListAsync()
            };
            return View(model);
        }

        // POST: Admin/SubscriptionProduct/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductId,SubscriptionId")] SubscriptionProduct subscriptionProduct)
        {
            if (ModelState.IsValid)
            {
                db.SubscriptionProducts.Add(subscriptionProduct);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(subscriptionProduct);
        }

        // GET: Admin/SubscriptionProduct/Edit/5
        public async Task<ActionResult> Edit(int? subscriptionId, int? productId)
        {
            if (subscriptionId == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionProduct subscriptionProduct = await GetSubscriptionProduct(subscriptionId, productId);
            if (subscriptionProduct == null)
            {
                return HttpNotFound();
            }
            return View(await subscriptionProduct.ConvertAsync(db));
        }

        // POST: Admin/SubscriptionProduct/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductId,SubscriptionId,OldProductId,OldSubscriptionId")] SubscriptionProduct subscriptionProduct)
        {
            if (ModelState.IsValid)
            {
                if (await subscriptionProduct.CanChangeAsync(db))
                    await subscriptionProduct.ChangeAsync(db);

                return RedirectToAction("Index");
            }
            return View(subscriptionProduct);
        }

        // GET: Admin/SubscriptionProduct/Delete/5
        public async Task<ActionResult> Delete(int? subscriptionId, int? productId)
        {
            if (subscriptionId == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionProduct subscriptionProduct = await GetSubscriptionProduct(subscriptionId, productId);
            if (subscriptionProduct == null)
            {
                return HttpNotFound();
            }
            return View(await subscriptionProduct.ConvertAsync(db));
        }

        // POST: Admin/SubscriptionProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int subscriptionId, int productId)
        {
            SubscriptionProduct subscriptionProduct = await GetSubscriptionProduct(subscriptionId, productId);
            db.SubscriptionProducts.Remove(subscriptionProduct);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private async Task<SubscriptionProduct> GetSubscriptionProduct(int? subscriptionId, int? productId)
        {
            int subId = 0, prdId = 0;
            try
            {
                int.TryParse(subscriptionId.ToString(), out subId);
                int.TryParse(productId.ToString(), out prdId);
            }
            catch { return null; }

            var subscriptionProduct = await db.SubscriptionProducts.FirstOrDefaultAsync(p => p.ProductId.Equals(prdId) && p.SubscriptionId.Equals(subId));
            return subscriptionProduct;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}