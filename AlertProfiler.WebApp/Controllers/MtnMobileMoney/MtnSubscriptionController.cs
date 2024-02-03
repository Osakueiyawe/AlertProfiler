using AlertProfiler.BusinessCore.BusinessLogic.MtnMomoAccountLinkage;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.CoreObject.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlertProfiler.WebApp.Controllers.MtnMobileMoney
{
    public class MtnSubscriptionController : Controller
    {
        // GET: MtnSubscription
        public ActionResult Index()
        {
            Response response = new Response();
            return View(response);
        }

        [HttpPost]
        public ActionResult LinkAccount(MtnAccountLinkage subscriberDetails)
        {
            Response response = new Response();
            try
            {
                subscriberDetails.countryId = Session["countryId"].ToString();
                response = AccountRegistration.LinkAccount(subscriberDetails);
            }
            catch (Exception ex)
            {

            }
            return View("Index",response);
        }
    }
}