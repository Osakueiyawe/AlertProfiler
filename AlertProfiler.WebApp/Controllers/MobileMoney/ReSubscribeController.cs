using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using System;
using System.Web.Mvc;

namespace AlertProfiler.WebApp.Controllers
{
    public class ReSubscribeController : BaseController
    {
      
        private DateTime ActionStartTime = DateTime.Now;
        private string className = "ReSubscribeController";
        private LoginResponse userData;

        public ReSubscribeController()
        {
            string methodName = "ReSubscribeController";
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

        // GET: Resubscribe
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
                var BeforeResult = ReSubscribeService.GetAccountListByPhoneNumberService(new AccountListByPhoneNumberRequest() { PhoneNumber = request.PhoneNumber });

                result = ReSubscribeService.GetAccountListByPhoneNumberService(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "View Account Already Suscribed ", userData.UserId, "-", result, userData.BranchCode, userData.CountryId);
                if (result.ResponseCode == "00")
                {
                    string eventbefore = string.Empty;
                    foreach (var item in BeforeResult.ListAccountByAccountNumberResponse)
                    {
                        eventbefore += item + ";";
                    }
                    string eventafter = $"{userData.UserId} resuscribing  {request.PhoneNumber} ";
                    
                    AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "Resuscribe user", userData.UserId, "-", result, userData.BranchCode, eventbefore, eventafter, userData.CountryId);

                }
                else
                {
                    AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", result, userData.BranchCode, userData.CountryId);

                }
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

            return View("ReSubscribe", result);
        }


        [HttpPost]
        [ActionName("ReSubscribeAction")]
        public ActionResult Resubscribe(ReSubscribeRequest request)
        {
            string methodName = "Resubscribe";
            var result = new ReSubscribeResponse();
            var response = new Response();

            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                result = ReSubscribeService.UpdateService(request);
               
                AuditLogService.CreateService(ActionStartTime, ActionEnum.CREATERECORD, "Subscribe User", userData.UserId, "-", result, userData.BranchCode, "Unsuscribed", "ReSuscribed", userData.CountryId);
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