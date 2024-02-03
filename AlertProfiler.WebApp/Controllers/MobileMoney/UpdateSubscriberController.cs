using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.WebApp.Controllers;
using System;
using System.Web.Mvc;

namespace AlertProfiler.Web.Controllers
{
    public class UpdateSubscriberController : BaseController
    {

       
        private DateTime ActionStartTime = DateTime.Now;
        private string className = "UpdateSubscriberController";
        private LoginResponse userData;
        public UpdateSubscriberController()
        {
            string methodName = "UpdateSubscriberController";

            try
            {
                //userData.UserId = System.Web.HttpContext.Current.Session["Username"] != null ? System.Web.HttpContext.Current.userData.UserId : string.Empty;
                //userData.BranchCode = System.Web.HttpContext.Current.Session["BranchCode"] != null ? System.Web.HttpContext.Current.BaseService.GetAppSetting("BranchCode").ToString() : string.Empty;

                //if (string.IsNullOrEmpty(userData.UserId))
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
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index");
        }

        [HttpPost]
        [ActionName("GetAccountListAction")]
        public ActionResult GetAccountList(AccountListByPhoneNumberRequest request)
        {
            string methodName = "GetAccountList";
            var response = new AccountListByPhoneNumberResponse();

            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                response = UpdateSubsriberService.GetAccountListByPhoneNumberService(request);
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

            return View("UpdateSubscriber", response);
        }

        [HttpPost]
        [ActionName("UpdateSubscriberAction")]
        public ActionResult UpdateSubscriber(UpdateSubsriberRequest request)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "UpdateSubscriber";
            var result = new UpdateResponse();
            var response = new Response();
            request.BranchCode = userData.BranchCode;
            request.UserId = userData.UserId;

            try
            {
                result = UpdateSubsriberService.UpdateService(request);
               
                    string eventbefore = $"Suscribes";
                    string eventafter = $"Updated Suscription";

                    AuditLogService.CreateService(ActionStartTime, ActionEnum.CREATERECORD, "Subscribe User", userData.UserId, "-", result, userData.BranchCode, eventbefore, eventafter, userData.CountryId);

               

                response.RequestId = result.RequestId;
                response.ResponseCode = result.ResponseCode;
                response.ResponseMessage = result.ResponseMessage;
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index", response);
        }

        [HttpPost]
        [ActionName("GetOtherAccountListAction")]
        public ActionResult GetOtherAccountList(AccountListByPhoneNumberRequest request)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "GetOtherAccountList";
            var response = new AccountListByPhoneNumberResponse();
            try
            {
                //response = SubscribeService.GetAccountListByPhoneNumberService(request);
                //AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", response);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return null;
        }


    }
}