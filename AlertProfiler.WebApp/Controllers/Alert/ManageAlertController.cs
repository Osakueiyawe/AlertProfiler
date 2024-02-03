using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.Data;
using AlertProfiler.CoreObject.DataTransferObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AlertProfiler.WebApp.Controllers
{
    public class ManageAlertController : BaseController
    {

        private DateTime ActionStartTime = DateTime.Now;
        private string className = "ManagerAlertController";
        private LoginResponse userData;

        public ManageAlertController()
        {
            string methodName = "ManagerAlertController";

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
            string methodName = "SMS Alert Approval Index Page";
            var response = new ManageAlertResponse();


            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                var request = new ManageAlertRequest()
                {
                    BranchCode = userData.BranchCode,
                    CountryId = userData.CountryId,
                    UserId = userData.UserId,
                    Role = userData.RoleId
                };

                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageAlertServices.GetSubscribersByBranchCodeService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details total records loaded for approval are: \r\n" + response?.Subscribers?.Count);

            }
            catch (Exception ex)
            {
                LogService.LogError(userData.CountryId, className, methodName, ex);
            }

            return View("Index", "_LayoutAlert", response);


        }

        //Get Subscriber Between Date Range
        [HttpPost]
        [ActionName("SubscriberBetweenDateRangeAction")]
        public ActionResult SubscriberBetweenDateRange(ManageAlertRequest request)
        {
            string methodName = "SubscriberBetweenDateRange";
            var response = new ManageAlertResponse();
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            request.BranchCode = userData.BranchCode;
            request.CountryId = userData.CountryId;
            request.UserId = userData.UserId;
            request.Role = userData.RoleId;
            try
            {
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageAlertServices.GetSubscriberBetweenDateRange(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(response));

                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index", response);
        }

        //Approve All Subscriber
        [HttpPost]
        [ActionName("ApproveAllSubscriberAction")]
        public ActionResult ApproveAllSubscriber(ManageAlertRequest request)
        {
            string methodName = "ApproveAllSubscriber";
            var response = new ManageAlertResponse();
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                request.CountryId = userData.CountryId;
                request.BranchCode = userData.BranchCode;
                request.UserId = userData.UserId;
                request.RequestId = Guid.NewGuid().ToString();
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageAlertServices.ApproveAllSubscriberService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(response));

                AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index", response);
        }


        public ActionResult DisApproveAllSubscriber(ManageAlertRequest request)
        {
            string methodName = "DisApproveAllSubscriber";
            var response = new ManageAlertResponse();


            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                request.CountryId = userData.CountryId;
                request.BranchCode = userData.BranchCode;
                request.UserId = userData.UserId;
                request.RequestId = Guid.NewGuid().ToString();

                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageAlertServices.DisApproveAllSubscriberService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(response));

                AuditLogService.CreateService(ActionStartTime, ActionEnum.UPDATERECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index", response);
        }

        //Approve Single Subscriber
        [HttpGet]
        public ActionResult Approve(string RequestId)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "Approve";
            var response = new ManageAlertResponse();
            var request = new ManageAlertRequest()
            {
                BranchCode = userData.BranchCode,
                RequestId = RequestId,
                UserId = userData.UserId,
                CountryId = userData.CountryId,
                Role = userData.RoleId
            };

            try
            {
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageAlertServices.ApproveSingleSubscriberService(request);

                //LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(response));

                LogService.LogInfo(request.CountryId, className, methodName, $"Result Details \r\n{response.ResponseCode}|{response.ResponseMessage}");

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
        public ActionResult DisApprove(string RequestId, string RejectionReason)
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "DisApprove";
            var response = new ManageAlertResponse();

            var request = new ManageAlertRequest()
            {
                CountryId = userData.CountryId,
                BranchCode = userData.BranchCode,
                RequestId = RequestId,
                UserId = userData.UserId,
                RejectionReason = RejectionReason
            };

            try
            {
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                response = ManageAlertServices.DisApproveSingleSubscriberService(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(response));

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
            string methodName = "Report";
            try
            {
                if (userData == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                LogService.LogInfo(userData.CountryId, className, methodName, "Request Details Country Id \r\n" + userData.CountryId);
                var response = ReportService.GetAlertReport(userData.CountryId);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);

                return View(response);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index");
        }

        public ActionResult FinacleAlertReport()
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string methodName = "Report";
            try
            {
                var response = ReportService.GetFinacleAlertReport(userData.CountryId);
                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);

                return View(response);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index");
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
                request.CountryId = userData.CountryId;
                request.BranchCode = userData.BranchCode;
                LogService.LogInfo(request.CountryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                var response = ReportService.GetReportBetweenDateRange(request);

                LogService.LogInfo(request.CountryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(response));

                AuditLogService.CreateService(ActionStartTime, ActionEnum.VIEWRECORD, "-", userData.UserId, "-", response, userData.BranchCode, userData.CountryId);

                return View("Report", response);
            }
            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
            }

            return View("Index");
        }

        [HttpPost]
        public FileContentResult Export()
        {
            string methodName = "Export";
            try
            {
                if (userData == null)
                {
                    RedirectToAction("Login", "Home");
                }

                var data = ReportService.GetAlertReport(userData.CountryId).AlertReportList;

                if (data.Count() <= 0)
                {
                    return null;
                }
                List<object> customers = (from audit in data
                                          select new[] {audit.BranchCode??"NA",
                                                            audit.AccountNumber??"NA",
                                                            audit.MobileNo??"NA",
                                                            audit.Email??"NA",
                                                            audit.IsApproved.ToString(),
                                                            audit.IsSMSAlert.ToString(),
                                                            audit.IsEmailAlert.ToString(),
                                                            audit.CreatedBy??"NA",
                                                            audit.DateCreated.ToString("MM/dd/yyyy h:mm tt")??"NA",
                                                            audit.Description??"NA",
                                                            audit.ApprovedBy??"NA",
                                                            audit.DateLastUpdated.ToString("MM/dd/yyyy h:mm tt")??"NA"
                                              }).ToList<object>();


                //Insert the Column Names.
                customers.Insert(0, new string[12] { "BranchCode", "AccountNumber", "MobileNo", "Email", "IsApproved", "IsSMSAlert", "IsEmailAlert", "CreatedBy", "DateCreated", "Description", "ApprovedBy", "DateLastUpdated" });

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

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "AlertRecords.csv");
            }


            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
                return null;
            }


        }

        [HttpPost]
        public FileContentResult ExportFinacleReport()
        {
            string methodName = "ExportFinacleReport";
            try
            {
                if (userData == null)
                {
                    RedirectToAction("Login", "Home");
                }
                var data = ReportService.GetFinacleAlertReport(userData.CountryId).FinacleAlertList;

                if (data.Count() <= 0)
                {
                    return null;
                }
                List<object> customers = (from audit in data
                                          select new[] {audit.USER_SOL??"NA",
                                                            audit.ACCOUNTNO??"NA",
                                                            audit.MOBILENO??"NA",
                                                            audit.EMAIL??"NA",
                                                            audit.STATUS_FLG.ToString(),
                                                            audit.SMS_ALERT_FLG.ToString(),
                                                            audit.EMAIL_ALERT_FLG.ToString(),
                                                            audit.CREATED_BY??"NA",
                                                            audit.CREATED_DATE.ToString("MM/dd/yyyy h:mm tt")??"NA"

                                              }).ToList<object>();


                //Insert the Column Names.
                customers.Insert(0, new string[9] { "BranchCode", "AccountNumber", "MobileNo", "Email", "IsApproved", "IsSMSAlert", "IsEmailAlert", "CreatedBy", "DateCreated" });

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

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "FinacleAlertRecords.csv");
            }


            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
                return null;
            }


        }
    }
}