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
    
    public partial class tblContractor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblContractor()
        {
            this.tblUsers = new HashSet<tblUser>();
        }
    
        public System.Guid? uqContractorCode { get; set; }
        public string strContractorName { get; set; }
        public System.DateTime dtCreatedAt { get; set; }
        public Nullable<System.DateTime> dtMoidifyAt { get; set; }
        public System.Guid uqCreatedBy { get; set; }
        public Nullable<System.Guid> uqModifyBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblUser> tblUsers { get; set; }
    }
}
