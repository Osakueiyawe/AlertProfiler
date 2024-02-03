using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.WebApp.Controllers;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace AlertProfiler.Web.Controllers
{
    public class SubscribeController : BaseController
    {
       
        private DateTime ActionStartTime = DateTime.Now;
        private string className = "SubscribeController";
        private LoginResponse userData;
        public SubscribeController()
        {
            string methodName = "SubscribeController";

            try
            {
               
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
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index");
        }

        [HttpPost]
        [ActionName("GetAccountListAction")]
        public ActionResult GetAccountList(AccountListByPhoneNumberRequest request)
        {
            string methodName = "GetAccountList";
            var result = new AccountListByPhoneNumberResponse();
            var response = new Response();
           
            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                request.CountryId = userData.CountryId;

               LogService.LogInfo(userData.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                result = SubscribeService.GetAccountListByPhoneNumberService(request);

                LogService.LogInfo(userData.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(result));

                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", result, userData.BranchCode, userData.CountryId);
                if (result.ResponseCode != "00")
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
        [ActionName("SubscribeAction")]
        public ActionResult Subscribe(SubscriberRequest request)
        {
            string methodName = "Subscribe";
            var result = new SubscriberResponse();
            var response = new Response();

            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                request.RequestId = Guid.NewGuid().ToString();
                request.UserId = userData.UserId;
                request.BranchCode = userData.BranchCode;

                LogService.LogInfo(userData.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                result = SubscribeService.CreateService(request);

                string eventbefore = $"NA";
                string eventafter = $"Suscribed {request.PhoneNumber} to {request.AccountNumber}; approval is pending";

                AuditLogService.CreateService(ActionStartTime, ActionEnum.CREATERECORD, "Subscribe User", userData.UserId, "-", result, userData.BranchCode,eventbefore,eventafter, userData.CountryId);

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