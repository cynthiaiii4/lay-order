using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using sys.Models;

namespace sys.Controllers
{
    public class ProductController : Controller
    {
        private Membersql db = new Membersql();

        #region 取得類別GET
        public ActionResult GetCategory()
        {
            return Content(JsonConvert.SerializeObject(db.ProductCategoryList.Select(x => x.PCName).ToList()));
        }
        #endregion

        #region 取得商品列表GET

        public ActionResult GetProduct(string type)
        {
            int PCid = 0;
            if (type == "all") 
            {
                return Content(JsonConvert.SerializeObject(db.ProductLists.Select(x => new { x.Id, x.Img, x.Name, x.Price}).ToList()));
            }
            switch (type)
            {
                case "rice":
                    PCid = 1;
                    break;
                case "noodle":
                    PCid = 2;
                    break;
                case "soup":
                    PCid = 3;
                    break;
                case "drink":
                    PCid = 4;
                    break;
            }

            return Content(JsonConvert.SerializeObject(db.ProductLists.Where(x=>x.PCid==PCid).Select(x=>new {x.Id,x.Img,x.Name,x.Price}).ToList()));
        }
        

        #endregion


        // GET: ProductLists
        public ActionResult Index()
        {
            var productLists = db.ProductLists.Include(p => p.ProductCategory);
            return View(productLists.ToList());
        }

        // GET: ProductLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductList productList = db.ProductLists.Find(id);
            if (productList == null)
            {
                return HttpNotFound();
            }
            return View(productList);
        }

        // GET: ProductLists/Create
        public ActionResult Create()
        {
            ViewBag.PCid = new SelectList(db.ProductCategoryList, "Id", "PCName");
            return View();
        }

        // POST: ProductLists/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PCid,Name,Price,Img,Sides1,Sides2,Sides3,Sides4")] ProductList productList)
        {
            if (ModelState.IsValid)
            {
                db.ProductLists.Add(productList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PCid = new SelectList(db.ProductCategoryList, "Id", "PCName", productList.PCid);
            return View(productList);
        }

        // GET: ProductLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductList productList = db.ProductLists.Find(id);
            if (productList == null)
            {
                return HttpNotFound();
            }
            ViewBag.PCid = new SelectList(db.ProductCategoryList, "Id", "PCName", productList.PCid);
            return View(productList);
        }

        // POST: ProductLists/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PCid,Name,Price,Img,Sides1,Sides2,Sides3,Sides4")] ProductList productList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PCid = new SelectList(db.ProductCategoryList, "Id", "PCName", productList.PCid);
            return View(productList);
        }

        // GET: ProductLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductList productList = db.ProductLists.Find(id);
            if (productList == null)
            {
                return HttpNotFound();
            }
            return View(productList);
        }

        // POST: ProductLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductList productList = db.ProductLists.Find(id);
            db.ProductLists.Remove(productList);
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
