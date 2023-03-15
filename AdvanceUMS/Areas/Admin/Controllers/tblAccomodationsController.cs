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
    public class tblAccomodationsController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();

        // GET: Admin/tblAccomodations
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
        #region Populate AccommodationList
        public ActionResult PopulateAccomodations(string Status)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
         
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
         
            var accommodationList = db.tblAccomodations.ToList();
            return PartialView("_List", accommodationList);
        }
        #endregion
        // GET: Admin/tblAccomodations/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccomodation tblAccomodation = db.tblAccomodations.Find(id);
            if (tblAccomodation == null)
            {
                return HttpNotFound();
            }
            return View(tblAccomodation);
        }

        // GET: Admin/tblAccomodations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblAccomodations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqAccommodationCode,strAccommodationName,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblAccomodation tblAccomodation)
        {
            if (ModelState.IsValid)
            {
                tblAccomodation.uqAccommodationCode = Guid.NewGuid();
                db.tblAccomodations.Add(tblAccomodation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblAccomodation);
        }

     


        

        // POST: Admin/tblAccomodations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblAccomodation tblAccomodation = db.tblAccomodations.Find(id);
            db.tblAccomodations.Remove(tblAccomodation);
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
        public JsonResult AddAccommodation(AccomodationViewModel objAccommodation)
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            //PopulateDropdown();
            try
            {
                tblAccomodation accomodation = new tblAccomodation();
                string desc = "";
                    if (objAccommodation.uqAccommodationCode == null)
                    {
                    accomodation.uqAccommodationCode = Guid.NewGuid();
                    accomodation.strAccommodationName = objAccommodation.strAccommodationName;
                    accomodation.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    accomodation.dtCreatedAt = dateTime;
                  
                        db.tblAccomodations.Add(accomodation);
                        desc = ("Insert Accomodation : " + accomodation.strAccommodationName);
                        Session["Add"] = "add";
                    }
                    else
                    {
                    accomodation.uqAccommodationCode = objAccommodation.uqAccommodationCode;
                    accomodation.strAccommodationName = objAccommodation.strAccommodationName;
                    accomodation.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    accomodation.uqCreatedBy = objAccommodation.uqCreatedBy;
                    accomodation.dtMoidifyAt = dateTime;
                    accomodation.dtCreatedAt = objAccommodation.dtCreatedAt;

                    db.Entry(accomodation).State = EntityState.Modified;
                    desc = ("Update Accomodation : " + accomodation.strAccommodationName);
                        Session["Update"] = "update";
                    }
                    db.SaveChanges();
                    dHelper.LogAction(desc);
                    res["status"] = "1";
                    //Response.Redirect("/admin/user");
                    res["url"] = "/Admin/tblAccomodations";
                
            }
            catch(Exception ex)
            {
               
                res["status"] = "0";
                throw ex;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public void PopulateDropdown()
        {
            var country = db.tblCountries.Where(s => s.IsDeleted == false).OrderBy(s => s.Name);
            ViewBag.CountryList = new SelectList(country, "ID", "Name");

            var role = db.tblRoles.Where(s => s.IsDeleted == false).OrderBy(s => s.Name);
            ViewBag.RoleList = new SelectList(role, "ID", "Name");

            List<SelectListItem> listGender = new List<SelectListItem>();
            listGender.Add(new SelectListItem { Text = "Male", Value = "Male", Selected = true });
            listGender.Add(new SelectListItem { Text = "FeMale", Value = "FeMale" });
            ViewBag.GenderList = new SelectList(listGender, "Value", "Text");
        }
        public ActionResult Edit(string ID)
        {
            //PopulateDropdown();
            //if (Session["edit"] != null)
            //{
            //    ViewBag.Message = "<div class=\"alert alert-danger fade in\">Edit is disabled in default user.</div>";
            //    Session.RemoveAll();
            //}
            Guid AccommodationID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var accommodationInfo = db.tblAccomodations.FirstOrDefault(s => s.uqAccommodationCode == AccommodationID);
            if (accommodationInfo != null)
            {
                AccomodationViewModel objAccommodation = new AccomodationViewModel();
                objAccommodation.uqAccommodationCode = accommodationInfo.uqAccommodationCode;
                objAccommodation.strAccommodationName = accommodationInfo.strAccommodationName;
                objAccommodation.dtCreatedAt = accommodationInfo.dtCreatedAt;
                objAccommodation.dtMoidifyAt = accommodationInfo.dtMoidifyAt;
                objAccommodation.uqCreatedBy = accommodationInfo.uqCreatedBy;
                objAccommodation.uqModifyBy = accommodationInfo.uqModifyBy;
                ViewBag.Edit = "1";
                return View(objAccommodation);
            }

            return View();
        }
        public JsonResult Delete(Guid ID)
        {
            var accommodationInfo = db.tblAccomodations.FirstOrDefault(s => s.uqAccommodationCode == ID);
            if (accommodationInfo != null)
            {


                tblAccomodation tblAccomodation = db.tblAccomodations.Find(accommodationInfo.uqAccommodationCode);
            db.tblAccomodations.Remove(tblAccomodation);
            db.SaveChanges();

                string Description = string.Format("Delete Accommodation [Accommodation: {0}]", accommodationInfo.strAccommodationName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete "+ accommodationInfo.strAccommodationName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblAccomodations/PopulateAccomodations";
                else
                    res["url"] = "/admin/tblAccomodations/PopulateAccomodations" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #endregion
    }
}
