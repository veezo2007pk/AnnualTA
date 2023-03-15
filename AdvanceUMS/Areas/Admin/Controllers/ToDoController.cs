using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Models;
using AdvanceUMS.Helper;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class ToDoController : Controller
    {
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        Dictionary<string, object> res = new Dictionary<string, object>();
        // GET: Admin/ToDo
        public ActionResult Index()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsView == false)
                return Redirect("/admin/notaccess");
            return View();
        }

        #region Create ToDo
        public ActionResult Create()
        {            
            return PartialView("_CreateUpdate", null);
        }

        [HttpPost]
        public JsonResult Create(tblToDo todo)
        {   
            tblToDo objTodo;
            if (todo.ID == Guid.Empty)
            {
                objTodo = new tblToDo();
                objTodo.ID = Guid.NewGuid();
                objTodo.IsCompleted = false;
                objTodo.CreateDate = DateTime.Now;
                objTodo.CreateBy = dHelper.CurrentLoginUser();            
            }
            else
                objTodo = db.tblToDoes.FirstOrDefault(s => s.ID == todo.ID);
            if (objTodo != null)
            {
                objTodo.Description = todo.Description;
                objTodo.ModifiedBy = dHelper.CurrentLoginUser();
                objTodo.ModifiedDate = DateTime.Now;
                
                if (todo.ID == Guid.Empty)
                {
                    db.tblToDoes.Add(objTodo);                    
                    string Description = string.Format("New Todo Add [Name: {0}]", objTodo.Description);
                    dHelper.LogAction(Description);
                }
                else
                {                    
                    string Description = string.Format("New Todo Update [Name: {0}]", objTodo.Description);
                    dHelper.LogAction(Description);
                }
                res["status"] = "1";
                db.SaveChanges();
                return Json(res, JsonRequestBehavior.AllowGet);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region UnFinish To Do List
        public ActionResult UnFinishToDoList()
        {
            var toDo = db.tblToDoes.Where(s => s.IsCompleted == false).ToList();
            Guid UserID = new Guid(dHelper.CurrentLoginUser());
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsAdmin == false)
                toDo = toDo = toDo.Where(s => s.UserID == UserID).ToList();
            toDo = toDo.OrderByDescending(s => s.CreateDate).ToList();
            return PartialView("_UnFinishList", toDo);
        }
        #endregion

        #region Finish To Do List
        public ActionResult FinishToDoList()
        {
            var toDo = db.tblToDoes.Where(s => s.IsCompleted == true).ToList();
            Guid UserID = new Guid(dHelper.CurrentLoginUser());
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsAdmin == false)
                toDo = toDo = toDo.Where(s => s.UserID == UserID).ToList();
            toDo = toDo.OrderByDescending(s => s.CreateDate).ToList();
            toDo = toDo.OrderByDescending(s => s.CreateDate).ToList();
            return PartialView("_FinishList", toDo);
        }
        #endregion

        #region Update ToDo
        public JsonResult UpdateStatus(Guid Id)
        {
            var status = db.tblToDoes.FirstOrDefault(s => s.ID == Id);
            if (status != null)
            {
                if (status.IsCompleted == true)
                {
                    res["success"] = 0;
                    status.IsCompleted = false;

                    string Description = string.Format("To Do Change [Name: {0},Status: InCompleted]", status.Description);
                    dHelper.LogAction(Description);
                }
                else
                {
                    res["success"] = 1;
                    status.IsCompleted = true;
                    status.CompleteDate = DateTime.Now;
                    string Description = string.Format("Status Change [Name: {0},Status: Completed]", status.Description);
                    dHelper.LogAction(Description);
                }
                status.ModifiedDate = DateTime.Now;
                status.ModifiedBy = dHelper.CurrentLoginUser();
                db.SaveChanges();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}