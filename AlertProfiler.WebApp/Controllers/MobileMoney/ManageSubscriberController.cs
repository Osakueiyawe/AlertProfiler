using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.WebApp.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace AlertProfiler.Web.Controllers
{
    public class ManageSubscriberController : BaseController
    {

        private static string countryId = BaseService.GetAppSetting("countryId");
        private DateTime ActionStartTime = DateTime.Now;
        private string className = "ManageSubscriberController";
        private LoginResponse userData;

        public ManageSubscriberController()
        {
            string methodName = "ManageSubscriberController";

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
                LogService.LogError(countryId, className, methodName, ex);
            }
        }


        // GET: ManageSubscriber
        public ActionResult Index()
        {
            string methodName = "MoMo Approval Index Page";
            var response = new ManageSubscriberResponse();
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var request = new ManageSubscriberRequest()
            {
                BranchCode = userData.BranchCode,
                UserId=userData.UserId,
                CountryId=userData.CountryId
            };

            try
            {
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageSubscriberService.GetSubscribersByBranchCodeService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details, total records fetched for approval is:  \r\n" + response?.Subscribers?.Count);

                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "Manage Subscriber", userData.UserId, "subscribe users for ussd", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
            }

            return View("Index", response);
        }

        //Get Subscriber Between Date Range
        [HttpPost]
        [ActionName("SubscriberBetweenDateRangeAction")]
        public ActionResult SubscriberBetweenDateRange(ManageSubscriberRequest request)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "SubscriberBetweenDateRange MTN MOMO";
            var response = new ManageSubscriberResponse();
            request.BranchCode = userData.BranchCode;

            try
            {
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageSubscriberService.GetSubscriberBetweenDateRange(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details, total records fetched for approval is:  \r\n" + response?.Subscribers?.Count);

                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
            }

            return View("Index", response);
        }

        //Approve All Subscriber
        [HttpPost]
        [ActionName("ApproveAllSubscriberAction")]
        public ActionResult ApproveAllSubscriber(ManageSubscriberRequest request)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "ApproveAllSubscriber";
            var response = new ManageSubscriberResponse();
            request.BranchCode = userData.BranchCode;
            request.UserId = userData.UserId;
            request.RequestId = Guid.NewGuid().ToString();

            try
            {
                response = ManageSubscriberService.ApproveAllSubscriberService(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
            }

            return View("Index", response);
        }

        //DisApprove All Subscriber
        [HttpPost]
        [ActionName("DisApproveAllSubscriberAction")]
        public ActionResult DisApproveAllSubscriber(ManageSubscriberRequest request)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "DisApproveAllSubscriber";
            var response = new ManageSubscriberResponse();
            request.BranchCode = userData.BranchCode;
            request.UserId = userData.UserId;
            request.RequestId = Guid.NewGuid().ToString();

            try
            {
                response = ManageSubscriberService.DisApproveAllSubscriberService(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
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
                UserId = userData.UserId,
                CountryId=userData.CountryId            
            };

            try
            {
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageSubscriberService.ApproveSingleSubscriberService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(response));

                AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
            }

            return View("Index", response);
        }

        //DisApprove Single Subscriber
        [HttpGet]
        public ActionResult DisApprove(string id)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "DisApprove";
            var response = new ManageSubscriberResponse();
            var request = new ManageSubscriberRequest()
            {
                BranchCode = userData.BranchCode,
                RequestId = id,
                UserId = userData.UserId,
                CountryId=userData.CountryId
            };

            try
            {
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageSubscriberService.DisApproveSingleSubscriberService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(response));

                AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
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
            try
            {
                var response = ReportService.GetSubscribersByBranchCodeService();
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);

                return View(response);
            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
            }

            return View("Index");
        }

        [HttpPost]
        public FileContentResult Export()
        {
            string methodName = "Export";
            try
            {


                var data = ReportService.GetSubscribersByBranchCodeService().ReportList;

                if (data.Count <= 0)
                {
                    return null;
                }
                List<object> customers = (from audit in data
                                          select new[] {audit.BranchCode??"NA",
                                                            audit.AccountNumber??"NA",
                                                            audit.PhoneNumber??"NA",
                                                            audit.RequestId??"NA",
                                                            audit.IsApproved.ToString(),                                                           
                                                            audit.CreatedBy??"NA",
                                                            audit.DateCreated.ToString("MM/dd/yyyy h:mm tt")??"NA",
                                                            audit.Description??"NA",
                                                            audit.ApprovedBy??"NA",
                                                            audit.DateLastUpdated.ToString("MM/dd/yyyy h:mm tt")??"NA"
                                              }).ToList<object>();


                //Insert the Column Names.
                customers.Insert(0, new string[10] { "BranchCode", "AccountNumber", "PhoneNumber", "RequestId", "IsApproved",  "CreatedBy", "DateCreated", "Description", "ApprovedBy", "DateLastUpdated" });

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < customers.Count; i++)
                {
                    string[] customer = (string[])customers[i];
                    for (int j = 0; j < customer.Length; j++)
                    {
                        //Append data with separator.
                        sb.Append(customer[j] + ',');
                    }

                    //Append new line character.
                    sb.Append("\r\n");

                }

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "MTNMoMo.csv");
            }


            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
                return null;
            }


        }

        [HttpPost]
        [ActionName("ReportAction")]
        public ActionResult SubscriberReport(ReportRequest request)
        {
            string methodName = "SubscriberReport";
            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                request.BranchCode = userData.BranchCode;
                var response = ReportService.GetReportBetweenDateRange(request);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);

                return View("Report", response);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index");
        }
    }
}