using FYP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Globalization;

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
            TempData["Score"] = 0;
            TempData["Score2"] = 0;
            return View();
        }

        public ActionResult DirectionsBasics()
        {
            return View();
        }

        public ActionResult DirQuiz()
        {
            //  List<DirectionQ> DQ = new List<DirectionQ>();
            if (TempData["i"] == null)
            {
                TempData["i"] = 1;
            }

            try
            {
                DirectionQ q = null;

                if (TempData["qid"] == null)
                {

                    q = db.DirectionQs.First();
                    var list = db.DirectionQs.Skip(Convert.ToInt32(TempData["i"].ToString()));
                    int qid = list.First().Qid;
                    TempData["qid"] = qid;
                }
                else
                {
                    int qid = Convert.ToInt32(TempData["qid"].ToString());
                    q = db.DirectionQs.Where(x => x.Qid == qid).SingleOrDefault();

                  
                    TempData["qid"] = qid;
                    TempData["i"] = Convert.ToInt32(TempData["i"].ToString()) + 1;
                }
                TempData.Keep();
                return View(q);
            }
            catch (Exception)
            {
                return RedirectToAction("DirectionStart");

            }

        }

        [HttpPost]
        public ActionResult DirQuiz(DirectionQ q)
        {
            return RedirectToAction("DirQuiz");
        }

        public ActionResult DirectionsP1()
        {

            if (TempData["qID"] == null)
            {
                TempData["qID"] = 1;
            }

            try
            {
                DirectionQ q = null;
                int qID = Convert.ToInt32(TempData["qID"].ToString());
                q = db.DirectionQs.Where(x => x.Qid == qID).SingleOrDefault();
                TempData["qID"] = ++q.Qid;
                TempData.Keep();
                Debug.WriteLine("before: " + q.CorrectAns);
                return View(q);
            }
            catch (Exception)
            {
                TempData["qID"] = 1;
                return RedirectToAction("ResultsD1");
            }
        }

        [HttpPost]
        public ActionResult DirectionsP1(DirectionQ q)
        {

            Debug.WriteLine("After: " + q.CorrectAns);
            string attemptedAns = null;
            if (q.option1 != null)
            {
                attemptedAns = "A";
            }
            else if (q.option2 != null)
            {
                attemptedAns = "B";
            }
            else if (q.option3 != null)
            {
                attemptedAns = "C";
            }
            else if (q.option4 != null)
            {
                attemptedAns = "D";
            }

            if (attemptedAns.Equals(q.CorrectAns))
            {
                TempData["Score"] = Convert.ToInt32(TempData["Score"]) + 1;
            }

            if (q.CorrectAns == null)
            {
                Debug.WriteLine("answer was passed in as null");
            }
            TempData.Keep();
            return RedirectToAction("DirectionsP1");

        }

        public ActionResult ResultsD1()
        {

            return View();
        }

        public ActionResult DirectionsP2()
        {
            if (TempData["q2ID"] == null)
            {
                TempData["q2ID"] = 1;
            }

            try
            {
                DirectionQ2 q = null;
                int q2ID = Convert.ToInt32(TempData["q2ID"].ToString());
                q = db.DirectionQ2.Where(x => x.Qid == q2ID).SingleOrDefault();
                TempData["q2ID"] = ++q.Qid;
                TempData.Keep();

                return View(q);
            }
            catch (Exception)
            {
                TempData["q2ID"] = 1;
                return RedirectToAction("ResultsD2");
            }
        }

        [HttpPost]
        public ActionResult DirectionsP2(string attemptedAns, DirectionQ2 q2)
        {
            string ans = attemptedAns;
            Debug.WriteLine("Given answer: " + ans);

            if (String.Compare(ans, q2.ans1, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0)
            {
                TempData["Score2"] = Convert.ToInt32(TempData["Score2"]) + 1;
            }
            else if (String.Compare(ans, q2.ans2, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
            {
                TempData["Score2"] = Convert.ToInt32(TempData["Score2"]) + 1;
            }
            else if (String.Compare(ans, q2.ans3, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
            {
                TempData["Score2"] = Convert.ToInt32(TempData["Score2"]) + 1;
            }
            else if (String.Compare(ans, q2.ans4, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
            {
                TempData["Score2"] = Convert.ToInt32(TempData["Score2"]) + 1;
            }

            Debug.WriteLine(TempData["Score2"]);
            Debug.WriteLine("Saved option 1 on the database: " + q2.ans1);
            Debug.WriteLine("Saved option 2 on the database: " + q2.ans2);
            Debug.WriteLine("Saved option 3 on the database: " + q2.ans3);
            Debug.WriteLine("Saved option 4 on the database: " + q2.ans4);
            TempData.Keep();


            return RedirectToAction("DirectionsP2");
        }

        public ActionResult ResultsD2()
        {

            return View();
        }

    }
}
