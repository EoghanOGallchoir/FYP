using FYP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace WebApplication3.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private msdb5455Entities db = new msdb5455Entities();

        public ActionResult Index()
        {

            List<User> user = new List<User>();
            foreach (var u in db.Users)
            {
                user.Add(u);
            }

            return View(user);
        }

        public ActionResult About()
        {
            var model = db.Sounds.ToList();
            return View(model);

        }

        public ActionResult GroupHome(int? id)
        {

            if (id == null)
            {
                 RedirectToAction("Create", "Groups");
            }

            TempData["GroupID"] = id;
            TempData.Keep();
            return View(db.Users.ToList().Where(x => x.GroupId == id));

        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // User user = db.Users.Find(id);
            ViewBag.id = id;
            ViewBag.GiD = TempData["GroupID"];
            List<User> user = new List<User>();
            foreach (var u in db.Users)
            {
                user.Add(u);
            }
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult Learn()
        {
            List<User> user = new List<User>();
            foreach (var u in db.Users)
            {
                user.Add(u);
            }

            return View(user);
        }

        public ActionResult Vocab()
        {
            List<User> user = new List<User>();
            foreach (var u in db.Users)
            {
                user.Add(u);
            }

            return View(user);
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