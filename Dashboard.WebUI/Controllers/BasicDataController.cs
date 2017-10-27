using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FrogDashboard.Authentication.MvcAttribute;
using FrogDashboard.Core.MVCAttributes;
using FrogDashboard.Core.Utility;
using FrogDashboard.Domain.BasicData;
using FrogDashboard.Domain.MVCExtension;
using FrogDashboard.Interface.BasicData;
using FrogDashboard.Service.Services.BasicData;
using System.Data;

namespace FrogDashboard.WebUI.Controllers
{
    [CustomHandleError()]
    [CustomAuthorize()]
    public class BasicDataController : Controller
    {
        private IBasAdjustmentTable<CBasAdjustmentTable> _BasAdjustmentTableDBAccess = new CBasAdjustmentTableService();
        private IBasNormalSchedule<CBasNormalSchedule> _BasNormalScheduleDBAccess = new CBasNormalScheduleService();
        private IBasSpecialScheduleIndex<CBasSpecialScheduleIndex> _BasSpecialScheduleIndexDBAccess = new CBasSpecialScheduleIndexService();
        private IBasNormalScheduleS<CBasNormalScheduleS> _BasNormalScheduleSDBAccess = new CBasNormalScheduleSService();
        private IBasNormalScheduleVCt<CBasNormalScheduleVCt> _BasNormalScheduleVCtDBAccess = new CBasNormalScheduleVCtService();
        private IBasNormalScheduleVCtDet<CBasNormalScheduleVCtDet> _BasNormalScheduleVCtDetDBAccess = new CBasNormalScheduleVCtDetService();
        private IBasNormalScheduleVTm<CBasNormalScheduleVTm> _BasNormalScheduleVTmDBAccess = new CBasNormalScheduleVTmService();
        private IBasNormalScheduleVTmDet<CBasNormalScheduleVTmDet> _BasNormalScheduleVTmDetDBAccess = new CBasNormalScheduleVTmDetService();
        private IBasSpecialDay<CBasSpecialDay> _BasSpecialDayDBAccess = new CBasSpecialDayService();
        private IBasSpecialSchedule<CBasSpecialSchedule> _BasSpecialScheduleDBAccess = new CBasSpecialScheduleService();
        private IBasPdcSequence<CBasPdcSequence> _BasPdcSequenceDBAccess = new CBasPdcSequenceService();

        #region weekday
        private enum weekday
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }
        #endregion

        #region BasNormalSchedule

        #region BasNormalSchedule
        public ActionResult BasNormalSchedule()
        {
            return View();
        }
        #endregion

        #region GetBasNormalSchedule
        public JsonResult GetBasNormalSchedule(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ScheduleNo", Request.QueryString["ScheduleNO"]);
            var data = _BasNormalScheduleDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasNormalScheduleId,
    								c.ScheduleNo,
    								c.EffectiveDate.ToString("yyyy-MM-dd"),
    								c.ExpirationDate.ToString("yyyy-MM-dd"),
    								c.Remark,
                                    c.CreatedUserCode,
                                    c.CreatedDate.ToString("yyyy-MM-dd")
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasNormalSchedule
        public JsonResult SaveBasNormalSchedule(CBasNormalSchedule BasNormalSchedule)
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
                CBasNormalSchedule tmpBasNormalSchedule = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasNormalSchedule.BasNormalScheduleId))
                {
                    tmpBasNormalSchedule = _BasNormalScheduleDBAccess.Get(BasNormalSchedule.BasNormalScheduleId);
                    TryUpdateModel<CBasNormalSchedule>(tmpBasNormalSchedule);
                }
                else
                {
                    tmpBasNormalSchedule = BasNormalSchedule;
                }
                string errMsg;
                bool Success;
                Success = _BasNormalScheduleDBAccess.Save(tmpBasNormalSchedule, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasNormalSchedule = tmpBasNormalSchedule }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasNormalScheduleById
        public JsonResult GetBasNormalScheduleById(string BasNormalScheduleId)
        {
            var BasNormalSchedule = _BasNormalScheduleDBAccess.Get(BasNormalScheduleId);
            return Json(new { Success = true, BasNormalSchedule = BasNormalSchedule }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region NormalScheduleImport
        /// <summary>
        /// Normal Schedule Import
        /// </summary>
        /// <param name="BasNormalScheduleId"></param>
        /// <param name="type">(Normal Schedule S,Normal Schedule V Cargo Type,Normal Schedule V Trans. Mode)</param>
        /// <returns></returns>
        public string NormalScheduleImport()
        {
            string basNormalScheduleId = Request.QueryString["basNormalScheduleId"];
            string type = Request.QueryString["FileType"];

            string errMsg;
            string strPath = Import(type, out errMsg);
            if (!strPath.IsNullString())
            {
                _BasNormalScheduleDBAccess.NormalScheduleImport(basNormalScheduleId, strPath, type, out errMsg);
            }
            else
            {
                if (errMsg.IsNullString())
                {
                    errMsg = "<=uploadError>";
                }
            }
            return errMsg + "</html>";
        }
        #endregion

        #endregion

        #region BasNormalScheduleS

        #region BasNormalScheduleS
        public ActionResult BasNormalScheduleS()
        {
            CBasNormalSchedule BasNormalSchedule = new CBasNormalSchedule();
            string BasNormalScheduleId = Request.QueryString["BasNormalScheduleId"];
            BasNormalSchedule = _BasNormalScheduleDBAccess.Get(BasNormalScheduleId);
            if (BasNormalSchedule == null)
            {
                return RedirectToAction("BasNormalSchedule", "BasicData");
            }
            return View(BasNormalSchedule);
        }
        #endregion

        #region GetBasNormalScheduleS
        public JsonResult GetBasNormalScheduleS(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("BasNormalScheduleId", Request.QueryString["BasNormalScheduleId"]);
            dic.Add("DealerCode", Request.QueryString["DealerCode"]);
            var data = _BasNormalScheduleSDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasNormalScheduleSId,
    								c.DealerCode,
    								c.FacingPdc,
    								c.Definition,
    								c.DealerType,
    								c.ShortDealerName,
    								c.Destination,
    								c.Province,
                                    Enum.GetName(typeof(weekday), c.CutoffDay),
    								c.CutoffTime.ToString("HH:mm:ss"),
    								Enum.GetName(typeof(weekday), c.PickupDay),
    								c.PickupTime.ToString("HH:mm:ss"),
    								c.TmsRouteName,
    								c.StopOrder.ToString(),
    								c.TransMode,
    								c.Overweek.ToString(),
                                    Enum.GetName(typeof(weekday), c.ArrivalDay),
    								c.Eta.HasValue ? c.Eta.Value.ToString("HH:mm:ss") : "",
    								c.ArrivalTime,
    								c.ArrivalTimeStart.ToString("HH:mm:ss"),
    								c.ArrivalTimeEnd.ToString("HH:mm:ss"),
    								c.Forwarder
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasNormalScheduleS
        public JsonResult SaveBasNormalScheduleS(CBasNormalScheduleS BasNormalScheduleS)
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
                CBasNormalScheduleS tmpBasNormalScheduleS = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasNormalScheduleS.BasNormalScheduleSId))
                {
                    tmpBasNormalScheduleS = _BasNormalScheduleSDBAccess.Get(BasNormalScheduleS.BasNormalScheduleSId);
                    TryUpdateModel<CBasNormalScheduleS>(tmpBasNormalScheduleS);
                }
                else
                {
                    tmpBasNormalScheduleS = BasNormalScheduleS;
                }
                string errMsg;
                bool Success;
                Success = _BasNormalScheduleSDBAccess.Save(tmpBasNormalScheduleS, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasNormalScheduleS = tmpBasNormalScheduleS }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasNormalScheduleSById
        public JsonResult GetBasNormalScheduleSById(string BasNormalScheduleSId)
        {
            var BasNormalScheduleS = _BasNormalScheduleSDBAccess.Get(BasNormalScheduleSId);
            return Json(new { Success = true, BasNormalScheduleS = BasNormalScheduleS }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region BasNormalScheduleVCt

        #region BasNormalScheduleVCt
        public ActionResult BasNormalScheduleVCt()
        {
            CBasNormalSchedule BasNormalSchedule = new CBasNormalSchedule();
            string BasNormalScheduleId = Request.QueryString["BasNormalScheduleId"];
            BasNormalSchedule = _BasNormalScheduleDBAccess.Get(BasNormalScheduleId);
            if (BasNormalSchedule == null)
            {
                return RedirectToAction("BasNormalSchedule", "BasicData");
            }
            return View(BasNormalSchedule);
        }
        #endregion

        
        #region GetBasNormalScheduleVCt
        public JsonResult GetBasNormalScheduleVCt(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("BasNormalScheduleId", Request.QueryString["BasNormalScheduleId"]);
            dic.Add("DealerCode", Request.QueryString["DealerCode"]);
            var data = _BasNormalScheduleVCtDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasNormalScheduleVCtId,
    								c.DealerCode,
    								c.FacingPdc,
    								c.DealerType,
    								c.DealerName,
    								c.Destination,
    								c.Province,
    								c.SecondVCutoffDay,
    								c.SecondVCutoffTime.ToString("HH:mm:ss"),
    								c.SecondVPickupDay,
    								c.SecondVPickupTime.ToString("HH:mm:ss"),
    								c.FirstVCutoffDay,
    								c.FirstVCutoffTime.HasValue ? c.FirstVCutoffTime.Value.ToString("HH:mm:ss") : "",
    								c.FirstVPickupDay,
    								c.FirstVPickupTime.HasValue ? c.FirstVPickupTime.Value.ToString("HH:mm:ss") : "",
    								c.FirstVTransMode,
    								c.FirstVLeadtime.HasValue ? c.FirstVLeadtime.Value.ToString() : "",
    								c.FirstVLeadtimeAmPm,
    								c.FirstVLeadtimeStart.HasValue ? c.FirstVLeadtimeStart.Value.ToString("HH:mm:ss") : "",
    								c.FirstVLeadtimeEnd.HasValue ? c.FirstVLeadtimeEnd.Value.ToString("HH:mm:ss") : ""
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasNormalScheduleVCt
        public JsonResult SaveBasNormalScheduleVCt(CBasNormalScheduleVCt BasNormalScheduleVCt)
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
                CBasNormalScheduleVCt tmpBasNormalScheduleVCt = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasNormalScheduleVCt.BasNormalScheduleVCtId))
                {
                    tmpBasNormalScheduleVCt = _BasNormalScheduleVCtDBAccess.Get(BasNormalScheduleVCt.BasNormalScheduleVCtId);
                    TryUpdateModel<CBasNormalScheduleVCt>(tmpBasNormalScheduleVCt);
                }
                else
                {
                    tmpBasNormalScheduleVCt = BasNormalScheduleVCt;
                }
                string errMsg;
                bool Success;
                Success = _BasNormalScheduleVCtDBAccess.Save(tmpBasNormalScheduleVCt, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasNormalScheduleVCt = tmpBasNormalScheduleVCt }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasNormalScheduleVCtById
        public JsonResult GetBasNormalScheduleVCtById(string BasNormalScheduleVCtId)
        {
            var BasNormalScheduleVCt = _BasNormalScheduleVCtDBAccess.Get(BasNormalScheduleVCtId);
            return Json(new { Success = true, BasNormalScheduleVCt = BasNormalScheduleVCt }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region BasNormalScheduleVCtDet

        #region GetBasNormalScheduleVCtDet
        public JsonResult GetBasNormalScheduleVCtDet(GridParam gp)
        {
            string basNormalScheduleVCtId = Request.QueryString["BasNormalScheduleVCtId"];
            var data = _BasNormalScheduleVCtDetDBAccess.GetList(basNormalScheduleVCtId).OrderBy(gp.sidx, gp.sord).ToList();

            var jsonData = new
            {
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasNormalScheduleVCtDetId,
    								c.Pdc,
    								c.CargoType,
    								c.SecondVTransMode,
    								c.SecondVLeadtime.ToString(),
    								c.SecondVLeadtimeAmPm,
    								c.SecondVLeadtimeStart.ToString("HH:mm:ss"),
    								c.SecondVLeadtimeEnd.ToString("HH:mm:ss")
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasNormalScheduleVCtDet
        public JsonResult SaveBasNormalScheduleVCtDet(CBasNormalScheduleVCtDet BasNormalScheduleVCtDet)
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
                CBasNormalScheduleVCtDet tmpBasNormalScheduleVCtDet = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasNormalScheduleVCtDet.BasNormalScheduleVCtDetId))
                {
                    tmpBasNormalScheduleVCtDet = _BasNormalScheduleVCtDetDBAccess.Get(BasNormalScheduleVCtDet.BasNormalScheduleVCtDetId);
                    TryUpdateModel<CBasNormalScheduleVCtDet>(tmpBasNormalScheduleVCtDet);
                }
                else
                {
                    tmpBasNormalScheduleVCtDet = BasNormalScheduleVCtDet;
                }
                string errMsg;
                bool Success;
                Success = _BasNormalScheduleVCtDetDBAccess.Save(tmpBasNormalScheduleVCtDet, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasNormalScheduleVCtDet = tmpBasNormalScheduleVCtDet }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasNormalScheduleVCtDetById
        public JsonResult GetBasNormalScheduleVCtDetById(string BasNormalScheduleVCtDetId)
        {
            var BasNormalScheduleVCtDet = _BasNormalScheduleVCtDetDBAccess.Get(BasNormalScheduleVCtDetId);
            return Json(new { Success = true, BasNormalScheduleVCtDet = BasNormalScheduleVCtDet }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region BasNormalScheduleVTm

        #region BasNormalScheduleVTm
        public ActionResult BasNormalScheduleVTm()
        {
            CBasNormalSchedule BasNormalSchedule = new CBasNormalSchedule();
            string BasNormalScheduleId = Request.QueryString["BasNormalScheduleId"];
            BasNormalSchedule = _BasNormalScheduleDBAccess.Get(BasNormalScheduleId);
            if (BasNormalSchedule == null)
            {
                return RedirectToAction("BasNormalSchedule", "BasicData");
            }
            return View(BasNormalSchedule);
        }
        #endregion

        #region GetBasNormalScheduleVTm
        public JsonResult GetBasNormalScheduleVTm(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("BasNormalScheduleId", Request.QueryString["BasNormalScheduleId"]);
            dic.Add("Destination", Request.QueryString["Destination"]);
            var data = _BasNormalScheduleVTmDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasNormalScheduleVTmId,
    								c.Destination,
    								c.Province,
    								c.SecondVCutoffDay,
    								c.SecondVCutoffTime.ToString("HH:mm:ss"),
    								c.SecondVPickupDay,
    								c.SecondVPickupTime.ToString("HH:mm:ss"),
    								c.FirstVCutoffDay,
    								c.FirstVCutoffTime.HasValue ? c.FirstVCutoffTime.Value.ToString("HH:mm:ss") : "",
    								c.FirstVPickupDay,
    								c.FirstVPickupTime.HasValue ? c.FirstVPickupTime.Value.ToString("HH:mm:ss") : "",
    								c.FirstVTransMode,
    								c.FirstVLeadtime.HasValue ? c.FirstVLeadtime.Value.ToString() : "",
    								c.FirstVLeadtimeAmPm,
    								c.FirstVLeadtimeStart.HasValue ? c.FirstVLeadtimeStart.Value.ToString("HH:mm:ss") : "",
    								c.FirstVLeadtimeEnd.HasValue ? c.FirstVLeadtimeEnd.Value.ToString("HH:mm:ss") : ""
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasNormalScheduleVTm
        public JsonResult SaveBasNormalScheduleVTm(CBasNormalScheduleVTm BasNormalScheduleVTm)
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
                CBasNormalScheduleVTm tmpBasNormalScheduleVTm = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasNormalScheduleVTm.BasNormalScheduleVTmId))
                {
                    tmpBasNormalScheduleVTm = _BasNormalScheduleVTmDBAccess.Get(BasNormalScheduleVTm.BasNormalScheduleVTmId);
                    TryUpdateModel<CBasNormalScheduleVTm>(tmpBasNormalScheduleVTm);
                }
                else
                {
                    tmpBasNormalScheduleVTm = BasNormalScheduleVTm;
                }
                string errMsg;
                bool Success;
                Success = _BasNormalScheduleVTmDBAccess.Save(tmpBasNormalScheduleVTm, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasNormalScheduleVTm = tmpBasNormalScheduleVTm }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasNormalScheduleVTmById
        public JsonResult GetBasNormalScheduleVTmById(string BasNormalScheduleVTmId)
        {
            var BasNormalScheduleVTm = _BasNormalScheduleVTmDBAccess.Get(BasNormalScheduleVTmId);
            return Json(new { Success = true, BasNormalScheduleVTm = BasNormalScheduleVTm }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region BasNormalScheduleVTmDet

        #region GetBasNormalScheduleVTmDet
        public JsonResult GetBasNormalScheduleVTmDet(GridParam gp)
        {
            string basNormalScheduleVTmId = Request.QueryString["BasNormalScheduleVTmId"];
            var data = _BasNormalScheduleVTmDetDBAccess.GetList(basNormalScheduleVTmId).OrderBy(gp.sidx, gp.sord).ToList();

            var jsonData = new
            {
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasNormalScheduleVTmDetId,
    								c.Pdc,
    								c.TransMode,
    								c.SecondVLeadtime.ToString(),
    								c.SecondVLeadtimeAmPm,
    								c.SecondVLeadtimeStart.ToString("HH:mm:ss"),
    								c.SecondVLeadtimeEnd.ToString("HH:mm:ss")
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasNormalScheduleVTmDet
        public JsonResult SaveBasNormalScheduleVTmDet(CBasNormalScheduleVTmDet BasNormalScheduleVTmDet)
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
                CBasNormalScheduleVTmDet tmpBasNormalScheduleVTmDet = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasNormalScheduleVTmDet.BasNormalScheduleVTmDetId))
                {
                    tmpBasNormalScheduleVTmDet = _BasNormalScheduleVTmDetDBAccess.Get(BasNormalScheduleVTmDet.BasNormalScheduleVTmDetId);
                    TryUpdateModel<CBasNormalScheduleVTmDet>(tmpBasNormalScheduleVTmDet);
                }
                else
                {
                    tmpBasNormalScheduleVTmDet = BasNormalScheduleVTmDet;
                }
                string errMsg;
                bool Success;
                Success = _BasNormalScheduleVTmDetDBAccess.Save(tmpBasNormalScheduleVTmDet, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasNormalScheduleVTmDet = tmpBasNormalScheduleVTmDet }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasNormalScheduleVTmDetById
        public JsonResult GetBasNormalScheduleVTmDetById(string BasNormalScheduleVTmDetId)
        {
            var BasNormalScheduleVTmDet = _BasNormalScheduleVTmDetDBAccess.Get(BasNormalScheduleVTmDetId);
            return Json(new { Success = true, BasNormalScheduleVTmDet = BasNormalScheduleVTmDet }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region BasSpecialDay

        #region BasSpecialDay
        public ActionResult BasSpecialDay()
        {
            return View();
        }
        #endregion

        #region GetBasSpecialDay
        public JsonResult GetBasSpecialDay(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("SpecialDayFrom", Request.QueryString["SpecialDayFrom"]);
            dic.Add("SpecialDayTo", Request.QueryString["SpecialDayTo"]);

            var data = _BasSpecialDayDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasSpecialDayId,
    								c.SpecialDay.ToString("yyyy-MM-dd"),
                                    c.Active?"Yes":"No",
                                    c.CreatedUserCode,
                                    c.CreatedDate.ToString("yyyy-MM-dd"),
                                    c.Remark
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasSpecialDay
        public JsonResult SaveBasSpecialDay(CBasSpecialDay BasSpecialDay)
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
                CBasSpecialDay tmpBasSpecialDay = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasSpecialDay.BasSpecialDayId))
                {
                    tmpBasSpecialDay = _BasSpecialDayDBAccess.Get(BasSpecialDay.BasSpecialDayId);
                    TryUpdateModel<CBasSpecialDay>(tmpBasSpecialDay);
                }
                else
                {
                    tmpBasSpecialDay = BasSpecialDay;
                }
                string errMsg;
                bool Success;
                Success = _BasSpecialDayDBAccess.Save(tmpBasSpecialDay, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasSpecialDay = tmpBasSpecialDay }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasSpecialDayById
        public JsonResult GetBasSpecialDayById(string BasSpecialDayId)
        {
            var BasSpecialDay = _BasSpecialDayDBAccess.Get(BasSpecialDayId);
            return Json(new { Success = true, BasSpecialDay = BasSpecialDay }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region BasSpecialSchedule

        #region BasSpecialSchedule
        public ActionResult BasSpecialSchedule()
        {
            CBasSpecialScheduleIndex BasSpecialScheduleIndex = new CBasSpecialScheduleIndex();
            string BasSpecialScheduleIndexId = Request.QueryString["BasSpecialScheduleIndexId"];
            BasSpecialScheduleIndex = _BasSpecialScheduleIndexDBAccess.Get(BasSpecialScheduleIndexId);
            if (BasSpecialScheduleIndex == null)
            {
                return RedirectToAction("BasSpecialScheduleIndex", "BasicData");
            }
            return View(BasSpecialScheduleIndex);
        }

        public ActionResult BasSpecialScheduleIndex()
        {
            return View();
        }
        #endregion

        #region GetBasSpecialSchedule
        public JsonResult GetBasSpecialSchedule(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("BasSpecialScheduleIndexId", Request.QueryString["BasSpecialScheduleIndexId"]);
            dic.Add("DealerCode", Request.QueryString["DealerCode"]);
            dic.Add("PickupDayFrom", Request.QueryString["PickupDayFrom"]);
            dic.Add("PickupDayTo", Request.QueryString["PickupDayTo"]);
            var data = _BasSpecialScheduleDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasSpecialScheduleId,
    								c.DealerCode,
    								c.DealerName,
    								c.DealerType,
    								c.Destination,
    								c.Province,
    								c.FacingPdc,
    								c.Definition,
    								c.CutoffDay.ToString("yyyy-MM-dd"),
    								c.CutoffTime.ToString("HH:mm:ss"),
    								c.PickupDay.ToString("yyyy-MM-dd"),
    								c.PickupTime.ToString("HH:mm:ss"),
    								c.TransMode,
    								c.ArrivalDay.ToString("yyyy-MM-dd"),
    								c.ArrivalTime,
    								c.ArrivalTimeStart.ToString("HH:mm:ss"),
    								c.ArrivalTimeEnd.ToString("HH:mm:ss")
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasSpecialSchedule
        public JsonResult SaveBasSpecialSchedule(CBasSpecialSchedule BasSpecialSchedule)
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
                CBasSpecialSchedule tmpBasSpecialSchedule = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasSpecialSchedule.BasSpecialScheduleId))
                {
                    tmpBasSpecialSchedule = _BasSpecialScheduleDBAccess.Get(BasSpecialSchedule.BasSpecialScheduleId);
                    TryUpdateModel<CBasSpecialSchedule>(tmpBasSpecialSchedule);
                }
                else
                {
                    tmpBasSpecialSchedule = BasSpecialSchedule;
                }
                string errMsg;
                bool Success;
                Success = _BasSpecialScheduleDBAccess.Save(tmpBasSpecialSchedule, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasSpecialSchedule = tmpBasSpecialSchedule }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasSpecialScheduleById
        public JsonResult GetBasSpecialScheduleById(string BasSpecialScheduleId)
        {
            var BasSpecialSchedule = _BasSpecialScheduleDBAccess.Get(BasSpecialScheduleId);
            return Json(new { Success = true, BasSpecialSchedule = BasSpecialSchedule }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SpecialScheduleImport
        /// <summary>
        /// Special Schedule Import
        /// </summary>
        public string SpecialScheduleImport()
        {
            string errMsg;
            string strPath = Import("Special Schedule", out errMsg);
            if (!strPath.IsNullString())
            {
                string BasSpecialScheduleIndexId = Request.QueryString["BasSpecialScheduleIndexId"];
                _BasSpecialScheduleDBAccess.SpecialScheduleImport(BasSpecialScheduleIndexId, strPath, out errMsg);
            }
            else
            {
                if (errMsg.IsNullString())
                {
                    errMsg = "<=uploadError>";
                }
            }
            return errMsg + "</html>";
        }
        #endregion

        #endregion

        #region BasSpecialScheduleIndex

        #region GetBasSpecialScheduleIndex
        public JsonResult GetBasSpecialScheduleIndex(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ScheduleNo", Request.QueryString["ScheduleNo"]);
            var data = _BasSpecialScheduleIndexDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasSpecialScheduleIndexId,
    								c.ScheduleNo,
    								c.EffectiveDate.ToString("yyyy-MM-dd"),
    								c.ExpirationDate.ToString("yyyy-MM-dd"),
    								c.Remark,
                                    c.CreatedUserCode,
                                    c.CreatedDate.ToString("yyyy-MM-dd")
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasSpecialScheduleIndex
        public JsonResult SaveBasSpecialScheduleIndex(CBasSpecialScheduleIndex BasSpecialScheduleIndex)
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
                CBasSpecialScheduleIndex tmpBasSpecialScheduleIndex = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasSpecialScheduleIndex.BasSpecialScheduleIndexId))
                {
                    tmpBasSpecialScheduleIndex = _BasSpecialScheduleIndexDBAccess.Get(BasSpecialScheduleIndex.BasSpecialScheduleIndexId);
                    TryUpdateModel<CBasSpecialScheduleIndex>(tmpBasSpecialScheduleIndex);
                }
                else
                {
                    tmpBasSpecialScheduleIndex = BasSpecialScheduleIndex;
                }
                string errMsg;
                bool Success;
                Success = _BasSpecialScheduleIndexDBAccess.Save(tmpBasSpecialScheduleIndex, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasSpecialScheduleIndex = tmpBasSpecialScheduleIndex }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasSpecialScheduleIndexById
        public JsonResult GetBasSpecialScheduleIndexById(string BasSpecialScheduleIndexId)
        {
            var BasSpecialScheduleIndex = _BasSpecialScheduleIndexDBAccess.Get(BasSpecialScheduleIndexId);
            return Json(new { Success = true, BasSpecialScheduleIndex = BasSpecialScheduleIndex }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region BasPdcSequence

        #region BasPdcSequence
        public ActionResult BasPdcSequence()
        {
            return View();
        }
        #endregion

        #region GetBasPdcSequence
        public JsonResult GetBasPdcSequence(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Pdc", Request.QueryString["Pdc"]);
            var data = _BasPdcSequenceDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
    								c.BasPdcSequenceId,
    								c.Pdc,
    								c.SequenceNumber.ToString(),
                                    c.CreatedUserCode,
                                    c.CreatedDate.ToString("yyyy-MM-dd"),
                                    c.Active?"<=Yes>":"<=No>"
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SaveBasPdcSequence
        public JsonResult SaveBasPdcSequence(CBasPdcSequence BasPdcSequence)
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
                CBasPdcSequence tmpBasPdcSequence = null;
                str = "<=SaveSuccess>";
                if (!string.IsNullOrEmpty(BasPdcSequence.BasPdcSequenceId))
                {
                    tmpBasPdcSequence = _BasPdcSequenceDBAccess.Get(BasPdcSequence.BasPdcSequenceId);
                    TryUpdateModel<CBasPdcSequence>(tmpBasPdcSequence);
                }
                else
                {
                    tmpBasPdcSequence = BasPdcSequence;
                }
                string errMsg;
                bool Success;
                Success = _BasPdcSequenceDBAccess.Save(tmpBasPdcSequence, out errMsg);
                return Json(new { Success = Success, Msg = Success ? str : errMsg, BasPdcSequence = tmpBasPdcSequence }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetBasPdcSequenceById
        public JsonResult GetBasPdcSequenceById(string BasPdcSequenceId)
        {
            var BasPdcSequence = _BasPdcSequenceDBAccess.Get(BasPdcSequenceId);
            return Json(new { Success = true, BasPdcSequence = BasPdcSequence }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region BasAdjustmentTable

        #region GetBasAdjustmentTable
        public JsonResult GetBasAdjustmentTable(GridParam gp)
        {
            int pageCount, totalCount;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("AdjustmentType", Request.QueryString["AdjustmentType"]);
            var data = _BasAdjustmentTableDBAccess.GetList(gp, dic, out pageCount, out totalCount).ToList();

            var jsonData = new
            {
                total = pageCount, //totalpages
                page = gp.page,
                records = totalCount,
                rows = (from c in data
                        select new
                        {
                            cell = new string[] {
                                    c.PrintDate.HasValue ? c.PrintDate.Value.ToString("yyyy-MM-dd") : "",
                                    c.TnNo,
                                    c.TnProperty
                            }
                        }
                        ).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AdjustmentTableImport
        /// <summary>
        ///Adjustment Table Import
        /// </summary>
        public string AdjustmentTableImport()
        {
            string errMsg;
            string adjustmentType = Request.QueryString["AdjustmentType"];
            //上传文件保存到服务器,返回保存的路径
            string strPath = Import("Adjustment Table", out errMsg);
            if (!strPath.IsNullString())
            {
                _BasAdjustmentTableDBAccess.AdjustmentTableImport(strPath, adjustmentType, out errMsg);
            }
            else
            {
                if (errMsg.IsNullString())
                {
                    errMsg = "<=uploadError>";
                }
            }
            return errMsg + "</html>";
        }
        #endregion

        #endregion

        #region Import
        /// <summary>
        /// Common Function
        /// </summary>
        /// <param name="type"></param>
        /// <param name="strMessage">Message</param>
        /// <returns>Import File Path</returns>
        public string Import(string type, out string strMessage)
        {
            string strFilePath = string.Empty;
            strMessage = "<=ImportSuccess>";
            try
            {
                if (Request.Files.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Request.Files[0].FileName) && Request.Files[0].ContentLength != 0)
                    {
                        string strextension = System.IO.Path.GetExtension(Request.Files[0].FileName);
                        string strFileName = System.IO.Path.GetFileNameWithoutExtension(Request.Files[0].FileName);
                        if (strextension.ToLower() == ".xls" || strextension.ToLower() == ".xlsx")
                        {
                            //服务器存放上传文件的路径
                            string strPath = Server.MapPath("~/Upload/" + type);
                            if (!System.IO.Directory.Exists(strPath))
                            {
                                System.IO.Directory.CreateDirectory(strPath);
                            }
                            strFilePath = strPath + "\\" + strFileName + Guid.NewGuid().ToString() + strextension;
                            if (System.IO.File.Exists(strFilePath))
                                System.IO.File.Delete(strFilePath);
                            Request.Files[0].SaveAs(strFilePath);
                        }
                        else
                        {
                            strMessage = "<=validationExcel>";
                        }
                    }
                    else
                    {
                        strMessage = "<=uploadError>";
                    }
                }
                else
                {
                    strMessage = "<=FileIsRequired>";
                }
            }
            catch (Exception exception)
            {
                strMessage = "<=ImportError>:" + exception.Message;
            }

            return strFilePath;
        }
        #endregion

        #region ExportBasNormalSchedule
        //导出用户导入进来的Schedule
        public JsonResult ExportBasNormalScheduleS(FormCollection collection)
        {
            string BasNormalScheduleId = Request.Form["BasNormalScheduleId"];
            string ScheduleType = Request.Form["ScheduleType"];
            var BasNormalSchedule = _BasNormalScheduleDBAccess.Get(BasNormalScheduleId);
            bool flag = BasNormalSchedule == null ? false : true;
            string fullPath = "";
            switch (ScheduleType)
            {
                case "s":
                    fullPath = BasNormalSchedule.NormalScheduleSFileName;
                    break;
                case "vct":
                    fullPath = BasNormalSchedule.NormalScheduleVCtFileName;
                    break;
                case "vtm":
                    fullPath = BasNormalSchedule.NormalScheduleVTmFileName;
                    break;
                default:
                    break;
            }
            string schedulePath = "";
            if (fullPath != "" && fullPath !=null)
            {
                int idx = fullPath.IndexOf("Upload");
                int strLength = fullPath.Length;
                schedulePath = "../../" + fullPath.Replace(@"\", @"/").Substring(idx, strLength - idx);
            }
            else
            {
                flag = false;
            }
            return Json(new
            {
                Success = flag,
                FileName = schedulePath,
                msg = "not find!"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportSpecialSchedule(FormCollection collection)
        {
            string SpecialScheduleIndexID = Request.Form["SpecialScheduleIndexID"];
            var SpecialSchedule = _BasSpecialScheduleIndexDBAccess.Get(SpecialScheduleIndexID);
            bool flag = SpecialSchedule == null ? false : true;
            string fullPath = SpecialSchedule.SpecilScheduleFile;
            string schedulePath = "";
            if (fullPath != "" && fullPath != null)
            {
                int idx = fullPath.IndexOf("Upload");
                int strLength = fullPath.Length;
                schedulePath = "../../" + fullPath.Replace(@"\", @"/").Substring(idx, strLength - idx);
            }
            else
            {
                flag = false;
            }
            return Json(new
            {
                Success = flag,
                FileName = schedulePath,
                msg = "not find!"
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
