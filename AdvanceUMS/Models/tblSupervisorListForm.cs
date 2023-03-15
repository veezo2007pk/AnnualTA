using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdvanceUMS.Models
{
    public class tblSupervisorListForm
    {
        public List<string> ids { get; set; }
        public List<string> hours { get; set; }
        public Contractor contractor { get; set; }
        public tblOvertime tblOvertime { get; set; }
        public tblSupervisor supervisor { get; set; }
        public tblArea tblArea { get; set; }
        public tblHOD   tblHOD { get; set; }

        public tblDepartment  tblDepartment { get; set; }
        public tblEngineer tblEngineer { get; set; }
        public tblJoiningStat   tblJoiningStat { get; set; }
        public List<tblSupervisor> supervisorList { get; set; }
        public AdminAttendanceCorrection_Result adminAttendanceCorrection_Results { get; set; }
        public List<tblWorker> WorkerList { get; set; }
        public tblWorker tblWorker { get; set; }
        public tblTrade tblTrade { get; set; }
        public tblShift tblShift { get; set; }
        public tblContractor tblContractor { get; set; }
        public tblSupervisor tblSupervisor { get; set; }

        public tblAttendance tblAttendance { get; set; }
        public List< tblAttendance> tblAttendanceList { get; set; }
        public List<tblTrade> tblTradeList { get; set; }
        public List<tblSupervisor> tblSupervisorList { get; set; }
    }
}