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
    
    public partial class UpdateNigthBackDate_Result
    {
        public System.Guid uqAttendanceCode { get; set; }
        public Nullable<System.Guid> uqWorkerCode { get; set; }
        public Nullable<System.DateTime> dtCheckIn { get; set; }
        public Nullable<System.DateTime> dtCheckOut { get; set; }
        public Nullable<bool> bolIsOvertime { get; set; }
        public Nullable<bool> bolIsVerified { get; set; }
        public Nullable<System.Guid> uqShiftCode { get; set; }
        public Nullable<System.Guid> uqTradeCode { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }
        public string Year { get; set; }
        public Nullable<bool> bolisAttendanceShiftToBackDate { get; set; }
    }
}
