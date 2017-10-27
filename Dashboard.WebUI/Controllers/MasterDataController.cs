using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FrogDashboard.Authentication.MvcAttribute;
using FrogDashboard.AutoInput.AutoInput;
using FrogDashboard.Core.Enumeration;
using FrogDashboard.Core.MVCAttributes;
using FrogDashboard.Core.Utility;
using FrogDashboard.Domain.CIM;
using FrogDashboard.Domain.MasterData;
using FrogDashboard.Domain.MVCExtension;
using FrogDashboard.Domain.SysManagement;
using FrogDashboard.Interface.BaseService;
using FrogDashboard.Interface.CIM;
using FrogDashboard.Interface.MasterData.SysManagement;
using FrogDashboard.Service.Services.CIM;
using FrogDashboard.Service.Services.MasterData;
using FrogDashboard.Service.Services.MasterData.SysManagement;
using FrogDashboard.Domain.ValidationAttributes;

namespace FrogDashboard.WebUI.Controllers
{
    [CustomHandleError()]
    [CustomAuthorize()]
    public class MasterDataController : Controller
    {
        #region 实例化定义
        private ISysUser<SysUser> _sysUserService = new SysUserService();
        private ISysUserRole<SysUserRole> _sysUserRoleService = new SysUserRoleService();
        private ISysDataAuth<SysDataAuth> _sysDataAuthService = new SysDataAuthService();
        private ISysRole<SysRole> _sysRoleService = new SysRoleService();
        private ISysFunction<SysFunction> _sysFunctionService = new SysFunctionService();
        private ISysRoleFunction<SysRoleFunction> _sysRoleFunctionService = new SysRoleFunctionService();
        private ISysOrganization<SysOrganization> _SysOrgService = new SysOrgService();


        private ICimCust<CCimCust> _CimCustDBAccess = new CCimCustService();
        private IBaseService<CBasCity> _BasCityDBAccess = new CBasCityService();
        private IBaseService<CBasProvince> _BasProvinceDBAccess = new CBasProvinceService();
        private IBaseService<CBasCodeDef> _BasCodeDefDBAccessService = new CSbCodeDefService();

        //数据字典
        private IBaseService<CBasCodeType> _sysDictionaryDBAccess = new CBasCodeTypeService();
        //数据字典明细
        private IBaseService<CBasCodeDef> _sysCodeDefService = new CSbCodeDefService();

        #endregion

        #region SysFunction

        #region SysFunction
        public ActionResult SysFunction()
        {
            ViewBag.GroupItem = CUtility.ConvertEnumToDictionary(typeof(FunctionGroupType));
            ViewBag.FunctionItem = CUtility.ConvertEnumToDictionary(typeof(FuntionType));
            return View();
        }

        public JsonResult GetParentByGroupType(int groupType)
        {
            var list = _sysFunctionService.GetList().Where(s => s.FunctionType == 1 && s.Active == true && s.GroupType == groupType).ToList();
            var obj = from m in list.ToList()
                      select new
                      {
                          SysFunctionId = m.SysFunctionId,
                          FunctionName = m.FunctionName
                      };

            return Json(new { Obj = obj }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetSysFunctionData
        /// <summary>
        /// 省份数据数邦定
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="UserCode"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public JsonResult GetSysFunctionData(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("FunctionName", Request.QueryString["FunctionName"]);
            dic.Add("ControllerName", Request.QueryString["ControllerName"]);
            dic.Add("FunctionType", Request.QueryString["FunctionType"]);
            dic.Add("GroupType", Request.QueryString["GroupType"]);
            var data = _sysFunctionService.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        let gpTypeName = GetGroupName(c.GroupType)
                        select new
                        {
                            cell = new string[] { 
                                c.SysFunctionId,
                                c.FunctionName,
                                c.ActionName,
                                c.ControllerName,
                                c.AreaName,
                                c.FunctionType.ToString(),
                                c.FunctionType==0?"<=Function>":"<=Page>",
                                c.GroupType.ToString(),
                                gpTypeName,
                                c.Active?"<=Yes>":"<=No>",
                                c.Sortid.ToString(),
                                c.Remarks,
                                c.ParentId,
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        private string GetGroupName(int iGroupType)
        {
            string sGroupName = "";
            var dic = CUtility.ConvertEnumToDictionary(typeof(FunctionGroupType));
            foreach (KeyValuePair<int, string> gpType in dic)
            {
                if (gpType.Key == iGroupType)
                {
                    sGroupName = "<=" + gpType.Value + ">";
                    break;
                }
            }
            return sGroupName;
        }
        #endregion

        #region SaveSysFunction
        /// <summary>
        /// 保存省信息
        /// </summary>
        /// <param name="SysUser"></param>
        /// <returns></returns>
        public JsonResult SaveSysFunction(SysFunction sysFunction)
        {
            string str = "<=SaveSuccess>";
            if (!ModelState.IsValid)
            {
                List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                str = string.Join(",", errorList.ToArray());
                return Json(new { Success = false, Msg = str }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string errMsg;
                SysFunction tmpFunction = null;
                if (!string.IsNullOrEmpty(sysFunction.SysFunctionId))
                {
                    tmpFunction = _sysFunctionService.Get(sysFunction.SysFunctionId);
                    TryUpdateModel<SysFunction>(tmpFunction);
                }
                else
                {
                    tmpFunction = sysFunction;
                    TryUpdateModel<SysFunction>(tmpFunction);
                }

                bool Success = _sysFunctionService.Save(tmpFunction, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DeleteSysFunction
        /// <summary>
        /// 删除省信息
        /// </summary>
        /// <param name="SysUserId"></param>
        /// <returns></returns>
        public JsonResult DeleteSysFunction(string SysFunctionId)
        {
            bool success = _sysFunctionService.Delete(SysFunctionId);

            return Json(new { Success = success, Msg = success ? "<=DeleteSuccess>" : "<=DeleteFail>" },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region SysUser

        #region SysUser
        public ActionResult SysUser()
        {
            ViewBag.LanguageItem = CUtility.ConvertEnumToDictionary(typeof(Language));
            ViewBag.teamItem = (from c in _sysCodeDefService.GetList()
                                where c.CBasCodeType.CodeType == Const.Consts.SysTeam && c.Active == true

                                select c).ToList();        

            return View();
        }

        public JsonResult GetRoleList(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                var list = _sysRoleService.GetList().ToList();
                var obj = from m in list.ToList()
                          select new
                          {
                              SysRoleId = m.SysRoleId,
                              RoleName = m.RoleName,
                              SysUserId = ""
                          };

                return Json(new { Obj = obj }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = _sysUserRoleService.GetList().Where(s => s.SysUserId == userId).ToList();
                var obj = from m in list.ToList()
                          select new
                          {
                              SysRoleId = m.SysRoleId,
                              RoleName = m.SysRole.RoleName,
                              SysUserId = m.SysUserId
                          };

                return Json(new { Obj = obj }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetSysUserData
        /// <summary>
        /// 省份数据数邦定
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="UserCode"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public JsonResult GetSysUserData(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("UserCode", Request.QueryString["UserCode"]);
            dic.Add("UserName", Request.QueryString["UserName"]);
            dic.Add("Active", Request.QueryString["Active"] == null ? "1" : Request.QueryString["Active"]);
            var data = _sysUserService.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
                                c.SysUserId,
                                c.UserCode,                                
                                c.Pwd,
                                c.RoleId,
                                c.SysRole.RoleName,
                                c.UserName,
                                c.Email,
                                c.PassCode,
                                c.ValidDateFrom.ToString("yyyy-MM-dd"),
                                c.ValidDateTo.ToString("yyyy-MM-dd"),
                                c.LanguageType==0?"<=English>":"<=SimplifiedChinese>",
                                c.Active?"<=Yes>":"<=No>",
                                c.Remarks,
                                c.Team,
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PassWordIsValid
        /// <summary>
        /// 密码校验：密码必须包含大写字母、小写字母、数字、特殊字符且密码长度大于等于8
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool PassWordIsValid(object value)
        {
            Regex lowCaseReg = new Regex(@"[a-z]+");
            bool lowCase = lowCaseReg.IsMatch(value.ToString());

            Regex upperCaseReg = new Regex(@"[A-Z]+");
            bool upperCase = upperCaseReg.IsMatch(value.ToString());

            Regex numberReg = new Regex(@"[0-9]+");
            bool number = numberReg.IsMatch(value.ToString());

            Regex specialCaseReg = new Regex(@"\W+|_");
            bool specialCase = specialCaseReg.IsMatch(value.ToString());

            return lowCase && upperCase && specialCase && number && value.ToString().Length >= 8;
        }
        #endregion

        #region SaveSysUser
        /// <summary>
        /// 保存省信息
        /// </summary>
        /// <param name="SysUser"></param>
        /// <returns></returns>
        public JsonResult SaveSysUser(SysUser sysUser)
        {
            string str = "<=SaveSuccess>";
            if (!ModelState.IsValid)
            {
                List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                str = string.Join(",", errorList.ToArray());
                return Json(new { Success = false, Msg = str }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string errMsg;
                SysUser tmpUser = null;
                if (!string.IsNullOrEmpty(sysUser.SysUserId))
                {
                    tmpUser = _sysUserService.Get(sysUser.SysUserId);
                    TryUpdateModel<SysUser>(tmpUser);
                }
                else
                {
                    tmpUser = sysUser;
                    TryUpdateModel<SysUser>(tmpUser);
                }
                var sysUserRole = _sysUserRoleService.GetList().Where(o => o.SysUserId == sysUser.SysUserId).Select(o => o.SysRoleId).Distinct().ToList();
                var sysRole = _sysRoleService.GetList().Where(o => sysUserRole.Contains(o.SysRoleId) || o.SysRoleId == sysUser.RoleId).ToList();
                if (sysRole.Count() > 0 && (sysRole.Count() != 1 || sysRole.Where(o => o.RoleCode.Contains("Operator")).Count() <= 0))
                {

                    if (!this.PassWordIsValid(sysUser.Pwd))
                    {
                        return Json(new { Success = false, Msg = "<=PasswordError>" }, JsonRequestBehavior.AllowGet);
                    }
                    //if (!this.PassWordIsValid("Lxy1314*"))
                    //{
                    //    return Json(new { Success = false, Msg = "<=PasswordError>" }, JsonRequestBehavior.AllowGet);
                    //}
                }
                bool success = _sysUserService.Save(tmpUser, out errMsg);
                string userId = success ? tmpUser.SysUserId : string.Empty;
                return Json(new { Success = success, Msg = success ? str : errMsg, SysUserId = userId }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DeleteSysUser
        /// <summary>
        /// 删除省信息
        /// </summary>
        /// <param name="SysUserId"></param>
        /// <returns></returns>
        public JsonResult DeleteSysUser(string SysUserId)
        {
            bool success = _sysUserService.Delete(SysUserId);

            return Json(new { Success = success, Msg = success ? "<=DeleteSuccess>" : "<=DeleteFail>" },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetSysRoleByUser
        /// <summary>
        /// 省份数据数邦定
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="SysRoleId"></param>
        /// <returns></returns>
        public JsonResult GetSysRoleByUser(GridParam gp)
        {
            int pageCount, totalCount;
            string userId = (Request.QueryString["sysUserId"] == "" ? " " : Request.QueryString["sysUserId"]);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("SysUserId", userId);
            var data = _sysUserRoleService.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] { 
                                c.SysUserRoleId,
                                c.SysUserId,
                                c.SysRoleId,                                
                                c.SysRole.RoleName,
                                //c.SysRole.SysOrganization.OrganizationName,
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetSysUserDataByRole
        /// <summary>
        /// 省份数据数邦定
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="UserCode"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public JsonResult GetSysUserDataByRole(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("UserCode", Request.QueryString["UserCode"]);
            dic.Add("UserName", Request.QueryString["UserName"]);
            dic.Add("Active", "1");
            var data = _sysUserService.GetList(gp, dic, out pageCount, out totalCount).ToList();

            int gridIndex = 0;
            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] { 
                                (++gridIndex).ToString(),
                                c.SysUserId,
                                c.UserCode,                                
                                c.UserName,
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region SysRole

        #region SysRole
        public ActionResult SysRole()
        {
            if (Request.Cookies["Lang"] == null)
                ViewBag.Lang = "English";
            else
                ViewBag.Lang = Request.Cookies["Lang"].Value;

            return View();
        }
        #endregion

        #region GetSysRoleData
        /// <summary>
        /// 省份数据数邦定
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="RoleName"></param>
        /// <param name="SysOrganizationId"></param>
        /// <returns></returns>
        public JsonResult GetSysRoleData(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("RoleCode", (Request.QueryString["RoleCode"] == null ? "" : Request.QueryString["RoleCode"]));
            dic.Add("RoleName", (Request.QueryString["RoleName"] == null ? "" : Request.QueryString["RoleName"]));
            //dic.Add("OrganizationName", (Request.QueryString["OrganizationName"] == null ? "" : Request.QueryString["OrganizationName"]));
            //dic.Add("SysOrganizationId", (Request.QueryString["SysOrganizationId"] == null ? "" : Request.QueryString["SysOrganizationId"]));
            var data = _sysRoleService.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
                                c.SysRoleId,
                                c.RoleCode,
                                c.RoleName,
                                //c.SysOrganizationId,
                                //c.SysOrganization.OrganizationName,
                                c.Remarks,
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        
        #endregion

        #region SysRoleDetail

        #region SysRole

        #region SysRoleDetail
        public ActionResult SysRoleDetail()
        {
            SysRole sysRole = new SysRole();
            string sysRoleId = Request.QueryString["sysRoleId"];
            if (!string.IsNullOrEmpty(sysRoleId))
            {
                sysRole = _sysRoleService.Get(sysRoleId);
            }
            return View(sysRole);
        }
        #endregion

        #region SaveSysRole
        /// <summary>
        /// 保存省信息
        /// </summary>
        /// <param name="SysRole"></param>
        /// <returns></returns>
        public JsonResult SaveSysRole(SysRole sysRole)
        {
            string str = "<=SaveSuccess>";
            if (!ModelState.IsValid)
            {
                List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                str = string.Join(",", errorList.ToArray());
                return Json(new { Success = false, Msg = str }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string errMsg;
                string isInOrg = "True";
                SysRole tmpRole = null;
                if (!string.IsNullOrEmpty(sysRole.SysRoleId))
                {
                    tmpRole = _sysRoleService.Get(sysRole.SysRoleId);
                    TryUpdateModel<SysRole>(tmpRole);
                }
                else
                {
                    tmpRole = sysRole;
                    TryUpdateModel<SysRole>(tmpRole);
                }

                bool success = _sysRoleService.Save(tmpRole, out errMsg);
                string roleId = success ? tmpRole.SysRoleId : string.Empty;
                //if (!string.IsNullOrEmpty(roleId))
                //{
                //    SysOrganization org = _SysOrgService.Get(tmpRole.SysOrganizationId);
                //    isInOrg = org.IsInterior.ToString();
                //}

                return Json(new { Success = success, Msg = success ? str : errMsg, RoleId = roleId, IsInOrg = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DeleteSysRole
        /// <summary>
        /// 删除省信息
        /// </summary>
        /// <param name="SysRoleId"></param>
        /// <returns></returns>
        public JsonResult DeleteSysRole(string SysRoleId)
        {
            bool success = _sysRoleService.Delete(SysRoleId);
            success = _sysUserRoleService.DeleteByRoleId(SysRoleId);
            success = _sysRoleFunctionService.DeleteByRoleId(SysRoleId);

            return Json(new { Success = success, Msg = success ? "<=DeleteSuccess>" : "<=DeleteFail>" },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult InitOrgTree(string root, int depth)
        {
            bool isInterior = true;
            string sysRoleId = Request.QueryString["sysRoleId"];
            if (!string.IsNullOrEmpty(sysRoleId))
            {
                SysRole role = _sysRoleService.Get(sysRoleId);
                if (role != null && role.SysOrganization != null)
                {
                    isInterior = role.SysOrganization.IsInterior;
                }
            }

            List<SysOrganization> list = null;
            if (isInterior)
                list = _SysOrgService.GetList().Where(s => s.ParentId == root).ToList();
            else
                list = _SysOrgService.GetList().Where(s => s.ParentId == root && s.IsInterior == false).ToList();


            var obj = from l in list.ToList()
                      let hasChild = HasChild(l.SysOrganizationId)
                      select new
                      {
                          text = l.OrganizationName,
                          hasChildren = hasChild,
                          id = l.SysOrganizationId,
                          expanded = false, //!hasCHild,
                          depth = depth + 1
                      };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region SysUserRole

        #region GetSysUserRoleData
        /// <summary>
        /// 省份数据数邦定
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="SysRoleId"></param>
        /// <returns></returns>
        public JsonResult GetSysUserRoleData(GridParam gp)
        {
            int pageCount, totalCount;
            string roleId = (Request.QueryString["sysRoleId"] == "" ? " " : Request.QueryString["sysRoleId"]);
            string userId = (Request.QueryString["sysUserId"] == "" ? " " : Request.QueryString["sysUserId"]);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("SysRoleId", roleId);
            dic.Add("SysUserId", userId);
            var data = _sysUserRoleService.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
                                c.SysUserRoleId,
                                c.SysRoleId,
                                c.SysUserId,
                                c.SysUser.UserCode,
                                c.SysUser.UserName,
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveSysUserRole
        /// <summary>
        /// 保存省信息
        /// </summary>
        /// <param name="SysUserRole"></param>
        /// <returns></returns>
        public JsonResult SaveSysUserRole(SysUserRole sysUserRole)
        {
            string str = "<=SaveSuccess>";
            if (!ModelState.IsValid)
            {
                List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                str = string.Join(",", errorList.ToArray());
                return Json(new { Success = false, Msg = str }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string errMsg;
                SysUserRole tmpUserRole = null;
                if (!string.IsNullOrEmpty(sysUserRole.SysUserRoleId))
                {
                    tmpUserRole = _sysUserRoleService.Get(sysUserRole.SysUserRoleId);
                    TryUpdateModel<SysUserRole>(tmpUserRole);
                }
                else
                {
                    tmpUserRole = sysUserRole;
                    TryUpdateModel<SysUserRole>(tmpUserRole);
                }

                bool Success = _sysUserRoleService.Save(tmpUserRole, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DeleteSysRole
        /// <summary>
        /// 删除省信息
        /// </summary>
        /// <param name="SysUserRoleId"></param>
        /// <returns></returns>
        public JsonResult DeleteSysUserRole(string SysUserRoleId)
        {
            string errMsg;
            bool success = _sysUserRoleService.Delete(SysUserRoleId, out errMsg);

            return Json(new { Success = success, Msg = success ? "<=DeleteSuccess>" : errMsg },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region SysDataAuth

        #region GetSysDataAuthData
        /// <summary>
        /// 省份数据数邦定
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="SysRoleId"></param>
        /// <returns></returns>
        public JsonResult GetSysDataAuthData(GridParam gp)
        {
            string roleId = (Request.QueryString["sysRoleId"] == null ? " " : Request.QueryString["sysRoleId"]);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("SysRoleId", roleId);
            var data = _sysDataAuthService.GetList(dic).ToList();

            var jsonData = from c in data
                           select new
                           {
                               c.SysDataAuthId,
                               c.SysRoleId,
                               c.SysRole.RoleName,
                               c.SysOrganizationId,
                               c.SysOrganization.OrganizationName,
                           };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveSysDataAuth
        /// <summary>
        /// 保存省信息
        /// </summary>
        /// <param name="SysDataAuth"></param>
        /// <returns></returns>
        public JsonResult SaveSysDataAuth(FormCollection collection)
        {
            List<SysDataAuth> lstAuth = new List<SysDataAuth>();
            var rowsLength = int.Parse(collection.Get("Count"));

            for (int i = 0; i < rowsLength; i++)
            {
                SysDataAuth dataAuth = null;
                string dataAuthId = collection.Get("SysDataAuthId" + i).Trim();

                if (dataAuthId.IsNullString())
                {
                    dataAuth = new SysDataAuth();
                    dataAuth.SysDataAuthId = "";
                }
                else
                    dataAuth = _sysDataAuthService.Get(dataAuthId);

                if (dataAuth != null)
                {
                    dataAuth.SysRoleId = collection.Get("SysRoleId" + i).Trim();
                    dataAuth.SysOrganizationId = collection.Get("SysOrganizationId" + i).Trim();
                }
                lstAuth.Add(dataAuth);
            }

            string str = "<=SaveSuccess>";
            if (!ModelState.IsValid)
            {
                List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                str = string.Join(",", errorList.ToArray());
                return Json(new { Success = false, Msg = str }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string errMsg;
                bool Success = _sysDataAuthService.Save(lstAuth, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DeleteSysDataAuth
        /// <summary>
        /// 删除省信息
        /// </summary>
        /// <param name="SysDataAuthId"></param>
        /// <returns></returns>
        public JsonResult DeleteSysDataAuth(string SysDataAuthId)
        {
            bool success = _sysDataAuthService.Delete(SysDataAuthId);

            return Json(new { Success = success, Msg = success ? "<=DeleteSuccess>" : "<=DeleteFail>" },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region SysRoleFunction

        #region GetSysRoleFunctionData
        /// <summary>
        /// 省份数据数邦定lxy
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="SysRoleId"></param>
        /// <returns></returns>
        public JsonResult GetSysRoleFunctionData(GridParam gp)
        {
            string sysRoleId = (Request.QueryString["sysRoleId"] == null ? " " : Request.QueryString["sysRoleId"]);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ParentId", Request.QueryString["ParentId"] == null ? "0" : Request.QueryString["ParentId"]);
            dic.Add("FunctionType", "0");
            var data = _sysFunctionService.GetList(dic).ToList();

            var jsonData = from c in data
                           let roleFunc = _sysRoleFunctionService.GetRoleFunc(c.SysFunctionId, sysRoleId)
                           select new
                           {
                               SysRoleFunctionId = roleFunc,
                               Assign = (roleFunc == "" ? "" : "checked"),
                               SysFunctionId = c.SysFunctionId,
                               FunctionName = c.FunctionName,
                               ActionName = c.ActionName,
                               ControllerName = c.ControllerName,
                           };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveSysRoleFunction
        /// <summary>
        /// 保存省信息
        /// </summary>
        /// <param name="SysRoleFunction"></param>
        /// <returns></returns>
        public JsonResult SaveSysRoleFunction(FormCollection collection)
        {
            int deleteCount = 0;
            int unSelectCount = 0;
            bool deleteAllFlag = false;
            List<SysRoleFunction> lstRoleFunc = new List<SysRoleFunction>();
            var rowsLength = int.Parse(collection.Get("Count"));
            string sysRoleId = collection.Get("sysRoleId").Trim();
            string MenuId = collection.Get("MenuId").Trim();

            for (int i = 0; i < rowsLength; i++)
            {

                SysRoleFunction roleFunc = null;
                string roleFunctionId = collection.Get("SysRoleFunctionId" + i).Trim();

                if (roleFunctionId.IsNullString())
                {
                    if (collection.Get("Assign" + i).Trim() == "True")
                    {
                        roleFunc = new SysRoleFunction();
                        roleFunc.SysRoleFunctionId = "";
                        roleFunc.SysRoleId = sysRoleId;
                        roleFunc.SysFunctionId = collection.Get("SysFunctionId" + i).Trim();
                        lstRoleFunc.Add(roleFunc);
                        continue;
                    }
                }
                else
                {
                    if (collection.Get("Assign" + i).Trim() == "False")
                    {
                        roleFunc = _sysRoleFunctionService.Get(roleFunctionId);
                        roleFunc.SysFunctionId = "";
                        lstRoleFunc.Add(roleFunc);
                        deleteCount++;
                        continue;
                    }
                }
                unSelectCount++;
            }

            if (deleteCount + unSelectCount == rowsLength)
            {
                deleteAllFlag = true;
            }

            string str = "<=SaveSuccess>";
            if (!ModelState.IsValid)
            {
                List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                str = string.Join(",", errorList.ToArray());
                return Json(new { Success = false, Msg = str }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string errMsg;
                bool Success = _sysRoleFunctionService.Save(lstRoleFunc, MenuId, deleteAllFlag, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #endregion

        #region SysFunction

        public JsonResult FunctionTree(string root, int depth)
        {
            dynamic obj = null;
            var list = _sysFunctionService.GetList().ToList();
            if (depth == 0)
            {
                var dic = CUtility.ConvertEnumToDictionary(typeof(FunctionGroupType));
                obj = from l in dic
                      select new
                      {
                          text = "<=" + l.Value + ">",
                          hasChildren = true,
                          id = l.Key,
                          expanded = false, //!hasCHild,
                          depth = depth + 1
                      };
            }
            if (depth == 1)
            {
                int intGroupType = Convert.ToInt32(root);
                obj = from l in list.Where(l => l.ParentId == null && l.GroupType == intGroupType && l.FunctionType == (int)FuntionType.Page).ToList()
                      //let hasCHild = _sysFunctionService.funcHasChild(l.SysFunctionId)
                      select new
                      {
                          text = l.FunctionName,
                          hasChildren = false,
                          id = l.SysFunctionId,
                          expanded = false, //!hasCHild,
                          depth = depth + 1
                      };

            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region 组织管理

        #region GetWarehouseAutoInputData
        /// <summary>
        /// 获取仓库联想控件数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWarehouseAutoInputData()
        {
            string condition = Request.Form["queryFieldvalue"].ToString();
            var data = _SysOrgService.GetList().Where(c => c.IsWarehouse && (c.OrganizationCode.Contains(condition) || c.OrganizationName.Contains(condition)));
            int totalCount = data.Count();
            int recordsPerPage = int.Parse(Request.Form["recordsPerPage"]);
            int currentPage = int.Parse(Request.Form["currentPage"]);

            var list = (from d in data.OrderByDescending(c => c.OrganizationCode).Skip(recordsPerPage * (currentPage - 1)).Take(recordsPerPage)
                        select new
                        {
                            d.SysOrganizationId,
                            d.OrganizationCode,
                            d.OrganizationName
                        }).ToList();

            AutoIput autoControl = new AutoIput(Request.Form, totalCount);

            return Json(new { msg = autoControl.GetResponseHtml(list) },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetOrganizationAutoInputData
        /// <summary>
        /// 获取组织联想控件数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOrganizationAutoInputData()
        {
            string condition = Request.Form["queryFieldvalue"].ToString();
            var data = _SysOrgService.GetList().Where(c => c.OrganizationCode.Contains(condition) || c.OrganizationName.Contains(condition));
            int totalCount = data.Count();
            int recordsPerPage = int.Parse(Request.Form["recordsPerPage"]);
            int currentPage = int.Parse(Request.Form["currentPage"]);

            var list = (from d in data.OrderByDescending(c => c.OrganizationCode).Skip(recordsPerPage * (currentPage - 1)).Take(recordsPerPage)
                        select new
                        {
                            d.SysOrganizationId,
                            d.OrganizationCode,
                            d.OrganizationName
                        }).ToList();

            AutoIput autoControl = new AutoIput(Request.Form, totalCount);

            return Json(new { msg = autoControl.GetResponseHtml(list) },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        private bool HasChild(string id)
        {
            return _SysOrgService.HasChild(id);
        }

        public ActionResult SysOrganization()
        {
            var org = _SysOrgService.GetList().OrderBy(c => c.CreateDate).FirstOrDefault();
            return View(org);
        }

        public JsonResult InitializeTree(string root, int depth)
        {
            var list = _SysOrgService.GetList().Where(s => s.ParentId == root).ToList();

            var obj = from l in list.ToList()
                      let hasChild = HasChild(l.SysOrganizationId)
                      select new
                      {
                          text = l.OrganizationName,
                          hasChildren = hasChild,
                          id = l.SysOrganizationId,
                          expanded = false, //!hasCHild,
                          depth = depth + 1,
                          data = new JavaScriptSerializer().Serialize(new
                          {
                              OrganizationName = l.OrganizationName,
                              OrganizationCode = l.OrganizationCode,
                              Remarks = l.Remarks,
                              IsInterior = l.IsInterior,
                              IsWarehouse = l.IsWarehouse,
                              ExistDealer = false
                          })
                      };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrganization(string id)
        {
            SysOrganization org = _SysOrgService.Get(id);
            return Json(new { org });
        }

        public JsonResult SaveOrganization(SysOrganization org)
        {
            int intDepth = Convert.ToInt32(Request.Form["Depth"]);

            string str = string.Empty;
            bool isWarehouse = Convert.ToBoolean(Request.Form["IsWarehouse"]);
            if (!ModelState.IsValid)
            {
                List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                str = string.Join(",", errorList.ToArray());
                return Json(new { Success = false, Msg = str }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                SysOrganization tmpOrg = null;
                str = "<=SaveSuccess>";
                if (!org.SysOrganizationId.IsNullString() && Request.Form["Son"] == null && Request.Form["Sibling"] == null)
                {
                    tmpOrg = _SysOrgService.Get(org.SysOrganizationId);
                    bool isInterior = tmpOrg.IsInterior;
                    TryUpdateModel<SysOrganization>(tmpOrg);
                    tmpOrg.IsInterior = isInterior;
                    tmpOrg.IsWarehouse = isWarehouse;
                }
                else
                {
                    bool isInterior = Convert.ToBoolean(Request.Form["Interior"]);
                    tmpOrg = org;
                    tmpOrg.SysOrganizationId = null;
                    tmpOrg.IsWarehouse = isWarehouse;

                    if (Request.Form["Son"] != null)
                    {
                        var parent = _SysOrgService.Get(Request.Form["Son"]);
                        tmpOrg.ParentId = parent.SysOrganizationId;

                        if (intDepth == 0)
                            tmpOrg.IsInterior = isInterior;
                        else
                            tmpOrg.IsInterior = parent.IsInterior;
                    }

                    if (Request.Form["Sibling"] != null)
                    {
                        //string parentId = _SysOrgService.Get(Request.Form["Sibling"]).ParentId;

                        var sibling = _SysOrgService.Get(Request.Form["Sibling"]);
                        string parentId = sibling.ParentId;

                        if (parentId.IsNullString())
                            return Json(new { Success = false, Msg = "<=AddRootError>" }, JsonRequestBehavior.AllowGet);

                        tmpOrg.ParentId = _SysOrgService.Get(Request.Form["Sibling"]).ParentId;
                        tmpOrg.IsInterior = sibling.IsInterior;
                    }
                }
                string errMsg;

                bool succes = _SysOrgService.Save(tmpOrg, out errMsg);

                return Json(new { Success = succes, Msg = errMsg, Text = tmpOrg.OrganizationName, Id = tmpOrg.SysOrganizationId },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteOrganziation(string id)
        {
            string errMsg = string.Empty;
            bool success = _SysOrgService.Delete(id, out errMsg);
            return Json(new { Success = success, Msg = errMsg }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetCityAutoInputData
        /// <summary>
        /// 获取城市联想控件数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCityAutoInputData()
        {
            string condition = Request.Form["queryFieldvalue"].ToString();
            var data = _BasCityDBAccess.GetList().Where(c => (c.CityCode.Contains(condition) || c.CityName.Contains(condition)) && c.Active);
            int totalCount = data.Count();
            int recordsPerPage = int.Parse(Request.Form["recordsPerPage"]);
            int currentPage = int.Parse(Request.Form["currentPage"]);

            var list = (from d in data.OrderByDescending(c => c.CityCode).Skip(recordsPerPage * (currentPage - 1)).Take(recordsPerPage)
                        select new
                        {
                            d.BasCityId,
                            d.CityCode,
                            d.CityName,
                            d.ProvinceCode,
                            d.ProvinceName,
                            d.ProvinceId
                        }).ToList();

            AutoIput autoControl = new AutoIput(Request.Form, totalCount);

            return Json(new { msg = autoControl.GetResponseHtml(list) },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetProvinceAutoInputData
        /// <summary>
        /// 获取省份联想控件数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProvinceAutoInputData()
        {
            string condition = Request.Form["queryFieldvalue"].ToString();
            var data = _BasProvinceDBAccess.GetList().Where(c => (c.ProvinceCode.Contains(condition) || c.ProvinceName.Contains(condition)) && c.Active);
            int totalCount = data.Count();
            int recordsPerPage = int.Parse(Request.Form["recordsPerPage"]);
            int currentPage = int.Parse(Request.Form["currentPage"]);

            var list = (from d in data.OrderByDescending(c => c.ProvinceCode).Skip(recordsPerPage * (currentPage - 1)).Take(recordsPerPage)
                        select new
                        {
                            d.BasProvinceId,
                            d.ProvinceCode,
                            d.ProvinceName
                        }).ToList();

            AutoIput autoControl = new AutoIput(Request.Form, totalCount);

            return Json(new { msg = autoControl.GetResponseHtml(list) },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetDealerAutoInputData
        /// <summary>
        /// Dealer联想控件数据源获取
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDealerAutoInputData()
        {
            string condition = Request.Form["queryFieldvalue"].ToString();
            IQueryable<CCimCust> data = null;
            string Dealer = ((int)Const.Consts.CustAttribute.Dealer).ToString();
            data = _CimCustDBAccess.GetList().Where(c => c.CustAttribute == Dealer
                                                    && c.Active && (c.CustCode.Contains(condition) || c.CustAlias.Contains(condition)));

            var citys = _BasCityDBAccess.GetList().ToList();
            int totalCount = data.Count();
            int recordsPerPage = int.Parse(Request.Form["recordsPerPage"]);
            int currentPage = int.Parse(Request.Form["currentPage"]);

            var list = (from d in data.OrderByDescending(c => c.CustCode).Skip(recordsPerPage * (currentPage - 1)).Take(recordsPerPage)
                        select new
                        {
                            d.CustCode,
                            d.CustName,
                            d.CustAlias,
                            d.CustTypeCode,
                            d.CityCode,
                            d.CityId,
                            d.RelatedWarehouseCode
                        }).ToList();

            var list2 = (from d in list
                         join c in citys on d.CityId equals c.BasCityId
                         select new
                         {
                             d.CustCode,
                             d.CustName,
                             d.CustAlias,
                             d.CustTypeCode,
                             d.CityCode,
                             c.ProvinceCode,
                             d.RelatedWarehouseCode
                         }).ToList();

            AutoIput autoControl = new AutoIput(Request.Form, totalCount);

            return Json(new { msg = autoControl.GetResponseHtml(list2) },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetPDCAutoInputData
        /// <summary>
        /// PDC联想控件数据源获取
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPDCAutoInputData()
        {
            string condition = Request.Form["queryFieldvalue"].ToString();
            IQueryable<CCimCust> data = null;
            string pdc = ((int)Const.Consts.CustAttribute.Warehouser).ToString();
            data = _CimCustDBAccess.GetList().Where(c => c.CustAttribute == pdc
                                                    && c.Active && (c.CustCode.Contains(condition) || c.CustName.Contains(condition)));

            var citys = _BasCityDBAccess.GetList().ToList();
            int totalCount = data.Count();
            int recordsPerPage = int.Parse(Request.Form["recordsPerPage"]);
            int currentPage = int.Parse(Request.Form["currentPage"]);

            var list = (from d in data.OrderByDescending(c => c.CustCode).Skip(recordsPerPage * (currentPage - 1)).Take(recordsPerPage)
                        select new
                        {
                            PdcCode = d.CustCode,
                            PdcName = d.CustName,
                            CityCode = d.CityCode
                        }).ToList();

            AutoIput autoControl = new AutoIput(Request.Form, totalCount);

            return Json(new { msg = autoControl.GetResponseHtml(list) },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetCustInfo
        public JsonResult GetCustInfo(GridParam gp)
        {
            string custAttribute = Request.QueryString["CustAttribute"];
            string shipperName = Server.UrlDecode(Request.QueryString["Shipper"]);
            string consigneeName = Server.UrlDecode(Request.QueryString["Consignee"]);
            string custInfo = consigneeName.IsNullString() ? shipperName : consigneeName;
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("custInfo", custInfo);
            dic.Add("custAttribute", custAttribute);

            var data = _CimCustDBAccess.GetCustInfoList(gp, dic, out pageCount, out totalCount).ToList();
            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
                                    c.CustCode,
                                    c.CustAlias
                            }

                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetForwarder
        /// <summary>
        /// 获取承运商
        /// </summary>
        /// <param name="gp"></param>
        /// <returns></returns>
        public JsonResult GetForwarder(GridParam gp)
        {
            var forwarderName = (from c in _CimCustDBAccess.GetForwarder()
                                 where c.CustAttribute == "2"
                                 select c).ToList();

            var jsonData = new
            {
                rows = (from c in forwarderName
                        select new
                        {
                            cell = new string[] {
                                c.CustCode,
                                c.CustName
                            }
                        }
                       ).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult GetTransportMode(GridParam gp)
        {
            var list = (from c in _BasCodeDefDBAccessService.GetList()
                        select c).ToList();
            var transportMode = list.Where(c => c.CBasCodeType.CodeType == Const.Consts.TransportMode).ToList();
            var jsonData = new
            {
                rows = (from c in transportMode
                        select new
                        {
                            cell = new string[]
                                {
                                    c.CodeValue
                                }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransportType(GridParam gp)
        {
            var list = (from c in _BasCodeDefDBAccessService.GetList()
                        select c).ToList();
            var transportType = list.Where(c => c.CBasCodeType.CodeType == Const.Consts.TransportType).ToList();
            var jsonData = new
            {
                rows = (from c in transportType
                        select new
                        {
                            cell = new string[]
                                {
                                    c.CodeValue
                                }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDealerType(GridParam gp)
        {
            var list = (from c in _BasCodeDefDBAccessService.GetList()
                        select c).ToList();
            var dealerType = list.Where(c => c.CBasCodeType.CodeType == Const.Consts.CustType).ToList();
            var jsonData = new
            {
                rows = (from c in dealerType
                        select new
                        {
                            cell = new string[]
                                {
                                    c.CodeValue
                                }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #region DataDictionary 数据字典

        #region DataDictionary
        /// <summary>
        /// 打开窗体
        /// </summary>
        /// <returns></returns>
        public ViewResult DataDictionary()
        {
            return View();
        }
        #endregion

        #region GetDictionaryData
        /// <summary>
        /// 数据字典数据邦定
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="moduleName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public JsonResult GetDictionaryData(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("CodeType", Request.QueryString["CodeType"]);
            dic.Add("CodeName", Request.QueryString["CodeName"]);
            var data = _sysDictionaryDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            int gridIndex = 0;
            var jsonData = new
            {

                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from d in data
                        select new
                        {
                            cell = new string[] { 
                                d.BasCodeTypeId.ToString(),
                                d.CodeType,
                                d.CodeName,
                                d.CodeGrade.ToString(),
                                d.CodeDesc,
                                d.CreatedUserId,
                                d.CreatedUserCode,
                                d.CreatedUserName,
                                d.CreatedOfficeId,
                                d.CreatedOfficeCode,
                                d.CreatedOfficeName,
                                d.CreatedDate.ToString(),
                                d.CodeWidth.ToString()
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveSbCodeType
        //<summary>
        //保存数据字典信息
        //</summary>
        //<param name="SbCity"></param>
        //<returns></returns>
        public JsonResult SaveSbCodeType(CBasCodeType CBasCodeType)
        {
            string str = string.Empty;
            if (!ModelState.IsValid)
            {
                List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                str = string.Join(",", errorList.ToArray());
                return Json(new { Success = false, Msg = str }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                CBasCodeType tmpDictionary = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(CBasCodeType.BasCodeTypeId))
                {
                    tmpDictionary = _sysDictionaryDBAccess.Get(CBasCodeType.BasCodeTypeId);
                    TryUpdateModel<CBasCodeType>(tmpDictionary);
                }
                else
                {
                    tmpDictionary = CBasCodeType;
                    TryUpdateModel<CBasCodeType>(tmpDictionary);
                }
                string errMsg;
                bool Success;
                Success = _sysDictionaryDBAccess.Save(tmpDictionary, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, Id = Success ? tmpDictionary.BasCodeTypeId : "" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DeleteSbCodeType
        /// <summary>
        /// 删除数据字典信息
        /// </summary>
        /// <param name="SbCityId"></param>
        /// <returns></returns>
        public JsonResult DeleteSbCodeType(string BasCodeTypeId)
        {
            bool success = _sysDictionaryDBAccess.Delete(BasCodeTypeId);
            return Json(new { Success = success, Msg = success ? "<=DeleteSuccess>" : "<=DeleteFail>" },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetCodeDefData
        /// <summary>
        /// 邦定数据字典明细表
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="moduleName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public JsonResult GetCodeDefData(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("SbCodeTypeId", Request.QueryString["BasCodeTypeId"]);
            //dic.Add("Active", Request.QueryString["Active"]);
            var data = _sysCodeDefService.GetList(gp, dic, out pageCount, out totalCount).ToList();

            int gridIndex = 0;
            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from d in data
                        select new
                        {
                            cell = new string[] { 
                                d.BasCodeDefId,
                                d.BasCodeTypeId,
                                d.CodeValue,
                                d.DisplayValue,
                                d.DisplayValueEn,
                                d.Active?"<=Yes>":"<=No>",
                                d.CreatedUserId,
                                d.CreatedUserCode,
                                d.CreatedUserName,
                                d.CreatedOfficeId,
                                d.CreatedOfficeCode,
                                d.CreatedOfficeName,
                                d.CreatedDate.ToString() 
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveSbCodeDef
        //<summary>
        //保存数据字典明细信息
        //</summary>
        //<param name="SbCity"></param>
        //<returns></returns>
        public JsonResult SaveSbCodeDef(CBasCodeDef CBasCodeDef)
        {
            string str = string.Empty;
            if (!ModelState.IsValid)
            {
                List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                str = string.Join(",", errorList.ToArray());
                return Json(new { Success = false, Msg = str }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                CBasCodeDef tmpDictionary = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(CBasCodeDef.BasCodeDefId))
                {
                    tmpDictionary = _sysCodeDefService.Get(CBasCodeDef.BasCodeDefId);
                    TryUpdateModel<CBasCodeDef>(tmpDictionary);
                }
                else
                {
                    tmpDictionary = CBasCodeDef;
                    TryUpdateModel<CBasCodeDef>(tmpDictionary);
                }
                string errMsg;
                bool Success;
                Success = _sysCodeDefService.Save(tmpDictionary, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, Id = Success ? tmpDictionary.BasCodeDefId : "" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DeleteSbCodeDef
        /// <summary>
        /// 删除数据字典信息
        /// </summary>
        /// <param name="SbCityId"></param>
        /// <returns></returns>
        public JsonResult DeleteSbCodeDef(string SbCodeDefId)
        {
            bool success = _sysCodeDefService.Delete(SbCodeDefId);
            return Json(new { Success = success, Msg = success ? "<=DeleteSuccess>" : "<=DeleteFail>" },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetTransportationModeAutoInputData
        /// <summary>
        /// 获取运输模式联想控件数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTransportationModeAutoInputData()
        {
            string condition = Request.Form["queryFieldvalue"].ToString();
            var data = _sysCodeDefService.GetList().Where(c => (c.CBasCodeType.CodeType == Const.Consts.TransportMode && (c.CodeValue.Contains(condition) || c.DisplayValue.Contains(condition))) && c.Active);
            int totalCount = data.Count();
            int recordsPerPage = int.Parse(Request.Form["recordsPerPage"]);
            int currentPage = int.Parse(Request.Form["currentPage"]);

            var list = (from d in data.OrderByDescending(c => c.CodeValue).Skip(recordsPerPage * (currentPage - 1)).Take(recordsPerPage)
                        select new
                        {
                            d.CodeValue,
                            d.DisplayValue,
                        }).ToList();

            AutoIput autoControl = new AutoIput(Request.Form, totalCount);

            return Json(new { msg = autoControl.GetResponseHtml(list) },
                             JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult Exist(string UserName)
        {
            CSbCodeDefService _service = new CSbCodeDefService();
            bool b = _service.Exists(UserName);
            return Json(b,JsonRequestBehavior.AllowGet);
        }
       

        #endregion
        #region 异步获取下拉选项值
        public ActionResult ProductNameSelect()
        {
            return Json(GetSelect(Const.Consts.sysAppList));
        }
        /// <summary>
        /// 取得画面Nature的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult NatureSelect()
        {
            return Json(GetSelect(Const.Consts.sysNature));
        }
       

        /// <summary>
        /// 取得画面Type的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult TypeSelect()
        {
            return Json(GetSelect(Const.Consts.sysType));
        }

        /// <summary>
        /// 取得画面District的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult DistrictSelect()
        {
            return Json(GetSelect(Const.Consts.sysDistrict));
        }
        /// <summary>
        /// 取得画面PaymentStatus的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult PaymentStatusSelect()
        {
            return Json(GetSelect(Const.Consts.sysPaymentStatus));
        }
        /// <summary>
        /// 取得画面Department的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult DepartmentSelect()
        {
            return Json(GetSelect(Const.Consts.sysDepartment));
        }

        /// <summary>
        /// 取得画面ITMember的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult ITMemberSelect()
        {
            return Json(GetSelect(Const.Consts.sysITMember));
        }

        /// <summary>
        /// 取得画面OverallStatus的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult OverallStatusSelect()
        {
            return Json(GetSelect(Const.Consts.sysOverallStatus));
        }

        /// <summary>
        /// 取得画面ESTEffortDay的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult ESTEffortDaySelect()
        {
            return Json(GetSelect(Const.Consts.sysEffortDay));
        }
        /// <summary>
        /// 取得画面CRType的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult CRTypeSelect()
        {
            return Json(GetSelect(Const.Consts.sysCRType));
        }
        /// <summary>
        /// 取得画面TeamSelect的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult TeamSelect()
        {
            return Json(GetSelect(Const.Consts.sysTeam));
        }

        /// <summary>
        /// 取得画面StatusSelect 的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult StatusSelect()
        {
            return Json(GetSelect(Const.Consts.sysStatus));
        }
         /// <summary>
        /// 取得画面PrioritySelect的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult PrioritySelect()
        {
            return Json(GetSelect(Const.Consts.sysPRIORITY));
        }
        /// <summary>
        /// 取得画面PrioritySelect的多选下拉值
        /// </summary>
        /// <returns></returns>
        public JsonResult CurrencyCodeSelect()
        {
            return Json(GetSelect(Const.Consts.sysCurrencyCode));
        }

        public List<comboDescriptor> GetSelect(string key)
        {
            CSbCodeDefService _service = new CSbCodeDefService();
            return _service.GetList(key).ToList();
        }
        #endregion

    }
}
