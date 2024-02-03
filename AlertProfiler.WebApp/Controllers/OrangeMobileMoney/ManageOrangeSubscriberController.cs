using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.WebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace AlertProfiler.Web.Controllers
{
    public class ManageOrangeSubscriberController : BaseController
    {
       
        private DateTime ActionStartTime = DateTime.Now;
        private string className = "ManageOrangeSubscriberController";
        private LoginResponse userData;

        public ManageOrangeSubscriberController()
        {
            string methodName = "ManageOrangeSubscriberController";

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


        // GET: ManageSubscriber
        public ActionResult Index()
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "Index";
            var response = new ManageSubscriberResponse();
            var request = new ManageSubscriberRequest()
            {
                BranchCode = userData.BranchCode
            };

            try
            {
                response = ManageOrangeSubscriberService.GetSubscribersByBranchCodeService(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "Manage Subscriber", userData.UserId, "subscribe users for ussd", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index", response);
        }

        //Approve Single Subscriber
        [HttpGet]
        public ActionResult Approve(string id)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "Approve";
            var response = new ManageSubscriberResponse();
            var request = new ManageSubscriberRequest()
            {
                BranchCode = userData.BranchCode,
                RequestId = id,
                UserId = userData.UserId
            };

            try
            {
                response = ManageOrangeSubscriberService.ApproveSingleSubscriberService(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index", response);
        }

        //DisApprove Single Subscriber
        [HttpPost]
        public ActionResult DisApprove(ManageSubscriberRequest request)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "DisApprove";
            var response = new ManageSubscriberResponse();
            request.BranchCode = userData.BranchCode;
            request.UserId = userData.UserId;
            

            try
            {
                response = ManageOrangeSubscriberService.DisApproveSingleSubscriberService(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index", response);
        }

        public ActionResult Report()
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "Report";
            var response = new ManageSubscriberResponse();
            var request = new ManageSubscriberRequest()
            {
                BranchCode = userData.BranchCode
            };

            try
            {
                response = ManageOrangeSubscriberService.GetSubscribersByBranchCodeService(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "Manage Subscriber", userData.UserId, "subscribe users for ussd", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index", response);
        }

    }
}