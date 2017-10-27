using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using FrogDashboard.WebUI.Models;
using FrogDashboard.Core.MVCAttributes;
using FrogDashboard.Service.Services.MasterData.SysManagement;
using FrogDashboard.Domain.SysManagement;
using FrogDashboard.Core.Enumeration;
using FrogDashboard.Core.Utility;
using FrogDashboard.Interface.MasterData.SysManagement;
using System.Reflection;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;

namespace FrogDashboard.WebUI.Controllers
{
    public class AccountController : Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            

            var name = System.Web.HttpContext.Current.User.Identity.Name;

          var testname = System.Net.Dns.GetHostName();

          var test = System.Environment.UserName;

          var tt = System.Environment.UserDomainName;

            if (Request.Cookies["Lang"] == null)
            {
                //string langFromBrowser = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
                //if (string.Compare("zh-CN", langFromBrowser, true) == 0)
                //{
                //    ViewBag.Lang = "SimplifiedChinese";
                //}
                //else
                //{
                //    ViewBag.Lang = "English";
                //}
                #region 默认都为英文版
                ViewBag.Lang = "English";
                #endregion
            }
            else
                ViewBag.Lang = Request.Cookies["Lang"].Value;

            #region authentication for exterior user
            //NameValueCollection appSettings = ConfigurationManager.AppSettings;
            //string strUrl = appSettings["ExteriorUrl"];
            //ViewBag.IsExteriorUser = Request.ServerVariables["server_name"].ToLower() == strUrl.ToLower();
            #endregion

            //var str = Request.Browser.Browser;
            //if (str == "Safari")
            //    return Redirect("../Account/LogOnForPad");
            //else
            return View();

        }

        public ActionResult LogOnForPad()
        {
            if (Request.Cookies["Lang"] == null)
            {
                //string langFromBrowser = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
                //if (string.Compare("zh-CN", langFromBrowser, true) == 0)
                //{
                //    ViewBag.Lang = "SimplifiedChinese";
                //}
                //else
                //{
                //    ViewBag.Lang = "English";
                //}
                #region 默认都为英文版
                ViewBag.Lang = "English";
                #endregion
            }
            else
                ViewBag.Lang = Request.Cookies["Lang"].Value;
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    //if (Url.IsLocalUrl(returnUrl))
                    //{
                    //    return Redirect(returnUrl);
                    //}
                    //else
                    //{
                        return RedirectToAction("Index", "Home");
                    //}
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            Authentication.Authentication.LogOff();
            FormsService.SignOut();
            Session["Lang"] = null;
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }
            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [CustomHandleError]
        public JsonResult UserLogOn(FormCollection collection)
        {

            Language lang;
            var service = new SysUserService();
            SysUser user = service.Login(collection["UserName"], collection["Password"]);

            #region authentication for exterior user
            //if (!string.IsNullOrEmpty(collection["KeyCode"]))
            //NameValueCollection appSettings = ConfigurationManager.AppSettings;
            //string strUrl = appSettings["ExteriorUrl"];
            //if (Request.ServerVariables["server_name"].ToLower() == strUrl.ToLower())
            //{
            //    string strkey = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            //    strkey = collection["UserName"].ToLower() + strkey;

            //    MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            //    UTF8Encoding encoder = new UTF8Encoding();
            //    byte[] hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(strkey));

            //    string strResult = BitConverter.ToString(hashedDataBytes).Replace("-", string.Empty);
            //    if (strResult.ToLower() != collection["KeyCode"].ToLower())
            //        return Json(new { success = false, msg = "<=KeyCodeError>" });

            //}
            #endregion

            if (user == null || string.Compare(user.Pwd, collection["Password"], false) != 0)
                return Json(new { success = false, msg = "<=PwdorNameError>" });
            else
            {
                DateTime dt = DateTime.Today;
                if (!(user.ValidDateFrom <= dt && user.ValidDateTo >= dt) || !user.Active)
                {
                    return Json(new { success = false, msg = "<=UserExpireError>" });
                }

                //密码过期修改
                if (!user.PasswordDate.HasValue)
                {
                    string errorMsg = string.Empty;
                    user.PasswordDate = DateTime.Now.Date;
                    bool success = service.Save(user, out errorMsg);
                    if (!success)
                        return Json(new { success = false, msg = errorMsg });
                }
                else
                {
                    if (user.PasswordDate.Value.AddDays(30).Date <= DateTime.Now.Date)
                    {
                        lang = (Language)int.Parse(collection["Lang"]);
                        Response.Cookies["Lang"].Value = lang == Language.English ? "English" : "SimplifiedChinese";
                        Response.Cookies["Lang"].Expires = DateTime.Now.AddDays(365);
                        return Json(new { success = false, msg = "expired" });
                    }
                }

                FormsService.SignIn(collection["UserName"], false);
                lang = (Language)int.Parse(collection["Lang"]);
                Response.Cookies["Lang"].Value = lang == Language.English ? "English" : "SimplifiedChinese";
                Response.Cookies["Lang"].Expires = DateTime.Now.AddDays(365);
                return Json(new { success = true, msg = "<LoginSuccess>" });
            }
        }

        public JsonResult UserLogForWinCE(FormCollection collection)
        {
            //Language lang;
            var service = new SysUserService();
            SysUser user = service.Login(collection["UserName"], collection["Password"]);

            if (user == null || string.Compare(user.Pwd, collection["Password"], false) != 0)
                return Json(new { success = false, msg = "<=PwdorNameError>" });
            else
            {
                DateTime dt = DateTime.Today;
                if (!(user.ValidDateFrom <= dt && user.ValidDateTo >= dt) || !user.Active)
                {
                    return Json(new { success = false, msg = "<=UserExpireError>" });
                }
                FormsService.SignIn(collection["UserName"], false);
                //lang = (Language)int.Parse(collection["Lang"]);
                //Response.Cookies["Lang"].Value = lang == Language.English ? "English" : "SimplifiedChinese";
                //Response.Cookies["Lang"].Expires = DateTime.Now.AddDays(365);
                return Json(new { success = true, msg = "<=LoginSuccess>" });
            }
        }

        public JsonResult UserLogOn_Pad(FormCollection collection)
        {
            Language lang;
            var service = new SysUserService();
            SysUser user = service.Login(collection["UserName"], collection["Password"]);

            if (user == null || string.Compare(user.Pwd, collection["Password"], false) != 0)
                return Json(new { success = false, msg = "<=PwdorNameError>" });
            else
            {
                DateTime dt = DateTime.Today;
                if (!(user.ValidDateFrom <= dt && user.ValidDateTo >= dt) || !user.Active)
                {
                    return Json(new { success = false, msg = "<=UserExpireError>" });
                }

                //密码过期修改
                if (!user.PasswordDate.HasValue)
                {
                    string errorMsg = string.Empty;
                    user.PasswordDate = DateTime.Now.Date;
                    bool success = service.Save(user, out errorMsg);
                    if (!success)
                        return Json(new { success = false, msg = errorMsg });
                }
                else
                {
                    if (user.PasswordDate.Value.AddDays(30).Date <= DateTime.Now.Date)
                    {
                        lang = (Language)int.Parse(collection["Lang"]);
                        Response.Cookies["Lang"].Value = lang == Language.English ? "English" : "SimplifiedChinese";
                        Response.Cookies["Lang"].Expires = DateTime.Now.AddDays(365);
                        return Json(new { success = false, msg = "expired" });
                    }
                }

                FormsService.SignIn(collection["UserName"], false);
                lang = (Language)int.Parse(collection["Lang"]);
                Response.Cookies["Lang"].Value = lang == Language.English ? "English" : "SimplifiedChinese";
                Response.Cookies["Lang"].Expires = DateTime.Now.AddDays(365);
                string s = "FrogDashboard.Service.Services.MasterData.SysManagement.SysFunctionService";
                ISysFunction<SysFunction> service_ = (ISysFunction<SysFunction>)Assembly.Load("FrogDashboard.Service").CreateInstance(s);
                var userRoleId = user.RoleId;
                if (service_.GetUserFunction(userRoleId).Where(c => c.ControllerName == "OrderManagement" && c.ActionName == "TmsForPad").Count() > 0)
                {
                    return Json(new { success = true, msg = "<LoginSuccess>", URL = "Pad" });
                }
                else
                    return Json(new { success = true, msg = "<LoginSuccess>", URL = "Index" });
            }
        }

        public JsonResult UserLogOff()
        {
            Authentication.Authentication.LogOff();
            FormsService.SignOut();
            Session["Lang"] = null;
            return Json(new { success = true });
        }

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

        [CustomHandleError]
        public JsonResult ChangePasswordPost(FormCollection collection)
        {
            var service = new SysUserService();
            SysUser user = service.Login(collection["UserCode"], collection["OldPwd"]);

            if (user == null || string.Compare(user.Pwd, collection["OldPwd"], false) != 0)
                return Json(new { success = false, msg = "<=OldPwdNotMatched>" });
            else
            {
                if (string.Compare(collection["NewPwd"], collection["ConfirmPwd"], false) != 0)
                    return Json(new { success = false, msg = "<=NewPwdNotMatched>" });

                if (string.Compare(collection["NewPwd"], user.Pwd, false) == 0)
                    return Json(new { success = false, msg = "<=ForbidMatchOldPwd>" });

                string errorMsg = string.Empty;
                user.PasswordDate = DateTime.Now.Date;

                user.Pwd = collection["NewPwd"];
                TryUpdateModel<SysUser>(user);

                //判断新密码规则,Operator不受密码规则控制
                var _sysUserRoleService = new SysUserRoleService();
                var _sysRoleService = new SysRoleService();
                var sysUserRole = _sysUserRoleService.GetList().Where(o => o.SysUserId == user.SysUserId).Select(o => o.SysRoleId).Distinct().ToList();
                var sysRole = _sysRoleService.GetList().Where(o => sysUserRole.Contains(o.SysRoleId) || o.SysRoleId == user.RoleId).ToList();
                if (sysRole.Count() > 0 && (sysRole.Count() != 1 || sysRole.Where(o => o.RoleCode.Contains("Operator")).Count() <= 0))
                {
                    if (!this.PassWordIsValid(user.Pwd))
                    {
                        return Json(new { success = false, msg = "<=PasswordError>" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (!ModelState.IsValid)
                {
                    List<string> errorList = ModelStateExtension.GetModelError(ViewData);
                    string str = string.Join(",", errorList.ToArray());
                    return Json(new { success = false, msg = str }, JsonRequestBehavior.AllowGet);
                }
                bool success = service.Save(user, out errorMsg);
                if (!success)
                    return Json(new { success = false, msg = errorMsg });

                FormsService.SignIn(collection["UserCode"], false);
                return Json(new { success = true, msg = "<=ModifySuccess>" });
            }
        }

        public ActionResult ForceModifyPassword(bool forceModify = true)
        {
            ViewBag.ForceModify = forceModify;
            return View();
        }
    }
}
