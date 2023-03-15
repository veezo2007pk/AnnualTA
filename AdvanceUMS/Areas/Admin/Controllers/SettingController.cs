using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Models;
using System.IO;
using AdvanceUMS.Helper;
using System.Data.SqlClient;
using System.Data;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class SettingController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        // GET: Admin/Setting
        public ActionResult Index()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsAdmin == false)
                return Redirect("/admin/notaccess");

            var setting = db.tblSettings.FirstOrDefault();
            if (setting != null)
            {
                ViewBag.LogoPath = setting.LogoPath;
                ViewBag.Fevicon = setting.FeviconPath;
                return View(setting);
            }
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(tblSetting objSetting, HttpPostedFileBase LogoPath, HttpPostedFileBase Fevicon)
        {
            var setting = db.tblSettings.FirstOrDefault();
            if (setting != null)
            {
                setting.Name = objSetting.Name;
                setting.SMTPEmail = objSetting.SMTPEmail;
                setting.SMTPHost = objSetting.SMTPHost;
                setting.SMTPPassword = objSetting.SMTPPassword;
                setting.SMTPPort = objSetting.SMTPPort;
                setting.SMTPUserName = objSetting.SMTPUserName;
                if (LogoPath != null)
                {
                    string _FileName = Guid.NewGuid().ToString();
                    string _ext = Path.GetExtension(LogoPath.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Images/"), _FileName + _ext);

                    if (System.IO.File.Exists(Server.MapPath(setting.LogoPath)))
                        System.IO.File.Delete(Server.MapPath(setting.LogoPath));

                    LogoPath.SaveAs(_path);
                    setting.LogoPath = "/Images/" + _FileName + _ext;
                }
                if (Fevicon != null)
                {
                    string _FileName = Guid.NewGuid().ToString();
                    string _ext = Path.GetExtension(Fevicon.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Images/"), _FileName + _ext);

                    if (System.IO.File.Exists(Server.MapPath(setting.FeviconPath)))
                        System.IO.File.Delete(Server.MapPath(setting.FeviconPath));

                    Fevicon.SaveAs(_path);
                    setting.FeviconPath = "/Images/" + _FileName + _ext;
                }
                
                ViewBag.LogoPath = setting.LogoPath;
                ViewBag.Fevicon = setting.FeviconPath;                
                db.SaveChanges();
                return View(objSetting);
            }

            return View();
        }

    }
}