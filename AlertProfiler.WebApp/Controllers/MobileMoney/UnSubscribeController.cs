using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.WebApp.Controllers;
using System;
using System.Web.Mvc;

namespace AlertProfiler.Web.Controllers
{
    public class UnSubscribeController : BaseController
    {
       
        private DateTime ActionStartTime = DateTime.Now;
        private string className = "UnSubscribeController";
        private LoginResponse userData;

        public UnSubscribeController()
        {
            string methodName = "UnSubscribeController";
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

        // GET: Unsubscribe
        public ActionResult Index()
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "Index";
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
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "GetAccountList";
            var result = new AccountListByPhoneNumberResponse();
            var response = new Response();

            try
            {
                result = UnSubscribeService.GetAccountListByPhoneNumberService(request);
             //   AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", result, userData.BranchCode);
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

            return View("UnSubscribe", result);
        }

        [HttpPost]
        [ActionName("UnSubscribeAction")]
        public ActionResult UnSubscribe(UnSubscribeRequest request)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "UnSubscribe";
            var response = new Response();
            var result = new UnSubscribeResponse();

            try
            {
                result = UnSubscribeService.UpdateService(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", result, userData.BranchCode, userData.CountryId);

                string eventbefore = $"Unsuscribed";
                string eventafter = $"UnSuscribed";

                AuditLogService.CreateService(ActionStartTime, ActionEnum.CREATERECORD, "UnSubscribe Account", userData.UserId, "-", result, userData.BranchCode, eventbefore, eventafter, userData.CountryId);



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