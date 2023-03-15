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
    public class tblSkillsController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblSkills
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

        // GET: Admin/tblSkills/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSkill tblSkill = db.tblSkills.Find(id);
            if (tblSkill == null)
            {
                return HttpNotFound();
            }
            return View(tblSkill);
        }

        // GET: Admin/tblSkills/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblSkills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqSkillCode,strSkillName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblSkill tblSkill)
        {
            if (ModelState.IsValid)
            {
                tblSkill.uqSkillCode = Guid.NewGuid();
                db.tblSkills.Add(tblSkill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblSkill);
        }

       

        // POST: Admin/tblSkills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqSkillCode,strSkillName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblSkill tblSkill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblSkill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblSkill);
        }

       

        // POST: Admin/tblSkills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblSkill tblSkill = db.tblSkills.Find(id);
            db.tblSkills.Remove(tblSkill);
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


        public JsonResult AddSkill(SkillViewModel objSkill)
        {
            try
            {

                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                tblSkill skill= new tblSkill();
                string desc = "";
                if (objSkill.uqSkillCode == null)
                {
                    skill.uqSkillCode = Guid.NewGuid();
                    skill.strSkillName = objSkill.strSkillName;
                    skill.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    skill.dtCreatedAt = dateTime;

                    db.tblSkills.Add(skill);
                    desc = ("Insert Skill : " + skill.strSkillName);
                    Session["Add"] = "add";
                }
                else
                {
                    skill.uqSkillCode = objSkill.uqSkillCode;
                    skill.strSkillName = objSkill.strSkillName;
                    skill.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    skill.uqCreatedBy = objSkill.uqCreatedBy;
                    skill.dtMoidifyAt = dateTime;
                    skill.dtCreatedAt = objSkill.dtCreatedAt;

                    db.Entry(skill).State = EntityState.Modified;
                    desc = ("Update Skill : " + skill.strSkillName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblSkills";

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
            Guid SkillID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var skillInfo = db.tblSkills.FirstOrDefault(s => s.uqSkillCode == SkillID);
            if (skillInfo != null)
            {
                SkillViewModel objSkill = new SkillViewModel();
                objSkill.uqSkillCode = skillInfo.uqSkillCode;
                objSkill.strSkillName = skillInfo.strSkillName;
                objSkill.dtCreatedAt = skillInfo.dtCreatedAt;
                objSkill.dtMoidifyAt = skillInfo.dtMoidifyAt;
                objSkill.uqCreatedBy = skillInfo.uqCreatedBy;
                objSkill.uqModifyBy = skillInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objSkill);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var skillInfo = db.tblSkills.FirstOrDefault(s => s.uqSkillCode == ID);
            if (skillInfo != null)
            {


                tblSkill tblSkill = db.tblSkills.Find(skillInfo.uqSkillCode);
                db.tblSkills.Remove(tblSkill);
                db.SaveChanges();

                string Description = string.Format("Delete Skill [Skill: {0}]", skillInfo.strSkillName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + skillInfo.strSkillName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblSkills/PopulateSkills";
                else
                    res["url"] = "/admin/tblSkills/PopulateSkills" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #region Populate SkillList
        public ActionResult PopulateSkills(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);

            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;

            var skillList = db.tblSkills.ToList();
            return PartialView("_List", skillList);
        }
        #endregion
        #endregion
    }
}
