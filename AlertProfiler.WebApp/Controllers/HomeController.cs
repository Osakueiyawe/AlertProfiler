using System;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json;
using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using System.Runtime.InteropServices;
using AlertProfiler.BusinessCore.Enum;
using AlertProfiler.CoreObject.Data;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using AlertProfiler.WebApp.Controllers;
using System.Web.Security;
using System.Web;
using System.Diagnostics.PerformanceData;
using AlertProfiler.WebApp.Settings;

namespace AlertProfiler.Web
{
    public class HomeController : BaseController
    {
        private string countryId = BaseService.GetAppSetting("countryId");
        private bool IsADactivated = bool.Parse(BaseService.GetAppSetting("IsADactivated"));
        private static bool IsBusinessDayActivated = bool.Parse(BaseService.GetAppSetting("IsBusinessDayActivated"));
        private static bool IsBusinessTimeActivated = bool.Parse(BaseService.GetAppSetting("IsBusinessTimeActivated"));
        private static int applicationTimeOut = int.Parse(BaseService.GetAppSetting("SessionThreshold"));
        private string className = "HomeController";

        public ActionResult Index()
        {
            var response = new LoginResponse();
            AuditLogService.CreateService(DateTime.Now, ActionEnum.LAUNCHPAGE, "site launched", "NA", "site launched", response, "",countryId);
            return View("login", response);
        }

        public ActionResult Login()
        {
            var response = new LoginResponse();
            return View("login", response);
        }

        [HttpPost]
       // [ActionName("LoginAction")]
        public ActionResult Login(LoginRequest request)
        {
            string methodName = "LogIn";
            var response = new LoginResponse();
            var password = request.Password;
            request.Password = null;

            try
            {
               

                request.Password = password;

                if (ModelState.IsValid)
                {
                    if (IsBusinessDayActivated == true)
                    {
                        #region Day Confirmation
                        DateTime today = DateTime.Today;
                        if (today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday)
                        {
                            response.UserId = request.UserId;
                            response.Password = null;
                            response.ResponseCode = "01";
                            response.ResponseMessage = "Login allowed on business days only";
                            AuditLogService.CreateService(DateTime.Now, ActionEnum.LOGIN, "Login", request.UserId, "login", response.ResponseMessage, "NA",countryId);
                            LogService.LogInfo(countryId, className, methodName, "business day is not today, login failed");
                            return View(response);
                        }
                        #endregion

                        #region Time Confirmation
                        if (IsBusinessTimeActivated==true)
                        {
                            int startTime = int.Parse(BaseService.GetAppSetting("StartTime"));
                            int closingTime = int.Parse(BaseService.GetAppSetting("ClosingTime"));
                            TimeSpan startOfBusiness = new TimeSpan(startTime, 0, 0);
                            TimeSpan closeOfBusiness = new TimeSpan(closingTime, 0, 0);
                            TimeSpan TimeofDay = DateTime.Now.TimeOfDay;
                            if (TimeofDay < startOfBusiness || TimeofDay > closeOfBusiness)
                            {
                                response.UserId = request.UserId;
                                response.Password = null;
                                response.ResponseCode = "01";
                                response.ResponseMessage = "Login allowed within business hours only";
                                AuditLogService.CreateService(DateTime.Now, ActionEnum.LOGIN, "Login", request.UserId, "login", response.ResponseMessage, "NA",countryId);
                                LogService.LogInfo(countryId, className, methodName, "business time has expired, login failed");
                                return View(response);
                            }
                        }
                      
                        #endregion
                    }
                    if (IsADactivated==true)
                    {

                        #region AD
                        request.CountryId = countryId;

                        //LogService.LogInfo(countryId, className, methodName, "Request Details \r\n" + JsonConvert.SerializeObject(request));

                        var result = LoginService.ValidateAndGetUserDetailService(request);
                        if (result.ResponseCode != "00")
                        {
                            response.UserId = request.UserId;
                            response.Password = null;
                            response.ResponseCode = "01";
                            response.ResponseMessage = result.ResponseMessage;
                            AuditLogService.CreateService(DateTime.Now, ActionEnum.LOGIN, "Login", request.UserId, "login", response.ResponseMessage, "NA",countryId);
                            LogService.LogInfo(countryId, className, methodName, $"login response is {result.ResponseMessage} ");
                            return View(response);
                        }

                        if (result.ResponseCode == "00")
                        {
                            if (string.IsNullOrEmpty(result?.CountryId))
                            {
                                response.UserId = request.UserId;                               
                                response.ResponseCode = "01";
                                response.ResponseMessage = "Login Failed, Unable to retrieve country code, please contact admin or try to login again";
                                LogService.LogInfo(countryId, className, methodName, response.ResponseMessage);
                                return RedirectToAction("Login", "Home");
                                //return View(response);
                            }

                          
                            string userData = JsonConvert.SerializeObject(result);                            
                            System.Web.HttpContext.Current.Session["userData"] = userData;
                            Session["username"] = result.Username;
                            Session["displayname"] = result.DisplayName;
                            HttpCookie userInfo = new HttpCookie("userData");
                            userInfo["userData"] = userData; 
                            userInfo.Expires.Add(new TimeSpan(0, 15, 0));
                            Response.Cookies.Add(userInfo);

                            Session["lang"] = SetLangauge.SetLangaugeSession(result.CountryId);

                            AuditLogService.CreateService(DateTime.Now, ActionEnum.LOGIN, "Login", request.UserId, "login", "login successful", result?.BranchCode, result?.CountryId);
                            AuditLogService.loguser(DateTime.Now, request.UserId, request.UserId, "", result?.BranchCode, result?.CountryId);

                            LogService.LogInfo(countryId, className, methodName, "Result Details \r\n" + JsonConvert.SerializeObject(result));

                            return RedirectToAction("Home", "Home");

                        }
                        #endregion
                    }
                    else
                    {
                        #region allowanyuser
                        if ( request.Password == "12345")
                        {
                           
                            response.DisplayName = "Test User";
                            response.UserId = request.UserId;
                            response.Username = request.UserId;
                            response.RoleId = "1";
                            response.RoleName = "maker";
                            response.BranchCode = BaseService.GetAppSetting("BranchCode").ToString();                           
                            response.BranchName = "Head Office";
                            response.FinacleUserId = "1";
                            response.RoleAction = "maker";
                            response.CountryId = request.CountryId;
                            Session["countryId"] = request.CountryId;
                            Session["username"] = response.Username;
                            Session["displayname"] = response.DisplayName;

                            Session["lang"] = SetLangauge.SetLangaugeSession(response.CountryId);


                            string userData = JsonConvert.SerializeObject(response);
                            System.Web.HttpContext.Current.Session["userData"] = userData;

                            HttpCookie userInfo = new HttpCookie("userData");
                            userInfo["userData"] = userData;
                            userInfo.Expires.Add(new TimeSpan(0, 15, 0));
                            Response.Cookies.Add(userInfo);

                            return RedirectToAction("Home", "Home");
                        }
                        else if (request.Password == "54321")
                        {
                           
                            response.DisplayName = "Test User";
                            response.UserId = request.UserId;
                            response.Username = request.UserId;
                            response.RoleId = "1";
                            response.RoleName = "checker";
                            response.BranchCode = BaseService.GetAppSetting("BranchCode").ToString();
                            response.BranchName = "Head Office";
                            response.FinacleUserId = "1";
                            response.RoleAction = "checker";
                            response.CountryId = BaseService.GetAppSetting("countryId");

                            Session["username"] = response.Username;
                            Session["displayname"] = response.DisplayName;

                            Session["lang"] = SetLangauge.SetLangaugeSession(response.CountryId);

                            string userData = JsonConvert.SerializeObject(response);
                            System.Web.HttpContext.Current.Session["userData"] = userData;

                            HttpCookie userInfo = new HttpCookie("userData");
                            userInfo["userData"] = userData;
                            userInfo.Expires.Add(new TimeSpan(0, 15, 0));
                            Response.Cookies.Add(userInfo);



                            return RedirectToAction("Home", "Home");
                        }
                        else if (request.Password == "1")
                        {
                           
                            response.DisplayName = "Test User";
                            response.UserId = request.UserId;
                            response.Username = request.UserId;
                            response.RoleId = "1";
                            response.RoleName = "support";
                            response.BranchCode = BaseService.GetAppSetting("BranchCode").ToString();                            
                            response.BranchName = "Head Office";
                            response.FinacleUserId = "1";
                            response.RoleAction = "support";
                            response.CountryId = BaseService.GetAppSetting("countryId");

                            Session["username"] = response.Username;
                            Session["displayname"] = response.DisplayName;

                            Session["lang"] = SetLangauge.SetLangaugeSession(response.CountryId);

                            string userData = JsonConvert.SerializeObject(response);
                            System.Web.HttpContext.Current.Session["userData"] = userData;

                            HttpCookie userInfo = new HttpCookie("userData");
                            userInfo["userData"] = userData;
                            userInfo.Expires.Add(new TimeSpan(0, 15, 0));
                            Response.Cookies.Add(userInfo);

                            return RedirectToAction("Home", "Home");
                        }

                        else
                        {
                            response.UserId = request.UserId;
                            response.Password = null;
                            response.ResponseCode = "01";
                            response.ResponseMessage = "Login Failed!";
                            AuditLogService.CreateService(DateTime.Now, ActionEnum.LOGIN, "Login", request.UserId, "login", response.ResponseMessage, "",countryId);
                            
                            return View(response);
                        }

                        #endregion
                    }


                }
            }
            catch (Exception ex)
            {
                response.UserId = request.UserId;
                response.ResponseCode = "01";
                response.ResponseMessage = "Cannot login at this time!";
                AuditLogService.CreateService(DateTime.Now, ActionEnum.LOGIN, "Login", request.UserId, "login", response.ResponseMessage, BaseService.GetAppSetting("BranchCode").ToString(), countryId);

                LogService.LogError(countryId, className, methodName, ex);
            }

            LogService.LogInfo(countryId, className, methodName, "routing back to login with response");
            return View(response);
        }

        public ActionResult LogOut()
        {
            string methodName = "LogOut";
            var response = new LoginResponse();
            try
            {
               var userData = RetrieveUserInfo();
                if (userData == null)
                {
                    LogService.LogInfo(countryId, className, methodName, "logging out");
                   return  RedirectToAction("Login", "Home");
                }
                LogService.LogInfo(countryId, className, methodName, "Request Details \r\n" + "Logged Out");
                AuditLogService.CreateService(DateTime.Now, ActionEnum.LOGOUT, "Logout", userData.UserId, "logout", "user logged out", userData.BranchCode, countryId);


                Session.Clear();
                Session.RemoveAll();

                //FormsAuthentication.SignOut();

                response.Password = null;
                response.ResponseMessage = "User Logged Out.";
                response.ResponseCode = "00";
                response.UserId = null;
                response.Password = null;
            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
            }

            return View("login", response);
        }

        public ActionResult Error(string errorCode)
        {
            ViewBag.ErrorCode = errorCode;
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult SessionTimeOut()
        {
            return View();
        }

        private string GetUrlTargetMessage(string returnUrl)
        {
            string methodName = "GetUrlTargetMessage";
            try
            {
                returnUrl = returnUrl.Replace(Helper.GetRootURL(), string.Empty);
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["VirtualDirectory"]))
                {
                    returnUrl = returnUrl.Replace(ConfigurationManager.AppSettings["VirtualDirectory"], string.Empty).ToLower();
                }
            }
            catch(Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
            }

            return string.Empty;
        }


        public ActionResult Home()
        {
            string methodName = "Home Action";
           // LogService.LogInfo(countryId, className, methodName, "withing home action");
            var userData = RetrieveUserInfo();
            if (userData == null)
            {
                LogService.LogInfo(countryId, className, methodName, "userdata is null routing to login");
                RedirectToAction("Login", "Home");
            }
            if (string.IsNullOrEmpty(userData?.Username))
            {
                RedirectToAction("Login", "Home");
            }
           
            using (AlertContext context=new AlertContext())
            {
               
               // var loginUser=context.UserLog.Where(x=>x.Email== username)?.OrderByDescending(x=>x.ID)?.Skip(1)?.Take(1);
                var loginUser=context.UserLog.Where(x=>x.Email== userData.Username && countryId==userData.CountryId)?.OrderByDescending(x=>x.ID);
                if (loginUser==null)
                {
                   
                    ViewBag.Date = "NA";
                }
                if (loginUser.Count() <= 0)
                {

                    ViewBag.Date = "NA";
                }
                if (loginUser.ToList().Count==1)
                {
                    ViewBag.Date = loginUser.FirstOrDefault().LastLoggedInDate;
                }
                else if (loginUser.Count() > 1)
                {
                    ViewBag.Date = loginUser?.Skip(1)?.Take(1).FirstOrDefault().LastLoggedInDate;
                }
            }
          //  LogService.LogInfo(countryId, className, methodName, "loading home view");
            return View();
        }

        public ActionResult AuditReport()
        {
            string methodName = "AuditReport";
            try
            {
                var userData = RetrieveUserInfo();
                if (userData == null)
                {
                    RedirectToAction("Login", "Home");
                }
                if (string.IsNullOrEmpty(userData.Username))
                {
                    RedirectToAction("Login", "Home");
                }
               
                using (AlertContext context = new AlertContext())
                {
                    var data = context.AuditTrail.Where(x=>x.CountryId==userData.CountryId).OrderByDescending(x=>x.ID).Take(50);
                    
                    return View(data.ToList());
                }
              
            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
                return View();
            }
            
        }


        public ActionResult AddUser()
        {
            string methodName = "AddsSer";
            try
            {
                var user = RetrieveUserInfo();
                if (user == null)
                {
                    RedirectToAction("Login", "Home");
                }
                using (AlertContext context = new AlertContext())
                {
                    var data = context.AuditTrail.OrderByDescending(x => x.ID).ToList();

                    return View(data);
                }

            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
                return View();
            }

        }

        [HttpPost]
        public FileContentResult Export()
        {
            string methodName = "Export";
            try
            {
                var user = RetrieveUserInfo();
                if (user == null)
                {
                    RedirectToAction("Login", "Home");
                }
                using (AlertContext context = new AlertContext())
                {
                    var data = context.AuditTrail.OrderByDescending(x => x.ID).ToList();

                    if (data.Count()<=0)
                    {
                        return null;
                    }
                    List<object> customers = (from audit in data
                                              select new[] {audit.SolID??"NA",
                                                            audit.Username??"NA",
                                                            audit.ClientIPAddress??"NA",
                                                            audit.AuditPage??"NA",
                                                            audit.AuditAction??"NA",                                                           
                                                            audit.AuditType??"NA",
                                                            audit.AuditMessage??"NA",
                                                            audit.AuditData??"NA",
                                                            audit.EventBefore??"NA",
                                                            audit.EventAfter??"NA",
                                                            audit.ActionStartTime.ToString("MM/dd/yyyy h:mm tt")??"NA",
                                                            audit.ActionEndTime.ToString("MM/dd/yyyy h:mm tt")??"NA"
                                }).ToList<object>();

                    //Insert the Column Names.
                    customers.Insert(0, new string[12] { "SolID", "Username", "ClientIPAddress", "AuditPage", "AuditAction", "AuditType", "AuditMessage", "AuditData", "EventBefore", "EventAfter", "ActionStartTime", "ActionEndTime" });

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

                    return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Grid.csv");
                }

            }
            catch (Exception ex)
            {
                LogService.LogError(countryId, className, methodName, ex);
                return  null;
            }

            
        }

       

    }

}