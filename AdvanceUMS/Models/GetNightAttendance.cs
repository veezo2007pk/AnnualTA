using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AdvanceUMS.Models
{
    public class GetNightAttendance
    {
        public List<tblAttendance> NightAttendance(string year)
        {
            List<tblAttendance> lst = new List<tblAttendance>();
            tblAttendance rptPO = new tblAttendance();
            using (SqlConnection con = new SqlConnection(ConnectionString.cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("UpdateNigthBackDate", con);
                
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@year", year);
              
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {

                    if (rdr["uqAttendanceCode"] == DBNull.Value)
                    {
                        rptPO.uqAttendanceCode = null;
                    }

                    else
                    {
                        rptPO.uqAttendanceCode =new Guid( rdr["uqAttendanceCode"].ToString());
                    }

                   

                    lst.Add(new tblAttendance
                    {
                        uqAttendanceCode = rptPO.uqAttendanceCode
                       






                    });
                }
                return lst;
            }
        }

    }
}