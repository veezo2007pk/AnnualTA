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
    public class tblEngineersController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        byte[] imgdata;
        Dictionary<string, object> res = new Dictionary<string, object>();
        public string test { get; set; }

        public ActionResult UpdateEngineerList(FormCollection formCollection)
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            string uqDepartmentCode = formCollection["uqDepartmentCode"].ToString();
            string uqHODId = formCollection["uqHODId"].ToString();
            string[] ids = formCollection["ID"].Split(new char[] { ',' });
            foreach (string id in ids)
            {

                //tblSupervisor Supervisor = new tblSupervisor();
                var engineerData = this.db.tblEngineers.Find(new Guid(id));
                engineerData.uqEngineerId = engineerData.uqEngineerId;

                engineerData.strEngineerName = engineerData.strEngineerName;
                engineerData.uqDepartmentCode = new Guid(uqDepartmentCode);

                engineerData.uqHODId = new Guid(uqHODId);
                engineerData.dtCreatedAt = engineerData.dtCreatedAt;
                engineerData.uqCreatedBy = engineerData.uqCreatedBy;
                engineerData.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                engineerData.dtMoidifyAt = dateTime;
                db.Entry(engineerData).State = EntityState.Modified;
                db.SaveChanges();
                //var employee = this.db.tbl.Find(int.Parse(id));
                //this.db.Employees.Remove(employee);

            }

            PopulateDropdown();
            List<tblSupervisor> supervisors = db.tblSupervisors.ToList();
            List<tblEngineer> engineers = db.tblEngineers.ToList();
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            tblSupervisorListForm _tblSupervisorListForm = new tblSupervisorListForm();
            _tblSupervisorListForm.supervisorList = db.tblSupervisors.ToList();

            var list = from e in supervisors
                       join a in engineers on e.uqEngineerCode equals a.uqEngineerId into table1
                       from a in table1.ToList().DefaultIfEmpty()
                       select new tblSupervisorListForm
                       {

                           tblSupervisor = e,
                           tblEngineer = a



                       };

            return View("Index");
        }



        
        public void PopulateDropdownByDepartmetFilter(string Departmentid)
        {
            var hod = db.tblHODs.Where(x => x.uqDepartmentCode == new Guid(Departmentid)).OrderBy(s => s.strHODName);
            ViewBag.hodList = new SelectList(hod, "uqHODId", "strHODName");



            var department = db.tblDepartments.OrderBy(s => s.strDepartmentName);
            ViewBag.departmentList = new SelectList(department, "uqDepartmentCode", "strDepartmentName");


        }

        public ActionResult FilterByDepartment(string uqDepartmentCode)
        {
            PopulateDropdownByDepartmetFilter(uqDepartmentCode);
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            List<tblEngineer> engineers = db.tblEngineers.ToList();
            List<tblHOD> hODs = db.tblHODs.ToList();
            List<tblDepartment> departments = db.tblDepartments.ToList();

            tblSupervisorListForm _tblSupervisorListForm = new tblSupervisorListForm();
            var list = from e in engineers
                       join a in hODs on e.uqHODId equals a.uqHODId into table1
                       from a in table1.ToList().DefaultIfEmpty()
                       join b in departments on e.uqDepartmentCode equals b.uqDepartmentCode into table2
                       from b in table2.ToList().DefaultIfEmpty()
                       where e.uqDepartmentCode == new Guid(uqDepartmentCode)
                       select new tblSupervisorListForm
                       {

                         
                           tblEngineer = e,
                           tblHOD=a,
                           tblDepartment=b



                       };
            //_tblSupervisorListForm.supervisorList = db.tblSupervisors.Where(x=>x.uqDepartmentCode== _supervisorListForm.supervisor.uqDepartmentCode).ToList();

            return PartialView("_ListFilterByDepartment", list);
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

        #region Populate UserList
        public ActionResult PopulateEngineers(string Status)
        {
            PopulateDropdown();
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
           
            List<tblEngineer> engineers = db.tblEngineers.ToList();
            List<tblHOD> hODs = db.tblHODs.ToList();
            List<tblDepartment> departments = db.tblDepartments.ToList();
            tblSupervisorListForm _tblSupervisorListForm = new tblSupervisorListForm();
            var list = from e in engineers
                       join a in hODs on e.uqHODId equals a.uqHODId into table1
                       from a in table1.ToList().DefaultIfEmpty()
                       join b in departments on e.uqDepartmentCode equals b.uqDepartmentCode into table2
                       from b in table2.ToList().DefaultIfEmpty()
                       select new tblSupervisorListForm
                       {

                        
                           tblEngineer = e,
                           tblHOD = a,
                           tblDepartment=b



                       };

            return PartialView("_List", list);
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

        public void PopulateDropdown()
        {
            var hod = db.tblHODs.OrderBy(s => s.strHODName);
            ViewBag.hodList = new SelectList(hod, "uqHODId", "strHODName");

        

            var department = db.tblDepartments.OrderBy(s => s.strDepartmentName);
            ViewBag.departmentList = new SelectList(department, "uqDepartmentCode", "strDepartmentName");


        }

        public JsonResult AddUser(EngineerViewModel objEngineer, HttpPostedFileBase Photo)
        {
            PopulateDropdown();
            try
            {
                tblEngineer Engineer = new tblEngineer();
                string desc = "";
                if (objEngineer.uqEngineerId == null)
                {

                   
                    Engineer.uqEngineerId = Guid.NewGuid();
                    Engineer.uqDepartmentCode = objEngineer.uqDepartmentCode;
                    Engineer.uqHODId = objEngineer.uqHODId;


                    Engineer.strEngineerName = objEngineer.strEngineerName;
                
                    Engineer.dtCreatedAt = DateTime.Now;
                    Engineer.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                 


                    db.tblEngineers.Add(Engineer);
                    desc = ("Insert Engineer : " + Engineer.strEngineerName);
                    Session["Add"] = "add";
                }
                else
                {
                    var author = db.tblEngineers.First(a => a.uqEngineerId == objEngineer.uqEngineerId);
                    // Debug.WriteLine("Image session"+Session["imgdata"].ToString());

                    author.uqDepartmentCode = objEngineer.uqDepartmentCode;
                    author.uqHODId = objEngineer.uqHODId;

                    author.uqEngineerId = objEngineer.uqEngineerId;
                  
                    author.strEngineerName = objEngineer.strEngineerName;
                 
                    author.dtCreatedAt = objEngineer.dtCreatedAt;
                  
                    author.uqModifyBy= new Guid(dHelper.CurrentLoginUser());
                    author.dtMoidifyAt = objEngineer.dtMoidifyAt;
                    db.Entry(author).State = EntityState.Modified;
                    desc = ("Update Engineer : " + Engineer.strEngineerName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblEngineers";

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
            Guid EngineerID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var EngineerInfo = db.tblEngineers.Where(s => s.uqEngineerId == EngineerID).FirstOrDefault();
            if (EngineerInfo != null)
            {
                EngineerViewModel objEngineer = new EngineerViewModel {
                    
                    uqEngineerId = EngineerInfo.uqEngineerId,
             
                strEngineerName = EngineerInfo.strEngineerName,
                uqDepartmentCode=EngineerInfo.uqDepartmentCode,
                uqHODId=EngineerInfo.uqHODId,
             
                dtCreatedAt = EngineerInfo.dtCreatedAt,
                uqCreatedBy = EngineerInfo.uqCreatedBy,
              
                uqModifyBy = new Guid(dHelper.CurrentLoginUser()),
                dtMoidifyAt = EngineerInfo.dtMoidifyAt,
            };
               
                ViewBag.Edit = "1";
                return View(objEngineer);
            }

            return View();
        }
        #endregion

        #region Delete User
        public JsonResult Delete(Guid ID)
        {
            var EngineerInfo = db.tblEngineers.FirstOrDefault(s => s.uqEngineerId == ID);
            if (EngineerInfo != null)
            {


                tblEngineer tblEngineer = db.tblEngineers.Find(EngineerInfo.uqEngineerId);
                db.tblEngineers.Remove(tblEngineer);
                db.SaveChanges();

                string Description = string.Format("Delete Engineer [EngineerInfo: {0}]", EngineerInfo.strEngineerName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + EngineerInfo.strEngineerName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblEngineers/PopulateEngineers";
                else
                    res["url"] = "/admin/tblEngineers/PopulateEngineers" + Query;
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