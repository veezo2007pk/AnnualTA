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
    public class tblShiftsController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblShifts
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

        // GET: Admin/tblShifts/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblShift tblShift = db.tblShifts.Find(id);
            if (tblShift == null)
            {
                return HttpNotFound();
            }
            return View(tblShift);
        }

        // GET: Admin/tblShifts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblShifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqShiftCode,strShiftName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblShift tblShift)
        {
            if (ModelState.IsValid)
            {
                tblShift.uqShiftId = Guid.NewGuid();
                db.tblShifts.Add(tblShift);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblShift);
        }

       

        // POST: Admin/tblShifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqShiftCode,strShiftName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblShift tblShift)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblShift).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblShift);
        }

       

        // POST: Admin/tblShifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblShift tblShift = db.tblShifts.Find(id);
            db.tblShifts.Remove(tblShift);
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


        public JsonResult AddShift(ShiftViewModel objShift)
        {
            try
            {

                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                tblShift Shift= new tblShift();
                string desc = "";
                if (objShift.uqShiftId == null)
                {
                    Shift.uqShiftId = Guid.NewGuid();
                    Shift.tmFrom = objShift.tmFrom;
                    Shift.tmTo = objShift.tmTo;
                    Shift.strShiftName = objShift.strShiftName;
                    Shift.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    Shift.dtCreatedAt = dateTime;

                    db.tblShifts.Add(Shift);
                    desc = ("Insert Shift : " + Shift.strShiftName);
                    Session["Add"] = "add";
                }
                else
                {
                    Shift.uqShiftId = objShift.uqShiftId;
                    Shift.strShiftName = objShift.strShiftName;
                    Shift.tmFrom = objShift.tmFrom;
                    Shift.tmTo = objShift.tmTo;
                    Shift.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    Shift.uqCreatedBy = objShift.uqCreatedBy;
                    Shift.dtMoidifyAt = dateTime;
                    Shift.dtCreatedAt = objShift.dtCreatedAt;

                    db.Entry(Shift).State = EntityState.Modified;
                    desc = ("Update Shift : " + Shift.strShiftName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblShifts";

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
            Guid ShiftID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var ShiftInfo = db.tblShifts.FirstOrDefault(s => s.uqShiftId == ShiftID);
            if (ShiftInfo != null)
            {
                ShiftViewModel objShift = new ShiftViewModel();
                objShift.uqShiftId = ShiftInfo.uqShiftId;
                objShift.tmFrom = ShiftInfo.tmFrom;
                objShift.tmTo = ShiftInfo.tmTo;
                objShift.strShiftName = ShiftInfo.strShiftName;
                objShift.dtCreatedAt = ShiftInfo.dtCreatedAt;
                objShift.dtMoidifyAt = ShiftInfo.dtMoidifyAt;
                objShift.uqCreatedBy = ShiftInfo.uqCreatedBy;
                objShift.uqModifyBy = ShiftInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objShift);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var ShiftInfo = db.tblShifts.FirstOrDefault(s => s.uqShiftId == ID);
            if (ShiftInfo != null)
            {


                tblShift tblShift = db.tblShifts.Find(ShiftInfo.uqShiftId);
                db.tblShifts.Remove(tblShift);
                db.SaveChanges();

                string Description = string.Format("Delete Shift [Shift: {0}]", ShiftInfo.strShiftName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + ShiftInfo.strShiftName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblShifts/PopulateShifts";
                else
                    res["url"] = "/admin/tblShifts/PopulateShifts" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #region Populate ShiftList
        public ActionResult PopulateShifts(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);

            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;

            var ShiftList = db.tblShifts.ToList();
            return PartialView("_List", ShiftList);
        }
        #endregion
        #endregion
    }
}
