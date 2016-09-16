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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);

            //var transaction = db.Transaction.Include(t => t.Account).Include(t => t.Category).Include(t => t.EnteredBy);
            var dbtransactionList = db.Transaction.Where(x=>x.Account.HouseholdId == household.Id && x.IsActive ==true).ToList();
            return View(dbtransactionList);
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create(int? id)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var model = new Transaction();
            model.Date = DateTimeOffset.UtcNow;
            ViewData["Household"] = household;

            if(id != null)
            {
                var account = db.Account.Find(id);
                if (household.Accounts.Contains(account))
                {
                    model.AccountId = account.Id;
                    model.Account = account;
                    ViewBag.AccountId = new SelectList(household.Accounts, "Id", "Name");
                    ViewBag.CategoryId = new SelectList(household.Categories, "Id", "Name");
                    ViewBag.CurrentHousehold = household;
                    if (household.BudgetItems.Count() > 0)
                    {
                        ViewBag.BudgetItemId = new SelectList(household.BudgetItems, "Id", "Name");
                    }
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Error", "Saver", new { error = "Error when trying to create a Transaction." });
                }

            }

            if (household.BudgetItems.Count() > 0)
            {
                ViewBag.BudgetItemId = new SelectList(household.BudgetItems, "Id", "Name");
            }
            ViewBag.AccountId = new SelectList(household.Accounts, "Id", "Name");
            ViewBag.CategoryId = new SelectList(household.Categories, "Id", "Name");
            ViewBag.CurrentHousehold = household;

            return View(model);
            //ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName");
            //ViewBag.TransactionTypeId = new SelectList(db.TransactionType, "Id", "Name");



        }

       //Import Transactions CSV! 

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IsActive,Date,Description,Amount,IsReconciled,ReconciledAmount,TransactionTypeId,CategoryId,EnteredById,AccountId,IsExpense")] Transaction transaction)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var transactionAccount = db.Account.Find(transaction.AccountId);

            if (ModelState.IsValid && transactionAccount.HouseholdId == household.Id)//protects againsts front end manipulation.
            {
                if (transaction.Amount != 0)
                {
                    var newTransaction = new Transaction();
                    if (transaction.IsExpense == true && transaction.Amount == Math.Abs(transaction.Amount) || transaction.Amount != Math.Abs(transaction.Amount))//checks for user input selection errors.
                    {
                        newTransaction.Amount = Math.Abs(transaction.Amount) * -1;//removed.ReconciledAMount
                        newTransaction.IsExpense = true;
                    }
                    else
                    {
                        newTransaction.Amount = transaction.Amount;//removed.ReconciledAMount
                        transaction.IsExpense = false;                   
                    }
                    
                    newTransaction.AccountId = transaction.AccountId;
                    newTransaction.CategoryId = transaction.CategoryId;
                    newTransaction.Date = transaction.Date;
                    newTransaction.Description = transaction.Description;
                    newTransaction.EnteredById = currentUser.Id;
                    newTransaction.IsActive = true;
                    newTransaction.IsReconciled = transaction.IsReconciled;
                    newTransaction.IsVoid = false;

                    db.Transaction.Add(newTransaction);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.AccountId = new SelectList(household.Accounts, "Id", "Name");
                    ViewBag.CategoryId = new SelectList(db.Category.ToList(), "Id", "Name");
                    ModelState.AddModelError("Amount", "Error With Transaction Amount!");
                    return View(transaction);
                }

            }

            ViewBag.AccountId = new SelectList(db.Account, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Account, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(household.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IsActive,Date,Description,Amount,IsReconciled,ReconciledAmount,TransactionTypeId,CategoryId,EnteredById,AccountId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Account, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name", transaction.CategoryId);

            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transaction.Find(id);
            db.Transaction.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
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
