﻿using System;
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

namespace FYP.Views
{
    public class GroupsController : Controller
    {
        private msdb5455Entities db = new msdb5455Entities();

        // GET: Groups
        public ActionResult Index()
        {
            return View(db.Groups.ToList());
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
                    if (result.isCreator == null || result.isCreator == false)
                    {
                        if (isValid == true)
                        {
                            Debug.WriteLine("result: " + result.GroupId + "group: " + group.GroupID);
                            result.GroupId = group.GroupID;
                            group.Creator = result.UserName;
                            group.GSize = group.GSize + 1;
                            result.isCreator = true;
                            db.Groups.Add(group);
                            db.SaveChanges();
                            return RedirectToAction("GroupHome", "Home");
                        }
                    }
                    ModelState.AddModelError("", "Cannot create a new group, still a group admin!");
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
