//------------------------------------------------------------------------------
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
    
    public partial class AdminAttendanceCorrection_Result
    {
        public string Year { get; set; }
        public System.Guid uqAttendanceCode { get; set; }
        public System.Guid uqId { get; set; }
        public Nullable<System.DateTime> dtCheckIn { get; set; }
        public Nullable<bool> bolIsOvertime { get; set; }
        public Nullable<bool> bolIsVerified { get; set; }
        public Nullable<System.Guid> uqShiftCode { get; set; }
        public Nullable<System.Guid> uqTradeCode { get; set; }
        public string strWorkderId { get; set; }
        public string strName { get; set; }
        public string strSupervisorName { get; set; }
        public string strEngineerName { get; set; }
        public string strContractorName { get; set; }
        public string strAreaName { get; set; }
        public string strDepartmentName { get; set; }
        public string strTradeName { get; set; }
        public string strShiftName { get; set; }
    }
}
