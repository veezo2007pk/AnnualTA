using AdvanceUMS.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AdvanceUMS.Report
{
    public partial class WorkAttendanceReport : System.Web.UI.Page
    {
       
            protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string searchText = string.Empty;

                if (Request.QueryString["year"] != null)
                {
                    searchText = Request.QueryString["year"].ToString();
                }

               
                using (var _context = new Entities())
                {
                    var summary = _context.WorkerAttendanceData(searchText).ToList();
                    CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/WorkerAttendance.rdlc");
                    CustomerListReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DataSet1", summary);
                    CustomerListReportViewer.LocalReport.DataSources.Add(rdc);
                    CustomerListReportViewer.ShowReportBody = true;
                    CustomerListReportViewer.ShowPromptAreaButton = true;
                    
                    CustomerListReportViewer.LocalReport.Refresh();
                    CustomerListReportViewer.DataBind();
                }
            }
        }

        protected void CustomerListReportViewer_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
                string searchText = string.Empty;

                if (Request.QueryString["year"] != null)
                {
                    searchText = Request.QueryString["year"].ToString();
                }


                using (var _context = new Entities())
                {
                    var summary = _context.WorkerAttendanceData(searchText).ToList();
                    CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/WorkerAttendance.rdlc");
                    CustomerListReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DataSet1", summary);
                    CustomerListReportViewer.LocalReport.DataSources.Add(rdc);
                    CustomerListReportViewer.ShowReportBody = true;
                    CustomerListReportViewer.ShowPromptAreaButton = true;

                    CustomerListReportViewer.LocalReport.Refresh();
                    CustomerListReportViewer.DataBind();
                }
            

        }
    }
}