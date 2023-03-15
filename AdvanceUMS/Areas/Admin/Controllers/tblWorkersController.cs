using AdvanceUMS.Helper;
using AdvanceUMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class tblWorkersController : Controller
    {
        string supervisorCode;
        string engineerCode;
        string hod;
        string contractorCode;
        Entities db = new Entities();
        GeneralHelper dHelper = new GeneralHelper();
        public byte[] imgdata { get; set; }

        Dictionary<string, object> res = new Dictionary<string, object>();
        public string test { get; set; }
        [HttpGet]
        public JsonResult GetWorkerId(string ID)
        {
            var showPiece = db.tblWorkers
                       .OrderByDescending(p => p.strWorkderId).Where(x => x.uqContractorCode == new Guid(ID))
                       .FirstOrDefault();
            ViewBag.strWorkerId = Convert.ToInt32(showPiece.strWorkderId) + 1;
            return Json(ViewBag.strWorkerId, JsonRequestBehavior.AllowGet);
        }
        // GET: Admin/User
        [HttpGet]
        public ActionResult Index(string year)
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            //if (control.IsLogin == false)
            //    return Redirect("/account/login");
            //if (control.IsView == false)
            //    return Redirect("/admin/notaccess");

            ViewBag.IsAdd = control.IsAdd;
            ViewBag.errorshift = TempData["errorshift"];
            if (ViewBag.errorshift == "errorshift")
            {
                Session["shift"] = "errorshift";
            }
            else
            {
                Session["shift"] = null;

            }
            ViewBag.year = year;
            //string Url = Request.RawUrl.ToString();
            //var control = dHelper.GetUserPermission(Url);
            //if (control.IsLogin == false)
            //    return Redirect("/account/login");
            //if (control.IsView == false)
            //    return Redirect("/admin/notaccess");

            //ViewBag.IsAdd = control.IsAdd;
            //if (Session["Add"] != null)
            //    ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully insert user.</div>";
            //else if (Session["Update"] != null)
            //    ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully update user.</div>";
            //Session.RemoveAll();


            return View();
        }

        public ActionResult Index()
        {
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            //if (control.IsLogin == false)
            //    return Redirect("/account/login");
            //if (control.IsView == false)
            //    return Redirect("/admin/notaccess");

            ViewBag.IsAdd = control.IsAdd;
            //if (Session["Add"] != null)
            //    ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully insert user.</div>";
            //else if (Session["Update"] != null)
            //    ViewBag.Message = "<div class=\"alert alert-success fade in\">successfully update user.</div>";
            //else if (Session["shift"] == null)
            //    ViewBag.message = "<div class='alert alert-danger text-center col-12' role='alert'>Worker already exist</div>";
            //Session.RemoveAll();

            return View();
        }

        #region Populate UserList
        public ActionResult PopulateWorkers(string Status)
        {
            PopulateDropdown();
            if (Status == null)
            {
                Status = dHelper.year();
            }
            var control = dHelper.GetUserPermission("");
            ViewBag.IsEdit = control.IsEdit;
            ViewBag.IsDelete = control.IsDelete;
            List<tblWorker> workers = db.tblWorkers.ToList();
            List<tblDepartment> departments = db.tblDepartments.ToList();
            List<tblShift> shifts = db.tblShifts.ToList();
            List<tblAttendance> Attendances = db.tblAttendances.ToList();
            List<tblTrade> trades = db.tblTrades.ToList();
            List<tblSupervisor> supervisors = db.tblSupervisors.ToList();
            List<tblArea> areas = db.tblAreas.ToList();
            List<tblEngineer> engineers = db.tblEngineers.ToList();
            List<tblJoiningStat> joiningStats = db.tblJoiningStats.ToList();
            if (Session["shift"] == "errorshift")
                ViewBag.shiftmessage = "<div class=\"alert alert-danger text-center col-12\">Worker already exist</div>";
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
                hod = new Guid(dHelper.CurrentHODLoginUser()).ToString();

            }
            if (contractorCode != null)
            {
                ViewBag.contractor = contractorCode;
                var list = db.procGetWorkerList(Status, new Guid(contractorCode), null, null, null).ToList();


                return PartialView("_List", list);
            }
            else if (supervisorCode != null)
            {
                ViewBag.supervisor = supervisorCode;
                var list = db.procGetWorkerList(Status, null, new Guid(supervisorCode), null, null).ToList();



                return PartialView("_List", list);
            }
            else if (engineerCode != null)
            {
                ViewBag.engineer = engineerCode;

                var list = db.procGetWorkerList(Status, null, null, new Guid(engineerCode), null).ToList();



                return PartialView("_List", list);
            }
            else if (hod != null)
            {
                ViewBag.hod = hod;

                var list = db.procGetWorkerList(Status, null, null, null, new Guid(hod)).ToList();

                return PartialView("_List", list);
            }

            else
            {
                var list = db.procGetWorkerList(Status, null, null, null, null).ToList();



                return PartialView("_List", list);
            }
        }
        #endregion

        #region Create & Update User
        public ActionResult Create()
        {

            string workercontractorCode;
            PopulateDropdown();
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            if (control.IsLogin == false)
                return Redirect("/account/login");
            if (control.IsView == false)
                return Redirect("/admin/notaccess");

            ViewBag.IsAdd = control.IsAdd;
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.add = "add";

            if (dHelper.CurrentContractorUser() != "")
            {
                contractorCode = new Guid(dHelper.CurrentContractorUser()).ToString();
                var showPiece = db.tblWorkers
                        .OrderByDescending(p => p.strWorkderId).Where(x => x.uqContractorCode == new Guid(contractorCode))
                        .FirstOrDefault();
                ViewBag.strWorkerId = Convert.ToInt32(showPiece.strWorkderId) + 1;
                ViewBag.strWorkerIdd = Convert.ToString(ViewBag.strWorkerId);


                WorkerViewModel workerView = new WorkerViewModel();
                workerView.uqContractorCode = new Guid(contractorCode);

                return View(workerView);



            }
            else
            {
                ViewBag.strWorkerIdd = "0";
                WorkerViewModel workerView = new WorkerViewModel();
                return View(workerView);
            }


        }

        public void PopulateDropdown()
        {
            if (dHelper.CurrentContractorUser() != "")
            {
                contractorCode = new Guid(dHelper.CurrentContractorUser()).ToString();
                var contractor = db.tblContractors.Where(x => x.uqContractorCode == new Guid(contractorCode));
                ViewBag.ContractorList = new SelectList(contractor, "uqContractorCode", "strContractorName");
            }
            else
            {
                var contractor = db.tblContractors.OrderBy(x => x.strContractorName);
                ViewBag.ContractorList = new SelectList(contractor, "uqContractorCode", "strContractorName");
            }

            var joiningStat = db.tblJoiningStats.OrderBy(s => s.strJoiningStatusCode);
            ViewBag.joiningStatList = new SelectList(joiningStat, "uqJoiningStatusCode", "strJoiningStatusCode");


            List<tblTrade> tradeList = db.tblTrades.ToList();
            ViewBag.tradeList = tradeList;
            //var trade = db.tblTrades.OrderBy(s => s.strTradeName);
            //ViewBag.tradeList = new SelectList(trade, "uqTradeCode", "strTradeName");

            var skill = db.tblSkills.OrderBy(s => s.strSkillName);
            ViewBag.skillList = new SelectList(skill, "uqSkillCode", "strSkillName");

            var experience = db.tblExperiences.OrderBy(s => s.strExperienceName);
            ViewBag.experienceList = new SelectList(experience, "uqExperienceCode", "strExperienceName");

            var area = db.tblAreas.OrderBy(s => s.strAreaName);
            ViewBag.areaList = new SelectList(area, "uqAreaCode", "strAreaName");

            var residence = db.tblResidences.OrderBy(s => s.strResidenceName);
            ViewBag.residenceList = new SelectList(residence, "uqResidenceCode", "strResidenceName");

            var department = db.tblDepartments.OrderBy(s => s.strDepartmentName);
            ViewBag.departmentList = new SelectList(department, "uqDepartmentCode", "strDepartmentName");


            var accommodation = db.tblAccomodations.OrderBy(s => s.strAccommodationName);
            ViewBag.accommodationList = new SelectList(accommodation, "uqAccommodationCode", "strAccommodationName");

            var supervisor = db.tblSupervisors.OrderBy(s => s.strSupervisorName);
            ViewBag.supervisorList = new SelectList(supervisor, "uqSupervisorId", "strSupervisorName");

            var shift = db.tblShifts.OrderBy(s => s.strShiftName);
            ViewBag.shiftList = new SelectList(shift, "uqShiftId", "strShiftName");

            var relationship = db.tblRelationships.OrderBy(s => s.strRelationship);
            ViewBag.relationshipList = new SelectList(relationship, "uqRelationshipCode", "strRelationship");

            var year = db.tblYears.OrderBy(s => s.intYearCode);
            ViewBag.yearList = new SelectList(year, "intYearCode", "strYearName");


            //var country = db.tblCountries.Where(s => s.IsDeleted == false).OrderBy(s => s.Name);
            //ViewBag.CountryList = new SelectList(country, "ID", "Name");

            //var role = db.tblRoles.Where(s => s.IsDeleted == false).OrderBy(s => s.Name);
            //ViewBag.RoleList = new SelectList(role, "ID", "Name");

            //List<SelectListItem> listGender = new List<SelectListItem>();
            //listGender.Add(new SelectListItem { Text = "Male", Value = "Male", Selected = true });
            //listGender.Add(new SelectListItem { Text = "FeMale", Value = "FeMale" });
            //ViewBag.GenderList = new SelectList(listGender, "Value", "Text");
        }

        public JsonResult AddUser(WorkerViewModel objWorker, HttpPostedFileBase Photo)
        {

            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);

            string workercontractorCode;
            PopulateDropdown();
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);


            ViewBag.IsAdd = control.IsAdd;
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.add = "add";
            string y = dHelper.year();
            try
            {
                tblWorker Worker = new tblWorker();
                string desc = "";
                if (objWorker.uqId == null)
                {
                    var duplicate = db.tblWorkers.Where(x => x.strCNIC == objWorker.strCNIC && x.Year==y).FirstOrDefault();
                    if (duplicate != null)
                    {
                        ViewBag.shiftmessage = "<div class=\"alert alert-danger text-center col-12\">Worker already exist</div>";
                        res["status"] = "3";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                    var duplicateworkerid = db.tblWorkers.Where(x => x.strWorkderId == objWorker.strWorkderId && x.Year == y).FirstOrDefault();
                    if (duplicateworkerid != null)
                    {
                        ViewBag.shiftmessage = "<div class=\"alert alert-danger text-center col-12\">Worker already exist</div>";
                        res["status"] = "3";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["imgdata"] != null)
                    {
                        //imgdata = ConvertHexToBytes(Session["imgdata"].ToString());
                        Worker.vbrPicture = (byte[])Session["imgdata"];
                    }
                   
                        int number = Convert.ToInt32(objWorker.strWorkderId);
                        number += 1;

                        objWorker.strWorkderId = number.ToString();
                    

                    Worker.uqId = Guid.NewGuid();
                    Worker.Year = dHelper.year();
                    Worker.uqContractorUserId = new Guid(dHelper.CurrentLoginUser());
                    Worker.strName = objWorker.strName;
                    Worker.strFatherName = objWorker.strFatherName;
                    Worker.strCNIC = objWorker.strCNIC;
                    Worker.strContact = objWorker.strContact;
                    Worker.strEmergencyNo = objWorker.strEmergencyNo;
                    Worker.intAge = objWorker.intAge;
                    Worker.strWorkderId = objWorker.strWorkderId;
                    Worker.strAddress = objWorker.strAddress;
                    Worker.uqTradeCode = objWorker.uqTradeCode;
                    Worker.strQualification = objWorker.strQualification;
                    Worker.uqSkillCode = objWorker.uqSkillCode;
                    Worker.uqExperienceCode = objWorker.uqExperienceCode;
                    Worker.strTAattended = objWorker.strTAattended;
                    Worker.dtLastTAyear = objWorker.dtLastTAyear;
                    Worker.intTARate = objWorker.intTARate;
                    Worker.uqSupervisorCode = objWorker.uqSupervisorCode;
                    Worker.uqShiftCode = objWorker.uqShiftCode;
                    Worker.intDehireStatus = 0;
                    Worker.uqAreaCode = objWorker.uqAreaCode;
                    Worker.dtDurationFrom = objWorker.dtDurationFrom;
                    Worker.dtDurationTo = objWorker.dtDurationTo;
                    Worker.uqResidenceCode = objWorker.uqResidenceCode;
                    if (ViewBag.IsAdmin == true)
                    {
                        Worker.uqContractorCode = objWorker.uqContractorCode;

                    }
                    else
                    {
                        Worker.uqContractorCode = new Guid(contractorCode);
                    }
                    Worker.uqDepartmentCode = objWorker.uqDepartmentCode;
                    Worker.intJoiningStatusCode = objWorker.intJoiningStatusCode;
                    Worker.strNextOfKin = objWorker.strNextOfKin;
                    Worker.strNextOfKinContactNo = objWorker.strNextOfKinContactNo;
                    Worker.uqRelationshipCode = objWorker.uqRelationshipCode;

                    Worker.uqAccommodationCode = objWorker.uqAccommodationCode;
                    //Worker.dtTAyear = Convert.ToDateTime(dHelper.year() + "-01-01 12:00");

                    Worker.intTACurrentRate = objWorker.intTACurrentRate;
                    Worker.dtCreatedAt = dateTime;
                    Worker.uqCreatedBy = new Guid(dHelper.CurrentLoginUser());
                    Worker.uqDepartmentCode = objWorker.uqDepartmentCode;


                    db.tblWorkers.Add(Worker);
                    desc = ("Insert Worker : " + objWorker.strName);
                    Session["Add"] = "add";
                }
                else
                {
                    var author = db.tblWorkers.Where(a => a.uqId == objWorker.uqId && a.Year == y).FirstOrDefault();
                    // Debug.WriteLine("Image session"+Session["imgdata"].ToString());

                    if (Session["imgdata"] != null)
                    {
                        //imgdata = ConvertHexToBytes(Session["imgdata"].ToString());
                        author.vbrPicture = (byte[])Session["imgdata"];
                    }

                    author.uqId = objWorker.uqId;
                    author.uqContractorUserId = new Guid(dHelper.CurrentLoginUser());
                    author.strName = objWorker.strName;
                    author.strFatherName = objWorker.strFatherName;
                    author.strCNIC = objWorker.strCNIC;
                    author.strContact = objWorker.strContact;
                    author.strEmergencyNo = objWorker.strEmergencyNo;
                    author.intAge = objWorker.intAge;
                    author.strWorkderId = objWorker.strWorkderId;
                    author.strAddress = objWorker.strAddress;
                    author.uqTradeCode = objWorker.uqTradeCode;
                    author.strQualification = objWorker.strQualification;
                    author.uqSkillCode = objWorker.uqSkillCode;
                    author.strNextOfKin = objWorker.strNextOfKin;
                    author.strNextOfKinContactNo = objWorker.strNextOfKinContactNo;
                    author.uqRelationshipCode = objWorker.uqRelationshipCode;

                    author.uqExperienceCode = objWorker.uqExperienceCode;
                    author.strTAattended = objWorker.strTAattended;
                    author.dtLastTAyear = objWorker.dtLastTAyear;
                    author.intTARate = objWorker.intTARate;
                    author.uqSupervisorCode = objWorker.uqSupervisorCode;

                    author.uqShiftCode = objWorker.uqShiftCode;
                    author.uqAreaCode = objWorker.uqAreaCode;
                    author.dtDurationFrom = objWorker.dtDurationFrom;
                    author.dtDurationTo = objWorker.dtDurationTo;
                    author.uqResidenceCode = objWorker.uqResidenceCode;
                    if (ViewBag.IsAdmin == true)
                    {
                        author.uqContractorCode = objWorker.uqContractorCode;

                    }
                    else
                    {
                        author.uqContractorCode = new Guid(contractorCode);
                    }
                    author.uqDepartmentCode = objWorker.uqDepartmentCode;

                    author.intJoiningStatusCode = objWorker.intJoiningStatusCode;
                    author.uqAccommodationCode = objWorker.uqAccommodationCode;
                    author.dtTAyear = objWorker.dtTAyear;
                    //Worker.vbrPicture = objWorker.vbrPicture;
                    author.intTACurrentRate = objWorker.intTACurrentRate;
                    author.dtCreatedAt = objWorker.dtCreatedAt;
                    author.uqDepartmentCode = objWorker.uqDepartmentCode;
                    author.uqModifyBy = new Guid(dHelper.CurrentLoginUser());
                    author.dtMoidifyAt = objWorker.dtMoidifyAt;
                    if (objWorker.intJoiningStatusCode == new Guid("D3B967C8-2F02-4937-8A79-29CABD9101D1") && author.bolIsWorkerApproveByEng == false)
                    {
                        author.bolIsWorkerApproveByEng = null;
                    }
                    db.Entry(author).State = EntityState.Modified;
                    desc = ("Update Worker : " + objWorker.strName);
                    Session["Update"] = "update";
                }
                db.SaveChanges();
                dHelper.LogAction(desc);
                res["status"] = "1";
                //Response.Redirect("/admin/user");
                res["url"] = "/Admin/tblWorkers";

            }
            catch (Exception ex)
            {

                res["status"] = "0";
                throw ex;
            }
            Session["imgdata"] = null;
            return Json(res, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Shift(string ID, string url)
        {

            ViewBag.message = "";
            ViewBag.strWorkerIdd = "0";
            PopulateDropdown();
            Guid WorkerID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            //if (control.IsEdit == false)
            //{
            //    if (dHelper.CurrentSupervisorLoginUser() != "")
            //    {
            //        if (dHelper.Decrypt(ID) != dHelper.CurrentSupervisorLoginUser())
            //            return Redirect("/admin/notaccess");
            //    }
            //    else if (dHelper.CurrentLoginUser() != "")
            //    {
            //        if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
            //            return Redirect("/admin/notaccess");
            //    }
            //    else if (dHelper.CurrentEngineerLoginUser() != "")
            //    {
            //        if (dHelper.Decrypt(ID) != dHelper.CurrentEngineerLoginUser())
            //            return Redirect("/admin/notaccess");
            //    }
            //    else if (dHelper.CurrentContractorUser() != "")
            //    {
            //        if (dHelper.Decrypt(ID) != dHelper.CurrentContractorUser())
            //            return Redirect("/admin/notaccess");
            //    }
            //}
            ViewBag.IsAdd = control.IsAdd;
            ViewBag.IsAdmin = control.IsAdmin;
            string desc = "";
            var WorkerInfo = db.tblWorkers.Where(s => s.uqId == WorkerID).FirstOrDefault();
            if (WorkerInfo != null)
            {
                string y = dHelper.year();
                var d = db.tblWorkers.Where(x => x.strCNIC == WorkerInfo.strCNIC && x.Year == y).FirstOrDefault();
                if (d != null)
                {
                    TempData["errorshift"] = "errorshift";
                    return Redirect(url);

                }
                var e = db.tblWorkers.Where(x => x.strWorkderId == WorkerInfo.strWorkderId && x.Year == y).FirstOrDefault();
                if (e != null)
                {
                    TempData["errorshift"] = "errorshift";
                    return Redirect(url);
                }
                else
                {
                    tblWorker objWorker = new tblWorker
                    {

                        uqId = Guid.NewGuid(),

                        Year = dHelper.year(),


                        uqContractorUserId = WorkerInfo.uqContractorUserId,
                        strName = WorkerInfo.strName,
                        strFatherName = WorkerInfo.strFatherName,
                        strCNIC = WorkerInfo.strCNIC,
                        strContact = WorkerInfo.strContact,
                        strEmergencyNo = WorkerInfo.strEmergencyNo,
                        intAge = WorkerInfo.intAge,
                        strWorkderId = WorkerInfo.strWorkderId,
                        strAddress = WorkerInfo.strAddress,
                        uqTradeCode = WorkerInfo.uqTradeCode,
                        strQualification = WorkerInfo.strQualification,
                        uqSkillCode = WorkerInfo.uqSkillCode,
                        uqExperienceCode = WorkerInfo.uqExperienceCode,
                        strTAattended = WorkerInfo.strTAattended,
                        dtLastTAyear = WorkerInfo.dtLastTAyear,
                        intTARate = WorkerInfo.intTARate,
                        uqSupervisorCode = WorkerInfo.uqSupervisorCode,
                        intJoiningStatusCode = new Guid("AB0AC9F1-1F95-403F-8322-DAE4CA038521"),
                        uqShiftCode = WorkerInfo.uqShiftCode,
                        uqAreaCode = WorkerInfo.uqAreaCode,
                        dtDurationFrom = WorkerInfo.dtDurationFrom,
                        dtDurationTo = WorkerInfo.dtDurationTo,
                        uqResidenceCode = WorkerInfo.uqResidenceCode,
                        uqContractorCode = WorkerInfo.uqContractorCode,
                        uqDepartmentCode = WorkerInfo.uqDepartmentCode,
                        strNextOfKin = WorkerInfo.strNextOfKin,
                        strNextOfKinContactNo = WorkerInfo.strNextOfKinContactNo,
                        uqRelationshipCode = WorkerInfo.uqRelationshipCode,


                        uqAccommodationCode = WorkerInfo.uqAccommodationCode,
                        //dtTAyear = Convert.ToDateTime(dHelper.year() + "-01-01 12:00"),
                        vbrPicture = WorkerInfo.vbrPicture,
                        intTACurrentRate = WorkerInfo.intTACurrentRate,
                        dtCreatedAt = WorkerInfo.dtCreatedAt,
                        uqCreatedBy = WorkerInfo.uqCreatedBy,

                        uqModifyBy = new Guid(dHelper.CurrentLoginUser()),
                        dtMoidifyAt = WorkerInfo.dtMoidifyAt,

                    };


                    db.tblWorkers.Add(objWorker);
                    desc = ("Insert Worker : " + objWorker.strName);
                    Session["Add"] = "add";
                    db.SaveChanges();
                    dHelper.LogAction(desc);
                    return Redirect(url);

                }


            }
            else
            {
                return null;

            }

        }

        public ActionResult Edit(string ID)
        {
            ViewBag.strWorkerIdd = "0";
            PopulateDropdown();
            Guid WorkerID = new Guid(ID);
            string Url = Request.RawUrl.ToString();
            var control = dHelper.GetUserPermission(Url);
            //if (control.IsEdit == false)
            //{
            //    if (dHelper.CurrentSupervisorLoginUser() != "")
            //    {
            //        if (dHelper.Decrypt(ID) != dHelper.CurrentSupervisorLoginUser())
            //            return Redirect("/admin/notaccess");
            //    }s
            //    else if (dHelper.CurrentLoginUser() != "")
            //    {
            //        if (dHelper.Decrypt(ID) != dHelper.CurrentLoginUser())
            //            return Redirect("/admin/notaccess");
            //    }
            //    else if (dHelper.CurrentEngineerLoginUser() != "")
            //    {
            //        if (dHelper.Decrypt(ID) != dHelper.CurrentEngineerLoginUser())
            //            return Redirect("/admin/notaccess");
            //    }
            //    else if (dHelper.CurrentContractorUser() != "")
            //    {
            //        if (dHelper.Decrypt(ID) != dHelper.CurrentContractorUser())
            //            return Redirect("/admin/notaccess");
            //    }
            //}
            ViewBag.IsAdd = control.IsAdd;
            ViewBag.IsAdmin = control.IsAdmin;

            var WorkerInfo = db.tblWorkers.Where(s => s.uqId == WorkerID).FirstOrDefault();
            if (WorkerInfo != null)
            {
                WorkerViewModel objWorker = new WorkerViewModel
                {

                    uqId = WorkerInfo.uqId,



                    uqContractorUserId = WorkerInfo.uqContractorUserId,
                    strName = WorkerInfo.strName,
                    strFatherName = WorkerInfo.strFatherName,
                    strCNIC = WorkerInfo.strCNIC,
                    strContact = WorkerInfo.strContact,
                    strEmergencyNo = WorkerInfo.strEmergencyNo,
                    intAge = WorkerInfo.intAge,
                    strWorkderId = WorkerInfo.strWorkderId,
                    strAddress = WorkerInfo.strAddress,
                    uqTradeCode = WorkerInfo.uqTradeCode,
                    strQualification = WorkerInfo.strQualification,
                    uqSkillCode = WorkerInfo.uqSkillCode,
                    uqExperienceCode = WorkerInfo.uqExperienceCode,
                    strTAattended = WorkerInfo.strTAattended,
                    dtLastTAyear = WorkerInfo.dtLastTAyear,
                    intTARate = WorkerInfo.intTARate,
                    uqSupervisorCode = WorkerInfo.uqSupervisorCode,
                    intJoiningStatusCode = WorkerInfo.intJoiningStatusCode,
                    uqShiftCode = WorkerInfo.uqShiftCode,
                    uqAreaCode = WorkerInfo.uqAreaCode,
                    dtDurationFrom = WorkerInfo.dtDurationFrom,
                    dtDurationTo = WorkerInfo.dtDurationTo,
                    uqResidenceCode = WorkerInfo.uqResidenceCode,
                    uqContractorCode = WorkerInfo.uqContractorCode,
                    uqDepartmentCode = WorkerInfo.uqDepartmentCode,
                    strNextOfKin = WorkerInfo.strNextOfKin,
                    strNextOfKinContactNo = WorkerInfo.strNextOfKinContactNo,
                    uqRelationshipCode = WorkerInfo.uqRelationshipCode,


                    uqAccommodationCode = WorkerInfo.uqAccommodationCode,
                    dtTAyear = WorkerInfo.dtTAyear,
                    vbrPicture = WorkerInfo.vbrPicture,
                    intTACurrentRate = WorkerInfo.intTACurrentRate,
                    dtCreatedAt = WorkerInfo.dtCreatedAt,
                    uqCreatedBy = WorkerInfo.uqCreatedBy,

                    uqModifyBy = new Guid(dHelper.CurrentLoginUser()),
                    dtMoidifyAt = WorkerInfo.dtMoidifyAt,
                };

                ViewBag.Edit = "1";
                return View(objWorker);
            }
            else
            {
                return null;

            }

        }
        #endregion

        #region Delete User
        public JsonResult Delete(Guid ID)
        {
            var WorkerInfo = db.tblWorkers.FirstOrDefault(s => s.uqId == ID);
            if (WorkerInfo != null)
            {


                tblWorker tblWorker = db.tblWorkers.Find(WorkerInfo.uqAreaCode);
                db.tblWorkers.Remove(tblWorker);
                db.SaveChanges();

                string Description = string.Format("Delete Worker [WorkerInfo: {0}]", WorkerInfo.strName);
                dHelper.LogAction(Description);
                res["success"] = 0;
                res["message"] = "<div class=\"alert alert-success fade in\">successfully delete " + WorkerInfo.strName + ".</div>";
                string Query = Request.UrlReferrer.Query;
                if (Query == "")
                    res["url"] = "/admin/tblWorkers/PopulateWorkers";
                else
                    res["url"] = "/admin/tblWorkers/PopulateWorkers" + Query;
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
        public ActionResult Capture(string data)
        {
            //StreamReader reader = new StreamReader(Request.InputStream);
            //string hexString = Server.UrlEncode(reader.ReadToEnd());
            //Session["imgdata"] = hexString;


            //string imageName = DateTime.Now.ToString("dd-MM-yy hh-mm-ss");
            //string imagePath = string.Format("~/Captures/{0}.png", imageName);
            //System.IO.File.WriteAllBytes(Server.MapPath(imagePath), ConvertHexToBytes(hexString));
            //var data = VirtualPathUtility.ToAbsolute(imagePath);
            //var result = data;
            //Session["test"]= result;f
            //return null;

            string fileName = DateTime.Now.ToString("dd-MM-yy hh-mm-ss");

            //Convert Base64 Encoded string to Byte Array.
            byte[] imageBytes = Convert.FromBase64String(data.Split(',')[1]);
            Session["imgdata"] = imageBytes;

            //Save the Byte Array as Image File.
            string filePath = string.Format("~/Captures/{0}.png", fileName);
            System.IO.File.WriteAllBytes(Server.MapPath(filePath), imageBytes);
            return Content("Hi there!");
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