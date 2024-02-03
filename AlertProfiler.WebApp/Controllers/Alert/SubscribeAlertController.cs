using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.WebApp.Controllers;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace AlertProfiler.Web.Controllers
{
    public class SubscribeAlertController : BaseController
    {
        private string userId = null;
        private string branchCode = null;
        private DateTime ActionStartTime = DateTime.Now;
        private string className = "SubscribeController";
        private LoginResponse userData;

        public SubscribeAlertController()
        {
            string methodName = "";

            try
            {
                //userId = System.Web.HttpContext.Current.Session["Username"] != null ? System.Web.HttpContext.Current.userData.UserId : string.Empty;
                //branchCode = System.Web.HttpContext.Current.Session["BranchCode"] != null ? System.Web.HttpContext.Current.BaseService.GetAppSetting("BranchCode").ToString() : string.Empty;

                //if (string.IsNullOrEmpty(userId))
                //{
                //    RedirectToAction("Login", "Home");
                //}
                userData = RetrieveUserInfo();
                if (userData ==null)
                {
                    RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }
        }

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
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWFORM, "-", userId, "-", response, branchCode, userData.CountryId);
                return View(response);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index");
            
        }

        [HttpPost]
        [ActionName("GetAccountDetailsAction")]
        public ActionResult GetAccountDetailsAction(AccountByAccountNumberRequest request)
        {
            string methodName = "GetAccountList";          
            var result = new AccountAndAlertResponse();
            var response = new Response();

            try
            {
                if (userData == null)
                {
                  return  RedirectToAction("Login", "Home");
                }

                request.CountryId = userData.CountryId;

                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                result = SuscribeMultipleAlertServices.GetAccountDetailsByAccountNumberService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(result));

                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userId, "-", result, branchCode, userData.CountryId);

                if (result.ResponseCode == "00" || result.ResponseCode =="100")
                {
                    return View("Subscribe", result);
                }
                else
                {
                    response.ResponseCode = result.ResponseCode;
                    response.ResponseMessage = result.ResponseMessage;
                    response.RequestId = result.RequestId;

                    return View("Index", response);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Subscribe", result);
        }

        [HttpPost]
        [ActionName("AlertSubscriberAction")]
        public ActionResult Subscribe(MultipleAlertSusbriberRequest request)
        {
            string methodName = "AlertSubscriberAction";
            var result = new AlertSuscriberResponse();
            var response = new Response();

            try
            {
                if (userData == null)
                {
                   return RedirectToAction("Login", "Home");
                }

                request.CountryId = userData.CountryId;
                request.RequestId = Guid.NewGuid().ToString();
                request.UserId = userData.UserId;
                request.BranchCode = userData.BranchCode;

                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                result = SuscribeMultipleAlertServices.CreateService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(result));

                string eventbefore = $"NA";
                string eventafter = $"Suscribed {request.AccountNumber}; approval is pending";

                AuditLogService.CreateService(ActionStartTime, ActionEnum.CREATERECORD, "Subscribe User", userId, "-", result, branchCode,eventbefore,eventafter, userData.CountryId);

                response.ResponseCode = result.ResponseCode;
                response.ResponseMessage = result.ResponseMessage;
                response.RequestId = result.RequestId;

            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index", response);
        }
    }
}