using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.WebApp.Controllers;
using System;
using System.Web.Mvc;

namespace AlertProfiler.Web.Controllers
{
    public class SubscribeOrangeController : BaseController
    {
       
        private DateTime ActionStartTime = DateTime.Now;
        private string className = "SubscribeOrangeController";
        private LoginResponse userData;
        public SubscribeOrangeController()
        {
            string methodName = "SubscribeOrangeController";

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
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {                
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
                result = OrangeSuscriberService.GetAccountListByPhoneNumberService(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, methodName, userData.UserId, "Validating account details", result, userData.BranchCode, userData.CountryId);
                if (result.ResponseCode != "00")
                {
                    response.ResponseCode = result.ResponseCode;
                    response.ResponseMessage = result.ResponseMessage;
                    response.RequestId = result.RequestId;

                    return View("Index", response);
                }
                return View("Subscribe", result);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
                response.ResponseCode = "06";
                response.ResponseMessage = "System Error, kindly retry or contact admin if issues persist";
                return View("Index", response);
            }

           
        }

        [HttpPost]
        [ActionName("SubscribeAction")]
        public ActionResult Subscribe(OrangeSusbcriberRequest request)
        {
            string methodName = "Subscribe";
            var result = new SubscriberResponse();
            var response = new Response();

            try
            {
                if (userData == null)
                {
                  return  RedirectToAction("Login", "Home");
                }
                request.RequestId = Guid.NewGuid().ToString();
                request.UserId = userData.UserId;
                request.BranchCode = userData.BranchCode;
                
                result = OrangeSuscriberService.CreateService(request);

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