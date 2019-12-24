using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using sys.Filters;
using sys.Models;

namespace sys.Areas.Admin.Controllers
{
    [PermissionFilter]
    [Authorize]
    public class BKTableController : Controller
    {
        private Membersql db = new Membersql();
        private const int PageSize = 10;
        // GET: Admin/BKTable
        public ActionResult Index(int? page)
        {
            if (!page.HasValue)
            {
                page = 0;
            }
            else
            {
                page--;//ToPagedList的pageIndex預設第一頁是0,第二頁是1，所以要-1才是真的頁面
            }
            return View(db.Accounts.Where(x=>x.IsTable==true).OrderByDescending(x=>x.Id).ToList().ToPagedList((int)page, PageSize));
        }

        // GET: Admin/BKTable/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Tel,Password,PasswordSalt,Name,Birth,County,Dist,Vertify,IsCheck,IsTable,Sent,wrong")] Account account)
        {
            try
            {
                //if (System.Text.RegularExpressions.Regex.IsMatch(account.Tel, @"^09[0-9]{8}$") == false)
                //{
                //    return Content("非手機電話號碼");
                //}
                if (db.Accounts.Where(x => x.Tel == account.Tel).Count() > 0)
                {
                    return Content("此帳號已存在，請勿重複設定");
                }
                //產生密碼鹽以及密碼加密
                string salt = Guid.NewGuid().ToString();
                account.Password = GenerateHashWithSalt(account.Password, salt);
                account.PasswordSalt = salt;
                account.Sent = 0;
                account.wrong = 0;
                account.IsTable = true;
                account.IsCheck = true;
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(account);
            }
        }
        #region 密碼加密
        public static string GenerateHashWithSalt(string password, string salt)
        {
            // merge password and salt together
            string sHashWithSalt = password + salt;
            // convert this merged value to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(sHashWithSalt);
            // use hash algorithm to compute the hash
            HashAlgorithm algorithm = new SHA256Managed();
            // convert merged bytes to a hash as byte array
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // return the has as a base 64 encoded string
            return Convert.ToBase64String(hash);
        }
        #endregion
        // GET: Admin/BKTable/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            ViewBag.PWD = account.Password;
            ViewBag.salt = account.PasswordSalt;
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Tel,Password,PasswordSalt,Name,Birth,County,Dist,Vertify,IsCheck,IsTable,Sent,wrong")] Account account, string NewPassword)
        {
            if (ModelState.IsValid)
            {
                //密碼加密
                if (!string.IsNullOrEmpty(NewPassword))
                {
                    string salt = Guid.NewGuid().ToString();
                    account.Password = GenerateHashWithSalt(NewPassword, salt);
                    account.PasswordSalt = salt;
                }
                account.IsTable = true;
                account.IsCheck = true;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // POST: Admin/BKTable/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult QRCode(int id)
        //{
        //    Account table = db.Accounts.Find(id);
        //    string url = "http://www.funcode-tech.com/Encoder_Service/img.aspx?custid=1&username=public&codetype=QR&EClevel=0&data=https://lay-order.rocket-coding.com/index.html#/login?"+ table.Password+"&"+ table.Id + "";
        //    ViewBag.url
        //}
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
