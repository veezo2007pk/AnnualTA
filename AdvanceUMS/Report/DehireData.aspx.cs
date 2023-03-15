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
    public partial class DehireData : System.Web.UI.Page
    {
       
            protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string searchText = string.Empty;
                string value = string.Empty;
                string year = string.Empty;
                if (Request.QueryString["searchText"] != null)
                {
                    searchText = Request.QueryString["searchText"].ToString();
                }
                if (Request.QueryString["value"] != null)
                {
                    value = Request.QueryString["value"].ToString();
                }
                if (Request.QueryString["year"] != null)
                {
                    year = Request.QueryString["year"].ToString();
                }

                using (var _context = new Entities())
                {
                    if (value == "engineer")
                    {
                        var summary = _context.DehireData(year).Where(x => x.strEngineerName == searchText).ToList();
                        CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/DehireData.rdlc");
                        CustomerListReportViewer.LocalReport.DataSources.Clear();
                        ReportDataSource rdc = new ReportDataSource("DataSet1", summary);
                        CustomerListReportViewer.LocalReport.DataSources.Add(rdc);
                        CustomerListReportViewer.ShowReportBody = true;
                        CustomerListReportViewer.ShowPromptAreaButton = true;

                        CustomerListReportViewer.LocalReport.Refresh();
                        CustomerListReportViewer.DataBind();
                    }
                    else if(value == "supervisor")
                    {
                        var summary = _context.DehireData(year).Where(x => x.strSupervisorName == searchText).ToList();
                        CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/DehireData.rdlc");
                        CustomerListReportViewer.LocalReport.DataSources.Clear();
                        ReportDataSource rdc = new ReportDataSource("DataSet1", summary);
                        CustomerListReportViewer.LocalReport.DataSources.Add(rdc);
                        CustomerListReportViewer.ShowReportBody = true;
                        CustomerListReportViewer.ShowPromptAreaButton = true;

                        CustomerListReportViewer.LocalReport.Refresh();
                        CustomerListReportViewer.DataBind();

                    }
                    else if (value == "contractor")
                    {
                        var summary = _context.DehireData(year).Where(x => x.strContractorName == searchText).ToList();
                        CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/DehireData.rdlc");
                        CustomerListReportViewer.LocalReport.DataSources.Clear();
                        ReportDataSource rdc = new ReportDataSource("DataSet1", summary);
                        CustomerListReportViewer.LocalReport.DataSources.Add(rdc);
                        CustomerListReportViewer.ShowReportBody = true;
                        CustomerListReportViewer.ShowPromptAreaButton = true;

                        CustomerListReportViewer.LocalReport.Refresh();
                        CustomerListReportViewer.DataBind();

                    }
                    else
                    {
                        var summary = _context.DehireData(year).ToList();
                        CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/DehireData.rdlc");
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

        protected void CustomerListReportViewer_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {

            string searchText = string.Empty;
            string value = string.Empty;
            string year = string.Empty;
            if (Request.QueryString["searchText"] != null)
            {
                searchText = Request.QueryString["searchText"].ToString();
            }
            if (Request.QueryString["value"] != null)
            {
                value = Request.QueryString["value"].ToString();
            }
            if (Request.QueryString["year"] != null)
            {
                year = Request.QueryString["year"].ToString();
            }

            using (var _context = new Entities())
            {
                if (value == "engineer")
                {
                    var summary = _context.DehireData(year).Where(x => x.strEngineerName == searchText).ToList();
                    CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/DehireData.rdlc");
                    CustomerListReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DataSet1", summary);
                    CustomerListReportViewer.LocalReport.DataSources.Add(rdc);
                    CustomerListReportViewer.ShowReportBody = true;
                    CustomerListReportViewer.ShowPromptAreaButton = true;

                    CustomerListReportViewer.LocalReport.Refresh();
                    CustomerListReportViewer.DataBind();
                }
                else if (value == "supervisor")
                {
                    var summary = _context.DehireData(year).Where(x => x.strSupervisorName == searchText).ToList();
                    CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/DehireData.rdlc");
                    CustomerListReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DataSet1", summary);
                    CustomerListReportViewer.LocalReport.DataSources.Add(rdc);
                    CustomerListReportViewer.ShowReportBody = true;
                    CustomerListReportViewer.ShowPromptAreaButton = true;

                    CustomerListReportViewer.LocalReport.Refresh();
                    CustomerListReportViewer.DataBind();

                }
                else if (value == "contractor")
                {
                    var summary = _context.DehireData(year).Where(x => x.strContractorName == searchText).ToList();
                    CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/DehireData.rdlc");
                    CustomerListReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DataSet1", summary);
                    CustomerListReportViewer.LocalReport.DataSources.Add(rdc);
                    CustomerListReportViewer.ShowReportBody = true;
                    CustomerListReportViewer.ShowPromptAreaButton = true;

                    CustomerListReportViewer.LocalReport.Refresh();
                    CustomerListReportViewer.DataBind();

                }
                else
                {
                    var summary = _context.DehireData(year).ToList();
                    CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/DehireData.rdlc");
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
}