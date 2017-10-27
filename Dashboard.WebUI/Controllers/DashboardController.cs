using FrogDashboard.Domain.Models.Dashboard;
using FrogDashboard.Domain.MVCExtension;
using FrogDashboard.Domain.ValidationAttributes;
using FrogDashboard.Service.Services.Dashboard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FrogDashboard.WebUI.Controllers
{
    public class DashboardController : Controller
    {
        private DashboardService dashboard = new DashboardService();
        public ActionResult DashboardIndex()
        {
            return View();
        }

        public JsonResult GetDashboardData(GridParam gp)
        {
            
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("UserId", System.Web.HttpContext.Current.User.Identity.Name);
            var data = dashboard.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = totalCount,
                rows = (from c in data
                        select new DashboardInfo()
                        {
                            Guid = c.Guid,
                            ReportName = c.ReportName,
                            ReportSpecName = c.ReportSpecName,
                            ReportType = c.ReportType == "PieChart" ? "Pie" : c.ReportType == "LineChart" ? "Line" : c.ReportType == "Bar" ? "Bar" : "Group Bar",
                            ReportSpec = c.ReportSpec
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除明细数据
        /// </summary>
        /// <param name="gp"></param>
        /// <returns></returns>
        public JsonResult DeleteDashboard(FormCollection collection) 
        {
            string id = collection["vIDs"];
            string[] ids = id.Split(',');
            try
            {
                dashboard.deleteDashboard(ids);
                return Json(new { flg = true });
            }
            catch 
            {
                return Json(new { flg = false });
            }
        }

        /// <summary>
        /// 取得首页所有报表的参数
        /// </summary>
        /// <returns></returns>
        public JsonResult GetReportParam()
        {
            var nodes = new List<object>();
            DataTable dt = dashboard.GetDashboard();
            DataTable dtDetail = dashboard.GetDashboardDetail();
            foreach (DataRow dr in dt.Rows) 
            {
                TreeDescriptor tree = new TreeDescriptor();
                tree.Id = "/" + dr["ReportName"].ToString();
                tree.Children = AddChildrenCategory(dr["Guid"].ToString(), dtDetail, dr["ReportName"].ToString());
                nodes.Add(new { reportName = tree.Id, SpecName = dr["ReportSpecName"].ToString(), url = ConfigurationManager.AppSettings["ReportingServiceURL"], children = tree.Children });
            }
            return Json(nodes);
        }

        private List<object> AddChildrenCategory(string guid, DataTable dtDetail, string reportName)
        {
            var nodes = new List<object>();
            foreach (DataRow dr in dtDetail.Rows) 
            {
                if (guid == dr["Guid"].ToString()) 
                {
                    TreeDescriptor tree = new TreeDescriptor();
                    tree.Id = dr["SqlField"].ToString();
                    if (reportName == "PipelineReport")
                    {
                        if ((dr["SqlField"].ToString() == "InitialStartDateFrom"
                         || dr["SqlField"].ToString() == "InitialStartDateTo"
                         || dr["SqlField"].ToString() == "InitialEndDateFrom"
                         || dr["SqlField"].ToString() == "InitialEndDateTo"
                         || dr["SqlField"].ToString() == "ACTStartDateFrom"
                         || dr["SqlField"].ToString() == "ACTStartDateTo"
                         || dr["SqlField"].ToString() == "ACTEndDateFrom"
                         || dr["SqlField"].ToString() == "ACTEndDateTo") && !string.IsNullOrEmpty(dr["SqlValue"].ToString()))
                        {
                            tree.Text = dr["SqlValue"].ToString().Substring(0, 7);
                        }
                        else 
                        {
                            tree.Text = dr["SqlValue"].ToString();
                        }
                    }
                    else 
                    {
                        tree.Text = dr["SqlValue"].ToString();
                    }
                    
                    nodes.Add(new { SqlField = tree.Id, SqlValue = tree.Text });
                }
            }
            return nodes;
        }
    }
}
