using AdvanceUMS.Helper;
using AdvanceUMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class ReportController : Controller
    {
        string engineerCode=null;
        string engineerName = null;
        string supervisorCode=null;
        string supervisorName = null;
        string contractorCode;
        string contractorName;
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();

        // GET: Admin/Report
        [HttpGet]
        public ActionResult Summary(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return View();
        }
        [HttpGet]
        public ActionResult MainAttendanceReport(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return View();
        }
        [HttpGet]
        public ActionResult WorkAttendanceReport(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return View();
        }
        [HttpGet]
        public ActionResult DailyPlanReport(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return View();
        }
        [HttpGet]
        public ActionResult AttendanceReportForEngineer(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            if (dHelper.CurrentEngineerLoginUser() != "")
            {
                engineerCode = new Guid(dHelper.CurrentEngineerLoginUser()).ToString();
            }
           
                ViewBag.engineercode = engineerCode;
            return View();
        }

    
[HttpGet]
public ActionResult DehireData(string year)
{
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            if (dHelper.CurrentEngineerLoginUser() != "")
            {
                engineerCode = new Guid(dHelper.CurrentEngineerLoginUser()).ToString();
                engineerName = dHelper.GerEngineerName(new Guid(engineerCode)).ToString();
                ViewBag.engineercode = engineerName;
                ViewBag.engineer = "engineer";
            }
            else if (dHelper.CurrentSupervisorLoginUser() != "")
            {
                supervisorCode = new Guid(dHelper.CurrentSupervisorLoginUser()).ToString();
                supervisorName = dHelper.GerSupervisorName(new Guid(supervisorCode)).ToString();
                ViewBag.supervisorCode = supervisorName;
                ViewBag.supervisor = "supervisor";
            }
            else if (dHelper.CurrentContractorUser() != "")
            {
                contractorCode = new Guid(dHelper.CurrentContractorUser()).ToString();

                contractorName = dHelper.GerContractorName(new Guid(contractorCode)).ToString();
                ViewBag.contractorCode = contractorName;
                ViewBag.contractor = "contractor";
            }


            return View();
        }
[HttpGet]
        public ActionResult OTReportForEngineer(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            if (dHelper.CurrentEngineerLoginUser() != "")
            {
                engineerCode = new Guid(dHelper.CurrentEngineerLoginUser()).ToString();
            }

            ViewBag.engineercode = engineerCode;
            return View();
        }
        [HttpGet]
        public ActionResult AttendanceReportForSupervisor(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            if (dHelper.CurrentSupervisorLoginUser() != "")
            {
                supervisorCode = new Guid(dHelper.CurrentSupervisorLoginUser()).ToString();
            }
            ViewBag.supervisorCode = supervisorCode;
            return View();
        }
        [HttpGet]
        public ActionResult OTReportForSupervisor(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            if (dHelper.CurrentSupervisorLoginUser() != "")
            {
                supervisorCode = new Guid(dHelper.CurrentSupervisorLoginUser()).ToString();
            }
            ViewBag.supervisorCode = supervisorCode;
            return View();
        }
        [HttpGet]
        public ActionResult DailyPlanReportForDay(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return View();
        }
        [HttpGet]
        public ActionResult DailyPlanReportForNight(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return View();
        }
        [HttpGet]
        public ActionResult BarChart(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return new CustomJsonResult { Data = new { Lists = db.DailyPlanReport(year).ToList() } };
        }
        [HttpGet]
        public ActionResult BarChartForDay(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return new CustomJsonResult { Data = new { Lists = db.DailyPlanReportForDay(dHelper.year()).ToList() } };
        }
        [HttpGet]
        public ActionResult BarChartForNight(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;

            return new CustomJsonResult { Data = new { Lists = db.DailyPlanReportForNight(dHelper.year()).ToList() } };
        }


        [HttpGet]
        public ActionResult PlanningReport(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return View();
        }
        [HttpGet]
        public ActionResult IRCOBackCard(string year)
        {
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            PopulateDropdown();
            contractorCode = new Guid(dHelper.CurrentContractorUser()).ToString();
         
            if (contractorCode == "116831d6-ec3e-4794-b88e-5b36e94e2ca0")
            {
                return View("ALLIEDBackCard");

            }
            else
            {
                return View("IRCOBackCard");

            }
           
        }
        [HttpGet]

        public ActionResult AttendanceReport(string year)
        {
            PopulateDropdown();
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            return View();
        }
        [HttpGet]
        public ActionResult FrontCard(string year)
        {
            if (year == null)
            {
                year = dHelper.year();
            }
            ViewBag.year = year;
            PopulateDropdown();
            contractorCode = new Guid(dHelper.CurrentContractorUser()).ToString();
            if (contractorCode == "116831d6-ec3e-4794-b88e-5b36e94e2ca0")
            {
                return View("FrontCard");

            }
            else
            {
                return View("IrcoCard");

            }
        }
        public void PopulateDropdown()
        {
             if (dHelper.CurrentContractorUser() != "")
            {
                contractorCode = new Guid(dHelper.CurrentContractorUser()).ToString();

            }
            if (contractorCode != null)
            {
                List<tblWorker> workers = db.tblWorkers.Where(x => x.uqContractorCode == new Guid(contractorCode)).ToList();
                var workerList = (from e in workers
                                  where e.Year == dHelper.year() && e.uqContractorCode==new Guid(contractorCode)

                                  select new SelectListItem
                                  {
                                      Text = e.strCNIC,
                                      Value = e.strCNIC.ToString()
                                  });
                ViewBag.workerList = new SelectList(workerList, "Value", "Text");

            }


            

            var year = db.tblYears.OrderBy(s => s.intYearCode);
            ViewBag.yearList = new SelectList(year, "intYearCode", "strYearName");



        }

    }
}