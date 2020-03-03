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
using PagedList;

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


        public ActionResult GroupHome(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";

            var user = User.Identity.Name;
            var g = db.Users.Where(x => x.UserName == user).Select(x => x.GroupId).FirstOrDefault();
          //  var gName = db.Groups.Where(x => x.GroupID == g).Select(x => x.GName).FirstOrDefault();

            ViewBag.GiD = g;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var group = from s in db.Groups
                           select s;

            var id = User.Identity.GetUserName();

            
            if (!String.IsNullOrEmpty(searchString))
            {
                group = group.Where(s => s.GName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    group = group.OrderByDescending(s => s.GName);
                    break;
                case "id_desc":
                    group = group.OrderByDescending(s => s.GroupID);
                    break;
                default: // Not: case "Default"
                    group = group.OrderBy(x => x.GName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            // return View(students.ToPagedList(pageNumber, pageSize));
           

            return View(group.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult MyGroup(string id)
        {

            var user = User.Identity.Name;
            var q = db.Users.Where(x => x.UserName == id).Select(x => x.GroupId);
            var g = db.Users.Where(x => x.UserName == user).Select(x => x.GroupId).FirstOrDefault();
            var gName = db.Groups.Where(x => x.GroupID == g).Select(x => x.GName).FirstOrDefault();

            ViewBag.GName = gName;
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
                    var r2 = db.Groups.SingleOrDefault(r => r.GroupID == id);
                    if (result.GroupId == id)
                    {
                        RedirectToAction("GroupHome", "Home");
                    }
                    else
                    {
                        Debug.WriteLine("result.Gid: " + result.GroupId);
                        result.GroupId = id;
                        r2.GSize = r2.GSize + 1;
                        Debug.WriteLine("model.gid: " + result.GroupId);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid password");
            }
            return View();
        }

        public ActionResult Leave(int? id)
        {
            ViewBag.GroupID = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        [HttpPost, ActionName("Leave")]
        [ValidateAntiForgeryToken]
        public ActionResult Leave(int id)
        {

            int Gid = id;
            Debug.WriteLine("g id:" + Gid);
          
            using (var context = new msdb5455Entities())
            {
                bool isValid = context.Groups.Any(x => x.GroupID == Gid);
                Debug.WriteLine("check:" + isValid);
                if (isValid == true)
                {
                    Debug.WriteLine("inside check");
                    string userId = User.Identity.GetUserName();
                    Debug.WriteLine("user: " + userId);
                    var result = db.Users.SingleOrDefault(s => s.UserName == userId);
                    Debug.WriteLine("result.Gid: " + result.GroupId);
                    result.GroupId = null;
                    if (result.isCreator == true)
                    {
                        result.isCreator = false;
                    }
                    Debug.WriteLine("model.gid: " + result.GroupId);
                    db.SaveChanges();
                    return RedirectToAction("GroupHome", "Home");
                }
               
            }
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

        public ActionResult GroupChat()
        {
            return View();
        }

    }
}