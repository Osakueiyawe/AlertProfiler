using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.WebApp.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlertProfiler.Web.Controllers.SMS
{
    public class UpdateAlertController : BaseController
    {

        //private string userId = null;
        //private string branchCode = null;
        private DateTime ActionStartTime = DateTime.Now;
        private string className = "UpdateAlertController";
        private LoginResponse userData;

        public UpdateAlertController()
        {
            string methodName = "UpdateAlertController";

            try
            {
                //userId = System.Web.HttpContext.Current.Session["Username"] != null ? System.Web.HttpContext.Current.userData.UserId : string.Empty;
                //branchCode = System.Web.HttpContext.Current.Session["BranchCode"] != null ? System.Web.HttpContext.Current.BaseService.GetAppSetting("BranchCode").ToString() : string.Empty;

                //if (string.IsNullOrEmpty(userId))
                //{
                //    RedirectToAction("Login", "Home");
                //}
                userData = RetrieveUserInfo();
                if (userData == null)
                {
                     RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }
        }

        // GET: UpdateSubscriber
        public ActionResult Index()
        {
            string methodName = "Index";

            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                var response = new Response();
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWFORM, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
                return View(response);
            }
            catch (Exception ex)
            {
                LogService.LogError(userData.CountryId, className, methodName, ex);
            }

            return View("Index");
        }

        [HttpPost]
        [ActionName("GetAccountListAction")]
        public ActionResult GetAccountList(AccountByAccountNumberRequest request)
        {
            string methodName = "GetAccountList";
            var response = new UpdateMultipleAlertResponse();

            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                request.CountryId = userData.CountryId;
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = UpdateMultipleAlertService.GetAccountByAccountNumber(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(response));
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);

                if (response.ResponseCode != "00")
                {
                    response.ResponseCode = response.ResponseCode;
                    response.ResponseMessage = response.ResponseMessage;
                    response.RequestId = response.RequestId;

                    return View("Index", response);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("UpdateProfile", response);
        }

        [HttpPost]
        [ActionName("UpdateProfile")]
        public ActionResult UpdateProfile(AlertSubscriberRequest request)
        {
            string methodName = "UpdateSubscriber";
            var result = new UpdateResponse();
            var response = new Response();
           

            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                request.BranchCode = userData.BranchCode;
                request.UserId = userData.UserId;
                request.CountryId = userData.CountryId;

                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                result = UpdateMultipleAlertService.UpdateService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(result));

                string eventbefore = $"Suscribes";
                string eventafter = $"Updated Suscription";

                AuditLogService.CreateService(ActionStartTime, ActionEnum.CREATERECORD, "Subscribe User", userData.UserId, "-", result, userData.BranchCode, eventbefore, eventafter, userData.CountryId);



                response.RequestId = result.RequestId;
                response.ResponseCode = result.ResponseCode;
                response.ResponseMessage = result.ResponseMessage;
            }
            catch (Exception ex)
            {
                LogService.LogError(userData.CountryId, className, methodName, ex);
            }

            return View("Index", response);
        }

        [HttpPost]
        [ActionName("GetOtherAccountListAction")]
        public ActionResult GetOtherAccountList(AccountListByPhoneNumberRequest request)
        {
            string methodName = "GetOtherAccountList";
            var response = new AccountListByPhoneNumberResponse();
            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                //response = SubscribeService.GetAccountListByPhoneNumberService(request);
                //AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userId, "-", response);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return null;
        }


    }
}