using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginUsingSission.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace LoginUsingSission.Controllers
{
    public class MainController : Controller
    {
        umsContext ORM = null;
        private readonly IDistributedCache _cache;

        public MainController(umsContext _ORM, IDistributedCache cache)
        {
            ORM = _ORM;
            _cache = cache;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterNewUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterNewUser(SystemUsers u)
        {
            try
            {
                u.Role = "Staff";
                u.Status = "Active";
                ORM.SystemUsers.Add(u);
                ORM.SaveChanges();
                ViewBag.Message = "用户 " + u.UserName + " 注册成功！";
            }
            catch(Exception e)
            {
                ViewBag.Message = "新用户注册失败! "+e.Message.ToString();
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.LTLD = Request.Cookies["LastLoggedInTime"];
            return View();
        }

        [HttpPost]
        public IActionResult Login(SystemUsers u)
        {
            var LoggedInUser = ORM.SystemUsers.Where(x => x.UserName == u.UserName && x.Password == u.Password).FirstOrDefault();

            if (LoggedInUser == null)
            {
                ViewBag.Message = "用户名或密码错误";
                return View();
            }

            //以下是使用AddDistributedMemoryCache把session存放在服务器内存中
            //HttpContext.Session.SetString("UserName", LoggedInUser.UserName);
            //HttpContext.Session.SetString("Role", LoggedInUser.Role);

            //以下是使用AddDistributedSqlServerCache储存Session时，把信息(不是用于本例中登陆的信息，因为这样写入是面对所有浏览器的)写入Session的方法 可以使用 SetAsync及SetStringAsync两种方法
            //byte[] currentVersion= Encoding.UTF8.GetBytes("Version 1.0");
            //var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(300));
            //await _cache.SetAsync("Version", currentVersion, options);
            //await _cache.SetStringAsync("testSave", "abc",options);

            //以下是使用Sqlserver存放登陆信息是的语句
            HttpContext.Session.SetString("UserName", LoggedInUser.UserName);
            HttpContext.Session.SetString("Role", LoggedInUser.Role);

            Response.Cookies.Append("LastLoggedInTime", DateTime.Now.ToString());

            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard() 
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Login");
            }
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");

            //以下是使用AddDistributedSqlServerCache储存Session时,读取session信息(不是用于本例中登陆的信息)
            //var encodedVersion = await _cache.GetAsync("currentVersion");
            //var currentVersion = "";
            //if (encodedVersion != null)
            //{
            //    currentVersion = Encoding.UTF8.GetString(encodedVersion);
            //}


            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}