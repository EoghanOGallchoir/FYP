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

        public ActionResult Vocab(int? page)
        {
            List<VocabList> vocabs = new List<VocabList>();
            foreach (var u in db.VocabLists)
            {
                vocabs.Add(u);
               
            }

            var vocab = from s in db.VocabLists
                        select s;

            vocab = vocab.OrderBy(x => x.ID);

            var userP = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.ProgressXP).SingleOrDefault();
            
            if (userP == 85)
            {
                vocab = vocab.Where(x => x.quizID == 5 && x.quizID == 4 && x.quizID == 3 && x.quizID == 2 && x.quizID == 1);
            }
            else if (userP == 65)
            {
                vocab = vocab.Where(x => x.quizID == 4 && x.quizID == 3 && x.quizID == 2 && x.quizID == 1);
            }
            else if (userP == 50)
            {
                vocab = vocab.Where(x => x.quizID == 3 && x.quizID == 2 && x.quizID == 1);
            }
            else if (userP == 35)
            {
                vocab = vocab.Where(x => x.quizID == 2 && x.quizID == 1);
            }
            else if (userP == 15)
            {
                vocab = vocab.Where(x => x.quizID == 1);
            }
            else if (userP == null)
            {
                vocab = vocab.Where(x => x.quizID == 0);
            }
            
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(vocab.ToPagedList(pageNumber, pageSize));
        }

       
    }
}