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
using MMLib.Extensions;

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
            TempData.Clear();
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
            
          
            String attemptedAns = Request.Form["Model.Qid"];
          
            Debug.WriteLine("atAns:" + attemptedAns);
            Debug.WriteLine("idq1: " + q.Qid);

            var db_ans = db.DirectionQs.Where(i => i.Qid == q.Qid-1).Select(i => i.CorrectAns).FirstOrDefault();
            Debug.WriteLine("idq1.1: " + db_ans);
          
            if (attemptedAns.Equals(db_ans))
            {
                
                TempData["Score"] = Convert.ToInt32(TempData["Score"]) + 1;
                Debug.WriteLine("Score: " + TempData["Score"]);
            }
            else
            {

               
                var wrong = q.Qid - 1;
                TempData["Wrong Answers:"] = Convert.ToString(TempData["Wrong Answers:"]) + "You got question "+ wrong.ToString() + " wrong." + "<br />";
              
            }
        
            if (db_ans == null)
            {
                Debug.WriteLine("answer was passed in as null");
            }


           

            TempData.Keep();

            if (Convert.ToInt32(TempData["Score"]) == 4)
            {
                if (ModelState.IsValid)
                 {
               
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
            var completed = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.quiz1).FirstOrDefault();
            TempData["Completed"] = completed;
            TempData.Keep();
            return View();
        }

    
        public ActionResult DirectionsP2()
        {

            var access = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.ProgressXP).SingleOrDefault();
            if (access == 15)
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

            return RedirectToAction("DirectionStart");

        }

        [HttpPost]
        public ActionResult DirectionsP2(string attemptedAns, DirectionQ2 q2)
        {
            string ans = attemptedAns.ToLower();
            ans = ans.RemoveDiacritics();
            Debug.WriteLine("accents removed? "+ ans);


            var db_ans = db.DirectionQ2.Where(i => i.Qid == q2.Qid - 1).Select(i => i.ans1).FirstOrDefault();
            var db_k1 = db.DirectionQ2.Where(i => i.Qid == q2.Qid - 1).Select(i => i.keyword1).FirstOrDefault();
            var db_k2 = db.DirectionQ2.Where(i => i.Qid == q2.Qid - 1).Select(i => i.keyword2).FirstOrDefault();



            Debug.WriteLine("db_ans 2 check? " + db_ans);
            string check1 = db_ans.ToLower();
            check1 = check1.RemoveDiacritics();
            Debug.WriteLine("Given answer: " + check1);

            Debug.WriteLine(TempData["Score2"]);
            Debug.WriteLine("Saved option 1 on the database: " + db_ans);
            
            TempData.Keep();


            string attempted = ans;
            int n = attempted.Length;

            string correctAns = db_ans;
            int m = correctAns.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }
            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }
            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (correctAns[j - 1] == attempted[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            var comp =  d[n, m];
            var similarity = (1.0 - ((double)comp / (double)Math.Max(attempted.Length, correctAns.Length)));
           
            //contains one of the keywords
            if (ans.Contains(db_k1) || ans.Contains(db_k2))
            {
                //similarity greater than .7 (70%)
                if (similarity > 0.7)
                {
                    TempData["Score2"] = Convert.ToInt32(TempData["Score2"]) + 1;
                }
            }
            else
            {


                var wrong = q2.Qid - 1;
                TempData["Wrong Answers2:"] = Convert.ToString(TempData["Wrong Answers2:"]) + "You got question " + wrong.ToString() + " wrong." + "<br />";
                
            }

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
            var completed = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.quiz2).FirstOrDefault();
            TempData["Completed2"] = completed;
            TempData.Keep();
            return View();
        }


        public ActionResult DirectionsP3()
        {
            var access = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.ProgressXP).SingleOrDefault();
            if (access == 35)
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
                    Debug.WriteLine("q ID: " + q3ID);
                    q = db.DirectionQ3.Where(x => x.Qid == q3ID).SingleOrDefault();
                    Debug.WriteLine("Q: " + q);
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

            }
            return RedirectToAction("DirectionStart");
            // return View();
        }


        [HttpPost]
        public ActionResult DirectionsP3(DirectionQ3 q)
        {

         
            String attemptedAns = Request.Form["Model.Qid"];

            Debug.WriteLine("ans: " + attemptedAns);
            var db_ans = db.DirectionQ3.Where(i => i.Qid == q.Qid - 1).Select(i => i.CorrectAns).FirstOrDefault();




            if (attemptedAns.Equals(db_ans))
            {
                Debug.WriteLine("im adding");
                TempData["Score3"] = Convert.ToInt32(TempData["Score3"]) + 1;
                Debug.WriteLine("Entered " + TempData["Score3"]);
            }
            else
            {


                var wrong = q.Qid - 1;
                TempData["Wrong Answers3:"] = Convert.ToString(TempData["Wrong Answers:3"]) + "Fuair tú ceist " + wrong.ToString() + " mícheart." + "<br />";
                
            }

            Debug.WriteLine(TempData["Score3"]);
            if (db_ans == null)
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
            var completed = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.quiz3).FirstOrDefault();
            TempData["Completed3"] = completed;
            TempData.Keep();
            return View();
        }

        public ActionResult SportStart()
        {

            TempData["Score4"] = 0;
            TempData["Score5"] = 0;
            TempData["Score6"] = 0;
            TempData["q4ID"] = 1;
            TempData["q5ID"] = 1;
            TempData["q6ID"] = 1;
            TempData.Keep();
            List<User> user = new List<User>();
            foreach (var u in db.Users)
            {
                user.Add(u);
            }

            return View(user);

        }

        public ActionResult SportP1()
        {
            var access = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.ProgressXP).SingleOrDefault();
            if (access == 50)
            {

                string userName = User.Identity.GetUserName();
                //var result = db.Users.SingleOrDefault(s => s.UserName == userName);

                if (TempData["q4ID"] == null)
                {
                    TempData["q4ID"] = 1;
                }

                try
                {
                    SportQ q = null;
                    int qID = Convert.ToInt32(TempData["q4ID"]);
                    q = db.SportQs.Where(x => x.Qid == qID).SingleOrDefault();
                    TempData["q4ID"] = ++q.Qid;
                    TempData.Keep();
                    Debug.WriteLine("before: " + q.correctAns);
                    return View(q);
                }
                catch (Exception)
                {
                    TempData["q4ID"] = 1;
                    TempData.Keep();
                    return RedirectToAction("ResultsS1");
                }

            }
            return RedirectToAction("SportStart");
        }

        [HttpPost]
        public ActionResult SportP1(SportQ q)
        {

         
            String attemptedAns = Request.Form["Model.Qid"];

            Debug.WriteLine("id: " + q.Qid);

            var db_ans = db.SportQs.Where(i => i.Qid == q.Qid - 1).Select(i => i.correctAns).FirstOrDefault();

            Debug.WriteLine("atAns:" + attemptedAns);
            Debug.WriteLine("Ans from db query: " + db_ans);


            if (attemptedAns.Equals(db_ans))
            {

                TempData["Score4"] = Convert.ToInt32(TempData["Score4"]) + 1;
                Debug.WriteLine("Score: " + TempData["Score4"]);
            }
            else
            {


                var wrong = q.Qid - 1;
                TempData["Wrong Answers4:"] = Convert.ToString(TempData["Wrong Answers:4"]) + "Fuair tú ceist " + wrong.ToString() + " mícheart." + "<br />";

            }

            if (db_ans == null)
            {
                Debug.WriteLine("answer was passed in as null");
            }



            TempData.Keep();

            if (Convert.ToInt32(TempData["Score4"]) == 4)
            {
                if (ModelState.IsValid)
                {
                    Debug.WriteLine("Model Valid");
                    // TempData.Keep();
                    Debug.WriteLine(TempData["Score4"]);
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
                                if (result.quiz4 != true)
                                {
                                    result.ProgressXP = result.ProgressXP + 15;
                                    Debug.WriteLine("progress incremented:" + result.ProgressXP);
                                    result.quiz4 = true;
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
            return RedirectToAction("SportP1");



        }

        public ActionResult ResultsS1()
        {
            var completed = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.quiz4).FirstOrDefault();
            TempData["Completed4"] = completed;
            TempData.Keep();
            return View();
        }

        public ActionResult SportP2()
        {
            var access = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.ProgressXP).SingleOrDefault();
            if (access == 65)
            {

                if (TempData["q2ID"] == null)
                {
                    TempData["q2ID"] = 1;
                }

                try
                {
                    SportQ2 q = null;
                    int q5ID = Convert.ToInt32(TempData["q5ID"].ToString());
                    q = db.SportQ2.Where(x => x.Qid == q5ID).SingleOrDefault();
                    TempData["q5ID"] = ++q.Qid;
                    TempData.Keep();

                    return View(q);
                }
                catch (Exception)
                {
                    TempData["q5ID"] = 1;
                    return RedirectToAction("ResultsS2");
                }
            }
            return RedirectToAction("SportStart");
        }

        [HttpPost]
        public ActionResult SportP2(string attemptedAns, SportQ2 q2)
        {
            string ans = attemptedAns.ToLower();
            ans = ans.RemoveDiacritics();
            Debug.WriteLine("accents removed? " + ans);
            
            var db_ans = db.SportQ2.Where(i => i.Qid == q2.Qid - 1).Select(i => i.ans1).FirstOrDefault();
            var db_k1 = db.SportQ2.Where(i => i.Qid == q2.Qid - 1).Select(i => i.keyword1).FirstOrDefault();
            var db_k2 = db.SportQ2.Where(i => i.Qid == q2.Qid - 1).Select(i => i.keyword2).FirstOrDefault();

            string check1 = db_ans.ToLower();
            check1 = check1.RemoveDiacritics();
            Debug.WriteLine("Given answer: " + check1);

            Debug.WriteLine(TempData["Score5"]);
            Debug.WriteLine("Saved option 1 on the database: " + db_ans);

            TempData.Keep();


            string x = ans;
            int n = x.Length;

            string t = db_ans;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == x[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            var comp = d[n, m];
            Debug.WriteLine("Comparison: " + comp);

            var similarity = (1.0 - ((double)comp / (double)Math.Max(x.Length, t.Length)));
            Debug.WriteLine("Similarity: " + similarity);

            //contains one of the keywords
            if (ans.Contains(db_k1) || ans.Contains(db_k2))
            {

                //similarity greater than .7 (70%)
                if (similarity > 0.7)
                {
                    TempData["Score5"] = Convert.ToInt32(TempData["Score5"]) + 1;
                }

            }
            else
            {


                var wrong = q2.Qid - 1;
                TempData["Wrong Answers5:"] = Convert.ToString(TempData["Wrong Answers:5"]) + "Fuair tú ceist " + wrong.ToString() + " mícheart." + "<br />";

            }

            if (Convert.ToInt32(TempData["Score5"]) == 4)
            {
                if (ModelState.IsValid)
                {
                    Debug.WriteLine("Model Valid");
                    TempData.Keep();
                    Debug.WriteLine(TempData["Score5"]);
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
                                if (result.quiz5 != true)
                                {
                                    result.ProgressXP = result.ProgressXP + 20;
                                    result.quiz5 = true;
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

            return RedirectToAction("SportP2");
        }

        public ActionResult ResultsS2()
        {
            var completed = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.quiz5).FirstOrDefault();
            TempData["Completed5"] = completed;
            TempData.Keep();
            return View();
        }

        public ActionResult SportP3()
        {
            var access = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.ProgressXP).SingleOrDefault();
            if (access == 85)
            {

                if (TempData["q6ID"] == null)
                {
                    TempData["q6ID"] = 1;
                }



                try
                {
                    Debug.WriteLine("Inside DP3 try");
                    SportQ3 q = null;
                    int q6ID = Convert.ToInt32(TempData["q6ID"]);
                    Debug.WriteLine("q ID: " + q6ID);
                    q = db.SportQ3.Where(x => x.Qid == q6ID).SingleOrDefault();
                    Debug.WriteLine("Q: " + q);
                    TempData["q6ID"] = ++q.Qid;
                    TempData.Keep();
                    Debug.WriteLine("before: " + q.correctAns);
                    return View(q);
                }
                catch (Exception)
                {
                    TempData["q6ID"] = 1;
                    Debug.WriteLine("Inside DP3 catch");
                    TempData.Keep();
                    return RedirectToAction("ResultsS3");
                }
            }
            return RedirectToAction("SportStart");
        }

        [HttpPost]
        public ActionResult SportP3(SportQ3 q)
        {
           // Debug.WriteLine("After: " + q.correctAns);
            String attemptedAns = Request.Form["Model.Qid"];

            Debug.WriteLine("ans: " + attemptedAns);
           
            Debug.WriteLine("ID: " + q.Qid);

            var x = db.SportQ3.Where(i => i.Qid == q.Qid-1).Select(i => i.correctAns).FirstOrDefault();
           
            Debug.WriteLine("correct answer from db query? "+ x);
           


            if (attemptedAns.Equals(x))
            {
                Debug.WriteLine("im adding");
                TempData["Score6"] = Convert.ToInt32(TempData["Score6"]) + 1;
                Debug.WriteLine("Entered " + TempData["Score6"]);
            }
            else
            {


                var wrong = q.Qid - 1;
                TempData["Wrong Answers6:"] = Convert.ToString(TempData["Wrong Answers:6"]) + "Fuair tú ceist " + wrong.ToString() + " mícheart." + "<br />";

            }
            Debug.WriteLine(TempData["Score6"]);
            if (x == null)
            {
                Debug.WriteLine("answer was passed in as null");
            }



            TempData.Keep();

            if (Convert.ToInt32(TempData["Score6"]) == 5)
            {
                if (ModelState.IsValid)
                {
                    Debug.WriteLine("Model Valid");
                    // TempData.Keep();
                    Debug.WriteLine(TempData["Score6"]);
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
                                if (result.quiz6 != true)
                                {
                                    result.ProgressXP = result.ProgressXP + 15;
                                    Debug.WriteLine("progress incremented:" + result.ProgressXP);
                                    result.quiz6 = true;
                                    db.SaveChanges();
                                    
                                }
                                
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
            return RedirectToAction("SportP3");
        }

        public ActionResult ResultsS3()
        {
            var completed = db.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.quiz6).FirstOrDefault();
            TempData["Completed6"] = completed;
            TempData.Keep();
            return View();
        }
    }


  


}
