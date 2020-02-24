using FYP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication3.Controllers
{

    public class HomeController : Controller
    {
        private msdb5455Entities db = new msdb5455Entities();

        public ActionResult Index()
        {

            List<AspNetUser> user = new List<AspNetUser>();
            foreach (var u in db.AspNetUsers)
            {
                user.Add(u);
            }
            ViewBag.person = user;
            return View(db.AspNetUsers);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Learn()
        {
            return View();
        }

        public ActionResult Vocab()
        {
            return View();
        }

        public ActionResult TestView()
        {
            List<AspNetUser> user = new List<AspNetUser>();
            foreach (var u in db.AspNetUsers)
            {
                user.Add(u);
            }
            ViewBag.person = user;
            return View(db.AspNetUsers);
        }

    }
}