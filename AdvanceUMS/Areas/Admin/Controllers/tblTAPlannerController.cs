using AdvanceUMS.Helper;
using AdvanceUMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class tblTAPlannerController : Controller
    {
        GeneralHelper dHelper = new GeneralHelper();
        Entities db = new Entities();
        public void PopulateDropdown()
        {




            var trade = db.tblTrades.OrderBy(s => s.strTradeName);
            ViewBag.tradeList = new SelectList(trade, "uqTradeCode", "strTradeName");


            var shift = db.tblShifts.OrderBy(s => s.strShiftName);
            ViewBag.shiftList = new SelectList(shift, "uqShiftId", "strShiftName");

            var department = db.tblDepartments.OrderBy(s => s.strDepartmentName);
            ViewBag.departmentList = new SelectList(department, "uqDepartmentCode", "strDepartmentName");

            var area = db.tblAreas.OrderBy(s => s.strAreaName);
            ViewBag.areaList = new SelectList(area, "uqAreaCode", "strAreaName");

            var year = db.tblYears.OrderBy(s => s.intYearCode);
            ViewBag.yearList = new SelectList(year, "intYearCode", "strYearName");

        }
        [HttpGet]
        public ActionResult Index(string year)
        {
            PopulateDropdown();
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            //if (control.IsLogin == false)
            //    return Redirect("/account/login");
            //if (control.IsView == false)
            //    return Redirect("/admin/notaccess");

            ViewBag.IsAdd = control.IsAdd;

            ViewBag.year = year;
            //string Url = Request.RawUrl.ToString();
            //var control = dHelper.GetUserPermission(Url);
            //if (control.IsLogin == false)
            //    return Redirect("/account/login");
            //if (control.IsView == false)
            //    return Redirect("/admin/notaccess");

            //ViewBag.IsAdd = control.IsAdd;
            //if (Session["Add"] != null)
            //    ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully insert user.</div>";
            //else if (Session["Update"] != null)
            //    ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully update user.</div>";
            //Session.RemoveAll();


            return View();
        }
        // GET: Home
        public ActionResult Index()
        {
            PopulateDropdown();
            return View();
        }

        public JsonResult GetEvents()
        {
            string year = dHelper.year().ToString();
            using (Entities dc = new Entities())
            {
                var events = dc.tblTAPlannings.Where(x=>x.Year== year).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SaveEvent(tblTAPlanning e)
        {
            var status = false;
            using (Entities dc = new Entities())
            {
               
                    //Update the event
                    var v = dc.tblTAPlannings.Where(a => a.intTAPlanningCode == e.intTAPlanningCode).FirstOrDefault();
                 
                    if (v != null)
                    {
                        v.intTAPlanningCode = e.intTAPlanningCode;
                        v.uqTradeCode = e.uqTradeCode;
                        var tradeName = dc.tblTrades.Where(a => a.uqTradeCode == e.uqTradeCode).FirstOrDefault();
                        v.strTradeName = tradeName.strTradeName;
                        v.uqShiftCode = e.uqShiftCode;
                        var shiftName = dc.tblShifts.Where(a => a.uqShiftId == e.uqShiftCode).FirstOrDefault();

                        v.strShiftName = shiftName.strShiftName;

                    v.uqAreaCode = e.uqAreaCode;
                    var areaName = dc.tblAreas.Where(a => a.uqAreaCode == e.uqAreaCode).FirstOrDefault();
                    v.strAreaName = areaName.strAreaName;

                    v.uqDepartmentCode = e.uqDepartmentCode;
                    var departmentName = dc.tblDepartments.Where(a => a.uqDepartmentCode == e.uqDepartmentCode).FirstOrDefault();
                    v.strDepartmentName = departmentName.strDepartmentName;
                    v.dtFrom = e.dtFrom;
                        v.dtTo = e.dtTo;
                        v.intDays = e.intDays;
                        v.dtMoidifyAt = DateTime.Now;
                        v.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                        dc.Entry(v).State = EntityState.Modified;

                        string Description = ("Updated annual TA - " + e.uqTradeCode);
                        dHelper.LogAction(Description);

                    }
               
                    else
                    {
                        var tradeName = dc.tblTrades.Where(a => a.uqTradeCode == e.uqTradeCode).FirstOrDefault();
                        var shiftName = dc.tblShifts.Where(a => a.uqShiftId == e.uqShiftCode).FirstOrDefault();
                    var areaName = dc.tblAreas.Where(a => a.uqAreaCode == e.uqAreaCode).FirstOrDefault();
                    var departmentName = dc.tblDepartments.Where(a => a.uqDepartmentCode == e.uqDepartmentCode).FirstOrDefault();
                  
                    for (var day = e.dtFrom.Date; day < e.dtTo; day = day.AddDays(1))
                    {

                        tblTAPlanning tblTA = new tblTAPlanning()
                        {
                            intTAPlanningCode = Guid.NewGuid(),
                            uqTradeCode = e.uqTradeCode,
                            strTradeName = tradeName.strTradeName,
                            uqShiftCode = e.uqShiftCode,

                            strShiftName = shiftName.strShiftName,
                            uqAreaCode = e.uqAreaCode,
                            strAreaName = areaName.strAreaName,
                            uqDepartmentCode = e.uqDepartmentCode,
                            strDepartmentName = departmentName.strDepartmentName,
                            dtFrom = day,
                            dtTo = day.AddDays(1),
                            intDays = e.intDays,
                            dtCreatedAt = DateTime.Now,
                            uqCreatedBy = new Guid(dHelper.CurrentLoginUser()),
                            Year = dHelper.year()

                        };




                        string Description = ("Insert annual TA - " + e.uqTradeCode);
                        dHelper.LogAction(Description);
                        dc.tblTAPlannings.Add(tblTA);
                       

                    }

                       
                    }

                dc.SaveChanges();
                status = true;

            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(string eventID)
        {
            var status = false;
            using (Entities dc = new Entities())
            {
                var v = dc.tblTAPlannings.Where(a => a.intTAPlanningCode ==new Guid( eventID)).FirstOrDefault();
                if (v != null)
                {
                    dc.tblTAPlannings.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
    }


}