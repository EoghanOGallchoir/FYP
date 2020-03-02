using FYP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace FYP.Controllers
{

    [Authorize]
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
            TempData["Score3"] = 0;
            TempData["qID"] = 1;
            TempData["q2ID"] = 1;
            TempData["q3ID"] = 1;
            TempData.Keep();
            List<User> user = new List<User>();
            foreach (var u in db.Users)
            {
                user.Add(u);
            }

            return View(user);
            
        }

   
        public ActionResult DirectionsP1()
        {
            string userName = User.Identity.GetUserName();
            //var result = db.Users.SingleOrDefault(s => s.UserName == userName);
            ViewBag.A = "A";
            ViewBag.B = "B";
            ViewBag.C = "C";
            ViewBag.D = "D";
                if (TempData["qID"] == null)
                {
                    TempData["qID"] = 1;
                }

                try
                {
                    DirectionQ q = null;
                    int qID = Convert.ToInt32(TempData["qID"]);
                    q = db.DirectionQs.Where(x => x.Qid == qID).SingleOrDefault();
                    TempData["qID"] = ++q.Qid;
                    TempData.Keep();
                    Debug.WriteLine("before: " + q.CorrectAns);
                    return View(q);
                }
                catch (Exception)
                {
                    TempData["qID"] = 1;
                    TempData.Keep();
                    return RedirectToAction("ResultsD1");
                }

          
        
    }

        [HttpPost]  
        public ActionResult DirectionsP1(DirectionQ q)
        {
            
            Debug.WriteLine("After: " + q.CorrectAns);
            String attemptedAns = Request.Form["Model.Qid"];
           // string attemptedAns = "F";
            if (attemptedAns == "1")
            {
                attemptedAns = "A";
            }
            else if (attemptedAns == "2")
            {
                attemptedAns = "B";
            }
            else if (attemptedAns == "3")
            {
                attemptedAns = "C";
            }
            else if (attemptedAns == "4")
            {
                attemptedAns = "D";
            }

            
            Debug.WriteLine("atAns:" + attemptedAns);
            
            

            if (attemptedAns.Equals(q.CorrectAns.ToString()))
            {
                
                TempData["Score"] = Convert.ToInt32(TempData["Score"]) + 1;
                Debug.WriteLine("Score: " + TempData["Score"]);
            }
        
            if (q.CorrectAns == null)
            {
                Debug.WriteLine("answer was passed in as null");
            }


           
            TempData.Keep();

            if (Convert.ToInt32(TempData["Score"]) == 4)
            {
                if (ModelState.IsValid)
                 {
                Debug.WriteLine("Model Valid");
               // TempData.Keep();
                Debug.WriteLine(TempData["Score"]);
                //Debug.WriteLine(scoreq);
                string userId = User.Identity.GetUserName();
                Debug.WriteLine("user.id.guid: " + userId);
                
                    try
                    {
                        using (var db = new msdb5455Entities())
                        {
                            var result = db.Users.SingleOrDefault(s => s.UserName == userId);
                            Debug.WriteLine("null? " + result);
                            if(result.ProgressXP == null)
                            {
                                result.ProgressXP = 0;
                            }
                            if (result != null) // if result is null then there is no student in the db with that id 
                            {
                                Debug.WriteLine("true? "+result);
                                if (result.quiz1 != true)
                                {
                                    result.ProgressXP = result.ProgressXP + 15;
                                    Debug.WriteLine("progress incremented:" + result.ProgressXP);
                                    result.quiz1 = true;
                                    db.SaveChanges();
                                  //  RedirectToAction("DirectionStart");
                                }
                               // RedirectToAction("DirectionStart");
                            }

                        }


                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting
                                // the current instance as InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                }
            }
            return RedirectToAction("DirectionsP1");

          

        }


        public ActionResult ResultsD1()
        {
            TempData.Keep();
            ViewBag.Score = TempData["Score"];
            return View();
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResultsD1([Bind(Include = "Id,ProgressXP,quiz1")] User user)
        {
            Debug.WriteLine("Entering post");
           // var scoreq = Request["score1"];
            if (ModelState.IsValid)
            {
                Debug.WriteLine("Model Valid");
                TempData.Keep();
                Debug.WriteLine(TempData["Score"]);
                //Debug.WriteLine(scoreq);
                string userId = User.Identity.GetUserName();
                Debug.WriteLine("user.id.guid: " + userId);
                if (Convert.ToInt32(TempData["Score"]) == 4)
                {
                    try
                    {
                        using (var db = new msdb5455Entities())
                        {
                            var result = db.Users.SingleOrDefault(s => s.UserName == userId);
                            Debug.WriteLine("null? " + result);
                            if (result != null) // if result is null then there is no student in the db with that id 
                            {
                                if (result.quiz1 != true)
                                {
                                    result.ProgressXP = result.ProgressXP + 10;
                                    result.quiz1 = true;
                                    db.SaveChanges();
                                    RedirectToAction("DirectionStart");
                                }
                                RedirectToAction("DirectionStart");
                            }

                        }


                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting
                                // the current instance as InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                }
            }
            return View(user);
        }
        */
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


            if (Convert.ToInt32(TempData["Score2"]) == 4)
            {
                if (ModelState.IsValid)
                {
                    Debug.WriteLine("Model Valid");
                    TempData.Keep();
                    Debug.WriteLine(TempData["Score2"]);
                    //Debug.WriteLine(scoreq);
                    string userId = User.Identity.GetUserName();
                    Debug.WriteLine("user.id.guid: " + userId);
                
                    try
                    {
                        using (var db = new msdb5455Entities())
                        {
                            var result = db.Users.SingleOrDefault(s => s.UserName == userId);
                            Debug.WriteLine("null? " + result);
                            if(result.ProgressXP == null)
                            {
                                result.ProgressXP = 0;
                            }
                            if (result != null) // if result is null then there is no student in the db with that id 
                            {
                                if (result.quiz2 != true)
                                {
                                    result.ProgressXP = result.ProgressXP + 20;
                                    result.quiz2 = true;
                                    db.SaveChanges();
                                   // RedirectToAction("DirectionStart");
                                }
                              //  RedirectToAction("DirectionStart");
                            }

                        }


                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting
                                // the current instance as InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                }
            }
            //return View(user);

            return RedirectToAction("DirectionsP2");
        }

        public ActionResult ResultsD2()
        {

            return View();
        }


        public ActionResult DirectionsP3()
        {
            
            if (TempData["q3ID"] == null)
            {
                TempData["q3ID"] = 1;
            }

            try
            {
                Debug.WriteLine("Inside DP3 try");
                DirectionQ3 q = null;
                int q3ID = Convert.ToInt32(TempData["q3ID"]);
                Debug.WriteLine("q ID: "+q3ID);
                q = db.DirectionQ3.Where(x => x.Qid == q3ID).SingleOrDefault();
                Debug.WriteLine("Q: "+ q);
                TempData["q3ID"] = ++q.Qid;
                TempData.Keep();
                Debug.WriteLine("before: " + q.CorrectAns);
                return View(q);
            }
            catch (Exception)
            {
                TempData["q3ID"] = 1;
                Debug.WriteLine("Inside DP3 catch");
                TempData.Keep();
                return RedirectToAction("ResultsD3");
            }


            // return View();
        }


        [HttpPost]
        public ActionResult DirectionsP3(DirectionQ3 q)
        {

            Debug.WriteLine("After: " +q.CorrectAns);
            String attemptedAns = Request.Form["Model.Qid"];

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
            Debug.WriteLine("ans: " + attemptedAns);
            Debug.WriteLine("correct: " + q.CorrectAns);

            

            if (attemptedAns.Equals(q.CorrectAns))
            {
                Debug.WriteLine("im adding");
                TempData["Score3"] = Convert.ToInt32(TempData["Score3"]) + 1;
                Debug.WriteLine("Entered " + TempData["Score3"]);
            }
            Debug.WriteLine(TempData["Score3"]);
            if (q.CorrectAns == null)
            {
                Debug.WriteLine("answer was passed in as null");
            }



            TempData.Keep();

            if (Convert.ToInt32(TempData["Score3"]) == 5)
            {
                if (ModelState.IsValid)
                {
                    Debug.WriteLine("Model Valid");
                    // TempData.Keep();
                    Debug.WriteLine(TempData["Score3"]);
                    //Debug.WriteLine(scoreq);
                    string userId = User.Identity.GetUserName();
                    Debug.WriteLine("user.id.guid: " + userId);

                    try
                    {
                        using (var db = new msdb5455Entities())
                        {
                            var result = db.Users.SingleOrDefault(s => s.UserName == userId);
                            Debug.WriteLine("null? " + result);
                            if (result.ProgressXP == null)
                            {
                                result.ProgressXP = 0;
                            }
                            if (result != null) // if result is null then there is no student in the db with that id 
                            {
                                Debug.WriteLine("true? " + result);
                                if (result.quiz3 != true)
                                {
                                    result.ProgressXP = result.ProgressXP + 15;
                                    Debug.WriteLine("progress incremented:" + result.ProgressXP);
                                    result.quiz3 = true;
                                    db.SaveChanges();
                                    //  RedirectToAction("DirectionStart");
                                }
                                // RedirectToAction("DirectionStart");
                            }

                        }


                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting
                                // the current instance as InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                }
            }
            return RedirectToAction("DirectionsP3");
        }

        public ActionResult ResultsD3()
        {
            return View();
        }
    }

   

}
