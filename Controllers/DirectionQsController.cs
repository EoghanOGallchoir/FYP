using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FYP.Models;

namespace FYP.Controllers
{
    public class DirectionQsController : Controller
    {
        private msdb5455Entities db = new msdb5455Entities();

        // GET: DirectionQs
        public ActionResult Index()
        {
            return View(db.DirectionQs.ToList());
        }

        // GET: DirectionQs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectionQ directionQ = db.DirectionQs.Find(id);
            if (directionQ == null)
            {
                return HttpNotFound();
            }
            return View(directionQ);
        }

        // GET: DirectionQs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DirectionQs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Qid,Question,option1,option2,option3,option4,CorrectAns")] DirectionQ directionQ)
        {
            if (ModelState.IsValid)
            {
                db.DirectionQs.Add(directionQ);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(directionQ);
        }

        // GET: DirectionQs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectionQ directionQ = db.DirectionQs.Find(id);
            if (directionQ == null)
            {
                return HttpNotFound();
            }
            return View(directionQ);
        }

        // POST: DirectionQs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Qid,Question,option1,option2,option3,option4,CorrectAns")] DirectionQ directionQ)
        {
            if (ModelState.IsValid)
            {
                db.Entry(directionQ).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(directionQ);
        }

        // GET: DirectionQs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectionQ directionQ = db.DirectionQs.Find(id);
            if (directionQ == null)
            {
                return HttpNotFound();
            }
            return View(directionQ);
        }

        // POST: DirectionQs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DirectionQ directionQ = db.DirectionQs.Find(id);
            db.DirectionQs.Remove(directionQ);
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
    }
}
