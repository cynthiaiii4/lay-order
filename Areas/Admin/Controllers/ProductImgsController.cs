﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sys.Models;

namespace sys.Areas.Admin.Controllers
{
    public class ProductImgsController : Controller
    {
        private Membersql db = new Membersql();

        // GET: Admin/ProductImgs
        public ActionResult Index(int id)
        {
            var productImg = db.ProductImg.Where(x=>x.Pid==id);
            return View(productImg.ToList());
        }

        // GET: Admin/ProductImgs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImg productImg = db.ProductImg.Find(id);
            if (productImg == null)
            {
                return HttpNotFound();
            }
            return View(productImg);
        }

        // GET: Admin/ProductImgs/Create
        public ActionResult Create(int?id)
        {
            ViewBag.Pid = new SelectList(db.ProductLists, "Id", "Name");
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Pid,Pimg")] ProductImg productImg, HttpPostedFileBase Pimg)
        {
            if (ModelState.IsValid)
            {
                db.ProductImg.Add(productImg);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Pid = new SelectList(db.ProductLists, "Id", "Name", productImg.Pid);
            return View(productImg);
        }

        // GET: Admin/ProductImgs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.id = id;
            var productImg = db.ProductImg.Where(x=>x.Pid==id);
            
            //ViewBag.Pid = new SelectList(db.ProductLists, "Id", "Name", productImg.Pid);
            return View(productImg);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Pid,Pimg")] ProductImg productImg, HttpPostedFileBase Pimg)
        {
            if (ModelState.IsValid)
            {
                if (Pimg != null)
                {
                    if (Pimg.ContentType.IndexOf("image", System.StringComparison.Ordinal) == -1)
                    {
                        ViewBag.Message = "檔案型態錯誤!";
                        return View();
                    }
                    //取得副檔名
                    string extension = Path.GetExtension(Pimg.FileName);
                    //新檔案名稱
                    string fileName = String.Format("{0:yyyyMMddhhmmsss}{1}", DateTime.Now, extension);
                    string savedName = Path.Combine(Server.MapPath("/Img/product"), fileName);
                    Pimg.SaveAs(savedName);
                    productImg.Pimg = fileName;
                }
                //db.Entry(productImg).State = EntityState.Modified;
                db.ProductImg.Add(productImg);
                db.SaveChanges();
                var newProductImg = db.ProductImg.Where(x => x.Pid == productImg.Pid).ToList();
                return RedirectToAction("Edit", "ProductImgs", new { id = productImg.Pid });
            }
            //ViewBag.Pid = new SelectList(db.ProductLists, "Id", "Name", productImg.Pid);
            return RedirectToAction("Edit", "ProductImgs", new { id = productImg.Pid });
        }

        // POST: Admin/ProductImgs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            ProductImg productImg = db.ProductImg.Find(id);
            db.ProductImg.Remove(productImg);
            db.SaveChanges();
            return RedirectToAction("Edit","ProductImgs",new { id=productImg.Pid});
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
