using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using sys.Filters;
using sys.Models;

namespace sys.Areas.Admin.Controllers
{
    [PermissionFilter]
    [Authorize]
    public class MembersController : Controller
    {
        private Membersql db = new Membersql();

        // GET: Members
        public ActionResult Index()
        {
            return View(db.Members.ToList());
        }

        // GET: Members/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // GET: Members/Create
        public ActionResult Create()
        {
            List<Permission> permission = db.Permissions.ToList();
            StringBuilder stringBuilder = new StringBuilder();
            getList(permission, stringBuilder);
            @ViewBag.data = stringBuilder.ToString();
            return View();
        }

        public void getList(List<Permission> permission, StringBuilder stringBuilder)
        {
            foreach (var item in permission)
            {
                if (stringBuilder.ToString().IndexOf("{id:" + item.id) == -1)
                {
                    stringBuilder.Append("{id:" + item.id + ",text:'" + item.PermissionName + "'");
                    if (permission.Where(x => x.Pid == item.id).Count() > 0)
                    {
                        stringBuilder.Append(",children:[");
                        var subP = permission.Where(x => x.Pid == item.id).ToList();
                        getList(subP, stringBuilder);
                        stringBuilder.Append("]");
                    }
                    stringBuilder.Append("},");
                }
            }
        }

        // POST: Members/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Account,Password,PasswordSalt,Name,Gender,Email,Permission")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(member);
        }


        // GET: Members/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Account,Password,PasswordSalt,Name,Gender,Email,Permission")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(member);
        }

        // GET: Members/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = db.Members.Find(id);
            db.Members.Remove(member);
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


        #region 驗證

        /// <summary>
        /// 產生驗證碼並寄發簡訊
        /// </summary>
        //public ActionResult SendSMS([Bind(Include = "Id,Account,Password,PasswordSalt,Name,Gender,Email,Permission,tel")] Member member)
        //{
        //    //隨機產生6位數字
        //    Random vertifyN = new Random();
        //    int vertify = vertifyN.Next(100000, 999999);
        //    string msg = "你好，您在lay-order的帳號驗證碼為" + vertify + "。請於驗證頁面輸入";
        //    string url = "https://api.every8d.com/API21/HTTP/sendSMS.ashx?UID=0932961027&PWD=layorder&SB=vertify&MSG="+msg+"&DEST="+member.Tel+"&ST=";
        //    if (GetVertifyNumber(url).StartsWith("-"))
        //    {
        //        //todo:回到錯誤頁??
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        //存到資料庫
        //        member.Account = vertify;
        //            db.Members.Add(member);
               
        //        db.SaveChanges();
        //        //todo:回到驗證頁??
        //        return RedirectToAction("Index");
        //    }
            
        //}
        //private static string GetVertifyNumber(string Url)
        //{
        //    try
        //    {
        //        //string targetURI = Url;
        //        var request = WebRequest.Create(Url);
        //        string text;
        //        var response = (HttpWebResponse)request.GetResponse();
        //        using (var sr = new StreamReader(response.GetResponseStream()))
        //        {
        //            text = sr.ReadToEnd();
        //        }

        //        return text;
        //    }
        //    catch (Exception)
        //    {
        //        return string.Empty;
        //    }
        //}

        #endregion
    }
}
