using AdvanceUMS.Helper;
using AdvanceUMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class tblClearanceController : Controller
    {
        string supervisorCode;
        string engineerCode;
        string hod;
        string contractorCode;
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        byte[] imgdata;
        Dictionary<string, object> res = new Dictionary<string, object>();
        public string test { get; set; }
        public ActionResult PopulateWorkers(string Status)
        {
            if (Status == null)
            {
                Status = dHelper.year();
            }
            PopulateDropdown();
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            List<tblWorker> workers = db.tblWorkers.ToList();
            List<tblDepartment> departments = db.tblDepartments.ToList();
            List<tblShift> shifts = db.tblShifts.ToList();
            List<tblAttendance> Attendances = db.tblAttendances.ToList();
            List<tblTrade> trades = db.tblTrades.ToList();
            List<tblSupervisor> supervisors = db.tblSupervisors.ToList();
            List<tblArea> areas = db.tblAreas.ToList();
            List<tblEngineer> engineers = db.tblEngineers.ToList();
            List<tblJoiningStat> joiningStats = db.tblJoiningStats.ToList();
            if (dHelper.CurrentSupervisorLoginUser() != "")
            {
                supervisorCode = new Guid(dHelper.CurrentSupervisorLoginUser()).ToString();

            }
            else if (dHelper.CurrentEngineerLoginUser() != "")
            {
                engineerCode = new Guid(dHelper.CurrentEngineerLoginUser()).ToString();

            }
            else if (dHelper.CurrentContractorUser() != "")
            {
                contractorCode = new Guid(dHelper.CurrentContractorUser()).ToString();

            }
            else if (dHelper.CurrentHODLoginUser() != "")
            {
                hod = new Guid(dHelper.CurrentHODLoginUser()).ToString();

            }
            if (contractorCode != null)
            {
                ViewBag.contractorCode = contractorCode;
                var list = db.procGetClearanceList(Status).Where(x => x.uqContractorCode == new Guid(contractorCode ) && x.ClearanceYear==Status  && x.bolIsClearance == true && x.dtClearanceRemarks !=null && x.dtEngineerRemarks!=null).ToList();


                return PartialView("_List", list);
            }
          
            else if (engineerCode != null)
            {
                ViewBag.engineer = engineerCode;

                var list = db.procGetClearanceList(Status).Where(x=>x.uqEngineerId==new Guid(engineerCode) && x.ClearanceYear == Status  && x.bolIsClearance == true && x.dtClearanceRemarks != null ).ToList();



                return PartialView("_List", list);
            }
          

            else
            {
                var list = db.procGetClearanceList(Status).Where(x=> x.WrokerYear == Status).ToList();



                return PartialView("_List", list);
            }
        }

        // GET: Admin/User
        public void PopulateDropdown()
        {
            var engineer = db.tblEngineers.OrderBy(s => s.strEngineerName);
            ViewBag.engineerList = new SelectList(engineer, "uqEngineerId", "strEngineerName");



            var department = db.tblDepartments.OrderBy(s => s.strDepartmentName);
            ViewBag.departmentList = new SelectList(department, "uqDepartmentCode", "strDepartmentName");

            var year = db.tblYears.OrderBy(s => s.intYearCode);
            ViewBag.yearList = new SelectList(year, "intYearCode", "strYearName");


        }
        public ActionResult Index()
        {
            PopulateDropdown();
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
        public ActionResult UpdateContractorClearance(tblClearance tblClearance)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    TimeZoneInfo timeZoneInfo;
                    DateTime dateTime;
                    //Set the time zone information to US Mountain Standard Time 
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    //Get date and time in US Mountain Standard Time 
                    dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);





                    var clearanceData = this.db.tblClearances.Find(tblClearance.uqClearanceCode);
                    clearanceData.uqClearanceCode = tblClearance.uqClearanceCode;
                    clearanceData.dtContractorRemarks = dateTime;
                    clearanceData.strContractotRemarks = tblClearance.strContractotRemarks;
                    clearanceData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    clearanceData.dtMoidifyAt = dateTime;
                    db.Entry(clearanceData).State = EntityState.Modified;
                    db.SaveChanges();


                    transaction.Commit();
                    string Description = ("update clearance - " + tblClearance.uqWorkerCode);
                    dHelper.LogAction(Description);

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




             return Redirect("/admin/tblClearance");
        }

        public ActionResult UpdateEngineerClearance(tblClearance tblClearance)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    TimeZoneInfo timeZoneInfo;
                    DateTime dateTime;
                    //Set the time zone information to US Mountain Standard Time 
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    //Get date and time in US Mountain Standard Time 
                    dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);



                 

                    var clearanceData = this.db.tblClearances.Find(tblClearance.uqClearanceCode);
                    clearanceData.uqClearanceCode = tblClearance.uqClearanceCode;
                    clearanceData.dtEngineerRemarks = dateTime;
                    clearanceData.strFeedback = tblClearance.strFeedback;
                    clearanceData.strEngineeerRemarks = tblClearance.strEngineeerRemarks;
                    clearanceData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    clearanceData.dtMoidifyAt = dateTime;
                    db.Entry(clearanceData).State = EntityState.Modified;
                    db.SaveChanges();


                    transaction.Commit();
                    string Description = ("update clearance - " + tblClearance.uqWorkerCode);
                    dHelper.LogAction(Description);

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




             return Redirect("/admin/tblClearance");
        }
        [HttpGet]
        public ActionResult Index(string year)
        {
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
        public ActionResult InsertClearance(tblClearance tblClearance)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    TimeZoneInfo timeZoneInfo;
                    DateTime dateTime;
                    //Set the time zone information to US Mountain Standard Time 
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    //Get date and time in US Mountain Standard Time 
                    dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);


                
                    tblClearance clearance = new tblClearance()
                    {
                        strClearanceRemarks= tblClearance.strClearanceRemarks,
                        strContractotRemarks=null,
                        strEngineeerRemarks=null,
                        strFeedback=null,
                        dtClearanceRemarks=dateTime,
                        dtContractorRemarks=null,
                        dtCreatedAt=dateTime,
                        dtEngineerRemarks=null,
                        dtMoidifyAt=null,
                        uqClearanceCode = Guid.NewGuid(),
                        uqCreatedBy= new Guid(dHelper.CurrentLoginUser().ToString()),
                        uqModifyBy=null,
                        uqWorkerCode=tblClearance.uqWorkerCode,
                        Year=dHelper.year()

                    };
                    db.tblClearances.Add(clearance);
                    db.SaveChanges();

                    var workerData = this.db.tblWorkers.Find(tblClearance.uqWorkerCode);
                    workerData.uqId = tblClearance.uqWorkerCode;
                    workerData.bolIsClearance = true;
                    workerData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    workerData.dtMoidifyAt = dateTime;
                    db.Entry(workerData).State = EntityState.Modified;
                    db.SaveChanges();


                    transaction.Commit();
                    string Description = ("Insert clearance - " + tblClearance.uqWorkerCode);
                    dHelper.LogAction(Description);

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




             return Redirect("/admin/tblClearance");
        }





    }
}