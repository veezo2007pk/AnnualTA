using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
namespace AdvanceUMS.Models
{
    public static class ConnectionString
    {
        //declare connection string  
        public static string cs = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

    }
}