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
    using System.Collections.Generic;
    
    public partial class tblRolePermission
    {
        public int ID { get; set; }
        public Nullable<int> ModuleID { get; set; }
        public Nullable<System.Guid> RoleID { get; set; }
        public Nullable<bool> IsView { get; set; }
        public Nullable<bool> IsAdd { get; set; }
        public Nullable<bool> IsEdit { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual tblModule tblModule { get; set; }
        public virtual tblRole tblRole { get; set; }
    }
}
