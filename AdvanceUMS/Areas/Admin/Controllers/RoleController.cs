using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Models;
using AdvanceUMS.Helper;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();
        // GET: Admin/Role
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
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully insert role.</div>";
            else if (Session["Update"] != null)
                ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully update role.</div>";
            Session.RemoveAll();
            return View();
        }

        #region Populate RoleList
        public ActionResult PopulateRole()
        {
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            var roleList = db.tblRoles.Where(s => s.IsDeleted == false).ToList();
            return PartialView("_List", roleList);
        }
        #endregion

        #region Create Or Update
        public ActionResult Create()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsAdmin == false)
                return Redirect("/admin/notaccess");
            PopulateModule();
            ViewBag.RoleID = null;
            return View();
        }

        public ActionResult Edit(Guid ID)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsAdmin == false)
                return Redirect("/admin/notaccess");

            if (Session["edit"] != null)
            {
                ViewBag.Message = "<div class=\"alert alert-danger fade in\">Edit is disabled in default role.</div>";
                Session.RemoveAll();
            }
            PopulateModule();
            ViewBag.RoleID = ID;
            var role = db.tblRoles.FirstOrDefault(s => s.ID == ID);
            if (role != null)
            {
                RoleViewModel objRole = new RoleViewModel();
                objRole.ID = role.ID;
                objRole.Name = role.Name;
                objRole.DisplayName = role.DisplayName;
                return View(objRole);
            }
            return View();
        }

        public void PopulateModule()
        {
            var module = db.ModuleList(null).ToList();
            ViewBag.ModuleList = module;
        }

        [HttpPost]
        public JsonResult Create(RoleViewModel role, FormCollection frmValue)
        {
            PopulateModule();
            try
            {
                string desc = "";
                tblRole objRole;
                if (role.ID == null)
                {
                    objRole = new tblRole();
                    objRole.ID = Guid.NewGuid();
                }
                else
                    objRole = db.tblRoles.FirstOrDefault(s => s.ID == role.ID);
                if (objRole != null)
                {
                    if (GeneralHelper.IsDemo == true)
                    {
                        if (objRole.IsDefault == true)
                        {
                            Session["edit"] = "edit";
                            res["status"] = "2";
                            res["message"] = "<div class=\"alert alert-danger fade in\">Edit is disabled in default role.</div>";
                            res["url"] = (Request.UrlReferrer.AbsoluteUri);
                            return Json(res, JsonRequestBehavior.AllowGet);
                        }
                    }
                    objRole.Name = role.Name;
                    objRole.DisplayName = role.DisplayName;
                    objRole.IsDefault = false;
                    objRole.IsDeleted = false;
                    objRole.LastModified = DateTime.Now;
                    objRole.LastModifiedBy = new Guid(dHelper.CurrentLoginUser());
                    var keys = frmValue.AllKeys.ToList();
                    if (role.ID == null)
                    {
                        objRole.InsertDate = DateTime.Now;
                        objRole.InsertBy = new Guid(dHelper.CurrentLoginUser());
                        db.tblRoles.Add(objRole);
                        desc = ("Insert Role : " + role.Name);
                        Session["Add"] = "add";
                    }
                    else
                    {
                        desc = ("Insert Role : " + role.Name);
                        Session["Update"] = "update";
                    }
                    db.SaveChanges();
                    //Delete all role permission
                    var deletePermision = db.tblRolePermissions.Where(s => s.RoleID == role.ID).ToList();
                    db.tblRolePermissions.RemoveRange(deletePermision);

                    foreach (var k in keys)
                    {
                        var actionType = k.Replace("[", "");
                        actionType = actionType.Replace("]", "");
                        int Moduleid = 0;
                        if (actionType.Contains("view"))
                            Moduleid = Convert.ToInt32(actionType.Replace("view", ""));
                        else if (actionType.Contains("create"))
                            Moduleid = Convert.ToInt32(actionType.Replace("create", ""));
                        else if (actionType.Contains("edit"))
                            Moduleid = Convert.ToInt32(actionType.Replace("edit", ""));
                        else if (actionType.Contains("delete"))
                            Moduleid = Convert.ToInt32(actionType.Replace("delete", ""));

                        if (Moduleid != 0)
                        {
                            tblRolePermission permission;
                            permission = db.tblRolePermissions.FirstOrDefault(s => s.RoleID == objRole.ID && s.ModuleID == Moduleid);
                            if (permission == null)
                            {
                                permission = new tblRolePermission();
                                db.tblRolePermissions.Add(permission);
                            }
                            if (permission != null)
                            {
                                permission.RoleID = objRole.ID;
                                permission.ModuleID = Convert.ToInt32(Moduleid);
                                if (actionType.Contains("view"))
                                    permission.IsView = true;
                                if (actionType.Contains("create"))
                                    permission.IsAdd = true;
                                if (actionType.Contains("edit"))
                                    permission.IsEdit = true;
                                if (actionType.Contains("delete"))
                                    permission.IsDeleted = true;
                                db.SaveChanges();
                            }
                        }
                    }
                    dHelper.LogAction(desc);
                    db.SaveChanges();
                    res["status"] = "1";
                    res["url"] = "/admin/role";
                    return Json(res, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex) { string m = ex.Message; }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete Role
        public JsonResult Delete(Guid ID)
        {
            var role = db.tblRoles.FirstOrDefault(s => s.ID == ID);
            if (role != null)
            {
                if (GeneralHelper.IsDemo == true)
                {
                    if (role.IsDefault == true)
                    {
                        res["success"] = 3;
                        res["message"] = "<div class=\"alert alert-danger fade in\">Delete is disabled in default role.</div>";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }
                role.IsDeleted = true;
                role.LastModified = DateTime.Now;
                role.LastModifiedBy = new Guid(dHelper.CurrentLoginUser());
                db.SaveChanges();

                string Description = string.Format("Delete Role [Name: {0}]", role.Name);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">Successfully delete role.</div>";
                res["url"] = "/admin/role/populaterole";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}