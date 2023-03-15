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
    public class tblDepartmentsController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblDepartments
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

        // GET: Admin/tblDepartments/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDepartment tblDepartment = db.tblDepartments.Find(id);
            if (tblDepartment == null)
            {
                return HttpNotFound();
            }
            return View(tblDepartment);
        }

        // GET: Admin/tblDepartments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblDepartments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqDepartmentCode,strDepartmentName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblDepartment tblDepartment)
        {
            if (ModelState.IsValid)
            {
                tblDepartment.uqDepartmentCode = Guid.NewGuid();
                db.tblDepartments.Add(tblDepartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblDepartment);
        }



        // POST: Admin/tblDepartments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqDepartmentCode,strDepartmentName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblDepartment tblDepartment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblDepartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblDepartment);
        }



        // POST: Admin/tblDepartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblDepartment tblDepartment = db.tblDepartments.Find(id);
            db.tblDepartments.Remove(tblDepartment);
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


        public JsonResult AddDepartment(DepartmentViewModel objDepartment)
        {
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                tblDepartment Department = new tblDepartment();
                string desc = "";
                if (objDepartment.uqDepartmentCode == null)
                {
                    Department.uqDepartmentCode = Guid.NewGuid();
                    Department.strDepartmentName = objDepartment.strDepartmentName;
                    Department.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    Department.dtCreatedAt = dateTime;

                    db.tblDepartments.Add(Department);
                    desc = ("Insert Department : " + Department.strDepartmentName);
                    Session["Add"] = "add";
                }
                else
                {
                    Department.uqDepartmentCode = objDepartment.uqDepartmentCode;
                    Department.strDepartmentName = objDepartment.strDepartmentName;
                    Department.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    Department.uqCreatedBy = objDepartment.uqCreatedBy;
                    Department.dtMoidifyAt = dateTime;
                    Department.dtCreatedAt = objDepartment.dtCreatedAt;

                    db.Entry(Department).State = EntityState.Modified;
                    desc = ("Update Department : " + Department.strDepartmentName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblDepartments";

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
            Guid DepartmentID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var DepartmentInfo = db.tblDepartments.FirstOrDefault(s => s.uqDepartmentCode == DepartmentID);
            if (DepartmentInfo != null)
            {
                DepartmentViewModel objDepartment = new DepartmentViewModel();
                objDepartment.uqDepartmentCode = DepartmentInfo.uqDepartmentCode;
                objDepartment.strDepartmentName = DepartmentInfo.strDepartmentName;
                objDepartment.dtCreatedAt = DepartmentInfo.dtCreatedAt;
                objDepartment.dtMoidifyAt = DepartmentInfo.dtMoidifyAt;
                objDepartment.uqCreatedBy = DepartmentInfo.uqCreatedBy;
                objDepartment.uqModifyBy = DepartmentInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objDepartment);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var DepartmentInfo = db.tblDepartments.FirstOrDefault(s => s.uqDepartmentCode == ID);
            if (DepartmentInfo != null)
            {


                tblDepartment tblDepartment = db.tblDepartments.Find(DepartmentInfo.uqDepartmentCode);
                db.tblDepartments.Remove(tblDepartment);
                db.SaveChanges();

                string Description = string.Format("Delete Department [Department: {0}]", DepartmentInfo.strDepartmentName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + DepartmentInfo.strDepartmentName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblDepartments/PopulateDepartments";
                else
                    res["url"] = "/admin/tblDepartments/PopulateDepartments" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #region Populate DepartmentList
        public ActionResult PopulateDepartments(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);

            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;

            var DepartmentList = db.tblDepartments.ToList();
            return PartialView("_List", DepartmentList);
        }
        #endregion
        #endregion
    }
}
