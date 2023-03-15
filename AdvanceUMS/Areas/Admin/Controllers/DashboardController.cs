using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Models;
using AdvanceUMS.Helper;
using System.Globalization;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        string supervisorCode;
        string engineerCode;
        string contractorCode;
        string hodCode;
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            //try
            //{
            //    string Url = Request.RawUrl.ToString();
            //    var control = dHelper.GetUserPermission(Url);
            //    if (control.IsLogin == false)
            //        return Redirect("/account/login");
            //    if (control.IsView == false)
            //        return Redirect("/admin/notaccess");

            //    int TotalUser = 0, CurrentMonth = 0, ActiveUser = 0, InActiveUser = 0;
            //    // Total Member
            //    var userMast = db.tblUsers.Where(s => s.IsDeleted == false);
            //    if (userMast.Count() > 0)
            //        TotalUser = userMast.Count();

            //    // Current Month Register Users
            //    var currentMonth = userMast.Where(s => s.InsertDate.Value.Month == DateTime.Now.Month).Count();
            //    if (currentMonth > 0)
            //        CurrentMonth = currentMonth;

            //    // Unconfirmed Users
            //    var activeUser = userMast.Where(s => s.IsActive == true).Count();
            //    if (activeUser > 0)
            //        ActiveUser = activeUser;

            //    // Banned Users
            //    var bannedUser = userMast.Where(s => s.IsActive == false).Count();
            //    if (bannedUser > 0)
            //        InActiveUser = bannedUser;

            //    ViewBag.TotalUser = TotalUser;
            //    ViewBag.CurrentMonth = CurrentMonth;
            //    ViewBag.ActiveUser = ActiveUser;
            //    ViewBag.InActiveUser = InActiveUser;

            //    ViewBag.IsAdmin = control.IsAdmin;
            //    PopulateChart();
            //    PopulateLatestUser();
            //}
            //catch
            //{
            //}
            if (dHelper.CurrentSupervisorLoginUser() != "")
            {
                supervisorCode = new Guid(dHelper.CurrentSupervisorLoginUser()).ToString();

            }
            else if (dHelper.CurrentEngineerLoginUser() != "")
            {
                engineerCode = new Guid(dHelper.CurrentEngineerLoginUser()).ToString();

            }
            else if (dHelper.CurrentContractorUser() != "")
            {
                contractorCode = new Guid(dHelper.CurrentContractorUser()).ToString();

            }
            else if (dHelper.CurrentHODLoginUser() != "")
            {
                hodCode = new Guid(dHelper.CurrentHODLoginUser()).ToString();

            }

            DateTime dt_tomarrow = DateTime.Now.AddDays(1);
            List<tblWorker> workers = db.tblWorkers.ToList();
            List<tblShift> shifts = db.tblShifts.ToList();
            List<tblTrade> trades = db.tblTrades.ToList();
            List<tblArea> areas = db.tblAreas.ToList();
            List<tblSupervisor> supervisors = db.tblSupervisors.ToList();
            List<tblContractor> contractors = db.tblContractors.ToList();
            List<tblAttendance> attendances = db.tblAttendances.ToList();
            List<tblDepartment> departments = db.tblDepartments.ToList();
            List<tblEngineer> engineers = db.tblEngineers.ToList();
            List<tblJoiningStat> joiningStats = db.tblJoiningStats.ToList();
            List<tblHOD> hODs = db.tblHODs.ToList();
            List<tblOvertime> overtimes = db.tblOvertimes.ToList();
            tblSupervisorListForm listForm = new tblSupervisorListForm();

            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            TimeSpan time = dateTime.TimeOfDay;

            if (supervisorCode != null)
            {
                ViewBag.supervisorCode = supervisorCode;

                var employeeRecord = from e in workers
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table1
                                     from d in table1.ToList()
                                     join i in shifts on e.uqShiftCode equals i.uqShiftId into table2
                                     from i in table2.ToList()
                                     join b in contractors on e.uqContractorCode equals b.uqContractorCode into table3
                                     from b in table3.ToList()
                                     join c in areas on e.uqAreaCode equals c.uqAreaCode into table4
                                     from c in table4.ToList()
                                     where e.uqSupervisorCode == new Guid(supervisorCode)
                                     && e.bolIsWorkerApproveByEng == true
                                     && e.intDehireStatus != 2
                                    && ((i.tmTo > i.tmFrom && new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) >= i.tmFrom && new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmTo) ||
  (i.tmTo < i.tmFrom && (new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmTo || new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) >= i.tmFrom)))

                                     //&& ((i.tmFrom > i.tmTo 
                                     //&& (new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) > i.tmFrom 
                                     //|| new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmTo))
                                     //|| (new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) > i.tmFrom 
                                     //&& new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second) < i.tmTo))

                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblContractor = b,
                                         tblArea = c

                                     };







                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return View();
            }
            else if (engineerCode != null)
            {
                ViewBag.engineerCode = engineerCode;

                var engAttendance = from e in workers
                                     join n in attendances on e.uqId equals n.uqWorkerCode into table4
                                     from n in table4.ToList()
                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on n.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     join c in contractors on e.uqContractorCode equals c.uqContractorCode into table6
                                     from c in table6.ToList()
                                     where e.intDehireStatus != 2 && (e.bolIsWorkerApproveByEng == true || e.bolIsWorkerApproveByEng != null)
 && a.uqEngineerCode == new Guid(engineerCode) && (n.bolIsVerified == false || n.bolIsVerified == null) && n.Year==dHelper.year()
                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblAttendance = n,
                                         tblArea = z,
                                         tblContractor = c

                                     };
                ViewBag.engAttendance = engAttendance.ToList().Count;






                var engOvertime = from e in workers
                                     join n in attendances on e.uqId equals n.uqWorkerCode into table4
                                     from n in table4.ToList()
                                     join c in overtimes on n.uqAttendanceCode equals c.uqAttendanceCode into table6
                                     from c in table6.ToList()
                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on e.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     where e.intDehireStatus != 2 && a.uqEngineerCode == new Guid(engineerCode) && n.bolIsOvertime == true && n.bolIsVerified == true && (c.bolIsVerified == null || c.bolIsVerified == false) && e.bolIsWorkerApproveByEng == true
                                     && c.Year == dHelper.year()
                                  select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblAttendance = n,
                                         tblArea = z,
                                         tblOvertime = c

                                     };



                ViewBag.engOvertime = engOvertime.ToList().Count;




                var engDehiringRequest= from e in workers
                           join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                           from a in table1.ToList()


                           join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                           from d in table2.ToList()
                           join f in shifts on e.uqShiftCode equals f.uqShiftId into table3
                           from f in table3.ToList()
                           where e.intDehireStatus == 1 && a.uqEngineerCode == new Guid(engineerCode) && e.Year == dHelper.year()
                                        //                   join f in Attendances on e.uqId equals f.uqWorkerCode into table3
                                        //                   from f in table3.ToList()
                                        //                   i.strShiftName == "Day" && i.tmTo < time


                                        select new tblSupervisorListForm
                           {
                               tblWorker = e,
                               tblTrade = d,
                               tblShift = f,
                               tblSupervisor = a



                           };
                ViewBag.engDehiringRequest = engDehiringRequest.ToList().Count;



                var engWorkerApproval = from e in workers

                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on e.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     join b in contractors on e.uqContractorCode equals b.uqContractorCode into table6
                                     from b in table6.ToList()
                                     where e.intDehireStatus != 2 && a.uqEngineerCode == new Guid(engineerCode) && e.intJoiningStatusCode == new Guid("D3B967C8-2F02-4937-8A79-29CABD9101D1") && (e.bolIsWorkerApproveByEng == null) && e.Year == dHelper.year()
                                        select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblContractor = b,
                                         tblArea = z

                                     };
                ViewBag.engWorkerApproval = engWorkerApproval.ToList().Count;



                var engClearance = db.procGetClearanceList(dHelper.year()).Where(x => x.uqEngineerId == new Guid(engineerCode) && x.bolIsClearance == true && x.dtClearanceRemarks != null && x.dtEngineerRemarks == null).ToList();
                ViewBag.engClearance = engClearance.ToList().Count;

                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return View();
            }
            else if (hodCode != null)
            {
                ViewBag.hodCode = hodCode;

                var employeeRecord = from e in workers
                                     join n in attendances on e.uqId equals n.uqWorkerCode into table4
                                     from n in table4.ToList()
                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join s in engineers on a.uqEngineerCode equals s.uqEngineerId into table10
                                     from s in table10.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on n.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()
                                     where e.intDehireStatus != 2 && (e.bolIsWorkerApproveByEng == true || e.bolIsWorkerApproveByEng != null)
 && s.uqHODId == new Guid(hodCode) && (n.bolIsVerified == false || n.bolIsVerified == null || n.bolIsVerified == true)
                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,
                                         tblShift = i,
                                         tblSupervisor = a,
                                         tblAttendance = n,
                                         tblArea = z,
                                         tblEngineer = s

                                     };




                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return View();
            }

            else if (contractorCode != null)
            {
                ViewBag.contractorCode = contractorCode;

                var employeeRecord = from e in workers
                                     join n in attendances on e.uqId equals n.uqWorkerCode into table4
                                     from n in table4.ToList()
                                     join a in supervisors on e.uqSupervisorCode equals a.uqSupervisorId into table1
                                     from a in table1.ToList()
                                     join s in engineers on a.uqEngineerCode equals s.uqEngineerId into table10
                                     from s in table10.ToList()
                                     join d in trades on e.uqTradeCode equals d.uqTradeCode into table2
                                     from d in table2.ToList()
                                     join i in shifts on n.uqShiftCode equals i.uqShiftId into table3
                                     from i in table3.ToList()
                                     join z in areas on e.uqAreaCode equals z.uqAreaCode into table5
                                     from z in table5.ToList()

                                         //                   join f in Attendances on e.uqId equals f.uqWorkerCode into table3
                                         //                   from f in table3.ToList()
                                         //                   i.strShiftName == "Day" && i.tmTo < time
                                     where e.uqContractorCode == new Guid(contractorCode) && e.intDehireStatus != 2 && e.bolIsWorkerApproveByEng == true

                                     select new tblSupervisorListForm
                                     {
                                         tblWorker = e,
                                         tblTrade = d,

                                         tblSupervisor = a,

                                         tblArea = z,

                                         tblAttendance = n,
                                         tblEngineer = s,
                                         tblShift = i




                                     };




                //_tblSupervisorListForm.WorkerList = db.tblWorkers.Where(x => x.uqSupervisorCode == new Guid(supervisorCode) && (x.dtLastCheckedIn <= dt_tomarrow.Date ||x.dtLastCheckedIn==null) ).ToList();

                return View();
            }
            else if (supervisorCode == null &&
             engineerCode == null &&
             contractorCode == null &&
             hodCode == null)
            {

                return View();
            }
            return View();
        }

        private void PopulateLatestUser()
        {
            var userInfo = db.tblUsers.Where(s => s.IsDeleted == false).OrderByDescending(s => s.InsertDate).Take(5);
            ViewBag.UserList = userInfo;
        }

        #region Dashboard Chart
        private void PopulateChart()
        {
            string MonthName = "", saleData = "";
            try
            {
                for (int i = 0; i < 12; i++)
                {
                    DateTime reportDate = (DateTime.Now.AddMonths(0 - 11 + i));
                    MonthName += "\"" + ToMonthName(reportDate) + "\",";

                    var Data = db.tblUsers.Where(s =>
                        s.InsertDate.Value.Month == reportDate.Month && s.InsertDate.Value.Year == reportDate.Year);
                    if (Data.Count() > 0)
                        saleData += Data.Count().ToString() + ",";
                    else
                        saleData += "0,";
                }
            }
            catch { }
            ViewBag.MonthName = MonthName.Remove(MonthName.Length - 1);
            ViewBag.Data = saleData.Remove(saleData.Length - 1);

        }
        #endregion

        #region Header
        //Get Header
        public ActionResult GetHeader()
        {
            try
            {
                string Url = Request.RawUrl.ToString();
                var control = dHelper.GetUserPermission(Url);
                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.UserID = dHelper.CurrentLoginUser();
                var setting = db.tblSettings.FirstOrDefault();
                if (setting != null)
                {
                    ViewBag.Name = setting.Name;
                }

                var userID = new Guid(dHelper.CurrentLoginUser());
                var userInfo = db.tblUsers.FirstOrDefault(s => s.ID == userID);
                if (userInfo != null)
                {
                    ViewBag.UserName = userInfo.UserName;
                    ViewBag.PhotoPath = userInfo.PhotoPath;
                    ViewBag.RegisterDate = userInfo.InsertDate.Value.ToString("dd-MMM-yyyy");
                }
            }
            catch
            { }
            return PartialView("_Header");
        }
        #endregion

        public string ToMonthName(DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        #region Sidebar Menu
        public ActionResult SidebarMenu()
        {
            try
            {
                var userID = new Guid(dHelper.CurrentLoginUser());
                var permission = dHelper.GetUserPermission("");
                ViewBag.IsAdmin = permission.IsAdmin;
                var userInfo = db.tblUsers.FirstOrDefault(s => s.ID == userID);
                if (userInfo != null)
                {
                    ViewBag.PhotoPath = userInfo.PhotoPath;
                    ViewBag.UserName = userInfo.UserName;
                    ViewBag.Name = userInfo.FirstName + " " + userInfo.LastName;


                    string str = "";
                    var menuList = dHelper.GetMenuList(userInfo.RoleID, permission.IsAdmin);
                    if (menuList.Count() > 0)
                    {
                        foreach (var menu in menuList)
                        {
                            var subMenu = dHelper.GetSubMenuList(userInfo.RoleID, menu.ID, permission.IsAdmin);
                            if (subMenu.Count() > 0)
                            {
                                str += string.Format(
                                    "<li data-toggle=\"slide\" class=\"slide\"><a class=\"side-menu__item\" href=\"#\"><i class=\"side-menu__icon fa {0}\"></i><span class=\"side-menu__label\">{1}</span><i style=\"color:#5b6e88\" class=\"fa fa-angle-down\" ></i></a></li>",
                                    menu.PageIcon, menu.ModuleName);

                                str += "<ul class=\"slide-menu\">";
                                foreach (var sub in subMenu)
                                    str += string.Format("<li><a class=\"slide-item\"href=\"{0}\">{2}</a></li>",
                                        sub.PageURL, sub.PageIcon, sub.ModuleName);
                                str += "</ul></li>";
                            }
                            else
                            {
                                str +=
                                    string.Format("<li id=\"{3}\" class=\"slide\"><a class=\"side-menu__item\" href=\"{0}\"><i class=\"side-menu__icon fa {1}\"></i><span class=\"side-menu__label\">{2}</span></a></li>",
                                        menu.PageURL, menu.PageIcon, menu.ModuleName, menu.PageSlug);
                            }
                        }
                    }
                    ViewBag.MenuList = str;
                    return PartialView("_Menu", null);
                }
                return PartialView("_Menu", null);
            }
            catch (Exception ex)
            {
                string Error = ex.Message.ToString();
                return PartialView("_Menu", null);
            }
        }
        #endregion
    }
}