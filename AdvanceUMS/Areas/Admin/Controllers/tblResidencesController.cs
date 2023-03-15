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
    public class tblResidencesController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblResidences
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

        // GET: Admin/tblResidences/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResidence tblResidence = db.tblResidences.Find(id);
            if (tblResidence == null)
            {
                return HttpNotFound();
            }
            return View(tblResidence);
        }

        // GET: Admin/tblResidences/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblResidences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqResidenceCode,strResidenceName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblResidence tblResidence)
        {
            if (ModelState.IsValid)
            {
                tblResidence.uqResidenceCode = Guid.NewGuid();
                db.tblResidences.Add(tblResidence);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblResidence);
        }



        // POST: Admin/tblResidences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqResidenceCode,strResidenceName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblResidence tblResidence)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblResidence).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblResidence);
        }



        // POST: Admin/tblResidences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblResidence tblResidence = db.tblResidences.Find(id);
            db.tblResidences.Remove(tblResidence);
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


        public JsonResult AddResidence(ResidenceViewModel objResidence)
        {
            try
            {

                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                tblResidence Residence = new tblResidence();
                string desc = "";
                if (objResidence.uqResidenceCode == null)
                {
                    Residence.uqResidenceCode = Guid.NewGuid();
                    Residence.strResidenceName = objResidence.strResidenceName;
                    Residence.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    Residence.dtCreatedAt = dateTime;

                    db.tblResidences.Add(Residence);
                    desc = ("Insert Residence : " + Residence.strResidenceName);
                    Session["Add"] = "add";
                }
                else
                {
                    Residence.uqResidenceCode = objResidence.uqResidenceCode;
                    Residence.strResidenceName = objResidence.strResidenceName;
                    Residence.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    Residence.uqCreatedBy = objResidence.uqCreatedBy;
                    Residence.dtMoidifyAt = dateTime;
                    Residence.dtCreatedAt = objResidence.dtCreatedAt;

                    db.Entry(Residence).State = EntityState.Modified;
                    desc = ("Update Residence : " + Residence.strResidenceName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblResidences";

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
            Guid ResidenceID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var ResidenceInfo = db.tblResidences.FirstOrDefault(s => s.uqResidenceCode == ResidenceID);
            if (ResidenceInfo != null)
            {
                ResidenceViewModel objResidence = new ResidenceViewModel();
                objResidence.uqResidenceCode = ResidenceInfo.uqResidenceCode;
                objResidence.strResidenceName = ResidenceInfo.strResidenceName;
                objResidence.dtCreatedAt = ResidenceInfo.dtCreatedAt;
                objResidence.dtMoidifyAt = ResidenceInfo.dtMoidifyAt;
                objResidence.uqCreatedBy = ResidenceInfo.uqCreatedBy;
                objResidence.uqModifyBy = ResidenceInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objResidence);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var ResidenceInfo = db.tblResidences.FirstOrDefault(s => s.uqResidenceCode == ID);
            if (ResidenceInfo != null)
            {


                tblResidence tblResidence = db.tblResidences.Find(ResidenceInfo.uqResidenceCode);
                db.tblResidences.Remove(tblResidence);
                db.SaveChanges();

                string Description = string.Format("Delete Residence [Residence: {0}]", ResidenceInfo.strResidenceName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + ResidenceInfo.strResidenceName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblResidences/PopulateResidences";
                else
                    res["url"] = "/admin/tblResidences/PopulateResidences" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #region Populate ResidenceList
        public ActionResult PopulateResidences(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);

            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;

            var ResidenceList = db.tblResidences.ToList();
            return PartialView("_List", ResidenceList);
        }
        #endregion
        #endregion
    }
}
