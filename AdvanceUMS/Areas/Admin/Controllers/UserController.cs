using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Models;
using AdvanceUMS.Helper;
using System.IO;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        Entities db=new Entities();
        GeneralHelper dHelper=new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();
                
        // GET: Admin/User
        public ActionResult Index()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsView == false)
                return Redirect("/admin/notaccess");
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsAdd = control.IsAdd;
            if (Session["Add"] != null)
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully insert user.</div>";
            else if (Session["Update"] != null)
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully update user.</div>";
            Session.RemoveAll();         
            return View();
        }

        #region Populate UserList
        public ActionResult PopulateUser(string Status)
        {

            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            ViewBag.IsAdmin = control.IsAdmin;
            var userList = db.tblUsers.Where(s => s.IsDeleted == false).ToList();
            if (Status == "Active")
                userList = userList.Where(s => s.IsActive == true).ToList();
            else if (Status == "Banned")
                userList = userList.Where(s => s.IsActive == false).ToList();
            else if (Status == "NewUser")
                userList = userList.Where(s => s.InsertDate.Value.Month == DateTime.Now.Month).ToList();
            return PartialView("_List", userList);
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
            ViewBag.IsAdmin = control.IsAdmin;
            return View();
        }

        public void PopulateDropdown()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            ViewBag.IsAdmin = control.IsAdmin;
            List<tblDepartment> departments = db.tblDepartments.ToList();

            List<tblSupervisor> supervisors = db.tblSupervisors.ToList();

            List<tblEngineer> engineers = db.tblEngineers.ToList();
            List<tblHOD> hODs = db.tblHODs.ToList();


            //var engineer = db.tblEngineers.OrderBy(s => s.strEngineerName);
            //ViewBag.engineerList = new SelectList(engineer, "uqEngineerId", "strEngineerName");



            var supervisorList = (from e in supervisors
                                     join d in departments on e.uqDepartmentCode equals d.uqDepartmentCode into table1
                                 from d in table1.ToList()

                                     select new SelectListItem
                                     {
                                         Text = e.strSupervisorName + " --- " + d.strDepartmentName,
                                         Value = e.uqSupervisorId.ToString()
                                     });
            ViewBag.supervisorList = new SelectList(supervisorList, "Value", "Text");

            var engineerList = (from e in engineers
                                     join d in departments on e.uqDepartmentCode equals d.uqDepartmentCode into table1
                                     from d in table1.ToList()

                                   select new SelectListItem
                                   {
                                       Text = e.strEngineerName + " --- " + d.strDepartmentName,
                                       Value = e.uqEngineerId.ToString()
                                   });

            ViewBag.engineerList = new SelectList(engineerList, "Value", "Text");




            var hodList = (from e in hODs
                                join d in departments on e.uqDepartmentCode equals d.uqDepartmentCode into table1
                                from d in table1.ToList()

                                select new SelectListItem
                                {
                                    Text = e.strHODName + " --- " + d.strDepartmentName,
                                    Value = e.uqHODId.ToString()
                                });

            ViewBag.hodList = new SelectList(hodList, "Value", "Text");


            //var supervisor = db.tblSupervisors.OrderBy(s => s.strSupervisorName);
            //ViewBag.supervisorList = new SelectList(supervisor, "uqSupervisorId", "strSupervisorName");

            var contractor = db.tblContractors.OrderBy(s => s.strContractorName);
            ViewBag.ContractorList = new SelectList(contractor, "uqContractorCode", "strContractorName");

            var country = db.tblCountries.Where(s => s.IsDeleted == false).OrderBy(s => s.Name);
            ViewBag.CountryList = new SelectList(country, "ID", "Name");

            var role= db.tblRoles.Where(s => s.IsDeleted == false).OrderBy(s => s.Name);
            ViewBag.RoleList = new SelectList(role, "ID", "Name");
            
            List<SelectListItem> listGender = new List<SelectListItem>();
            listGender.Add(new SelectListItem { Text = "Male", Value = "Male", Selected = true });
            listGender.Add(new SelectListItem { Text = "FeMale", Value = "FeMale" });
            ViewBag.GenderList = new SelectList(listGender, "Value", "Text");
        }                
        
        public JsonResult AddUser(UserViewModel objUser, HttpPostedFileBase Photo)
        {
            PopulateDropdown();
            try
            {
                tblUser user;
                if (objUser.ID == null)
                {
                    user = new tblUser();
                    user.PhotoPath = "/Content/assets/dist/img/avatar5.png";
                }
                else
                {
                    user = db.tblUsers.FirstOrDefault(s => s.ID == objUser.ID);
                }
                if (ViewBag.IsAdmin == true)
                {
                    if (user != null)
                    {
                        if (GeneralHelper.IsDemo == true)
                        {
                            if (user.IsDefault == true)
                            {
                                Session["edit"] = "Edit";
                                res["status"] = "2";
                                res["message"] = "<div class=\"alert alert-danger fade in\">Edit is disabled in default user.</div>";
                                res["url"] = (Request.UrlReferrer.AbsoluteUri);
                                return Json(res, JsonRequestBehavior.AllowGet);
                            }
                        }

                        user.FirstName = objUser.FirstName;
                        user.LastName = objUser.LastName;
                        user.IsActive = true;
                        user.Phone = objUser.Phone;
                        user.Address = objUser.Address;
                        user.Gender = objUser.Gender;
                        user.CountryID = Convert.ToInt32(objUser.CountryID);
                        if (objUser.BirthDate != null)
                            user.DateOfBirth = Convert.ToDateTime(objUser.BirthDate);
                        user.Email = objUser.Email;
                        //user.Password = dHelper.Encrypt(objUser.Password);
                        user.Password =objUser.Password;

                        user.IsDefault = false;
                        user.IsDeleted = false;
                        user.FacebookLink = objUser.FacebookLink;
                        user.TwitterLink = objUser.TwitterLink;
                        user.LinkedInLink = objUser.LinkedInLink;
                        user.GoogleLink = objUser.GoogleLink;
                        user.SkypeID = objUser.SkypeID;
                        user.IsSupervisor = objUser.IsSupervisor;
                        user.uqSupervisorCode = objUser.uqSupervisorCode;
                        user.IsEngineer = objUser.IsEngineer;
                        user.uqEngineerCode = objUser.uqEngineerCode;
                        user.IsHOD = objUser.IsHOD;
                        user.uqHODCode = objUser.uqHODCode;
                        user.uqContractorCode = objUser.uqContractorCode;
                        if (Photo != null)
                        {
                            string _FileName = Guid.NewGuid().ToString();
                            string _ext = Path.GetExtension(Photo.FileName);
                            string _path = Path.Combine(Server.MapPath("~/Images/Profile/"), _FileName + _ext);

                            if (System.IO.File.Exists(Server.MapPath(user.PhotoPath)))
                                System.IO.File.Delete(Server.MapPath(user.PhotoPath));

                            Photo.SaveAs(_path);
                            user.PhotoPath = "/Images/Profile/" + _FileName + _ext;
                        }
                        string desc = "";
                        if (objUser.ID == null)
                        {
                            user.ID = Guid.NewGuid();

                            user.RoleID = objUser.RoleID;
                            user.UserName = objUser.UserName;
                            user.FailedPasswordCount = 0;
                            user.LastLoginDate = DateTime.Now;
                            user.InsertDate = DateTime.Now;
                            user.uqContractorCode = objUser.uqContractorCode;
                            user.LastModifiedBy = new Guid(dHelper.CurrentLoginUser());
                            user.IsLockedOut = false;
                            db.tblUsers.Add(user);
                            desc = ("Insert User : " + user.UserName);
                            Session["Add"] = "add";
                        }
                        else
                        {
                            desc = ("Update User : " + user.UserName);
                            Session["Update"] = "update";
                        }
                        db.SaveChanges();
                        dHelper.LogAction(desc);
                        res["status"] = "1";
                        //Response.Redirect("/admin/user");
                        res["url"] = "/admin/user";
                    }
                }
                else
                {
                    if (user != null)
                    {
                        if (GeneralHelper.IsDemo == true)
                        {
                            if (user.IsDefault == true)
                            {
                                Session["edit"] = "Edit";
                                res["status"] = "2";
                                res["message"] = "<div class=\"alert alert-danger fade in\">Edit is disabled in default user.</div>";
                                res["url"] = (Request.UrlReferrer.AbsoluteUri);
                                return Json(res, JsonRequestBehavior.AllowGet);
                            }
                        }


                        //user.Password = dHelper.Encrypt(objUser.Password);
                        user.Password = objUser.Password;



                        string desc = "";
                        if (objUser.ID == null)
                        {
                            user.ID = Guid.NewGuid();

                            user.RoleID = objUser.RoleID;
                            user.UserName = objUser.UserName;
                            user.FailedPasswordCount = 0;
                            user.LastLoginDate = DateTime.Now;
                            user.InsertDate = DateTime.Now;
                            user.uqContractorCode = objUser.uqContractorCode;
                            user.LastModifiedBy = new Guid(dHelper.CurrentLoginUser());
                            user.IsLockedOut = false;
                            db.tblUsers.Add(user);
                            desc = ("Insert User : " + user.UserName);
                            Session["Add"] = "add";
                        }
                        else
                        {
                            desc = ("Update User : " + user.UserName);
                            Session["Update"] = "update";
                        }
                        db.SaveChanges();
                        dHelper.LogAction(desc);
                        res["status"] = "1";
                        //Response.Redirect("/admin/user");
                        res["url"] = "/admin/user";
                    }
                }
            }
            catch
            {
                res["status"] = "0";
            }
            
            return Json(res,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string ID)
        {
            PopulateDropdown();
            if (Session["edit"] != null)
            {
                ViewBag.Message = "<div class=\"alert alert-danger fade in\">Edit is disabled in default user.</div>";
                Session.RemoveAll();
            }
            Guid UserID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            //if (control.IsEdit == false)
            //{
            //    if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
            //        return Redirect("/admin/notaccess");                
            //}
            ViewBag.IsAdmin = control.IsAdmin;
            var userInfo = db.tblUsers.FirstOrDefault(s => s.ID == UserID);
            if (userInfo != null)
            {
                UserViewModel objUser = new UserViewModel();
                objUser.ID = userInfo.ID;
                objUser.IsSupervisor = userInfo.IsSupervisor;
                objUser.uqSupervisorCode = userInfo.uqSupervisorCode;
                objUser.IsEngineer = userInfo.IsEngineer;
                objUser.uqEngineerCode = userInfo.uqEngineerCode;
                objUser.IsHOD = userInfo.IsHOD;
                objUser.uqHODCode = userInfo.uqHODCode;
                objUser.FirstName = userInfo.FirstName;
                objUser.LastName = userInfo.LastName;
                objUser.BirthDate = userInfo.DateOfBirth == null ? null : userInfo.DateOfBirth.Value.ToString("dd-MMM-yyyy");
                objUser.Gender = userInfo.Gender;
                objUser.CountryID = userInfo.CountryID;
                objUser.Phone = userInfo.Phone;
                objUser.Email = userInfo.Email;
                objUser.Address = userInfo.Address;
                objUser.RoleID = userInfo.RoleID;
                objUser.FacebookLink = userInfo.FacebookLink;
                objUser.TwitterLink = userInfo.TwitterLink;
                objUser.GoogleLink = userInfo.GoogleLink;
                objUser.LinkedInLink = userInfo.LinkedInLink;
                objUser.SkypeID = userInfo.SkypeID;
                objUser.UserName = userInfo.UserName;
                objUser.Password =userInfo.Password;
                objUser.cPassword =userInfo.Password;
                objUser.uqContractorCode = userInfo.uqContractorCode;
                ViewBag.PhotoPath = userInfo.PhotoPath;

                ViewBag.Edit = "1";
                return View(objUser);
            }
            
            return View();
        }
        #endregion

        #region Delete User
        public ActionResult Delete(Guid ID)
        {
            var userInfo = db.tblUsers.FirstOrDefault(s => s.ID == ID);
            if (userInfo != null)
            {
                if (GeneralHelper.IsDemo == true)
                {
                    if (userInfo.IsDefault == true)
                    {
                        res["success"] = 3;
                        res["message"] = "<div class=\"alert alert-danger fade in\">delete is disabled in default user.</div>";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }
                tblUser tblUser = db.tblUsers.Find(userInfo.ID);
                IList<tblUserLog> data = new List<tblUserLog>();
                data = db.tblUserLogs.Where(x => x.UserID == ID).ToList();
                if(data != null)
                {
                    db.tblUserLogs.RemoveRange(data);

                }
                db.tblUsers.Remove(tblUser);
            
                db.SaveChanges();
              

                string Description = string.Format("Delete User [Name: {0}]", userInfo.FirstName + " " + userInfo.LastName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete user.</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/user/populateuser";
                else
                    res["url"] = "/admin/user/populateuser" + Query;
            }
            return RedirectToAction("Index", "User");
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
                userInfo.LastModifiedBy =new Guid(dHelper.CurrentLoginUser());
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
    }
}