using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Helper;
using AdvanceUMS.Models;
using System.Web.Security;

namespace AdvanceUMS.Controllers
{
    public class AccountController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        MailHelper dMailHelper=new MailHelper();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            var setting = db.tblSettings.FirstOrDefault();
            if (setting != null)
            {
                HttpCookie cookie1 = new HttpCookie("User");
                cookie1.Values.Add("LogoPath", setting.LogoPath);
                cookie1.Values.Add("FeviconPath", setting.FeviconPath);
                ViewBag.SiteLogo = setting.LogoPath;
                ViewBag.Name = setting.Name;
                Response.Cookies.Add(cookie1);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel objLogin)
        {
            HttpCookie cookie1 = new HttpCookie("User");
            //var password = dHelper.Encrypt(objLogin.Password);
            var password = objLogin.Password;
            var userLogin = db.tblUsers.FirstOrDefault(s =>
                s.Email == objLogin.Email || s.UserName == objLogin.Email && s.Password == password);
            if (userLogin != null)
            {
                cookie1.Values.Add("UserID", userLogin.ID.ToString());
                cookie1.Values.Add("SupervisorID", userLogin.uqSupervisorCode.ToString());
                cookie1.Values.Add("EngineerID", userLogin.uqEngineerCode.ToString());
                cookie1.Values.Add("ContractorID", userLogin.uqContractorCode.ToString());
                cookie1.Values.Add("uqHODCode", userLogin.uqHODCode.ToString());

                cookie1.Expires = DateTime.Now.AddHours(3);
                Response.Cookies.Add(cookie1);
                userLogin.LastLoginDate = DateTime.Now;
               // cookie1.Value['UserID'];
                dHelper.LogAction(userLogin.Email + " successfully login.");
                return Redirect("~/admin/blank");
            }
            else
            {
                ViewBag.Message = "<div class=\"alert alert-danger fade in\">Invalid Username and password.</div>.";
                return View();
            }
        }

        public ActionResult ForgotPassword()
        {
            var setting = db.tblSettings.FirstOrDefault();
            if (setting != null)
            {
                ViewBag.SiteLogo = setting.LogoPath;
                ViewBag.Name = setting.Name;
            }
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(LoginViewModel objLogin)
        {
            var userLogin = db.tblUsers.FirstOrDefault(s => s.Email == objLogin.Email || s.UserName == objLogin.Email);
            if (userLogin != null)
            {
                var returnValue = dMailHelper.SendMail(userLogin.Email, "Subject", "Body");
                if (returnValue == "Successful")
                    ViewBag.Message =
                        "<div class=\"alert alert-success fade in\">Password sent to your email address.</div>.";
                else
                    ViewBag.Message =
                        "<div class=\"alert alert-danger fade in\">" + returnValue + "</div>.";
                return View();
            }
            else
            {
                ViewBag.Message = "<div class=\"alert alert-danger fade in\">Invalid Username and password.</div>.";
                return View();
            }
        }

        //Logout user 
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            Response.Cookies["User"].Expires = DateTime.MinValue;
            Response.Cookies.Clear();
            HttpCookie cookie1 = new HttpCookie("User");
            cookie1.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie1);
            return RedirectToAction("Login", "Account");
        }
    }
}