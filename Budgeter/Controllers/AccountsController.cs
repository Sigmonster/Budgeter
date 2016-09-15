using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;
using Microsoft.AspNet.Identity;

namespace Budgeter.Controllers
{
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        public ActionResult Index()
        {
            var currentUser = GetCurrentUser();
            var currentHousehold = db.Household.Find(currentUser.HouseholdId);
            var account = db.Account.Include(a => a.Household);
            return View(account.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Balance")] AccountCreateVM account)
        {
            if (ModelState.IsValid)
            {
                var currentUser = GetCurrentUser();
                var currentHousehold = db.Household.Find(currentUser.HouseholdId);
                var newAccount = new Account();
                newAccount.Name = account.Name;

                //newAccount.ReconciledBalance = (decimal)account.Balance;
                //newAccount.Balance = (decimal)account.Balance;// Set an automatic transaction create.
                newAccount.IsActive = true;
                newAccount.HouseholdId = currentHousehold.Id;
                db.Account.Add(newAccount);
                db.SaveChanges();
                if (account.Balance != 0)
                {
                    var newTransaction = new Transaction();
                    newTransaction.AccountId = newAccount.Id;
                    newTransaction.Amount = account.Balance;
                    newTransaction.CategoryId = db.Category.FirstOrDefault(x => x.Name == "Transfer").Id;
                    newTransaction.Date = DateTimeOffset.UtcNow;
                    newTransaction.Description = "Account Starting Amount";
                    newTransaction.EnteredById = currentUser.Id;
                    newTransaction.IsActive = true;
                    newTransaction.IsExpense = (account.Balance == Math.Abs(account.Balance) * -1) ? true : false;
                    newTransaction.IsReconciled = true;
                    newTransaction.IsVoid = false;
                    db.Transaction.Add(newTransaction);
                    db.SaveChanges();
                    //if (account.Balance == account.Balance * -1)//checks user input
                    //{
                    //    newTransaction.IsExpense = true;
                    //}
                    //else
                    //{
                    //    newTransaction.IsExpense = false;
                    //}


                }
                return RedirectToAction("Index");
            }

            //ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IsActive,Name,Balance,ReconciledBalance,HouseholdId")] Account account)
        {
            if (ModelState.IsValid)
            {
                var currentUser = GetCurrentUser();
                var currentHousehold = db.Household.Find(currentUser.HouseholdId);
                var currentAccount = db.Account.Find(account.Id);
                if(currentHousehold.Accounts.Contains(currentAccount) && currentAccount.IsActive == true)
                {
                    var currentBalance = currentAccount.Balance;
                    var differenceInBalance = (decimal) 0.0;
                    //Check Balance Difference & Do Calculations
                    if (currentAccount.Balance != account.Balance)
                    {
                        differenceInBalance = currentBalance - account.Balance;
                        //currentAccount.Balance = account.Balance; //make this work with the new calculated property.
                        //currentAccount.ReconciledBalance = currentAccount.ReconciledBalance - differenceInBalance;
                    }
                    currentAccount.Name = account.Name;
                    currentAccount.IsActive = account.IsActive;
                    
                    db.Entry(currentAccount).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("MyHousehold", "Saver");
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var currentAccount = db.Account.Find(id);
            var currentUser = GetCurrentUser();
            var currentHousehold = db.Household.Find(currentUser.HouseholdId);
            if (currentHousehold.Accounts.Contains(currentAccount))
            {
                currentAccount.IsActive = false;
                db.Entry(currentAccount).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("MyHousehold", "Saver");
        }

        //######## Controller Helpers ########
        public ApplicationUser GetCurrentUser()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            return user;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
