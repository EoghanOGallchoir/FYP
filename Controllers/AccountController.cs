using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using System.Web.Security;
using System.Diagnostics;

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
        public ActionResult Register(User model)
        {
            using (var context = new msdb5455Entities())
            {
                bool exists = context.Users.Any(x => x.Id == model.Id);
                Debug.WriteLine("Exist?: " + exists);
                if (exists)
                {
                    RedirectToAction("Register");

                }
                else
                {
                    context.Users.Add(model);
                    context.SaveChanges();
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "username in use");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.Member model)
        {
            using (var context = new msdb5455Entities())
            {
                bool isValid = context.Users.Any(x => x.UserName == model.UserName && x.Password == model.Password);
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