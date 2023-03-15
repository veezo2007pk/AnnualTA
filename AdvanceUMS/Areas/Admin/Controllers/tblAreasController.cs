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
    public class tblAreasController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblAreas
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

        // GET: Admin/tblAreas/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblArea tblArea = db.tblAreas.Find(id);
            if (tblArea == null)
            {
                return HttpNotFound();
            }
            return View(tblArea);
        }

        // GET: Admin/tblAreas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblAreas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqAreaCode,strAreaName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblArea tblArea)
        {
            if (ModelState.IsValid)
            {
                tblArea.uqAreaCode = Guid.NewGuid();
                db.tblAreas.Add(tblArea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblArea);
        }



        // POST: Admin/tblAreas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqAreaCode,strAreaName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblArea tblArea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblArea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblArea);
        }



        // POST: Admin/tblAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblArea tblArea = db.tblAreas.Find(id);
            db.tblAreas.Remove(tblArea);
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


        public JsonResult AddArea(AreaViewModel objArea)
        {
            try
            {
                tblArea Area = new tblArea();
                string desc = "";
                if (objArea.uqAreaCode == null)
                {
                    Area.uqAreaCode = Guid.NewGuid();
                    Area.strAreaName = objArea.strAreaName;
                    Area.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    Area.dtCreatedAt = DateTime.Now;

                    db.tblAreas.Add(Area);
                    desc = ("Insert Area : " + Area.strAreaName);
                    Session["Add"] = "add";
                }
                else
                {
                    Area.uqAreaCode = objArea.uqAreaCode;
                    Area.strAreaName = objArea.strAreaName;
                    Area.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    Area.uqCreatedBy = objArea.uqCreatedBy;
                    Area.dtMoidifyAt = DateTime.Now;
                    Area.dtCreatedAt = objArea.dtCreatedAt;

                    db.Entry(Area).State = EntityState.Modified;
                    desc = ("Update Area : " + Area.strAreaName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblAreas";

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
            Guid AreaID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var AreaInfo = db.tblAreas.FirstOrDefault(s => s.uqAreaCode == AreaID);
            if (AreaInfo != null)
            {
                AreaViewModel objArea = new AreaViewModel();
                objArea.uqAreaCode = AreaInfo.uqAreaCode;
                objArea.strAreaName = AreaInfo.strAreaName;
                objArea.dtCreatedAt = AreaInfo.dtCreatedAt;
                objArea.dtMoidifyAt = AreaInfo.dtMoidifyAt;
                objArea.uqCreatedBy = AreaInfo.uqCreatedBy;
                objArea.uqModifyBy = AreaInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objArea);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var AreaInfo = db.tblAreas.FirstOrDefault(s => s.uqAreaCode == ID);
            if (AreaInfo != null)
            {


                tblArea tblArea = db.tblAreas.Find(AreaInfo.uqAreaCode);
                db.tblAreas.Remove(tblArea);
                db.SaveChanges();

                string Description = string.Format("Delete Area [Area: {0}]", AreaInfo.strAreaName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + AreaInfo.strAreaName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblAreas/PopulateAreas";
                else
                    res["url"] = "/admin/tblAreas/PopulateAreas" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #region Populate AreaList
        public ActionResult PopulateAreas(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);

            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;

            var AreaList = db.tblAreas.ToList();
            return PartialView("_List", AreaList);
        }
        #endregion
        #endregion
    }
}
