using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using System.Web.Security;
using System.Diagnostics;
using System.Web.Helpers;
using System.Text;
using System.Security.Cryptography;

namespace FYP.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Member model)
        {
            using (var context = new msdb5455Entities())
            {
                bool exists = context.Users.Any(x => x.UserName == model.UserName);
                Debug.WriteLine("Exist?: " + exists);
                if (exists)
                {
                    RedirectToAction("Register");
                    ModelState.AddModelError("", "username in use");
                }
                else
                {
                    if (model.Password == model.ConfirmPassword)
                    {
                        FYP.Models.User user = new Models.User();

                        Guid userGuid = System.Guid.NewGuid();


                        user.UserName = model.UserName;
                        user.Password = HashSHA1(model.Password + userGuid.ToString());
                        user.UserGuid = userGuid;
                                               
                        context.Users.Add(user);
                        context.SaveChanges();
                        return RedirectToAction("Login");
                    }
                   
                }
                
            }

            return View();
        }

        public static string HashSHA1(string password)
        {
            var sha1 = SHA1.Create();
            var inputBytes = Encoding.ASCII.GetBytes(password);
            var hash = sha1.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        [HttpPost]
        public ActionResult Login(Models.Member model)
        {
            using (var context = new msdb5455Entities())
            {


                var dbUserGuid = context.Users.Where(x => x.UserName == model.UserName).Select(x => x.UserGuid).FirstOrDefault();
                Debug.WriteLine("guid " + dbUserGuid);
                string hashedPassword = HashSHA1(model.Password + dbUserGuid);
                Debug.WriteLine("hashed? " + hashedPassword);


                bool isValid = context.Users.Any(x => x.UserName == model.UserName && x.Password == hashedPassword);
                Debug.WriteLine("check:" + isValid);
                if (isValid == true)
                {
                    Debug.WriteLine("inside check");
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    TempData["ID"] = model.Id;
                    TempData.Keep();
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid email and/or password");
            }
            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}