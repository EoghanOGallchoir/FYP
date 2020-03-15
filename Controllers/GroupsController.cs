using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using Microsoft.AspNet.Identity;
using PagedList;

namespace FYP.Views
{
    public class GroupsController : Controller
    {
        private msdb5455Entities db = new msdb5455Entities();

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
            Debug.WriteLine("someting: " + q);

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
            Debug.WriteLine("model:" + model.GroupPass);
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
                        RedirectToAction("GroupHome", "Groups");
                    }
                    else
                    {
                        Debug.WriteLine("result.Gid: " + result.GroupId);
                        result.GroupId = id;
                        
                        r2.GSize = r2.GSize + 1;
                        Debug.WriteLine("model.gid: " + result.GroupId);
                        db.SaveChanges();
                        return RedirectToAction("MyGroup", "Groups", new { id = User.Identity.Name});
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
                    var r2 = db.Groups.SingleOrDefault(r => r.GroupID == id);
                    r2.GSize = r2.GSize - 1;
                    if (result.isCreator == true)
                    {
                        result.isCreator = false;
                    }
                    Debug.WriteLine("model.gid: " + result.GroupId);
                    db.SaveChanges();
                    return RedirectToAction("GroupHome", "Groups");
                }

            }
            return View();
        }


        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
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

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupPass,GSize,GName,Creator")] Group group)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserName();
                using (var context = new msdb5455Entities())
                {
                    bool isValid = context.Users.Any(x => x.UserName == userId);
                    var result = db.Users.SingleOrDefault(s => s.UserName == userId);
                    
                    Debug.WriteLine("check:" + isValid);
                   
                        if (isValid == true)
                        {
                            Debug.WriteLine("result: " + result.GroupId + "group: " + group.GroupID);
                            result.GroupId = group.GroupID;
                            group.Creator = result.UserName;
                            group.GSize = group.GSize + 1;
                            result.isCreator = true;
                            db.Groups.Add(group);
                            db.SaveChanges();
                            return RedirectToAction("GroupHome", "Groups");
                        }
                    return View();
                }
            }

            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
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

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,GroupPass,GSize,GName")] Group group)
        {
            var user = User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyGroup", "Home", new { id = user});
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
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

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
          
            string userId = User.Identity.GetUserName();
           
            using (var context = new msdb5455Entities())
            {
                bool isValid = context.Users.Any(x => x.UserName == userId);
                var result = db.Users.SingleOrDefault(s => s.GroupId == id);
                var r2 = db.Groups.SingleOrDefault(r => r.GroupID == id);
                
                if (isValid == true)
                {
                    result.GroupId = null;
                    result.isCreator = false;
                }
            }
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("GroupHome", "Home");
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
