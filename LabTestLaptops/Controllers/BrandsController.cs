using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using LabTestLaptops;

namespace LabTestLaptops.Controllers
{
    public class BrandsController : Controller
    {
        private LaptopLLCEntities db = new LaptopLLCEntities();

        // GET: Brands
        public ActionResult Index()
        {
            return View(db.Brands.ToList());
        }

        // GET: Brands/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }


        // GET: Brands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Brands.Add(brand);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(brand);
        }

        // GET: Brands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(brand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        public ActionResult SelectBrand()
        {
            ViewBag.BrandID = new SelectList(db.Brands.ToList(), "ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult SelectBrand(int? BrandID)
        {
            if (BrandID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Brand brand = db.Brands.Find(BrandID);
            var routeObj = new { id = brand.ID };

            return RedirectToAction("LaptopsInBrand", routeObj);
        }

        public ActionResult LaptopsInBrand(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        public ActionResult BuyLaptop(int id)
        {
            Laptop laptop = db.Laptops.Find(id);

            ViewBag.UserID = new SelectList(db.Customers.ToList(), "ID", "Email");
            return View(laptop);
        }

        [HttpPost]
        public ActionResult BuyLaptop(int LaptopId, int UserID, int Quantity)
        {
            Laptop laptop = db.Laptops.Find(LaptopId);
            Customer customer = db.Customers.Find(UserID);

            bool success;
            string message;

            int cost = laptop.DollarValue * Quantity;

            if (cost <= customer.Wallet && laptop.QuantityAvailable >= Quantity)
            {
                success = true;
                customer.Wallet -= cost;
                laptop.QuantityAvailable -= Quantity;
                db.SaveChanges();

                return RedirectToAction("PurchaseResult", new
                {
                    Success = success,
                    laptopId = laptop.ID,
                    customerId = customer.ID
                });
            }
            else 
            {
                message = "Error processing your purchase. ";
                success = false;
                if (cost > customer.Wallet)
                {
                    message += $"Insufficient funds to buy {Quantity} {laptop.Make}(s).";
                } else if (Quantity > laptop.QuantityAvailable)
                {
                    message += $"Insufficient Quantity of Laptops available({laptop.QuantityAvailable}) to fulfill purchase quantity ({Quantity}).";
                }
                return RedirectToAction("PurchaseResult", new { Success = success, Message = message });
            }
        }

        public ActionResult PurchaseResult(bool Success, string Message, int? customerId, int? laptopId)
        {
            Customer customer = db.Customers.Find(customerId);
            Laptop laptop = db.Laptops.Find(laptopId);
            string message;

            if(Success)
            {
                message = $"Congratulations on receiving your new {laptop.Brand.Name} {laptop.Make}. Your remaining balance is {customer.Wallet}.";
            } else
            {
                message = Message;
            }

            ViewBag.Message = message;
            return View();
        }

        // GET: Brands/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Brand brand = db.Brands.Find(id);
            db.Brands.Remove(brand);
            db.SaveChanges();
            return RedirectToAction("Index");
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
