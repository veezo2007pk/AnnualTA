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
    public class tblTradesController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblTrades
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

        // GET: Admin/tblTrades/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblTrade tblTrade = db.tblTrades.Find(id);
            if (tblTrade == null)
            {
                return HttpNotFound();
            }
            return View(tblTrade);
        }

        // GET: Admin/tblTrades/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblTrades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqTradeCode,strTradeName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblTrade tblTrade)
        {
            if (ModelState.IsValid)
            {
                tblTrade.uqTradeCode = Guid.NewGuid();
                db.tblTrades.Add(tblTrade);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblTrade);
        }



        // POST: Admin/tblTrades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqTradeCode,strTradeName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblTrade tblTrade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblTrade).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblTrade);
        }



        // POST: Admin/tblTrades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblTrade tblTrade = db.tblTrades.Find(id);
            db.tblTrades.Remove(tblTrade);
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


        public JsonResult AddTrade(TradeViewModel objTrade)
        {
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                tblTrade Trade = new tblTrade();
                string desc = "";
                if (objTrade.uqTradeCode == null)
                {
                    Trade.uqTradeCode = Guid.NewGuid();
                    Trade.strTradeName = objTrade.strTradeName;
                    Trade.intRate = objTrade.intRate;
                    Trade.intMealAllowance = objTrade.intMealAllowance;
                    Trade.intOvertimeRate = objTrade.intOvertimeRate;
                    Trade.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    Trade.dtCreatedAt = dateTime;

                    db.tblTrades.Add(Trade);
                    desc = ("Insert Trade : " + Trade.strTradeName);
                    Session["Add"] = "add";
                }
                else
                {
                    Trade.uqTradeCode = objTrade.uqTradeCode;
                    Trade.strTradeName = objTrade.strTradeName;
                    Trade.intRate = objTrade.intRate;
                    Trade.intMealAllowance = objTrade.intMealAllowance;
                    Trade.intOvertimeRate = objTrade.intOvertimeRate;
                    Trade.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    Trade.uqCreatedBy = objTrade.uqCreatedBy;
                    Trade.dtMoidifyAt = dateTime;
                    Trade.dtCreatedAt = objTrade.dtCreatedAt;

                    db.Entry(Trade).State = EntityState.Modified;
                    desc = ("Update Trade : " + Trade.strTradeName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblTrades";

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
            Guid TradeID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var TradeInfo = db.tblTrades.FirstOrDefault(s => s.uqTradeCode == TradeID);
            if (TradeInfo != null)
            {
                TradeViewModel objTrade = new TradeViewModel();
                objTrade.uqTradeCode = TradeInfo.uqTradeCode;
                objTrade.intRate = TradeInfo.intRate;
                objTrade.intMealAllowance = TradeInfo.intMealAllowance;
                objTrade.intOvertimeRate = TradeInfo.intOvertimeRate;
                objTrade.strTradeName = TradeInfo.strTradeName;
                objTrade.dtCreatedAt = TradeInfo.dtCreatedAt;
                objTrade.dtMoidifyAt = TradeInfo.dtMoidifyAt;
                objTrade.uqCreatedBy = TradeInfo.uqCreatedBy;
                objTrade.uqModifyBy = TradeInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objTrade);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var TradeInfo = db.tblTrades.FirstOrDefault(s => s.uqTradeCode == ID);
            if (TradeInfo != null)
            {


                tblTrade tblTrade = db.tblTrades.Find(TradeInfo.uqTradeCode);
                db.tblTrades.Remove(tblTrade);
                db.SaveChanges();

                string Description = string.Format("Delete Trade [Trade: {0}]", TradeInfo.strTradeName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + TradeInfo.strTradeName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblTrades/PopulateTrades";
                else
                    res["url"] = "/admin/tblTrades/PopulateTrades" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #region Populate TradeList
        public ActionResult PopulateTrades(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);

            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;

            var TradeList = db.tblTrades.ToList();
            return PartialView("_List", TradeList);
        }
        #endregion
        #endregion
    }
}
