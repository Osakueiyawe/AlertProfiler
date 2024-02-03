using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AlertProfiler.WebApp.Controllers
{
    public class BaseController : Controller, System.Web.SessionState.IRequiresSessionState
    {
        // GET: Base
        

        //protected void NotifyUser(string message, NotificationType notificationType)
        //{
        //    var notification = new Notification(message, (int)notificationType);
        //    TempData["Notification"] = notification;
        //}

        //public void InsertAuditTrail(string userid, string fullname, int roleId, string branchCode, string countryId, AuditTrailAction action, decimal objectid, AuditTrailObjectType objecttype, string comments)
        //{
        //    var ipAddress = IpAddressService.DetermineIPAddress();
        //    var macAddress = IpAddressService.GetMacAddress();

        //    var audit = new AuditLog
        //    {
        //        CountryId = countryId,
        //        BranchCode = branchCode,
        //        UserId = userid,
        //        FullName = fullname,
        //        RoleId = roleId,
        //        IpAddress = ipAddress,
        //        MacAddress = macAddress,
        //        ActionType = action.ToString(),
        //        ObjectId = (int)objectid,
        //        ObjectType = objecttype.ToString(),
        //        Comments = comments,
        //        DateCreated = DateTime.Now
        //    };

        //    //Run insert as task
        //    new Task(() => { auditService.InsertAuditTrail(audit); }).Start();

        //    return;

        //}

        public static LoginResponse RetrieveUserInfo()
        {
           // LogService.LogInfo("01", "basecontroller", "RetrieveUserInfo", "about processing data");
            try
            {


                #region session
                if (System.Web.HttpContext.Current.Session["userData"] == null)
                {
                  //  LogService.LogInfo("01", "basecontroller", "RetrieveUserInfo", "session is null");
                    return null;
                }
                var responseString = System.Web.HttpContext.Current.Session["userData"] as string;
                #endregion
                #region cookie
                //string responseString = Request.Cookies["userData"].Value;
                //responseString = responseString.Substring(9, responseString.Length-9);
                #endregion
              //  LogService.LogInfo("01", "basecontroller", "RetrieveUserInfo", "session not null about desrializing");
                var response = JsonConvert.DeserializeObject<LoginResponse>(responseString);
               // LogService.LogInfo("01", "basecontroller", "RetrieveUserInfo", "rturnign deserialized data");
                return response;
            }
            catch (Exception ex)
            {
                LogService.LogError("00", "BaseController", "RetrieveUserInfo", ex);
                return null;
            }
        }
    }
}