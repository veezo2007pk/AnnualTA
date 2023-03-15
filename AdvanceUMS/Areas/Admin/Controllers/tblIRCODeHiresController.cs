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
    public class tblIRCODeHiresController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        byte[] imgdata;
        Dictionary<string, object> res = new Dictionary<string, object>();
        public string test { get; set; }
        
                    public ActionResult insertSingleOvertime(String id, int intRecommendedHrs)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                   

                        //tblSupervisor Supervisor = new tblSupervisor();
                        tblOvertime objOvertime = new tblOvertime()
                        {
                           uqOvertimeCode=new Guid(),
                            dtCreatedAt = DateTime.Now,
                            dtMoidifyAt = null,
                            uqAttendanceCode = new Guid(id),
                            uqCreatedBy = new Guid(dHelper.CurrentLoginUser().ToString()),
                            uqModifyBy = null,
                           intRecommendedHrs=intRecommendedHrs



                        };

                        db.tblOvertimes.Add(objOvertime);
                        db.SaveChanges();

                        //string lastid = objOvertime.uqWorkerCode.ToString();

                        var AttendanceData = this.db.tblAttendances.Find(new Guid(id));
                    AttendanceData.uqAttendanceCode = new Guid(id);
                    AttendanceData.bolIsOvertime = true;
                    AttendanceData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    AttendanceData.dtMoidifyAt = DateTime.Now;
                        db.Entry(AttendanceData).State = EntityState.Modified;
                        db.SaveChanges();
                 



                    transaction.Commit();

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }




            return View("Index");
        }


        public ActionResult DeHire(FormCollection formCollection)
        {


            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string id = formCollection["uqId"].ToString();
                   
                  
                      



                    var WorkerData = this.db.tblWorkers.Find(new Guid(id));
                    WorkerData.uqId = new Guid(id);
                    //WorkerData.bolIsDehire = true;
                    WorkerData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    WorkerData.dtMoidifyAt = DateTime.Now;
                    db.Entry(WorkerData).State = EntityState.Modified;
                    db.SaveChanges();





                    transaction.Commit();

                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                    ViewBag.ResultMessage = "Error occured, records rolledback.";
                }
            }





            return View("Index");
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
            PopulateDropdown();
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            tblSupervisorListForm _tblSupervisorListForm = new tblSupervisorListForm();
            //string supervisorCode = new Guid(dHelper.CurrentSupervisorLoginUser()).ToString();
            DateTime dt_tomarrow = DateTime.Now.AddDays(1);
            List<tblWorker> workers = db.tblWorkers.ToList();
            List<tblShift> shifts = db.tblShifts.ToList();
            List<tblAttendance> Attendances = db.tblAttendances.ToList();
            List<tblTrade> trades = db.tblTrades.ToList();
            var todaysDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            TimeSpan time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));
            DateTime dt = DateTime.Now;
            DateTime dateOnly = dt.Date;

            var list = from e in workers
                     
                   
                       join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                       from d in table2.ToList()
                       join f in shifts on e.uqShiftCode equals f.uqShiftId into table3
                       from f in table3.ToList()
                       where  e.uqContractorCode==new Guid( "87FC3445-A7FB-45BD-B93B-D4CF3B6D277F")
                       //                   join f in Attendances on e.uqId equals f.uqWorkerCode into table3
                       //                   from f in table3.ToList()
                       //                   i.strShiftName == "Day" && i.tmTo < time


                       select new tblSupervisorListForm
            {
                tblWorker = e,
               tblTrade = d,
               tblShift=f,
             


            };



            //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

            return PartialView("_List", list);
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