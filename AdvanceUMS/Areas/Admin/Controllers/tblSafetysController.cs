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

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class tblSafetysController : Controller
    {
        string supervisorCode;
        string engineerCode;
        string contractorCode;
        string hodCode;
        string userCode;
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        byte[] imgdata;
        Dictionary<string, object> res = new Dictionary<string, object>();
        public string test { get; set; }





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
                    string Description = ("Safety verify - " + attendaceData.uqAttendanceCode);
                    dHelper.LogAction(Description);

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




             return Redirect("/admin/tblSafetys");
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
                        var attendaceData = this.db.tblAttendances.Find(new Guid(id));
                        attendaceData.uqAttendanceCode = new Guid(id);
                        attendaceData.bolIsVerified = true;
                        attendaceData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                        attendaceData.dtMoidifyAt = dateTime;
                        db.Entry(attendaceData).State = EntityState.Modified;
                        db.SaveChanges();
                        string Description = ("Safety verify - " + attendaceData.uqAttendanceCode);
                        dHelper.LogAction(Description);
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





             return Redirect("/admin/tblSafetys");
        }


        public ActionResult insertSingleAttendance(String id)
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);

            try
                {
                   

                      
                        var WorkerData = this.db.tblWorkers.Find(new Guid(id));
                        WorkerData.uqId = new Guid(id);
                WorkerData.bolIsSafetyVerified = true;
                WorkerData.dtSafety = dateTime;
                        WorkerData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                        WorkerData.dtMoidifyAt = dateTime;
                        db.Entry(WorkerData).State = EntityState.Modified;
                        db.SaveChanges();
                 





                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong

                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }





             return Redirect("/admin/tblSafetys");
        }


        public ActionResult InsertAttendance(FormCollection formCollection)
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);

            try
                {
                    string[] ids = formCollection["ID"].Split(new char[] { ',' });
                    foreach (string id in ids)
                    {


                        var WorkerData = this.db.tblWorkers.Find(new Guid(id));
                        WorkerData.uqId = new Guid(id);
                    WorkerData.bolIsSafetyVerified = true;
                        WorkerData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                        WorkerData.dtMoidifyAt = dateTime;
                        db.Entry(WorkerData).State = EntityState.Modified;
                        db.SaveChanges();
                    }




                }
                catch (Exception)
                {
                
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            


         

             return Redirect("/admin/tblSafetys");
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
            if (Session["Add"] != null)
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully insert user.</div>";
            else if (Session["Update"] != null)
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully update user.</div>";
            Session.RemoveAll();
            return View();
        }
        public ActionResult PopulateSupervisors(string Status)
        {
            if (Status == null)
            {
                Status = dHelper.year();
            }
            PopulateDropdown();
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
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
            else 
            {
                userCode = new Guid(dHelper.CurrentLoginUser()).ToString();


            }
          

            DateTime dt_tomarrow = DateTime.Now.AddDays(1);
            List<tblWorker> workers = db.tblWorkers.ToList();
            List<tblShift> shifts = db.tblShifts.ToList();
            List<tblTrade> trades = db.tblTrades.ToList();
            List<tblArea> areas = db.tblAreas.ToList();
            List<tblHOD> hODs = db.tblHODs.ToList();
            List<tblSupervisor> supervisors = db.tblSupervisors.ToList();
            List<tblAttendance> attendances = db.tblAttendances.ToList();
            List<tblDepartment> departments = db.tblDepartments.ToList();
            List<tblEngineer> engineers = db.tblEngineers.ToList();
            List<tblContractor> contractors = db.tblContractors.ToList();
            List<tblJoiningStat> joiningStats = db.tblJoiningStats.ToList();
            tblSupervisorListForm listForm = new tblSupervisorListForm();
            if(userCode != null)
            {
                ViewBag.userCode = userCode;

                var employeeRecord = from e in workers

                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join b in engineers on a.uqEngineerCode equals b.uqEngineerId into table4
                                     from b in table4.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on e.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     join c in contractors on e.uqContractorCode equals c.uqContractorCode into table6
                                     from c in table6.ToList()
                                     where e.Year==Status && e.intDehireStatus != 2 && (e.bolIsSafetyVerified == false|| e.bolIsSafetyVerified == null) && e.bolIsWorkerApproveByEng==true
                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblEngineer = b,
                                         tblContractor = c,
                                         tblArea = z

                                     };




                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return PartialView("_List", employeeRecord);
            }

            if (supervisorCode != null)
            {
                ViewBag.supervisorCode = supervisorCode;



                var employeeRecord = from e in workers

                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join b in engineers on a.uqEngineerCode equals b.uqEngineerId into table4
                                     from b in table4.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on e.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     join c in contractors on e.uqContractorCode equals c.uqContractorCode into table6
                                     from c in table6.ToList()
                                     where e.Year == Status && e.uqSupervisorCode == new Guid(supervisorCode) && e.intDehireStatus != 2 && e.bolIsSafetyVerified == true
                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblEngineer = b,
                                         tblContractor = c,
                                         tblArea = z

                                     };
                
            



            //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

            return PartialView("_List", employeeRecord);
            }
            else if(engineerCode != null)
            {
                ViewBag.engineerCode = engineerCode;

                var employeeRecord = from e in workers
                                 
                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join b in engineers on a.uqEngineerCode equals b.uqEngineerId into table4
                                     from b in table4.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on e.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     join c in contractors on e.uqContractorCode equals c.uqContractorCode into table6
                                     from c in table6.ToList()
                                     where e.Year == Status && e.intDehireStatus != 2 && e.bolIsSafetyVerified == true && a.uqEngineerCode == new Guid(engineerCode) 
                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                        tblEngineer=b,
                                        tblContractor=c,
                                         tblArea=z

                                     };




                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return PartialView("_List", employeeRecord);
            }
            else if (hodCode != null)
            {
                ViewBag.hodCode = hodCode;
                var employeeRecord = from e in workers

                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join b in engineers on a.uqEngineerCode equals b.uqEngineerId into table4
                                     from b in table4.ToList()

                                     join s in hODs on b.uqHODId equals s.uqHODId into table7
                                     from s in table7.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on e.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     join c in contractors on e.uqContractorCode equals c.uqContractorCode into table6
                                     from c in table6.ToList()
                                     where e.Year == Status && e.intDehireStatus != 2 && e.bolIsSafetyVerified == true && s.uqHODId == new Guid(hodCode)
                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblHOD=s,
                                         tblEngineer = b,
                                         tblContractor = c,
                                         tblArea = z

                                     };
               




                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return PartialView("_List", employeeRecord);
            }

            else if (contractorCode != null)
            {
                ViewBag.contractorCode = contractorCode;
                var employeeRecord = from e in workers

                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join b in engineers on a.uqEngineerCode equals b.uqEngineerId into table4
                                     from b in table4.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on e.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     join c in contractors on e.uqContractorCode equals c.uqContractorCode into table6
                                     from c in table6.ToList()
                                     where e.Year == Status && e.uqContractorCode == new Guid(contractorCode) && e.bolIsSafetyVerified == true
                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblEngineer = b,
                                         tblContractor = c,
                                         tblArea = z

                                     };

              
             



                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return PartialView("_List", employeeRecord);
            }
            return null;
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