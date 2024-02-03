using AlertProfiler.BusinessCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlertProfiler.WebApp.Settings
{
    public class SetLangauge
    {
        static string className = "SetLangauge";
        public static string SetLangaugeSession(string countryCode)
        {
            string methodName = "SetLangaugeSession";
            try
            {
                switch (countryCode)
                {
                    case "01":
                    case "02":
                    case "06":
                    case "07":
                        return BaseService.GetAppSetting("English");
                    case "03":
                    case "04":
                    case "05":
                        return BaseService.GetAppSetting("French");
                    
                    default:
                        return BaseService.GetAppSetting("English");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(countryCode, className, methodName, ex);
                return   BaseService.GetAppSetting("English");
            }
        }
    }
}