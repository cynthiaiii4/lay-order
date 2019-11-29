using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace sys.Models
{
    public class AccountsController : Controller
    {
        private Membersql db = new Membersql();

        // GET: Accounts
        public ActionResult Index()
        {
            return View(db.Accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Tel,Password,PasswordSalt,Name,Birth,City,Dist,Vertify,Check,IsTable")] Account account)
        {
            if (db.Accounts.Where(x => x.Tel == account.Tel).Count()>0)
            {
                return Content("此電話已存在，請勿重複申請");
            }


            //產生密碼鹽以及密碼加密
                string salt = Guid.NewGuid().ToString();
                account.Password = GenerateHashWithSalt(account.Password,salt);
                account.PasswordSalt = salt;
                //寄發SMS
                string text =SendSMS(account.Tel);
                if (text == "fail")
                {
                    return Content("fail");
                }

                account.Vertify = text;
                db.Accounts.Add(account);
                db.SaveChanges();
            
                return Content("success");
            
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

        #region 驗證碼

        /// <summary>
        /// 產生驗證碼並寄發簡訊
        /// </summary>
        public string SendSMS(string Tel)
        {
            //隨機產生6位數字變成驗證碼
            Random vertifyN = new Random();
            int vertify = vertifyN.Next(100000, 999999);
            //組成簡訊內容並寄發
            string msg = "你好，您在lay-order的帳號驗證碼為" + vertify + "。請於驗證頁面輸入";
            msg=HttpUtility.UrlEncode(msg);
            string url = "http://api.every8d.com/API21/HTTP/sendSMS.ashx?UID=0932961027&PWD=layorder&SB=vertify&MSG=" + msg + "&DEST=" + Tel + "&ST=";
            string text =GetVertifyNumber(url);
            //判斷回傳值
            if (text.StartsWith("-"))
            {
                return "fail";
            }
            else
            {
                return vertify.ToString();
            }

        }
        //戳簡訊API
        private static string GetVertifyNumber(string Url)
        {
            try
            {
                var request = WebRequest.Create(Url);
                string text;
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }

                return text;
            }
            catch (Exception e)
            {
                //return string.Empty;
                throw;
            }
        }

        #endregion


        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Tel,Password,PasswordSalt,Name,Birth,City,Dist,Vertify,Check,IsTable")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
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
