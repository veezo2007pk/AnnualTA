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
    public class tblContractorsController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblContractors
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

        // GET: Admin/tblContractors/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblContractor tblContractor = db.tblContractors.Find(id);
            if (tblContractor == null)
            {
                return HttpNotFound();
            }
            return View(tblContractor);
        }

        // GET: Admin/tblContractors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblContractors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqContractorCode,strContractorName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblContractor tblContractor)
        {
            if (ModelState.IsValid)
            {
                tblContractor.uqContractorCode = Guid.NewGuid();
                db.tblContractors.Add(tblContractor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblContractor);
        }



        // POST: Admin/tblContractors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqContractorCode,strContractorName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblContractor tblContractor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblContractor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblContractor);
        }



        // POST: Admin/tblContractors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblContractor tblContractor = db.tblContractors.Find(id);
            db.tblContractors.Remove(tblContractor);
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


        public JsonResult AddContractor(ContractorViewModel objContractor)
        {
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                tblContractor Contractor = new tblContractor();
                string desc = "";
                if (objContractor.uqContractorCode == null)
                {
                    Contractor.uqContractorCode = Guid.NewGuid();
                    Contractor.strContractorName = objContractor.strContractorName;
                    Contractor.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    Contractor.dtCreatedAt = dateTime;

                    db.tblContractors.Add(Contractor);
                    desc = ("Insert Contractor : " + Contractor.strContractorName);
                    Session["Add"] = "add";
                }
                else
                {
                    Contractor.uqContractorCode = objContractor.uqContractorCode;
                    Contractor.strContractorName = objContractor.strContractorName;
                    Contractor.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    Contractor.uqCreatedBy = objContractor.uqCreatedBy;
                    Contractor.dtMoidifyAt = dateTime;
                    Contractor.dtCreatedAt = objContractor.dtCreatedAt;

                    db.Entry(Contractor).State = EntityState.Modified;
                    desc = ("Update Contractor : " + Contractor.strContractorName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblContractors";

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
            Guid ContractorID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var ContractorInfo = db.tblContractors.FirstOrDefault(s => s.uqContractorCode == ContractorID);
            if (ContractorInfo != null)
            {
                ContractorViewModel objContractor = new ContractorViewModel();
                objContractor.uqContractorCode = ContractorInfo.uqContractorCode;
                objContractor.strContractorName = ContractorInfo.strContractorName;
                objContractor.dtCreatedAt = ContractorInfo.dtCreatedAt;
                objContractor.dtMoidifyAt = ContractorInfo.dtMoidifyAt;
                objContractor.uqCreatedBy = ContractorInfo.uqCreatedBy;
                objContractor.uqModifyBy = ContractorInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objContractor);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var ContractorInfo = db.tblContractors.FirstOrDefault(s => s.uqContractorCode == ID);
            if (ContractorInfo != null)
            {


                tblContractor tblContractor = db.tblContractors.Find(ContractorInfo.uqContractorCode);
                db.tblContractors.Remove(tblContractor);
                db.SaveChanges();

                string Description = string.Format("Delete Contractor [Contractor: {0}]", ContractorInfo.strContractorName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + ContractorInfo.strContractorName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblContractors/PopulateContractors";
                else
                    res["url"] = "/admin/tblContractors/PopulateContractors" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #region Populate ContractorList
        public ActionResult PopulateContractors(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);

            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;

            var ContractorList = db.tblContractors.ToList();
            return PartialView("_List", ContractorList);
        }
        #endregion
        #endregion
    }
}
