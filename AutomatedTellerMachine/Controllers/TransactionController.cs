using AutomatedTellerMachine.Models;
using AutomatedTellerMachine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutomatedTellerMachine.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Deposit(int checkingAccountId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Deposit(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                CheckingAccount acc = db.CheckingAccounts.Where(a => a.Id == transaction.CheckingAccountId).First();
                db.Transactions.Add(transaction);
                db.SaveChanges();

                var serv = new CheckingAccountService(db);
                serv.UpdateBalance(acc.Id);

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Withdraw(int checkingAccountId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Withdraw(Transaction transaction)
        {
            CheckingAccount acc = db.CheckingAccounts.Where(a => a.Id == transaction.CheckingAccountId).First();
            if (acc.Balance < transaction.Amount)
            {
                ModelState.AddModelError("Amount", "You have insuficient funds!");
            }
            if (ModelState.IsValid)
            {
                transaction.Amount *= -1;
                db.Transactions.Add(transaction);

                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult QuickCash(int accId)
        {
            //CheckingAccount acc = db.CheckingAccounts.Where(a => a.Id == accId).First();
            //acc.Balance -= 100;
            Transaction transaction = new Transaction { Amount = -100, CheckingAccountId = accId };
            db.Transactions.Add(transaction);
            db.SaveChanges();

            var serv = new CheckingAccountService(db);
            serv.UpdateBalance(accId);

            return View();
        }

        public ActionResult Statement(int accId)
        {
            
            List<Transaction> tranList = db.Transactions.Where(t => t.CheckingAccountId == accId).ToList();
            return View(tranList.Skip(tranList.Count - 10));
        }
    }
}