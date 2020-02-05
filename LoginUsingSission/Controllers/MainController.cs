using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoginUsingSission.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginUsingSission.Controllers
{
    public class MainController : Controller
    {
        umsContext ORM = null;
        public MainController(umsContext _ORM)
        {
            ORM = _ORM;
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
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}