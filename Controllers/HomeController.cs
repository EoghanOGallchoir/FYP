using FYP.Models;
using Microsoft.AspNet.Identity;
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

        public ActionResult GroupHome()
        {
            List <Group> groups = new List<Group>();
            foreach (var u in db.Groups)
            {
                groups.Add(u);
            }

            return View(groups);

        }

        public ActionResult MyGroup(string id)
        {
                      
            var q = db.Users.Where(x => x.UserName == id).Select(x => x.GroupId);



            Debug.WriteLine("someting: "+q);

            return View(db.Users.Where(x => x.GroupId == q.FirstOrDefault()));
        }

        public ActionResult Join(int? id)
        {
            ViewBag.Gid = id;
            TempData["GGG"] = id;
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public ActionResult Join(FYP.Models.Group model)
        {
            int id = Convert.ToInt32(TempData["GGG"]);
            Debug.WriteLine("model:"+model.GroupPass);
            Debug.WriteLine("model:" + TempData["GGG"]);
            using (var context = new msdb5455Entities())
            {
                bool isValid = context.Groups.Any(x => x.GroupPass == model.GroupPass && x.GroupID == id);
                Debug.WriteLine("check:" + isValid);
                if (isValid == true)
                {
                    Debug.WriteLine("inside check");
                    string userId = User.Identity.GetUserName();
                    Debug.WriteLine("user: " + userId);
                    var result = db.Users.SingleOrDefault(s => s.UserName == userId);
                    Debug.WriteLine("result.Gid: "+result.GroupId);
                    result.GroupId = id;
                    Debug.WriteLine("model.gid: "+ result.GroupId);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid password");
            }
            return View();
        }

        public ActionResult Leave()
        {
            //are you sure etc.
            return View();
        }

        [HttpPost]
        public ActionResult Leave(int? id)
        {

            return View();
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
            TempData.Keep();
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

        

    }
}