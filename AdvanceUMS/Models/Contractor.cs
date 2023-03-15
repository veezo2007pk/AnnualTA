using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdvanceUMS.Models
{
    public class Contractor
    {
        public Guid uqId { get; set; }
        public string strWorkderId { get; set; }
        public string strName { get; set; }
        public string strSupervisorName { get; set; }
        public string strEngineerName { get; set; }
        public string strAreaName { get; set; }
        public string strDepartmentName { get; set; }
        public string strTradeName { get; set; }
        public string strShiftName { get; set; }
        public DateTime dtCheckIn { get; set; }
        public bool bolIsVerified { get; set; }


      
    }
}