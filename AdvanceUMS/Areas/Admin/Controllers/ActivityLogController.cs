using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Models;
using AdvanceUMS.Helper;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class ActivityLogController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        // GET: Admin/ActivityLog
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetActivityLog()
        {
            var control = dHelper.GetUserPermission("");
            if (control.IsLogin == false)
                return Redirect("/account/login");
            var activity = dHelper.GetActivityLog(Guid.Empty);
            if (control.IsAdmin == false)
                activity = dHelper.GetActivityLog(new Guid(dHelper.CurrentLoginUser()));            
            return PartialView("_ActivityLog", activity);
        }
    }
}