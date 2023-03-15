using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Models;
using AdvanceUMS.Helper;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class ModuleController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();
        // GET: Admin/Module
        public ActionResult Index()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsAdmin == false)
                return Redirect("/admin/notaccess");
            
            return View();
        }

        public ActionResult PopulateModule()
        {
            var module = db.ModuleList(null).ToList();
            ViewBag.ModuleList = module;
            return PartialView("_List");
        }

        public void PopulateDropdown()
        {
            var module = db.tblModules.Where(s => s.IsDeleted == false && s.ParentModuleID == 0);
            ViewBag.ModuleList = new SelectList(module, "ID", "ModuleName");          
        }

        #region Create Module
        public ActionResult Create()
        {
            PopulateDropdown();
            return PartialView("_CreateOrUpdate", null);
        }

        [HttpPost]
        public JsonResult Create(tblModule module)
        {
            tblModule objModule;
            if (module.ID <=0)
            {
                objModule = new tblModule();                                                
            }
            else
                objModule = db.tblModules.FirstOrDefault(s => s.ID == module.ID);
            if (objModule != null)
            {
                if (GeneralHelper.IsDemo == true)
                {
                    if (objModule.IsDefault == true)
                    {
                        res["status"] = "2";
                        res["message"] = "<div class=\"alert alert-danger fade in\">Edit is disabled in default module.</div>";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }
                int oldSrNo = 0;
                if (module.DisplayOrder.HasValue)
                    oldSrNo = module.DisplayOrder.Value;

                objModule.ParentModuleID = module.ParentModuleID == null ? 0 : module.ParentModuleID;
                objModule.DisplayOrder = module.DisplayOrder;
                objModule.ModuleName = module.ModuleName;
                objModule.PageIcon = module.PageIcon;
                objModule.PageUrl = module.PageUrl;
                objModule.PageSlug = dHelper.GetModuleSlug(module.ModuleName, module.ParentModuleID);
                objModule.IsDeleted = false;
                objModule.IsActive = true;
                objModule.IsDefault = false;
                if (module.ID <= 0)
                {
                    db.tblModules.Add(objModule);
                    string Description = string.Format("New Module Add [Name: {0}]", objModule.ModuleName);
                    dHelper.LogAction(Description);
                }
                else
                {
                    string Description = string.Format("New Module Update [Name: {0}]", objModule.ModuleName);
                    dHelper.LogAction(Description);
                }
                res["status"] = "1";
                db.SaveChanges();
                if (Request.QueryString["ModuleID"] != null)
                {
                    int newSrNo = Convert.ToInt32(module.DisplayOrder);
                    db.ModuleUpdateSortOrder(oldSrNo, newSrNo, Convert.ToInt32(Request.QueryString["ModuleID"]), module.ParentModuleID);
                }
                else
                {
                    db.ModuleInsertSortOrder(module.ID, module.ParentModuleID);
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region
        public ActionResult Edit(int Id)
        {
            PopulateDropdown();
            var module = db.tblModules.FirstOrDefault(s => s.ID == Id);
            if (module != null)
            {
                return PartialView("_CreateOrUpdate", module);
            }
            return View();
        }
        #endregion

        #region Delete Module
        public JsonResult Delete(int ID)
        {
            var module = db.tblModules.FirstOrDefault(s => s.ID == ID);
            if (module != null)
            {
                if (GeneralHelper.IsDemo == true)
                {
                    if (module.IsDefault == true)
                    {
                        res["success"] = 3;
                        res["message"] = "<div class=\"alert alert-danger fade in\">Delete is disabled in default module.</div>";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }
                module.IsDeleted = true;                
                db.SaveChanges();

                string Description = string.Format("Delete Module [Name: {0}]", module.ModuleName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">Successfully deleted module.</div>";
                res["url"] = "/admin/module/PopulateModule";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}