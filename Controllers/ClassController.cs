using FYP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYP.Controllers
{
    public class ClassController : Controller
    {

        private msdb5455Entities db = new msdb5455Entities();
        // GET: Class
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DirectionStart()
        {
            return View();
        }

        public ActionResult DirectionsBasics()
        {
            return View();
        }

        public ActionResult DirectionsP1()
        {
            List<DirectionQ> DQ = new List<DirectionQ>();
            
                foreach (var dir in db.DirectionQs)
            {
                if (dir.CorrectAns == dir.option1)
                {
                    ViewBag.RightAnswer = dir.option1;
                }
                else if (dir.CorrectAns == dir.option2)
                {
                    ViewBag.RightAnswer = dir.option2;
                }
                else if (dir.CorrectAns == dir.option3)
                {
                    ViewBag.RightAnswer = dir.option3;
                }
                else if (dir.CorrectAns == dir.option4)
                {
                    ViewBag.RightAnswer = dir.option4;
                }
                DQ.Add(dir);
            }

                return View(db.DirectionQs);
        }

        public ActionResult DirectionsP2()
        {
            List<DirectionQ> DQ = new List<DirectionQ>();
            foreach (var dir in db.DirectionQs)
            {
                DQ.Add(dir);
            }

            return View(db.DirectionQs);
        }
    }
}
