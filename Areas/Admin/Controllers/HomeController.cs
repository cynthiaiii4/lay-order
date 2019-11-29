using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sys.Models;
using Member = Microsoft.Ajax.Utilities.Member;

namespace sys.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public Membersql _db=new Membersql();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetJson()
        {
            //API寫法
            var about = _db.Members.ToList();
            string json = JsonConvert.SerializeObject(about);
            return Content(json,"application/json");
            //GUID寫法
            //string guid = Guid.NewGuid().ToString();
        }

        public ActionResult GetJson1()
        {
            //當要串成不規則的json給別人時使用
            var about = _db.Members.ToList();

            //如果資料庫不想要全部顯示,只想顯示部分欄位要用Select
            var newAbout = about.Select(x => new {Account = x.Account, name = x.Name});
            //JObject,list內只能是基本型別
            List<string> Account =new List<string>();
            List<string> name = new List<string>();
            string guid = Guid.NewGuid().ToString();

            JObject json = new JObject();
            //取得資料庫應有資訊
            foreach (Models.Member link in about)
            {
                Account.Add(link.Account);
                name.Add(link.Name);
            }
            json.Add(new JProperty("Account", Account));
            json.Add(new JProperty("name", name));
            //加一個資料庫沒有的內容一起串成Json(屬性名稱,資料)
            json.Add(new JProperty("guid", guid));


            //雙層
            JObject jsonNew = new JObject();
            jsonNew.Add("第二層屬性","ohya");

            //加到第一層的物件裡
            json.Add(new JProperty("第二層", jsonNew));

            var jsonContent = JsonConvert.SerializeObject(json,Formatting.Indented);

            return Content(jsonContent, "application/json");
           
        }


        public ActionResult Login()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Account, string Password)
        {
            Models.Member member = _db.Members.FirstOrDefault(x=>x.Account==Account && x.Password==Password);
            if (member == null)
            {
                ViewBag.Message = "登入失敗";
                return View();
            }
            string userData = JsonConvert.SerializeObject(member);
            SetAuthenTicket(userData, member.Account);


            return RedirectToAction("Index","Members");
        }
        //驗證函數
        void SetAuthenTicket(string userData, string userId)
        {
            //宣告一個驗證票
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userId, DateTime.Now, DateTime.Now.AddHours(3), false, userData);
            //加密驗證票
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            //建立Cookie
            HttpCookie authenticationcookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            //將Cookie寫入回應
            Response.Cookies.Add(authenticationcookie);

        }
    }
}