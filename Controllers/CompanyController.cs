using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sys.Models;

namespace sys.Controllers
{
    public class CompanyController : Controller
    {
        private Membersql db = new Membersql();

        #region 是否營業GET
        public ActionResult IsOpen()
        {
            DateTime today = DateTime.UtcNow.AddHours(23);
            if (db.holiday.Where(x => x.EndTime > today && x.StartTime < today).Count() > 0)
            {
                return Content("no");
            }
            int id = db.CompanySet.Count();
            Company company = db.CompanySet.Find(id);
            DateTime date = DateTime.Today.AddHours(23);
            string startTime = date.ToString("yyyy-MM-dd") + " " + company.StartTime;
            string endTime = date.ToString("yyyy-MM-dd") + " " + company.EndTime;
            DateTime start = Convert.ToDateTime(startTime);
            DateTime end = Convert.ToDateTime(endTime);

            if (start < today && end > today)
            {
                return Content("yes");
            }
            return Content("no");
        }
        #endregion

        #region 備餐時間GET

        public ActionResult PreTime()
        {
            int preTime = db.CompanySet.OrderByDescending(x => x.Id).FirstOrDefault().PrepareTime;
            return Content(preTime.ToString());
        }

        #endregion

        #region 店家電話

        public ActionResult GetTel()
        {
            return Content(db.CompanySet.OrderByDescending(x => x.Id).FirstOrDefault().Tel);
        }

        #endregion
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
