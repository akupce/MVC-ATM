using AutomatedTellerMachine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using AutomatedTellerMachine.Services;

namespace AutomatedTellerMachine.Controllers
{
    public class TransfersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Transfers
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
        public ActionResult MakeTransfer(int checkingAccountId)
        {
            ViewBag.ReceiverId = new SelectList(db.CheckingAccounts.Where(a => a.Id != checkingAccountId), "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult MakeTransfer(Transfers transfer)
        {
            if (ModelState.IsValid)
            {

                var userId = User.Identity.GetUserId();
                transfer.SenderId = db.CheckingAccounts.Where(c => c.ApplicationUserId == userId).First().Id;

                CheckingAccount senderAcc = db.CheckingAccounts.Where(a => a.Id == transfer.SenderId).First();
                ViewBag.ReceiverId = new SelectList(db.CheckingAccounts.Where(a => a.Id != senderAcc.Id), "Id", "Name");
                if (senderAcc.Balance < transfer.Amount)
                {
                    //ViewBag.TheMessage = "Insufficient funds in the account";
                    ModelState.AddModelError("Amount", "You have insuficient funds!");
                    return View();
                }

                Transaction senderTrans = new Transaction();
                senderTrans.CheckingAccountId = transfer.SenderId;
                senderTrans.Amount = -transfer.Amount;
                db.Transactions.Add(senderTrans);

                CheckingAccount receiverAcc = db.CheckingAccounts.Where(a => a.Id == transfer.ReceiverId).First();
                Transaction receiverTrans = new Transaction();
                receiverTrans.Amount = transfer.Amount;
                //receiverAcc.Balance += transfer.Amount;
                receiverTrans.CheckingAccountId = transfer.ReceiverId;
                db.Transactions.Add(receiverTrans);
                db.SaveChanges();

                var serv = new CheckingAccountService(db);
                serv.UpdateBalance(senderAcc.Id);
                serv.UpdateBalance(receiverAcc.Id);

                //return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}