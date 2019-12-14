using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sys.Areas.Admin.ViewModel;
using sys.Models;

namespace sys.Areas.Admin.ViewModel
{
    public class BKCompanyController : Controller
    {
        private Membersql db = new Membersql();
        //GET
        public ActionResult Index()
        {
            BKCompanyViewModel company = new BKCompanyViewModel();
            Company original = db.CompanySet.Find(1);
            IEnumerable<holiday> holiday = db.holiday.OrderByDescending(x=>x.StartTime).ToList().Take(10);
            company.Tel = original.Tel;
            company.BigQty = original.BigQty;
            company.StartTime = original.StartTime;
            company.EndTime = original.EndTime;
            company.PrepareTime = original.PrepareTime;
            company.Holidays = holiday;
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BKCompanyViewModel company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return View(company);
            }
            return View(company);
        }
        [HttpPost]
        public ActionResult Create(holiday holiday)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return View(company);
            }
            return RedirectToAction("Index","BKCompany");
        }
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.CompanySet.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index", "BKCompany");
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
