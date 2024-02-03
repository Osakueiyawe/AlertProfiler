using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AlertProfiler.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var logPath = AppDomain.CurrentDomain.BaseDirectory + @"\log4net.config";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(logPath));
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            var languageSession = "en";
            if (context != null && context.Session != null)
            {
                languageSession = context.Session["lang"] != null ? context.Session["lang"].ToString() : "en";
            }
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(languageSession);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(languageSession);

        }

        protected void Application_AcquireRequestState(Object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            var languageSession = "en";
            if (context != null && context.Session != null)
            {
                languageSession = context.Session["lang"] != null ? context.Session["lang"].ToString() : "en";
            }
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(languageSession);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(languageSession);
        }

    }
}
