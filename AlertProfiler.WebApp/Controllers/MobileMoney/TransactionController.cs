using AlertProfiler.BusinessCore.Services;
using AlertProfiler.CoreObject.DataTransferObjects;
using AlertProfiler.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AlertProfiler.WebApp.Controllers.MobileMoney
{
    public class TransactionController : BaseController
    {

        private LoginResponse userData;
        private string className = "TransactionController";
        public TransactionController()
        {
            string methodName = "TransactionController";

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
        // GET: Transaction
        public ActionResult Index()
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var record = ManageTransactionRepository.GetWalletToAccountTransactionLog();
            return View(record);
        }

        public ActionResult AccountToWallet()
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var record = ManageTransactionRepository.GetAccountToWalletTransaction();
            return View(record);
        }

        public ActionResult WalletToAccount()
        {
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var record = ManageTransactionRepository.GetWalletToAccountTransactionLog();
            return View(record);
        }

        [HttpPost]
        public FileContentResult AccountToWalletExport()
        {
            string methodName = "AccountToWalletExport";
            try
            {
                if (userData == null)
                {
                    RedirectToAction("Login", "Home");
                }
                var data = ManageTransactionRepository.GetAccountToWalletTransaction();              

                if (data.Count() <= 0)
                {
                    return null;
                }
                List<object> customers = (from audit in data
                                          select new[] 
                                          {
                                                            audit.AccountNumber??"NA",
                                                            audit.WalletId??"NA",
                                                            audit.ClientReferenceId??"NA",
                                                            audit.Amount.ToString(),
                                                            audit.IsPosted.ToString(),
                                                            audit.NoOfAttempts.ToString(),
                                                            audit.DateCreated.ToString("MM/dd/yyyy h:mm tt"),
                                                            audit.LienName??"NA",
                                                            audit.CoreBankingResponseCode??"NA",
                                                            audit.CoreBankingResponseMessage??"NA",
                                                            audit.FinacleTranId??"NA",
                                                            audit.TransactionReference??"NA"

                                          }).ToList<object>();


                //Insert the Column Names.
                customers.Insert(0, new string[12] { "AccountNumber", "MobileNo", "MTN Reference No", "Amount", "Status", "NoOfAttempts", "Date", "Lien Status", "ResponseCode","ResponseMessage", "FinacleTranId","Finacle Reference" });

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

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "AccountToWallet.csv");
            }


            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
                return null;
            }


        }

        [HttpPost]
        public FileContentResult WalletToAccountExport()
        {
            string methodName = "WalletToAccountExport";
            try
            {
                if (userData == null)
                {
                    RedirectToAction("Login", "Home");
                }
                var data = ManageTransactionRepository.GetWalletToAccountTransactionLog();

                if (data.Count() <= 0)
                {
                    return null;
                }
                List<object> customers = (from audit in data
                                          select new[]
                                          {
                                                            audit.DestinationAccountNumber??"NA",
                                                            audit.WalletId??"NA",
                                                            audit.ClientReferenceId??"NA",
                                                            audit.Amount.ToString(),
                                                            audit.IsPosted.ToString(),
                                                            audit.RetrialCount.ToString(),
                                                            audit.RequestDate.ToString("MM/dd/yyyy h:mm tt"),
                                                            audit.Narration??"NA",
                                                            audit.CoreBankingResponseCode??"NA",
                                                            audit.CoreBankingResponseMessage??"NA",
                                                            audit.FinacleTranId??"NA",
                                                            audit.TransactionReference??"NA"

                                          }).ToList<object>();


                //Insert the Column Names.
                customers.Insert(0, new string[12] { "AccountNumber", "MobileNo", "MTN Reference No", "Amount", "Status", "NoOfAttempts", "Date", "Narration", "ResponseCode", "ResponseMessage", "FinacleTranId", "Finacle Reference" });

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

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "WalletToAccount.csv");
            }


            catch (Exception ex)
            {
                LogService.LogError("", className, methodName, ex);
                return null;
            }


        }
    }
}