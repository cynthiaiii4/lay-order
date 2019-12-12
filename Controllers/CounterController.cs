using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using Newtonsoft.Json;
using sys.Models;

namespace sys.Controllers
{
    public class CounterController : Controller
    {
        private Membersql db = new Membersql();

        //櫃台
        #region 30.顯示訂單總表GET
        public ActionResult ShowOrderList(string type, string status, int page)
        {
            if (Session["EmployeeID"] == null)
            {
                return Content("未登入");
            }
            int PageSize = 9;
            page = page - 1;
            var result = db.Orders.Select(x => new
            {
                Orderid = x.Id,
                isTable = x.Account.IsTable,
                customer = x.Account.Name,
                tel = x.Account.Tel,
                ordertime = x.OrderTime,
                gettime = x.GetTime,
                total = x.OrderDetails.Sum(w => w.Price * w.Qty),
                status = x.Status
            });

            bool isTable = false;
            if (type == "forhere")
            {
                isTable = true;
            }

            if (!string.IsNullOrEmpty(type))
            {
                result = result.Where(x => x.isTable == isTable);
            }
            if (!string.IsNullOrEmpty(status))
            {
                result = result.Where(x => x.status == status);
            }
            var finalResult = result.OrderBy(x => x.gettime).ToPagedList(page, PageSize);
            return Content(JsonConvert.SerializeObject(finalResult));
        }

        #endregion

        #region 33取消訂單GET

        public ActionResult CancelOrder(int id)
        {
            try
            {
                if (Session["EmployeeID"] == null)
                {
                    return Content("未登入");
                }
                Order order = db.Orders.Find(id);
                order.Status = "cancel";
                db.SaveChanges();
                return Content("success");
            }
            catch
            {
                return Content("fail");
            }
        }

        #endregion

        #region 34結帳GET

        public ActionResult CheckOrder(int id)
        {
            try
            {
                if (Session["EmployeeID"] == null)
                {
                    return Content("未登入");
                }
                Order order = db.Orders.Find(id);
                order.Status = "paid";
                db.SaveChanges();
                return Content("success");
            }
            catch
            {
                return Content("fail");
            }
        }

        #endregion

        #region 32送餐完畢
        public ActionResult Orderdelivered(int id)
        {
            try
            {
                if (Session["EmployeeID"] == null)
                {
                    return Content("fail");
                }
                OrderDetail orderDetail = db.OrderDetails.Find(id);
                orderDetail.Status = "done";
                db.SaveChanges();
                return Content("success");
            }
            catch
            {
                return Content("fail");
            }
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
