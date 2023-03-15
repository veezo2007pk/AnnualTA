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
    public class tblHODsController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        byte[] imgdata;
        Dictionary<string, object> res = new Dictionary<string, object>();
        public string test { get; set; }

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
        public ActionResult PopulateHODs(string Status)
        {
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            var userList = db.tblHODs.ToList();

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
            ViewBag.add = "add";
            return View();
        }

        public void PopulateDropdown()
        {
            var hod = db.tblHODs.OrderBy(s => s.strHODName  );
            ViewBag.hodList = new SelectList(hod, "uqHODId", "strHODName");



            var department = db.tblDepartments.OrderBy(s => s.strDepartmentName);
            ViewBag.departmentList = new SelectList(department, "uqDepartmentCode", "strDepartmentName");


        }

        public JsonResult AddUser(HODViewModel objHOD, HttpPostedFileBase Photo)
        {
            PopulateDropdown();
            try
            {
                tblHOD HOD = new tblHOD();
                string desc = "";
                if (objHOD.uqHODId == null)
                {


                    HOD.uqHODId = Guid.NewGuid();

                    HOD.strHODName = objHOD.strHODName;

                    HOD.dtCreatedAt = DateTime.Now;
                    HOD.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());



                    db.tblHODs.Add(HOD);
                    desc = ("Insert HOD : " + HOD.strHODName);
                    Session["Add"] = "add";
                }
                else
                {
                    var author = db.tblHODs.First(a => a.uqHODId == objHOD.uqHODId);
                    // Debug.WriteLine("Image session"+Session["imgdata"].ToString());



                    author.uqHODId = objHOD.uqHODId;

                    author.strHODName = objHOD.strHODName;

                    author.dtCreatedAt = objHOD.dtCreatedAt;

                    author.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    author.dtMoidifyAt = objHOD.dtMoidifyAt;
                    db.Entry(author).State = EntityState.Modified;
                    desc = ("Update HOD : " + HOD.strHODName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblHODs";

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
            Guid HODID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsEdit == false)
            {
                if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
                    return Redirect("/admin/notaccess");
            }
            ViewBag.IsAdmin = control.IsAdmin;
            var HODInfo = db.tblHODs.Where(s => s.uqHODId == HODID).FirstOrDefault();
            if (HODInfo != null)
            {
                HODViewModel objHOD = new HODViewModel
                {

                    uqHODId = HODInfo.uqHODId,

                    strHODName = HODInfo.strHODName,

                    dtCreatedAt = HODInfo.dtCreatedAt,
                    uqCreatedBy = HODInfo.uqCreatedBy,

                    uqModifyBy = new Guid(dHelper.CurrentLoginUser()),
                    dtMoidifyAt = HODInfo.dtMoidifyAt,
                };

                ViewBag.Edit = "1";
                return View(objHOD);
            }

            return View();
        }
        #endregion

        #region Delete User
        public JsonResult Delete(Guid ID)
        {
            var HODInfo = db.tblHODs.FirstOrDefault(s => s.uqHODId == ID);
            if (HODInfo != null)
            {


                tblHOD tblHOD = db.tblHODs.Find(HODInfo.uqHODId);
                db.tblHODs.Remove(tblHOD);
                db.SaveChanges();

                string Description = string.Format("Delete HOD [HODInfo: {0}]", HODInfo.strHODName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + HODInfo.strHODName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblHODs/PopulateHODs";
                else
                    res["url"] = "/admin/tblHODs/PopulateHODs" + Query;
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
            Session["test"] = result;
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