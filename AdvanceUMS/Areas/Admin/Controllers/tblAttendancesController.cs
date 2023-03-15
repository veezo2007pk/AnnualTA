using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Models;
using AdvanceUMS.Helper;
using System.IO;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Data.Entity.Core.Objects;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class tblAttendancesController : Controller
    {

        string supervisorCode;
        string engineerCode;
        string contractorCode;
        string hodCode;
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        byte[] imgdata;
        Dictionary<string, object> res = new Dictionary<string, object>();
        public string test { get; set; }

        public ActionResult AddAttendance(WorkerViewModel tblWorker)
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
                    //Print out the date and time
                    Console.WriteLine(dateTime.ToString("yyyy-MM-dd HH-mm-ss"));
                    //tblSupervisor Supervisor = new tblSupervisor();
                    var Worker = this.db.tblWorkers.Where(x => x.strWorkderId == tblWorker.strWorkderId).FirstOrDefault();
                    tblAttendance objAttendance = new tblAttendance()
                    {
                        dtCheckIn = tblWorker.dtCheckIn,
                        dtCheckOut = null,
                        dtCreatedAt = dateTime,
                        dtMoidifyAt = null,
                        uqAttendanceCode = Guid.NewGuid(),
                        uqCreatedBy = new Guid(dHelper.CurrentLoginUser().ToString()),
                        uqShiftCode = Worker.uqShiftCode,
                        uqTradeCode = Worker.uqTradeCode,
                        uqModifyBy = null,
                        uqWorkerCode = Worker.uqId,
                        Year=dHelper.year()



                    };


                    db.tblAttendances.Add(objAttendance);
                    db.SaveChanges();
                    string Description = ("Insert Attendance for worker - " + objAttendance.uqWorkerCode);
                    dHelper.LogAction(Description);
                  




                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




            return Json("sds", JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateAttendance(tblAttendance tblAttendance)
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            DateTime dtCheckin;
         
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            dtCheckin= TimeZoneInfo.ConvertTime(tblAttendance.dtCheckIn.Value, timeZoneInfo);
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var attendaceData = this.db.tblAttendances.Find(new Guid(tblAttendance.uqAttendanceCode.ToString()));
                    attendaceData.uqAttendanceCode = new Guid(tblAttendance.uqAttendanceCode.ToString());
                    attendaceData.dtCheckIn = dtCheckin;
                    attendaceData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    attendaceData.dtMoidifyAt = dateTime;
                    db.Entry(attendaceData).State = EntityState.Modified;
                    db.SaveChanges();






                    transaction.Commit();
                    string Description = ("Verify Attendance for worker - " + attendaceData.uqWorkerCode);
                    dHelper.LogAction(Description);

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




            return Json("sds", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Revert(String id)
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var attendanceInfo = db.tblAttendances.FirstOrDefault(s => s.uqAttendanceCode == new Guid(id));
                    if (attendanceInfo != null)
                    {


                        tblAttendance tblSupervisor = db.tblAttendances.Find(attendanceInfo.uqAttendanceCode);
                        db.tblAttendances.Remove(tblSupervisor);
                        db.SaveChanges();
                    }


                    var WorkerData = this.db.tblWorkers.Where(x => x.uqId == attendanceInfo.uqWorkerCode).FirstOrDefault();
                    WorkerData.uqId = WorkerData.uqId;
                    WorkerData.dtLastCheckedIn = null;
                    WorkerData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    WorkerData.dtMoidifyAt = dateTime;
                    db.Entry(WorkerData).State = EntityState.Modified;
                    db.SaveChanges();







                    transaction.Commit();
                    string Description = ("Revert Attendance for worker - " + WorkerData.uqId);
                    dHelper.LogAction(Description);

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




            return Redirect("/admin/tblAttendances");
        }

        public ActionResult verifySingleAttendance(String id)
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var attendaceData = this.db.tblAttendances.Find(new Guid(id));
                    attendaceData.uqAttendanceCode = new Guid(id);
                    attendaceData.bolIsVerified = true;
                    attendaceData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    attendaceData.dtMoidifyAt = dateTime;
                    db.Entry(attendaceData).State = EntityState.Modified;
                    db.SaveChanges();

                  




                    transaction.Commit();
                    string Description = ("Verify Attendance for worker - " + attendaceData.uqWorkerCode);
                    dHelper.LogAction(Description);

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




            return Redirect("/admin/tblAttendances");
        }


        public ActionResult verifyAttendance(FormCollection formCollection)
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string[] ids = formCollection["ID"].Split(new char[] { ',' });
                    foreach (string id in ids)
                    {
                        var attendaceData = this.db.tblAttendances.Where(x=>x.uqAttendanceCode==new Guid(id)).FirstOrDefault();
                     
                        attendaceData.bolIsVerified = true;
                        attendaceData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                        attendaceData.dtMoidifyAt = dateTime;
                        db.Entry(attendaceData).State = EntityState.Modified;
                        db.SaveChanges();
                        string Description = ("Verify Attendance for worker - "+ attendaceData.uqWorkerCode);
                        dHelper.LogAction(Description);
                    }



                    transaction.Commit();
                 

                }
                catch (Exception ex)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }





            return Redirect("/admin/tblAttendances");
        }


        public ActionResult insertSingleAttendance(String id)
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
                    //Print out the date and time
                    Console.WriteLine(dateTime.ToString("yyyy-MM-dd HH-mm-ss"));
                    //tblSupervisor Supervisor = new tblSupervisor();
                    var Worker = this.db.tblWorkers.Find(new Guid(id));
                    tblAttendance objAttendance = new tblAttendance()
                        {
                            dtCheckIn = dateTime,
                            dtCheckOut = null,
                            dtCreatedAt = dateTime,
                            dtMoidifyAt = null,
                            uqAttendanceCode = Guid.NewGuid(),
                            uqCreatedBy = new Guid(dHelper.CurrentLoginUser().ToString()),
                            uqShiftCode=Worker.uqShiftCode,
                            uqTradeCode=Worker.uqTradeCode,
                            uqModifyBy = null,
                            Year=dHelper.year(),
                            uqWorkerCode = new Guid(id), 
                        bolisAttendanceShiftToBackDate=false



                        };


                        db.tblAttendances.Add(objAttendance);
                        db.SaveChanges();
                    string Description = ("Insert Attendance for worker - "+ objAttendance.uqWorkerCode);
                    dHelper.LogAction(Description);
                    string lastid = objAttendance.uqWorkerCode.ToString();

                        var WorkerData = this.db.tblWorkers.Find(new Guid(lastid));
                        WorkerData.uqId = new Guid(id);
                        WorkerData.dtLastCheckedIn = dateTime;
                        WorkerData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                        WorkerData.dtMoidifyAt = dateTime;
                        db.Entry(WorkerData).State = EntityState.Modified;
                        db.SaveChanges();
                    GetNightAttendance getNightAttendance = new GetNightAttendance();

                   var  tblAttendances = db.UpdateNigthBackDate(dHelper.year()).ToList();
                    foreach (var item in tblAttendances)
                    {
                        var data = db.tblAttendances.Where(x => x.uqAttendanceCode == item.uqAttendanceCode).FirstOrDefault();
                        data.dtCheckIn = data.dtCheckIn.Value.AddDays(-1);
                        data.bolisAttendanceShiftToBackDate = true;
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();



                        var worker = db.tblWorkers.Where(x => x.uqId == item.uqWorkerCode).FirstOrDefault();
                        worker.dtLastCheckedIn = data.dtCheckIn;


                        db.Entry(worker).State = EntityState.Modified;
                        db.SaveChanges();
                    }





                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    throw ex;
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




            return Redirect("/admin/tblAttendances");
        }


        public ActionResult InsertAttendance(FormCollection formCollection)
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string[] ids = formCollection["ID"].Split(new char[] { ',' });
                    foreach (string id in ids)
                    {
                        var Worker = this.db.tblWorkers.Find(new Guid(id));
                        //tblSupervisor Supervisor = new tblSupervisor();
                        tblAttendance objAttendance = new tblAttendance()
                        {
                            dtCheckIn = dateTime,
                            dtCheckOut = null,
                            dtCreatedAt = dateTime,
                            dtMoidifyAt = null,
                            uqAttendanceCode = Guid.NewGuid(),
                            uqShiftCode=Worker.uqShiftCode,
                            uqTradeCode = Worker.uqTradeCode,
                            uqCreatedBy = new Guid(dHelper.CurrentLoginUser().ToString()),
                            uqModifyBy = null,
                            uqWorkerCode = new Guid(id)



                        };

                        db.tblAttendances.Add(objAttendance);
                        db.SaveChanges();
                        string Description = ("Insert Attendance for worker - "+ objAttendance.uqWorkerCode);
                        dHelper.LogAction(Description);
                        string lastid = objAttendance.uqWorkerCode.ToString();

                        var WorkerData = this.db.tblWorkers.Find(new Guid(lastid));
                        WorkerData.uqId = new Guid(id);
                        WorkerData.dtLastCheckedIn = dateTime;
                        WorkerData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                        WorkerData.dtMoidifyAt = dateTime;
                        db.Entry(WorkerData).State = EntityState.Modified;
                        db.SaveChanges();
                    }



                    transaction.Commit();

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }



         

            return Redirect("/admin/tblAttendances");
        }
        [HttpGet]
        public ActionResult Index(string year)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            ViewBag.IsAdd = control.IsAdd;
            ViewBag.IsAdmin = control.IsAdmin;
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
        // GET: Admin/User
        public ActionResult Index()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsView == false)
                return Redirect("/admin/notaccess");

            ViewBag.IsAdd = control.IsAdd;
            ViewBag.IsAdmin = control.IsAdmin;
            if (Session["Add"] != null)
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully insert user.</div>";
            else if (Session["Update"] != null)
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully update user.</div>";
            Session.RemoveAll();
            return View();
        }
        public void UpdateOldShift()
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);


            string s = dHelper.year();
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
                hodCode = new Guid(dHelper.CurrentHODLoginUser()).ToString();

            }

            List<tblWorker> workers = db.tblWorkers.Where(x=>x.Year==s).ToList();
            List<tblShift> shifts = db.tblShifts.ToList();
            var employeeRecord = from e in workers
                                 join i in shifts on e.uqShiftCode equals i.uqShiftId into table2
                                 from i in table2.ToList()
                                 where /*e.uqSupervisorCode == new Guid(supervisorCode)*/

                                 e.bolIsWorkerApproveByEng == true
                                 && e.intDehireStatus != 2
                                 && (i.tmFrom>= new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) && new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second)>i.tmTo||i.tmTo>i.tmFrom && new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second)>i.tmTo||i.tmTo>i.tmFrom &&i.tmFrom>= new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second))
                                 //|| (new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmFrom 
                                 //&& new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) > i.tmTo)) 
                                 //&& (new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmFrom 
                                 //|| new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) > i.tmTo))
            select new tblSupervisorListForm
                                 {
                                     tblWorker = e,                                   
                                     tblShift = i

                                 };



            foreach (var item in employeeRecord)
            {
                if(item.tblWorker.dtLastCheckedIn != null)
                {
                    var WorkerData = this.db.tblWorkers.Find(item.tblWorker.uqId);
                    WorkerData.uqId = item.tblWorker.uqId;
                    WorkerData.dtLastCheckedIn = null;
                    WorkerData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    WorkerData.dtMoidifyAt = dateTime;
                    db.Entry(WorkerData).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public ActionResult PopulateSupervisors(string Status)
        {
            if (Status == null)
            {
                Status = dHelper.year();
            }
            PopulateDropdown();
            if (dHelper.CurrentSupervisorLoginUser() != "")
            {
                UpdateOldShift();
            }
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            TimeSpan time = dateTime.TimeOfDay;
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            ViewBag.IsAdmin = control.IsAdmin;
            tblSupervisorListForm _tblSupervisorListForm = new tblSupervisorListForm();
           
            if (dHelper.CurrentSupervisorLoginUser() != ""){
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
                hodCode = new Guid(dHelper.CurrentHODLoginUser()).ToString();

            }

            DateTime dt_tomarrow = DateTime.Now.AddDays(1);
            List<tblWorker> workers = db.tblWorkers.ToList();
            List<tblShift> shifts = db.tblShifts.ToList();
            List<tblTrade> trades = db.tblTrades.ToList();
            List<tblArea> areas = db.tblAreas.ToList();
            List<tblSupervisor> supervisors = db.tblSupervisors.ToList();
            List<tblContractor> contractors = db.tblContractors.ToList();
            List<tblAttendance> attendances = db.tblAttendances.ToList();
            List<tblDepartment> departments = db.tblDepartments.ToList();
            List<tblEngineer> engineers = db.tblEngineers.ToList();
            List<tblJoiningStat> joiningStats = db.tblJoiningStats.ToList();
            List<tblHOD> hODs = db.tblHODs.ToList();
            tblSupervisorListForm listForm = new tblSupervisorListForm();

            TimeSpan start = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
            TimeSpan end = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
            if (supervisorCode != null)
            {
                ViewBag.supervisorCode = supervisorCode;

                var employeeRecord = from e in workers
                                    
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table1
                                     from d in table1.ToList()
                                     join i in shifts on e.uqShiftCode equals i.uqShiftId into table2
                                     from i in table2.ToList()
                                     join b in contractors on e.uqContractorCode equals b.uqContractorCode into table3
                                     from b in table3.ToList()
                                     join c in areas on e.uqAreaCode equals c.uqAreaCode into table4
                                     from c in table4.ToList()
                                     where e.uqSupervisorCode == new Guid(supervisorCode) 
                                     && e.bolIsWorkerApproveByEng==true 
                                 
                                     && e.intDehireStatus != 2
                                     && e.Year==Status
                                    && ((i.tmTo > i.tmFrom && new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) >= i.tmFrom && new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmTo) ||
  (i.tmTo < i.tmFrom && (new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmTo || new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) >= i.tmFrom)))

                                     //&& ((i.tmFrom > i.tmTo 
                                     //&& (new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) > i.tmFrom 
                                     //|| new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmTo))
                                     //|| (new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) > i.tmFrom 
                                     //&& new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmTo))

                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblContractor=b,
                                         tblArea=c,
                                         

                                     };


                
            



            //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

            return PartialView("_List", employeeRecord);
            }
            else if(engineerCode != null)
            {
                ViewBag.engineerCode = engineerCode;

                var employeeRecord = from e in workers
                                     join n in attendances on e.uqId equals n.uqWorkerCode into table4
                                     from n in table4.ToList()
                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on n.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     join c in contractors on e.uqContractorCode equals c.uqContractorCode into table6
                                     from c in table6.ToList()
                                     where e.intDehireStatus != 2 && n.Year == Status && (e.bolIsWorkerApproveByEng == true || e.bolIsWorkerApproveByEng != null)
 && a.uqEngineerCode == new Guid(engineerCode) && (n.bolIsVerified ==false || n.bolIsVerified == null) 
                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblAttendance =n,
                                         tblArea=z,
                                         tblContractor=c

                                     };




                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return PartialView("_List", employeeRecord);
            }
            else if (hodCode != null)
            {
                ViewBag.hodCode = hodCode;

                var employeeRecord = from e in workers
                                     join n in attendances on e.uqId equals n.uqWorkerCode into table4
                                     from n in table4.ToList()
                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join s in engineers on a.uqEngineerCode equals s.uqEngineerId into table10
                                     from s in table10.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on n.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     where e.intDehireStatus != 2 && n.Year == Status && (e.bolIsWorkerApproveByEng == true || e.bolIsWorkerApproveByEng != null)
 && s.uqHODId == new Guid(hodCode) && (n.bolIsVerified == false || n.bolIsVerified == null || n.bolIsVerified==true) 
                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblAttendance = n,
                                         tblArea = z,
                                         tblEngineer=s

                                     };




                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return PartialView("_List", employeeRecord);
            }

            else if (contractorCode != null)
            {
                ViewBag.contractorCode = contractorCode;

                var employeeRecord = from e in workers
                                     join n in attendances on e.uqId equals n.uqWorkerCode into table4
                                     from n in table4.ToList()
                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join s in engineers on a.uqEngineerCode equals s.uqEngineerId into table10
                                     from s in table10.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on n.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()

                                         //                   join f in Attendances on e.uqId equals f.uqWorkerCode into table3
                                         //                   from f in table3.ToList()
                                         //                   i.strShiftName == "Day" && i.tmTo < time
                                     where e.uqContractorCode == new Guid(contractorCode) && n.Year == Status && e.intDehireStatus != 2  && e.bolIsWorkerApproveByEng == true

                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                        
                                         tblSupervisor = a,
                                       
                                         tblArea = z,
                                         
                                         tblAttendance = n,
                                         tblEngineer = s,
                                         tblShift=i




                                     };




                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return PartialView("_List", employeeRecord);
            }
            else if( supervisorCode==null &&
             engineerCode == null &&
             contractorCode == null &&
             hodCode == null )
            {
              
                return PartialView("_List");
            }
            return null;
        }
        public JsonResult GetbyID(string ID)
        {
            var data = db.AdminAttendanceCorrection().Where(x => x.uqAttendanceCode == new Guid(ID)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAttendancetList(string strworkerId)
        {
            //Pass The All Student Record From Controller To View For Show The All Data For User
            var data = db.AdminAttendanceCorrection().Where(x=>x.strWorkderId==strworkerId &&x.Year==dHelper.year()).ToList();
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region Populate UserList
        public ActionResult FilterByDepartment(tblSupervisorListForm _supervisorListForm)
        {
            PopulateDropdownByDepartmetFilter(_supervisorListForm.supervisor.uqDepartmentCode);
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            tblSupervisorListForm _tblSupervisorListForm = new tblSupervisorListForm();
            _tblSupervisorListForm.supervisorList = db.tblSupervisors.Where(x=>x.uqDepartmentCode== _supervisorListForm.supervisor.uqDepartmentCode).ToList();
          
            return PartialView("_ListFilterByDepartment", _tblSupervisorListForm);
        }
        #endregion

        #region Create & Update User
        public ActionResult Create()
        {
            PopulateDropdown();
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsView == false)
                return Redirect("/admin/notaccess");
            ViewBag.add = "add";
            return View();
        }
        public ActionResult UpdateSupervisorList(FormCollection formCollection)
        {
            string uqDepartmentCode = formCollection["supervisor.uqDepartmentCode"].ToString();
            string uqEngineerCode = formCollection["supervisor.uqEngineerCode"].ToString();
            string[] ids = formCollection["ID"].Split(new char[] { ',' });
            foreach (string id in ids)
            {
              
                //tblSupervisor Supervisor = new tblSupervisor();
                var SupervisorData = this.db.tblSupervisors.Find(new Guid(id));
                SupervisorData.uqSupervisorId = SupervisorData.uqSupervisorId;

                SupervisorData.strSupervisorName = SupervisorData.strSupervisorName;
                SupervisorData.uqDepartmentCode = new Guid(uqDepartmentCode);

                SupervisorData.uqEngineerCode = new Guid(uqEngineerCode);
                SupervisorData.dtCreatedAt = SupervisorData.dtCreatedAt;
                SupervisorData.uqCreatedBy = SupervisorData.uqCreatedBy;
                SupervisorData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                SupervisorData.dtMoidifyAt = DateTime.Now;
                db.Entry(SupervisorData).State = EntityState.Modified;
                db.SaveChanges();
                //var employee = this.db.tbl.Find(int.Parse(id));
                //this.db.Employees.Remove(employee);

            }
            
            PopulateDropdown();
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            tblSupervisorListForm _tblSupervisorListForm = new tblSupervisorListForm();
            _tblSupervisorListForm.supervisorList = db.tblSupervisors.ToList();

            return PartialView("_ListFilterByDepartment", _tblSupervisorListForm);
        }

        public void PopulateDropdownByDepartmetFilter( Guid? Departmentid)
        {
            var engineer = db.tblEngineers.Where(x=>x.uqDepartmentCode== Departmentid).OrderBy(s => s.strEngineerName);
            ViewBag.engineerList = new SelectList(engineer, "uqEngineerId", "strEngineerName");



            var department = db.tblDepartments.OrderBy(s => s.strDepartmentName);
            ViewBag.departmentList = new SelectList(department, "uqDepartmentCode", "strDepartmentName");


        }
        public void PopulateDropdown()
        {
            var engineer = db.tblEngineers.OrderBy(s => s.strEngineerName);
            ViewBag.engineerList = new SelectList(engineer, "uqEngineerId", "strEngineerName");

        

            var department = db.tblDepartments.OrderBy(s => s.strDepartmentName);
            ViewBag.departmentList = new SelectList(department, "uqDepartmentCode", "strDepartmentName");

            var year = db.tblYears.OrderBy(s => s.intYearCode);
            ViewBag.yearList = new SelectList(year, "intYearCode", "strYearName");


        }

        public JsonResult AddUser(SupervisorViewModel objSupervisor, HttpPostedFileBase Photo)
        {
            PopulateDropdown();
            try
            {
                tblSupervisor Supervisor = new tblSupervisor();
                string desc = "";
                if (objSupervisor.uqSupervisorId == null)
                {

                   
                    Supervisor.uqSupervisorId = Guid.NewGuid();
                 
                    Supervisor.strSupervisorName = objSupervisor.strSupervisorName;
                    Supervisor.uqDepartmentCode = objSupervisor.uqDepartmentCode;

                    Supervisor.uqEngineerCode = objSupervisor.uqEngineerCode;
                    Supervisor.dtCreatedAt = DateTime.Now;
                    Supervisor.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                 


                    db.tblSupervisors.Add(Supervisor);
                    desc = ("Insert Supervisor : " + Supervisor.strSupervisorName);
                    Session["Add"] = "add";
                }
                else
                {
                    var author = db.tblSupervisors.First(a => a.uqSupervisorId == objSupervisor.uqSupervisorId);
                    // Debug.WriteLine("Image session"+Session["imgdata"].ToString());

                 

                    author.uqSupervisorId = objSupervisor.uqSupervisorId;
                  
                    author.strSupervisorName = objSupervisor.strSupervisorName;
                    Supervisor.uqDepartmentCode = objSupervisor.uqDepartmentCode;

                    Supervisor.uqEngineerCode = objSupervisor.uqEngineerCode;
                    author.dtCreatedAt = objSupervisor.dtCreatedAt;
                  
                    author.uqModifyBy= new Guid(dHelper.CurrentLoginUser());
                    author.dtMoidifyAt = objSupervisor.dtMoidifyAt;
                    db.Entry(author).State = EntityState.Modified;
                    desc = ("Update Supervisor : " + Supervisor.strSupervisorName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblSupervisors";

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
            PopulateDropdown();
            Guid SupervisorID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var SupervisorInfo = db.tblSupervisors.Where(s => s.uqSupervisorId == SupervisorID).FirstOrDefault();
            if (SupervisorInfo != null)
            {
                SupervisorViewModel objSupervisor = new SupervisorViewModel {
                    
                    uqSupervisorId = SupervisorInfo.uqSupervisorId,
             uqDepartmentCode=SupervisorInfo.uqDepartmentCode,
             uqEngineerCode=SupervisorInfo.uqEngineerCode,
                strSupervisorName = SupervisorInfo.strSupervisorName,
             
                dtCreatedAt = SupervisorInfo.dtCreatedAt,
                uqCreatedBy = SupervisorInfo.uqCreatedBy,
              
                uqModifyBy = new Guid(dHelper.CurrentLoginUser()),
                dtMoidifyAt = SupervisorInfo.dtMoidifyAt,
            };
               
                ViewBag.Edit = "1";
                return View(objSupervisor);
            }

            return View();
        }
        #endregion

        #region Delete User
        public JsonResult Delete(Guid ID)
        {
            var SupervisorInfo = db.tblSupervisors.FirstOrDefault(s => s.uqSupervisorId == ID);
            if (SupervisorInfo != null)
            {


                tblSupervisor tblSupervisor = db.tblSupervisors.Find(SupervisorInfo.uqSupervisorId);
                db.tblSupervisors.Remove(tblSupervisor);
                db.SaveChanges();

                string Description = string.Format("Delete Supervisor [SupervisorInfo: {0}]", SupervisorInfo.strSupervisorName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + SupervisorInfo.strSupervisorName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblSupervisors/PopulateSupervisors";
                else
                    res["url"] = "/admin/tblSupervisors/PopulateSupervisors" + Query;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Update User Status
        public JsonResult UpdateStatus(Guid Id)
        {
            var userInfo = db.tblUsers.FirstOrDefault(s => s.ID == Id);
            if (userInfo != null)
            {
                if (GeneralHelper.IsDemo == true)
                {
                    if (userInfo.IsDefault == true)
                    {
                        res["success"] = 3;
                        res["message"] = "<div class=\"alert alert-danger fade in\">status change is disabled in default user.</div>";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }
                if (userInfo.IsActive == true)
                {
                    res["success"] = 0;
                    userInfo.IsActive = false;

                    string Description = string.Format("Update User Status [Name: {0}] (InActive)", userInfo.FirstName + " " + userInfo.LastName);
                    dHelper.LogAction(Description);
                }
                else
                {
                    res["success"] = 1;
                    userInfo.IsActive = true;

                    string Description = string.Format("Edit User [Name: {0}] (Active)", userInfo.FirstName + " " + userInfo.LastName);
                    dHelper.LogAction(Description);
                }
                userInfo.LastModified = DateTime.Now;
                userInfo.LastModifiedBy = new Guid(dHelper.CurrentLoginUser());
                db.SaveChanges();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region User Profile
        public new ActionResult Profile(Guid ID)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission("");
            if (control.IsLogin == false)
                return Redirect("/account/login");

            var userInfo = db.tblUsers.FirstOrDefault(s => s.ID == ID);
            if (userInfo != null)
            {
                var role = db.tblRoles.FirstOrDefault(s => s.ID == userInfo.RoleID);
                if (role != null)
                    ViewBag.RoleName = role.Name;
                ViewBag.PhotoPath = userInfo.PhotoPath;
                return View(userInfo);
            }
            return View();
        }
        #endregion

        #region User ActivityLog
        public ActionResult GetActivityLog()
        {
            var activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));
            return PartialView("_ActivityLog", activity);
        }
        #endregion

        [HttpPost]
        public ActionResult Capture()
        {
            StreamReader reader = new StreamReader(Request.InputStream);
            string hexString = Server.UrlEncode(reader.ReadToEnd());
            Session["imgdata"] = hexString;
           
           
            string imageName = DateTime.Now.ToString("dd-MM-yy hh-mm-ss");
            string imagePath = string.Format("~/Captures/{0}.png", imageName);
            System.IO.File.WriteAllBytes(Server.MapPath(imagePath), ConvertHexToBytes(hexString));
            var data = VirtualPathUtility.ToAbsolute(imagePath);
            var result = data;
            Session["test"]= result;
            return null;
        }

        [HttpPost]
        public ContentResult GetCapture()
        {
            string url = Session["test"].ToString();
            //Session["test"] = null;
            return Content(url);
        }

        private static byte[] ConvertHexToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}