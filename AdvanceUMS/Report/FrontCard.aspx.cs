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
    public partial class FrontCard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string searchText = string.Empty;
                string year = string.Empty;
                if (Request.QueryString["searchText"] != null)
                {
                    searchText = Request.QueryString["searchText"].ToString();
                }
                if (Request.QueryString["year"] != null)
                {
                    year = Request.QueryString["year"].ToString();
                }

                using (var _context = new Entities())
                {
                    var summary = _context.FrontCard(year).Where(x=>x.strCNIC== searchText).ToList();
                    CustomerListReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/RDLC/FrontCard.rdlc");
                    CustomerListReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DataSet1", summary);
                    CustomerListReportViewer.LocalReport.DataSources.Add(rdc);
                    CustomerListReportViewer.LocalReport.Refresh();
                    CustomerListReportViewer.DataBind();
                }
            }
        }
    }
}