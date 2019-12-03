using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace sys.Models
{
    public class AccountsController : Controller
    {
        private Membersql db = new Membersql();

        // POST: Accounts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。

        #region 註冊API
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Tel,Password,PasswordSalt,Name,Birth,City,Dist,Vertify,Check,IsTable")] Account account)
        {
            try
            {
                if (db.Accounts.Where(x => x.Tel == account.Tel).Count() > 0)
                {
                    return Content("此電話已存在，請勿重複申請");
                }
                //產生密碼鹽以及密碼加密
                string salt = Guid.NewGuid().ToString();
                account.Password = GenerateHashWithSalt(account.Password, salt);
                account.PasswordSalt = salt;
                //寄發SMS
                string text = SendSMS(account.Tel);
                if (text == "fail")
                {
                    return Content("fail");
                }
                account.Vertify = text;
                //記住總共發過幾次簡訊
                account.Sent = 1;
                //輸錯驗證次數預設0
                account.wrong = 0;
                db.Accounts.Add(account);
                db.SaveChanges();
                //存註冊ID給驗證API用
                //Session["RegisteredId"] = account.Id.ToString();
                HttpCookie RegisteredId = new HttpCookie("RegisteredId");
                RegisteredId.Value= account.Id.ToString();
                Response.Cookies.Add(RegisteredId);
                //存註冊電話給重新寄發簡訊用
                //Session["RegisteredTel"] = account.Tel;
                //HttpCookie RegisteredTel = new HttpCookie("RegisteredTel");
                //RegisteredTel.Value = account.Tel;
                //Response.Cookies.Add(RegisteredTel);
                //記住總共發過幾次簡訊
                //Session["RegisteredSend"] =1;
                //HttpCookie RegisteredSend = new HttpCookie("RegisteredSend");
                //RegisteredSend.Value = 1.ToString();
                //Response.Cookies.Add(RegisteredSend);
                //驗證次數預設0
                //Session["RegisteredFail"] = 0;
                //HttpCookie RegisteredFail = new HttpCookie("RegisteredFail");
                //RegisteredFail.Value = 0.ToString();
                //Response.Cookies.Add(RegisteredFail);
                
                return Content("success");
            }
            catch(Exception)
            {
                return Content("fail");
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
            msg = HttpUtility.UrlEncode(msg);
            string url = "http://api.every8d.com/API21/HTTP/sendSMS.ashx?UID=0932961027&PWD=layorder&SB=vertify&MSG=" + msg + "&DEST=" + Tel + "&ST=";
            string text = GetAPIResponse(url);
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
        private static string GetAPIResponse(string Url)
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
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion


        #endregion

        #region 確認驗證碼
        public ActionResult Vertify(string Vertify)
        {
            HttpCookie RegisteredId = Request.Cookies["RegisteredId"];
            int id = Convert.ToInt32(RegisteredId.Value);
            //int id = Convert.ToInt32(Session["RegisteredId"]);
            //Account account = db.Accounts.Find(Convert.ToInt32(RegisteredId.Value));
            Account account = db.Accounts.Find(id);
            //if (Request.Cookies["RegisteredFail"] == null)
            //{
            //    HttpCookie RegisteredFail2 = new HttpCookie("RegisteredFail");
            //    RegisteredFail2.Value = 0.ToString();
            //    Response.Cookies.Add(RegisteredFail2);
            //}

            //HttpCookie RegisteredFail = Request.Cookies["RegisteredFail"];
            if (db.Accounts.Where(x => x.Id == id && x.Vertify == Vertify).Count() > 0)
            {
                account.IsCheck = true;
                account.Sent = 0;
                account.wrong = 0;
                db.SaveChanges();
                return Content("success");
            }
            else
            {
                //RegisteredFail.Value = (Convert.ToInt32(RegisteredFail.Value) + 1).ToString();
                //Response.Cookies.Add(RegisteredFail);
                //int fail = Convert.ToInt32(Session["RegisteredFail"]);
                int fail = account.wrong;
                fail = fail + 1;
                //Session["RegisteredFail"] = fail;
                account.wrong = fail;
                //if (RegisteredFail.Value == "3")
                if (fail==3)
                {
                    account.Vertify = 0.ToString();
                    //db.SaveChanges();
                    //RegisteredFail.Value = 0.ToString();
                    //Response.Cookies.Add(RegisteredFail);
                    //fail = 0;
                    //Session["RegisteredFail"] = fail;
                    account.wrong = 0;
                    db.SaveChanges();
                    return Content("驗證碼輸入失敗3次，請重新取得驗證碼");
                }
                db.SaveChanges();
                return Content("驗證失敗，請重新輸入");
            }

        }
        #endregion

        #region 重新寄發驗證碼
        public ActionResult ReSendSMS()
        {
            try
            {
                    HttpCookie RegisteredId = Request.Cookies["RegisteredId"];
                //    HttpCookie RegisteredTel = Request.Cookies["RegisteredTel"];
                //    HttpCookie RegisteredSend = Request.Cookies["RegisteredSend"];
                //    Account account = db.Accounts.Find(Convert.ToInt32(RegisteredId.Value));
                //    if (RegisteredSend.Value == "3")
                //    {
                //        return Content("已寄發3次驗證碼，請您再次確認電話是否正確");
                //    }
                //int id = Convert.ToInt32(Session["RegisteredId"]);
                //int sent = Convert.ToInt32(Session["RegisteredSend"]);
                
                int id = Convert.ToInt32(RegisteredId.Value);
                Account account = db.Accounts.Find(id);
                int sent = account.Sent;
                //Account account = db.Accounts.Find(Convert.ToInt32(RegisteredId.Value));
                //Account account = db.Accounts.Find(id);
                //if (RegisteredSend.Value == "3")
                if (sent == 3)
                {
                    return Content("已寄發3次驗證碼，請您再次確認電話是否正確");
                }

                //隨機產生6位數字變成驗證碼
                Random vertifyN = new Random();
                int vertify = vertifyN.Next(100000, 999999);
                //組成簡訊內容並寄發
                string msg = "你好，您在lay-order的帳號驗證碼為" + vertify + "。請於驗證頁面輸入";
                msg = HttpUtility.UrlEncode(msg);
                //string url =
                //    "http://api.every8d.com/API21/HTTP/sendSMS.ashx?UID=0932961027&PWD=layorder&SB=vertify&MSG=" + msg +
                //    "&DEST=" +Session["RegisteredTel"] + "&ST=";
                string url =
                    "http://api.every8d.com/API21/HTTP/sendSMS.ashx?UID=0932961027&PWD=layorder&SB=vertify&MSG=" + msg +
                    "&DEST=" + account.Tel + "&ST=";
                string text = GetAPIResponse(url);
                //判斷回傳值
                if (text.StartsWith("-"))
                {
                    return Content("fail");
                }
                else
                {
                    account.Vertify = vertify.ToString();
                    account.Sent = sent + 1;
                    db.SaveChanges();
                    //RegisteredSend.Value = (Convert.ToInt32(RegisteredSend.Value) + 1).ToString();
                    //Response.Cookies.Add(RegisteredSend);
                    //sent = sent + 1;
                    //Session["RegisteredSend"] = sent;
                    return Content("success");
                }
            }
            catch
            {
                return Content("fail");
            }
           
        }


        #endregion

        #region 優惠劵
        public ActionResult Voucher()
        {
            DateTime now= DateTime.UtcNow.AddHours(08);
            return Content(JsonConvert.SerializeObject(db.Vouchers.Where(x => x.EndTime > now && x.StartTime < now).ToList()));
        }
        #endregion

        #region 登入
        [HttpPost]
        public ActionResult Login(string Tel,string Password)
        {
            //先對密碼進行加密以利比對
            Account account =db.Accounts.SingleOrDefault(x=>x.Tel==Tel);
            Password = GenerateHashWithSalt(Password, account.PasswordSalt);
            //進行比對
            if (db.Accounts.Where(x => x.Tel == Tel && x.Password == Password && x.IsCheck==true).Count()>0)
            {
                Session["Login"] = true;
                Session["IsTable"] = account.IsTable;
                Session["Id"] = account.Id;
                Session["Tel"] = account.Tel;
                Session["Name"] = account.Name;
                return Content("success");
            }
            if (db.Accounts.Where(x => x.Tel == Tel && x.Password == Password && x.IsCheck==false).Count() > 0)
            {
                return Content("此電話號碼尚未進行驗證");
            }
            return Content("登入失敗");
        }
        #endregion

        #region 檢查是否登入/GET
        public ActionResult CheckLogin()
        {
            if (Session["Login"] != null)
            {
                return Content("True");
            }

            return Content("False");
        }
        #endregion

        #region 內用外帶GET
        public ActionResult IsTable()
        {
            if (Session["IsTable"] != null)
            {
                if (Session["IsTable"].ToString() == "True")
                {
                    return Content("內用");
                }
            }
            return Content("外帶");
        }


        #endregion

        #region 顯示會員資料GET
        public ActionResult ShowUser()
        {
            if (Session["Id"] == null)
            {
                return Content("未登入");
            }
            int id = Convert.ToInt32(Session["Id"]);
            return Content(JsonConvert.SerializeObject(db.Accounts.Where(x=>x.Id==id).Select(x=>new {x.Tel,x.Name,x.Birth,x.City,x.Dist})));
        }
        #endregion

        #region 修改會員資料POST
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(string Name, string NewPassword, string Birth, string County, string Dist)
        {
            try
            {
                if (Session["Id"] == null)
                {
                    return Content("未登入");
                }

                int id = Convert.ToInt32(Session["Id"]);
                Account account = db.Accounts.Find(id);
                account.Name = Name;
                account.Birth = Birth;
                account.City = County;
                account.Dist = Dist;
                if (!string.IsNullOrEmpty(NewPassword))
                {
                    string salt = Guid.NewGuid().ToString();
                    account.Password = GenerateHashWithSalt(account.Password, salt);
                    account.PasswordSalt = salt;
                }
                db.SaveChanges();
                return Content("success");
            }
            catch
            {
                return Content("fail");
            }
        }


        #endregion


        #region 點餐頁面會員資料GET
        public ActionResult OrderMember()
        {
            if (Session["Id"] == null)
            {
                return Content("");
            }
            string Tel = Session["Tel"].ToString();
            string Name = Session["Name"].ToString();
            return Content(Tel + "," + Name);
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
