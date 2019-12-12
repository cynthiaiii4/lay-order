using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using sys.Models;

namespace sys.Areas.Admin.Controllers
{
    public class BKBannersController : Controller
    {
        private Membersql db = new Membersql();

        // GET: Admin/BKBanners
        public ActionResult Index()
        {
            return View(db.Banners.ToList());
        }

        // GET: Admin/BKBanners/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/BKBanners/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Img,StartTime,EndTime")] Banner banner, HttpPostedFileBase Img)
        {
            if (ModelState.IsValid)
            {
                if (Img != null)
                {
                    if (Img.ContentType.IndexOf("image", System.StringComparison.Ordinal) == -1)
                    {
                        ViewBag.Message = "檔案型態錯誤!";
                        return View(banner);
                    }
                    //取得副檔名
                    string extension = Path.GetExtension(Img.FileName);
                    //新檔案名稱
                    string fileName = String.Format("{0:yyyyMMddhhmmsss}.{1}", DateTime.Now, extension);
                    string savedName = Path.Combine(Server.MapPath("/Img/BannerImg"), fileName);
                    Img.SaveAs(savedName);
                    banner.Img = fileName;
                }
                db.Banners.Add(banner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(banner);
        }

        // GET: Admin/BKBanners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            ViewBag.Img = banner.Img;
            ViewBag.StartTime = banner.StartTime.ToString("yyyy-MM-dd");
            ViewBag.EndTime = banner.EndTime.ToString("yyyy-MM-dd");
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
        }

        // POST: Admin/BKBanners/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Img,StartTime,EndTime")] Banner banner, HttpPostedFileBase Img)
        {
            if (ModelState.IsValid)
            {
                if (Img != null)
                {
                    if (Img.ContentType.IndexOf("image", System.StringComparison.Ordinal) == -1)
                    {
                        ViewBag.Message = "檔案型態錯誤!";
                        return View(banner);
                    }
                    //取得副檔名
                    string extension = Path.GetExtension(Img.FileName);
                    //新檔案名稱
                    string fileName = String.Format("{0:yyyyMMddhhmmsss}.{1}", DateTime.Now, extension);
                    string savedName = Path.Combine(Server.MapPath("/Img/BannerImg"), fileName);
                    Img.SaveAs(savedName);
                    banner.Img = fileName;
                }
                db.Entry(banner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(banner);
        }

        // GET: Admin/BKBanners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
        }

        // POST: Admin/BKBanners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Banner banner = db.Banners.Find(id);
            db.Banners.Remove(banner);
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
