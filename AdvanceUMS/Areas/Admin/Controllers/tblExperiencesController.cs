using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Helper;
using AdvanceUMS.Models;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class tblExperiencesController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblExperiences
        public ActionResult Index()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsView == false)
                return Redirect("/admin/notaccess");

            ViewBag.IsAdd = control.IsAdd;
            if (Session["Add"] != null)
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully insert user.</div>";
            else if (Session["Update"] != null)
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully update user.</div>";
            Session.RemoveAll();
            return View();
        }

        // GET: Admin/tblExperiences/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblExperience tblExperience = db.tblExperiences.Find(id);
            if (tblExperience == null)
            {
                return HttpNotFound();
            }
            return View(tblExperience);
        }

        // GET: Admin/tblExperiences/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblExperiences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqExperienceCode,strExperienceName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblExperience tblExperience)
        {
            if (ModelState.IsValid)
            {
                tblExperience.uqExperienceCode = Guid.NewGuid();
                db.tblExperiences.Add(tblExperience);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblExperience);
        }



        // POST: Admin/tblExperiences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqExperienceCode,strExperienceName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblExperience tblExperience)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblExperience).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblExperience);
        }



        // POST: Admin/tblExperiences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblExperience tblExperience = db.tblExperiences.Find(id);
            db.tblExperiences.Remove(tblExperience);
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


        public JsonResult AddExperience(ExperienceViewModel objExperience)
        {
            try
            {
                tblExperience Experience = new tblExperience();
                string desc = "";
                if (objExperience.uqExperienceCode == null)
                {
                    Experience.uqExperienceCode = Guid.NewGuid();
                    Experience.strExperienceName = objExperience.strExperienceName;
                    Experience.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    Experience.dtCreatedAt = DateTime.Now;

                    db.tblExperiences.Add(Experience);
                    desc = ("Insert Experience : " + Experience.strExperienceName);
                    Session["Add"] = "add";
                }
                else
                {
                    Experience.uqExperienceCode = objExperience.uqExperienceCode;
                    Experience.strExperienceName = objExperience.strExperienceName;
                    Experience.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    Experience.uqCreatedBy = objExperience.uqCreatedBy;
                    Experience.dtMoidifyAt = DateTime.Now;
                    Experience.dtCreatedAt = objExperience.dtCreatedAt;

                    db.Entry(Experience).State = EntityState.Modified;
                    desc = ("Update Experience : " + Experience.strExperienceName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblExperiences";

            }
            catch (Exception ex)
            {

                res["status"] = "0";
                throw ex;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(string ID)
        {
            //PopulateDropdown();
            //if (Session["edit"] != null)
            //{
            //    ViewBag.Message = "<div class=\"alert alert-danger fade in\">Edit is disabled in default user.</div>";
            //    Session.RemoveAll();
            //}
            Guid ExperienceID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var ExperienceInfo = db.tblExperiences.FirstOrDefault(s => s.uqExperienceCode == ExperienceID);
            if (ExperienceInfo != null)
            {
                ExperienceViewModel objExperience = new ExperienceViewModel();
                objExperience.uqExperienceCode = ExperienceInfo.uqExperienceCode;
                objExperience.strExperienceName = ExperienceInfo.strExperienceName;
                objExperience.dtCreatedAt = ExperienceInfo.dtCreatedAt;
                objExperience.dtMoidifyAt = ExperienceInfo.dtMoidifyAt;
                objExperience.uqCreatedBy = ExperienceInfo.uqCreatedBy;
                objExperience.uqModifyBy = ExperienceInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objExperience);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var ExperienceInfo = db.tblExperiences.FirstOrDefault(s => s.uqExperienceCode == ID);
            if (ExperienceInfo != null)
            {


                tblExperience tblExperience = db.tblExperiences.Find(ExperienceInfo.uqExperienceCode);
                db.tblExperiences.Remove(tblExperience);
                db.SaveChanges();

                string Description = string.Format("Delete Experience [Experience: {0}]", ExperienceInfo.strExperienceName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + ExperienceInfo.strExperienceName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblExperiences/PopulateExperiences";
                else
                    res["url"] = "/admin/tblExperiences/PopulateExperiences" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #region Populate ExperienceList
        public ActionResult PopulateExperiences(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);

            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;

            var ExperienceList = db.tblExperiences.ToList();
            return PartialView("_List", ExperienceList);
        }
        #endregion
        #endregion
    }
}
