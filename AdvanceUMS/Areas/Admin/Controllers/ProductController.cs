using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Helper;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        GeneralHelper dHelper = new GeneralHelper();
        // GET: Admin/Product
        public ActionResult Index()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsView == false)
                return Redirect("/admin/notaccess");
            ViewBag.IsAdd = control.IsAdd;
            return View();
        }

        public ActionResult Create()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsView == false)
                return Redirect("/admin/notaccess");
            return View();
        }
    }
}