using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dashboard.WebUI.Models;
using Dashboard.Domain.SysManagement;
using Dashboard.Core.DBTransaction;
using Dashboard.Core.MVCAttributes;
using Dashboard.Authentication;
using Dashboard.Interface.MasterData.SysManagement;
using System.Reflection;
using Dashboard.Domain.MVCExtension;



namespace Dashboard.WebUI.Controllers
{
    [CustomHandleError()]
    [Dashboard.Authentication.MvcAttribute.CustomAuthorize()]
    public class HomeController : Controller
    {

        // public SysUserDBAccess _userAccess = new SysUserDBAccess();

        public ActionResult Index()
        {
            string domainAndName = System.Web.HttpContext.Current.User.Identity.Name;

            var userRoles = Authentication.Authentication.GetUserRoles();
            ViewBag.UserRoles = userRoles;

            ViewBag.SysRole = Authentication.Authentication.CurrentUserRole;
            string s = "Dashboard.Service.Services.MasterData.SysManagement.SysFunctionService";
            ISysFunction<SysFunction> service_ = (ISysFunction<SysFunction>)Assembly.Load("Dashboard.Service").CreateInstance(s);
            var userRoleId = Authentication.Authentication.CurrentUser.RoleId;
            var str = Request.Browser.Browser;
            if (str == "Safari" && service_.GetUserFunction(userRoleId).Where(c => c.ControllerName == "OrderManagement" && c.ActionName == "TmsForPad").Count() > 0)
                return Redirect("/OrderManagement/TmsOrderForPad");
            return View();
        }

        public ActionResult SetRole(string sysRoleId)
        {
            Authentication.Authentication.SetCurrentUserRole(sysRoleId);
            return RedirectToAction("Index");
        }

        public JsonResult SaveUser(SysUser user, FormCollection fs)
        {

            //if (string.IsNullOrEmpty(user.UserName))
            //    return Json(new { Success = false, Msg = "用户名不能为空。" }, JsonRequestBehavior.AllowGet);

            //if (fs["createFlag"] == "true")
            //{
            //    var tmpUser = _userAccess.GetEntity(user.UserName);
            //    if (tmpUser != null)
            //    {
            //        return Json(new { Success = false, Msg = "存在重复用户名。" }, JsonRequestBehavior.AllowGet);
            //    }
            //}
            //else
            //{
            //    user = _userAccess.GetEntity(user.UserName);
            //}

            //TryUpdateModel<SysUser>(user);
            //user.IsValid = fs["IsValid"] != null;
            //bool success = _userAccess.SaveSysUser(user, fs["SysRoleId"]);
            //return Json(new { Success = success, Msg = "操作成功" }, JsonRequestBehavior.AllowGet);
            return null;
        }

        public JsonResult GetSysUsers(DataTableParameter param)
        {
            ////int gridIndex = 0;
            //int pageSize = param.iDisplayLength;
            //int pageIndex = param.iDisplayStart;
            //int totalRecords, totalPages;

            //string userName = Request.QueryString["userName"];
            //bool isValid = Request.QueryString["isValid"] == "true" ? true : false;

            //var dataList = from d in _userAccess.GetUsers(pageSize, out totalPages, pageIndex, out totalRecords, userName, isValid)
            //               select new
            //               {
            //                   ID = 1,
            //                   d.UserName,
            //                   Isvalid = d.IsValid ? "是" : "否",
            //                   d.Remark,
            //               };




            //var dataSource = new List<object>();
            //foreach (var data in dataList)
            //{
            //    dataSource.Add(new string[]{data.ID.ToString(),data.UserName.ToString(),data.Isvalid, 
            //            data.Remark});
            //}

            //return Json(new
            //{
            //    sEcho = param.sEcho,
            //    iTotalRecords = totalRecords,
            //    iTotalDisplayRecords = totalRecords,
            //    aaData = dataSource
            //}, JsonRequestBehavior.AllowGet);
            return null;
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult SetLang(string strLang)
        {
            Response.Cookies["Lang"].Value = strLang;
            Response.Cookies["Lang"].Expires = DateTime.Now.AddDays(365);
            //Session["Lang"] = strLang;
            return RedirectToAction("Index");
        }
    }
}
