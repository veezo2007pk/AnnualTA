﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AdvanceUMS.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblAccomodation> tblAccomodations { get; set; }
        public virtual DbSet<tblArea> tblAreas { get; set; }
        public virtual DbSet<tblAttendance> tblAttendances { get; set; }
        public virtual DbSet<tblClearance> tblClearances { get; set; }
        public virtual DbSet<tblContractor> tblContractors { get; set; }
        public virtual DbSet<tblContractorUser> tblContractorUsers { get; set; }
        public virtual DbSet<tblCountry> tblCountries { get; set; }
        public virtual DbSet<tblDehireLog> tblDehireLogs { get; set; }
        public virtual DbSet<tblDepartment> tblDepartments { get; set; }
        public virtual DbSet<tblEngineer> tblEngineers { get; set; }
        public virtual DbSet<tblExperience> tblExperiences { get; set; }
        public virtual DbSet<tblHOD> tblHODs { get; set; }
        public virtual DbSet<tblJoiningStat> tblJoiningStats { get; set; }
        public virtual DbSet<tblModule> tblModules { get; set; }
        public virtual DbSet<tblOvertime> tblOvertimes { get; set; }
        public virtual DbSet<tblRelationship> tblRelationships { get; set; }
        public virtual DbSet<tblResidence> tblResidences { get; set; }
        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblRolePermission> tblRolePermissions { get; set; }
        public virtual DbSet<tblSetting> tblSettings { get; set; }
        public virtual DbSet<tblShift> tblShifts { get; set; }
        public virtual DbSet<tblSkill> tblSkills { get; set; }
        public virtual DbSet<tblSupervisor> tblSupervisors { get; set; }
        public virtual DbSet<tblTAPlanning> tblTAPlannings { get; set; }
        public virtual DbSet<tblToDo> tblToDoes { get; set; }
        public virtual DbSet<tblTrade> tblTrades { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblUserLog> tblUserLogs { get; set; }
        public virtual DbSet<tblWorker> tblWorkers { get; set; }
        public virtual DbSet<tblYear> tblYears { get; set; }
    
        public virtual ObjectResult<AdminAttendanceCorrection_Result> AdminAttendanceCorrection()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<AdminAttendanceCorrection_Result>("AdminAttendanceCorrection");
        }
    
        public virtual ObjectResult<AdminOTCorrection_Result> AdminOTCorrection()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<AdminOTCorrection_Result>("AdminOTCorrection");
        }
    
        public virtual ObjectResult<DailyPlanReport_Result> DailyPlanReport(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<DailyPlanReport_Result>("DailyPlanReport", yearParameter);
        }
    
        public virtual ObjectResult<DailyPlanReportForDay_Result> DailyPlanReportForDay(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<DailyPlanReportForDay_Result>("DailyPlanReportForDay", yearParameter);
        }
    
        public virtual ObjectResult<DailyPlanReportForNight_Result> DailyPlanReportForNight(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<DailyPlanReportForNight_Result>("DailyPlanReportForNight", yearParameter);
        }
    
        public virtual ObjectResult<DehireData_Result> DehireData(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<DehireData_Result>("DehireData", yearParameter);
        }
    
        public virtual ObjectResult<FrontCard_Result> FrontCard(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<FrontCard_Result>("FrontCard", yearParameter);
        }
    
        public virtual int insertWorker(Nullable<int> intContractorUserId, string strName, string strFatherName, string strCNIC, string strContact, string strEmergencyNo, Nullable<int> intAge, string strWorkderId, string strAddress, string strTrade, string strQualification, string strSkillLevel, string strExperience, string strTAattended, Nullable<System.DateTime> dtLastTAyear, Nullable<decimal> intTACurrentRate, Nullable<decimal> intTARate, string strSupervisorName, string strAreaEngName, string strAreaAssigned, Nullable<System.DateTime> dtDurationFrom, Nullable<System.DateTime> dtDurationTo, string strResidence, string strContractor, string strDepartment, string strAccomodation, Nullable<System.DateTime> dtTAyear, byte[] vbrPicture)
        {
            var intContractorUserIdParameter = intContractorUserId.HasValue ?
                new ObjectParameter("intContractorUserId", intContractorUserId) :
                new ObjectParameter("intContractorUserId", typeof(int));
    
            var strNameParameter = strName != null ?
                new ObjectParameter("strName", strName) :
                new ObjectParameter("strName", typeof(string));
    
            var strFatherNameParameter = strFatherName != null ?
                new ObjectParameter("strFatherName", strFatherName) :
                new ObjectParameter("strFatherName", typeof(string));
    
            var strCNICParameter = strCNIC != null ?
                new ObjectParameter("strCNIC", strCNIC) :
                new ObjectParameter("strCNIC", typeof(string));
    
            var strContactParameter = strContact != null ?
                new ObjectParameter("strContact", strContact) :
                new ObjectParameter("strContact", typeof(string));
    
            var strEmergencyNoParameter = strEmergencyNo != null ?
                new ObjectParameter("strEmergencyNo", strEmergencyNo) :
                new ObjectParameter("strEmergencyNo", typeof(string));
    
            var intAgeParameter = intAge.HasValue ?
                new ObjectParameter("intAge", intAge) :
                new ObjectParameter("intAge", typeof(int));
    
            var strWorkderIdParameter = strWorkderId != null ?
                new ObjectParameter("strWorkderId", strWorkderId) :
                new ObjectParameter("strWorkderId", typeof(string));
    
            var strAddressParameter = strAddress != null ?
                new ObjectParameter("strAddress", strAddress) :
                new ObjectParameter("strAddress", typeof(string));
    
            var strTradeParameter = strTrade != null ?
                new ObjectParameter("strTrade", strTrade) :
                new ObjectParameter("strTrade", typeof(string));
    
            var strQualificationParameter = strQualification != null ?
                new ObjectParameter("strQualification", strQualification) :
                new ObjectParameter("strQualification", typeof(string));
    
            var strSkillLevelParameter = strSkillLevel != null ?
                new ObjectParameter("strSkillLevel", strSkillLevel) :
                new ObjectParameter("strSkillLevel", typeof(string));
    
            var strExperienceParameter = strExperience != null ?
                new ObjectParameter("strExperience", strExperience) :
                new ObjectParameter("strExperience", typeof(string));
    
            var strTAattendedParameter = strTAattended != null ?
                new ObjectParameter("strTAattended", strTAattended) :
                new ObjectParameter("strTAattended", typeof(string));
    
            var dtLastTAyearParameter = dtLastTAyear.HasValue ?
                new ObjectParameter("dtLastTAyear", dtLastTAyear) :
                new ObjectParameter("dtLastTAyear", typeof(System.DateTime));
    
            var intTACurrentRateParameter = intTACurrentRate.HasValue ?
                new ObjectParameter("intTACurrentRate", intTACurrentRate) :
                new ObjectParameter("intTACurrentRate", typeof(decimal));
    
            var intTARateParameter = intTARate.HasValue ?
                new ObjectParameter("intTARate", intTARate) :
                new ObjectParameter("intTARate", typeof(decimal));
    
            var strSupervisorNameParameter = strSupervisorName != null ?
                new ObjectParameter("strSupervisorName", strSupervisorName) :
                new ObjectParameter("strSupervisorName", typeof(string));
    
            var strAreaEngNameParameter = strAreaEngName != null ?
                new ObjectParameter("strAreaEngName", strAreaEngName) :
                new ObjectParameter("strAreaEngName", typeof(string));
    
            var strAreaAssignedParameter = strAreaAssigned != null ?
                new ObjectParameter("strAreaAssigned", strAreaAssigned) :
                new ObjectParameter("strAreaAssigned", typeof(string));
    
            var dtDurationFromParameter = dtDurationFrom.HasValue ?
                new ObjectParameter("dtDurationFrom", dtDurationFrom) :
                new ObjectParameter("dtDurationFrom", typeof(System.DateTime));
    
            var dtDurationToParameter = dtDurationTo.HasValue ?
                new ObjectParameter("dtDurationTo", dtDurationTo) :
                new ObjectParameter("dtDurationTo", typeof(System.DateTime));
    
            var strResidenceParameter = strResidence != null ?
                new ObjectParameter("strResidence", strResidence) :
                new ObjectParameter("strResidence", typeof(string));
    
            var strContractorParameter = strContractor != null ?
                new ObjectParameter("strContractor", strContractor) :
                new ObjectParameter("strContractor", typeof(string));
    
            var strDepartmentParameter = strDepartment != null ?
                new ObjectParameter("strDepartment", strDepartment) :
                new ObjectParameter("strDepartment", typeof(string));
    
            var strAccomodationParameter = strAccomodation != null ?
                new ObjectParameter("strAccomodation", strAccomodation) :
                new ObjectParameter("strAccomodation", typeof(string));
    
            var dtTAyearParameter = dtTAyear.HasValue ?
                new ObjectParameter("dtTAyear", dtTAyear) :
                new ObjectParameter("dtTAyear", typeof(System.DateTime));
    
            var vbrPictureParameter = vbrPicture != null ?
                new ObjectParameter("vbrPicture", vbrPicture) :
                new ObjectParameter("vbrPicture", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("insertWorker", intContractorUserIdParameter, strNameParameter, strFatherNameParameter, strCNICParameter, strContactParameter, strEmergencyNoParameter, intAgeParameter, strWorkderIdParameter, strAddressParameter, strTradeParameter, strQualificationParameter, strSkillLevelParameter, strExperienceParameter, strTAattendedParameter, dtLastTAyearParameter, intTACurrentRateParameter, intTARateParameter, strSupervisorNameParameter, strAreaEngNameParameter, strAreaAssignedParameter, dtDurationFromParameter, dtDurationToParameter, strResidenceParameter, strContractorParameter, strDepartmentParameter, strAccomodationParameter, dtTAyearParameter, vbrPictureParameter);
        }
    
        public virtual ObjectResult<MainAttendanceReport_Result> MainAttendanceReport(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<MainAttendanceReport_Result>("MainAttendanceReport", yearParameter);
        }
    
        public virtual ObjectResult<ModuleDeleteSortOrder_Result> ModuleDeleteSortOrder(Nullable<int> moduleID, Nullable<int> pModuleID)
        {
            var moduleIDParameter = moduleID.HasValue ?
                new ObjectParameter("ModuleID", moduleID) :
                new ObjectParameter("ModuleID", typeof(int));
    
            var pModuleIDParameter = pModuleID.HasValue ?
                new ObjectParameter("PModuleID", pModuleID) :
                new ObjectParameter("PModuleID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ModuleDeleteSortOrder_Result>("ModuleDeleteSortOrder", moduleIDParameter, pModuleIDParameter);
        }
    
        public virtual int ModuleInsertSortOrder(Nullable<int> moduleID, Nullable<int> pModuleID)
        {
            var moduleIDParameter = moduleID.HasValue ?
                new ObjectParameter("ModuleID", moduleID) :
                new ObjectParameter("ModuleID", typeof(int));
    
            var pModuleIDParameter = pModuleID.HasValue ?
                new ObjectParameter("PModuleID", pModuleID) :
                new ObjectParameter("PModuleID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ModuleInsertSortOrder", moduleIDParameter, pModuleIDParameter);
        }
    
        public virtual ObjectResult<ModuleList_Result> ModuleList(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ModuleList_Result>("ModuleList", iDParameter);
        }
    
        public virtual int ModuleUpdateSortOrder(Nullable<int> oldSortOrder, Nullable<int> newSortOrder, Nullable<int> moduleID, Nullable<int> pModuleID)
        {
            var oldSortOrderParameter = oldSortOrder.HasValue ?
                new ObjectParameter("OldSortOrder", oldSortOrder) :
                new ObjectParameter("OldSortOrder", typeof(int));
    
            var newSortOrderParameter = newSortOrder.HasValue ?
                new ObjectParameter("NewSortOrder", newSortOrder) :
                new ObjectParameter("NewSortOrder", typeof(int));
    
            var moduleIDParameter = moduleID.HasValue ?
                new ObjectParameter("ModuleID", moduleID) :
                new ObjectParameter("ModuleID", typeof(int));
    
            var pModuleIDParameter = pModuleID.HasValue ?
                new ObjectParameter("PModuleID", pModuleID) :
                new ObjectParameter("PModuleID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ModuleUpdateSortOrder", oldSortOrderParameter, newSortOrderParameter, moduleIDParameter, pModuleIDParameter);
        }
    
        public virtual ObjectResult<PlanningData_Result> PlanningData(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("year", year) :
                new ObjectParameter("year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PlanningData_Result>("PlanningData", yearParameter);
        }
    
        public virtual ObjectResult<PlanningReport_Result> PlanningReport(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PlanningReport_Result>("PlanningReport", yearParameter);
        }
    
        public virtual ObjectResult<procGetClearanceList_Result> procGetClearanceList(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<procGetClearanceList_Result>("procGetClearanceList", yearParameter);
        }
    
        public virtual ObjectResult<procGetWorkerList_Result> procGetWorkerList(string year, Nullable<System.Guid> contractorCode, Nullable<System.Guid> supervisorCode, Nullable<System.Guid> engineerCode, Nullable<System.Guid> hod)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            var contractorCodeParameter = contractorCode.HasValue ?
                new ObjectParameter("contractorCode", contractorCode) :
                new ObjectParameter("contractorCode", typeof(System.Guid));
    
            var supervisorCodeParameter = supervisorCode.HasValue ?
                new ObjectParameter("supervisorCode", supervisorCode) :
                new ObjectParameter("supervisorCode", typeof(System.Guid));
    
            var engineerCodeParameter = engineerCode.HasValue ?
                new ObjectParameter("engineerCode", engineerCode) :
                new ObjectParameter("engineerCode", typeof(System.Guid));
    
            var hodParameter = hod.HasValue ?
                new ObjectParameter("hod", hod) :
                new ObjectParameter("hod", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<procGetWorkerList_Result>("procGetWorkerList", yearParameter, contractorCodeParameter, supervisorCodeParameter, engineerCodeParameter, hodParameter);
        }
    
        public virtual ObjectResult<TradeSummary_Result> TradeSummary(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<TradeSummary_Result>("TradeSummary", yearParameter);
        }
    
        public virtual ObjectResult<UserLog_Result> UserLog()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<UserLog_Result>("UserLog");
        }
    
        public virtual ObjectResult<WorkerAttendanceData_Result> WorkerAttendanceData(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WorkerAttendanceData_Result>("WorkerAttendanceData", yearParameter);
        }
    
        public virtual ObjectResult<AttendanceReport_Result> AttendanceReport()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<AttendanceReport_Result>("AttendanceReport");
        }
    
        public virtual ObjectResult<UpdateNigthBackDate_Result> UpdateNigthBackDate(string year)
        {
            var yearParameter = year != null ?
                new ObjectParameter("year", year) :
                new ObjectParameter("year", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<UpdateNigthBackDate_Result>("UpdateNigthBackDate", yearParameter);
        }
    
        public virtual ObjectResult<AttendanceForEngineer_Result> AttendanceForEngineer(Nullable<System.Guid> engineerCode)
        {
            var engineerCodeParameter = engineerCode.HasValue ?
                new ObjectParameter("engineerCode", engineerCode) :
                new ObjectParameter("engineerCode", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<AttendanceForEngineer_Result>("AttendanceForEngineer", engineerCodeParameter);
        }
    
        public virtual ObjectResult<AttendanceForSupervisor_Result> AttendanceForSupervisor(Nullable<System.Guid> supervisorCode)
        {
            var supervisorCodeParameter = supervisorCode.HasValue ?
                new ObjectParameter("supervisorCode", supervisorCode) :
                new ObjectParameter("supervisorCode", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<AttendanceForSupervisor_Result>("AttendanceForSupervisor", supervisorCodeParameter);
        }
    
        public virtual ObjectResult<OTForEngineer_Result> OTForEngineer(Nullable<System.Guid> engineerCode)
        {
            var engineerCodeParameter = engineerCode.HasValue ?
                new ObjectParameter("engineerCode", engineerCode) :
                new ObjectParameter("engineerCode", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<OTForEngineer_Result>("OTForEngineer", engineerCodeParameter);
        }
    
        public virtual ObjectResult<OTForSupervisor_Result> OTForSupervisor(Nullable<System.Guid> supervisorCode)
        {
            var supervisorCodeParameter = supervisorCode.HasValue ?
                new ObjectParameter("supervisorCode", supervisorCode) :
                new ObjectParameter("supervisorCode", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<OTForSupervisor_Result>("OTForSupervisor", supervisorCodeParameter);
        }
    
        public virtual ObjectResult<WorkerAttendanceDataForEng_Result> WorkerAttendanceDataForEng(string year, Nullable<System.Guid> engineerCode)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            var engineerCodeParameter = engineerCode.HasValue ?
                new ObjectParameter("engineerCode", engineerCode) :
                new ObjectParameter("engineerCode", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WorkerAttendanceDataForEng_Result>("WorkerAttendanceDataForEng", yearParameter, engineerCodeParameter);
        }
    
        public virtual ObjectResult<WorkerAttendanceDataForSupervisor_Result> WorkerAttendanceDataForSupervisor(string year, Nullable<System.Guid> supervisorCode)
        {
            var yearParameter = year != null ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(string));
    
            var supervisorCodeParameter = supervisorCode.HasValue ?
                new ObjectParameter("supervisorCode", supervisorCode) :
                new ObjectParameter("supervisorCode", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WorkerAttendanceDataForSupervisor_Result>("WorkerAttendanceDataForSupervisor", yearParameter, supervisorCodeParameter);
        }
    }
}
