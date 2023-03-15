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
    public class tblJoiningStatsController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblJoiningStats
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

        // GET: Admin/tblJoiningStats/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblJoiningStat tblJoiningStat = db.tblJoiningStats.Find(id);
            if (tblJoiningStat == null)
            {
                return HttpNotFound();
            }
            return View(tblJoiningStat);
        }

        // GET: Admin/tblJoiningStats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblJoiningStats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqJoiningStatusCode,strJoiningStatusCode,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblJoiningStat tblJoiningStat)
        {
            if (ModelState.IsValid)
            {
                tblJoiningStat.uqJoiningStatusCode = Guid.NewGuid();
                db.tblJoiningStats.Add(tblJoiningStat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblJoiningStat);
        }



        // POST: Admin/tblJoiningStats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqJoiningStatusCode,strJoiningStatusCode,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblJoiningStat tblJoiningStat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblJoiningStat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblJoiningStat);
        }



        // POST: Admin/tblJoiningStats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblJoiningStat tblJoiningStat = db.tblJoiningStats.Find(id);
            db.tblJoiningStats.Remove(tblJoiningStat);
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


        public JsonResult AddJoiningStat(JoiningStatViewModel objJoiningStat)
        {
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                tblJoiningStat JoiningStat = new tblJoiningStat();
                string desc = "";
                if (objJoiningStat.uqJoiningStatusCode == null)
                {
                    JoiningStat.uqJoiningStatusCode = Guid.NewGuid();
                    JoiningStat.strJoiningStatusCode = objJoiningStat.strJoiningStatusCode;
                    JoiningStat.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    JoiningStat.dtCreatedAt = dateTime;

                    db.tblJoiningStats.Add(JoiningStat);
                    desc = ("Insert JoiningStat : " + JoiningStat.strJoiningStatusCode);
                    Session["Add"] = "add";
                }
                else
                {
                    JoiningStat.uqJoiningStatusCode = objJoiningStat.uqJoiningStatusCode;
                    JoiningStat.strJoiningStatusCode = objJoiningStat.strJoiningStatusCode;
                    JoiningStat.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    JoiningStat.uqCreatedBy = objJoiningStat.uqCreatedBy;
                    JoiningStat.dtMoidifyAt = dateTime;
                    JoiningStat.dtCreatedAt = objJoiningStat.dtCreatedAt;

                    db.Entry(JoiningStat).State = EntityState.Modified;
                    desc = ("Update JoiningStat : " + JoiningStat.strJoiningStatusCode);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblJoiningStats";

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
            Guid JoiningStatID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var JoiningStatInfo = db.tblJoiningStats.FirstOrDefault(s => s.uqJoiningStatusCode == JoiningStatID);
            if (JoiningStatInfo != null)
            {
                JoiningStatViewModel objJoiningStat = new JoiningStatViewModel();
                objJoiningStat.uqJoiningStatusCode = JoiningStatInfo.uqJoiningStatusCode;
                objJoiningStat.strJoiningStatusCode = JoiningStatInfo.strJoiningStatusCode;
                objJoiningStat.dtCreatedAt = JoiningStatInfo.dtCreatedAt;
                objJoiningStat.dtMoidifyAt = JoiningStatInfo.dtMoidifyAt;
                objJoiningStat.uqCreatedBy = JoiningStatInfo.uqCreatedBy;
                objJoiningStat.uqModifyBy = JoiningStatInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objJoiningStat);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var JoiningStatInfo = db.tblJoiningStats.FirstOrDefault(s => s.uqJoiningStatusCode == ID);
            if (JoiningStatInfo != null)
            {


                tblJoiningStat tblJoiningStat = db.tblJoiningStats.Find(JoiningStatInfo.uqJoiningStatusCode);
                db.tblJoiningStats.Remove(tblJoiningStat);
                db.SaveChanges();

                string Description = string.Format("Delete JoiningStat [JoiningStat: {0}]", JoiningStatInfo.strJoiningStatusCode);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + JoiningStatInfo.strJoiningStatusCode + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblJoiningStats/PopulateJoiningStats";
                else
                    res["url"] = "/admin/tblJoiningStats/PopulateJoiningStats" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #region Populate JoiningStatList
        public ActionResult PopulateJoiningStats(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);

            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;

            var JoiningStatList = db.tblJoiningStats.ToList();
            return PartialView("_List", JoiningStatList);
        }
        #endregion
        #endregion
    }
}
