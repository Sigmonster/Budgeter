using System.Web.Mvc;
using Budgeter.Models;
using System.Text;
using Microsoft.AspNet.Identity;
using Budgeter.Helpers;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Web;
using CsvHelper;
using System.IO;
using System.Threading.Tasks;

namespace Budgeter.Controllers
{
    public class SaverController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult MyHousehold()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var household = currentUser.Household;
            ViewBag.CategoryId = new SelectList(household.Categories, "Id", "Name");

            return View(household);
        }

        //######## Send Invitations ########
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InviteEmails(ICollection<string> emailInvites)
        {
            var currentUser =  GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var invitations = emailInvites;
            var appHelper = new AppHelper(db);

            foreach (var item in invitations) /*(int i = 0; i < invitations.Count(); i++)*/
            {
                var emailInvitee = item.ToString();
                if (household.Members.Where(u => u.Email.ToLower() == emailInvitee.ToLower()).Count() == 0 && emailInvitee.ToCharArray().Count() > 4)
                {
                    var result1 = household.InvitedRegisteredUsers.Where(u => u.Email.ToLower() == emailInvitee.ToLower()).Count();
                    var result2 = db.Users.Where(u => u.Email.ToLower() == emailInvitee.ToLower()).Count();
                    var result3 = household.InvitedNotRegisteredEmail.Where(u => u.Email.ToLower() == emailInvitee.ToLower() && u.HouseholdId == household.Id).Count();
                    if (result1 == 0 && result2 == 1)
                    {
                        var userInvitee = db.Users.FirstOrDefault(u => u.Email == emailInvitee);
                        appHelper.AddRegisteredInvitation(userInvitee, household.Id);
                    }
                    else if (result3 == 0 && result2 == 0)
                    {
                        appHelper.AddNonRegisteredInvitation(emailInvitee, household.Id);
                    }

                }
            }

            return RedirectToAction("MyHousehold");
        }

        //######## Accept Invitation ########
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvitationJoinForm(int id)
        {
            var household = db.Household.Find(id);
            var currentUser = GetCurrentUser();

            if (currentUser.Household != null)
            {
                return RedirectToAction("Error", "Saver", new { error = "Leave your current household before trying to join a new one." });
            }

            if (currentUser.Invitations.Contains(household))
            {
                var appHelper = new AppHelper(db);
                household.Members.Add(currentUser);
                appHelper.RemoveUserInvitation(household, currentUser.Id);
            }

            return RedirectToAction("MyHousehold", "Saver");
        }

        //######## Decline Invitation ########
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvitationDeclineForm(int id)
        {
            var household = db.Household.Find(id);
            var currentUser = GetCurrentUser();
            var appHelper = new AppHelper(db);

            if (currentUser.Invitations.Contains(household))
            {
                appHelper.RemoveUserInvitation(household, currentUser.Id);
            }
            return RedirectToAction("Index", "Home");
        }

        //######## Revoke Invitation ########
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RevokeInvitationForm(string email)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var appHelper = new AppHelper(db);

            if (household.InvitedNotRegisteredEmail.Where(u=>u.Email == email).Count() == 1)
            {
                var invitationId = household.InvitedNotRegisteredEmail.FirstOrDefault(x => x.Email == email).Id;
                appHelper.RemoveNonRegisteredInvitation(invitationId);
            }
            else if (household.InvitedRegisteredUsers.Where(u=>u.Email == email).Count() == 1)
            {
                var userId = db.Users.FirstOrDefault(u => u.Email == email).Id;
                appHelper.RemoveUserInvitation(household, userId);
            }

            return RedirectToAction("Edit", "Households", new { id = household.Id});
        }

        //######## Revoke Invitation ########
        [HttpPost]
        public ActionResult RevokeInvitationPartialForm(string email)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var appHelper = new AppHelper(db);

            if (household.InvitedNotRegisteredEmail.Where(u => u.Email == email).Count() == 1)
            {
                var invitationId = household.InvitedNotRegisteredEmail.FirstOrDefault(x => x.Email == email).Id;
                appHelper.RemoveNonRegisteredInvitation(invitationId);
            }
            else if (household.InvitedRegisteredUsers.Where(u => u.Email == email).Count() == 1)
            {
                var userId = db.Users.FirstOrDefault(u => u.Email == email).Id;
                appHelper.RemoveUserInvitation(household, userId);
            }
            var newhousehold = db.Household.Find(currentUser.HouseholdId);
            return RedirectToAction("MyHousehold");
            //return PartialView("/Views/Shared/_MyHouseholdInvitationsPartial.cshtml", newhousehold );
        }

        //######## Leave Current Household ########
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveHousehold()
        {
            var currentUser = GetCurrentUser();
            var appHelper = new AppHelper(db);
            appHelper.LeaveHousehold(currentUser.Id);

            return RedirectToAction("Index", "Home");
        }

        //#########################################
        //######## Account Manage Section #########
        public ActionResult AccountManage(int? accounts)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            
            //var accountManageVM = new AccountManageVM();
            //accountManageVM.Household = household;
            ViewBag.Accounts = new SelectList(household.Accounts, "Id", "Name");
            if (accounts != null)
            {
                var account = db.Account.Find(accounts);
                //accountManageVM.Account = account;

                if (household.Accounts.Contains(account))
                {
                    //var modelTransaction = new Transaction();
                    //modelTransaction.Date = DateTime.Now;
                    //modelTransaction.AccountId = account.Id;
                    //modelTransaction.Account = account;
                    ViewBag.AccountId = new SelectList(household.Accounts, "Id", "Name");
                    ViewBag.CategoryId = new SelectList(household.Categories, "Id", "Name");
                    //ViewBag.CurrentHousehold = household;
                    //accountManageVM.Transaction = modelTransaction;
                    var model = GetAccountFetchVM(account.Id);

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Error", "Saver", new { error = "Error Loading Page" });
                }

            }
            var accountFetchVM = new AccountFetchVM();
            return View(accountFetchVM);
        }
        //Helper Build ViewModel
        private AccountFetchVM GetAccountFetchVM(int accounts)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var account = db.Account.Find(accounts);
            var transactionList = account.Transactions.Where(t=>t.IsActive == true).OrderByDescending(t=>t.Date).ToList();

            var accountFetchVM = new AccountFetchVM();
            accountFetchVM.TransactionList = transactionList;
            accountFetchVM.Transaction = new Transaction();
            accountFetchVM.Transaction.AccountId = accounts;
            accountFetchVM.Account = account;
            accountFetchVM.Household = household;
            ViewBag.AccountId = new SelectList(household.Accounts, "Id", "Name");
            ViewBag.CategoryId = new SelectList(household.Categories, "Id", "Name");

            return accountFetchVM;
        }
        private List<Transaction> GetActiveTransactions(int accountId)
        {
            var account = db.Account.Find(accountId);
            var transactionList = account.Transactions.Where(t => t.IsActive == true).OrderByDescending(t => t.Date).ToList();
            ViewBag.TransactionsListTitle = "Transactions for : " + account.Name;
            return transactionList;
        }
        private List<Transaction> GetInActiveTransactions(int accountId)
        {
            var account = db.Account.Find(accountId);
            var transactionList = account.Transactions.Where(t => t.IsActive == false).OrderByDescending(t => t.Date).ToList();
            ViewBag.TransactionsListTitle = "Deleted Transactions for : " + account.Name;
            return transactionList;
        }

        [HttpPost]
        public ActionResult VoidTransaction(string id, string voidAction)
        {
            int transactionID = 0;
            int.TryParse(id, out transactionID);
            bool voidRequest = (voidAction == "true")? true : false;

            var transaction = db.Transaction.Find(transactionID);
            transaction.IsVoid = voidRequest;
            db.Entry(transaction).State = EntityState.Modified;
            db.SaveChanges();
            var model = GetActiveTransactions(transaction.AccountId);
            return PartialView("_DisplayTransactionsPartial", model);
        }

        //Processed/Pending Transactions (Reconcile)
        [HttpPost]
        public ActionResult ReconcileTransaction(string id, string reconcileAction)
        {
            int transactionID = 0;
            int.TryParse(id, out transactionID);
            bool reconcileRequest = (reconcileAction == "true") ? true : false;

            var transaction = db.Transaction.Find(transactionID);
            transaction.IsReconciled = reconcileRequest;
            db.Entry(transaction).State = EntityState.Modified;
            db.SaveChanges();
            var model = GetActiveTransactions(transaction.AccountId); 
            return PartialView("_DisplayTransactionsPartial", model);
        }

        [HttpPost]
        public ActionResult ActiveStatusToggleTransaction(string id, string isActiveAction)
        {
            int transactionID = 0;
            int.TryParse(id, out transactionID);
            bool isActiveActionRequest = (isActiveAction == "true") ? true : false;
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var account = db.Account.Find(transactionID);
            var model = new List<Transaction>();

            var transaction = db.Transaction.Find(transactionID);
            transaction.IsActive = isActiveActionRequest;
            db.Entry(transaction).State = EntityState.Modified;
            db.SaveChanges();
            if(transaction.IsActive == true)
            {
                model = GetInActiveTransactions(transaction.AccountId);
            }
            else if(transaction.IsActive == false)
            {
                model = GetActiveTransactions(transaction.AccountId);
            }
            return PartialView("_DisplayTransactionsPartial", model);
        }

        //AccountManage AJAX GET Action - returns partial view.
        public PartialViewResult _DisplayTransactionsPartial(int accounts)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var account = db.Account.Find(accounts);
            var transactionList = account.Transactions.Where(t => t.IsActive == true).OrderByDescending(t => t.Date).ToList();
            ViewBag.TransactionsListTitle = "Transactions for : " + account.Name;
            return PartialView("_DisplayTransactionsPartial", transactionList);
        }
        public PartialViewResult DisplayArchivedTransactions(string id)
        {
            int accountID = 0;
            int.TryParse(id, out accountID);
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var account = db.Account.Find(accountID);

            var model = GetInActiveTransactions(account.Id);

            return PartialView("_DisplayTransactionsPartial", model);
        }
        public PartialViewResult _AccountManageHeadPartial(int accounts)
        {
            var account = db.Account.Find(accounts);

            return PartialView("_AccountManageHeadPartial", account);
        }
        public PartialViewResult _AddTransactionPartial(int accounts)
        {
            var model = GetAddTransactionModel(accounts);

            return PartialView("_AddTransactionPartial",model);
        }

        private Transaction GetAddTransactionModel(int accounts)
        {
            var account = db.Account.Find(accounts);
            var model = new Transaction();
            model.AccountId = account.Id;
            ViewBag.CategoryId = new SelectList(account.Household.Categories, "Id", "Name", model.CategoryId);

            return model;
        }

        public PartialViewResult EditTransaction(string id)
        {
            int transactionID = 0;
            int.TryParse(id, out transactionID);

            var model = GetTransactionModel(transactionID);

            return PartialView("_EditTransactionPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTransaction([Bind(Include ="Id, Date, Description, Amount, IsReconciled, IsExpense, CategoryId, AccountId" )]Transaction transaction)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var editedTransaction = transaction;
            var account = household.Accounts.First(a=>a.Id == editedTransaction.AccountId);

            if (ModelState.IsValid)
            {
                var currentTransaction = db.Transaction.Find(transaction.Id);
                db.Entry(currentTransaction).State = EntityState.Modified;
                currentTransaction.Date = editedTransaction.Date;
                currentTransaction.Description = editedTransaction.Description;
                currentTransaction.Amount = editedTransaction.Amount;
                currentTransaction.IsReconciled = editedTransaction.IsReconciled;
                currentTransaction.IsExpense = editedTransaction.IsExpense;
                currentTransaction.CategoryId = editedTransaction.CategoryId;
                db.SaveChanges();
            }

            var model = GetTransactionModel(account.Id);

            return PartialView("_EditTransactionPartial", model);
        }

        private Transaction GetTransactionModel(int transactionID)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var model = db.Transaction.Find(transactionID);
            ViewBag.CategoryId = new SelectList(household.Categories, "Id", "Name", model.CategoryId);

            return model;
        }
        //##############################################
        //##### START Budget ActionResults Section######
        //##############################################

        public ActionResult BudgetManage()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);

            return View();
        }

        public PartialViewResult _BudgetAddPartial()
        {
            var model = GetNewBudgetItem();

            return PartialView("_BudgetAddPartial", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddBudgetPartial([Bind(Include = "Amount, Name, CategoryId, HouseholdId")] BudgetItem budget)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);

            budget.HouseholdId = household.Id;
            budget.Amount = Math.Abs(budget.Amount);
            budget.IsActive = true;
            db.BudgetItem.Add(budget);
            await db.SaveChangesAsync();

            return Content("Success");//Partials reloaded with .Load to avoid repopulating add budget with same data.
        }

        public ActionResult DisplayActiveBudgets()
        {
            var model = GetActiveBudgets();

            return PartialView("_DisplayBudgetsPartial", model);
        }



        //########################################
        //Budget Helpers START
        private BudgetItem GetNewBudgetItem()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);

            var model = new BudgetItem();
            model.HouseholdId = household.Id;
            ViewBag.CategoryId = new SelectList(household.Categories, "Id", "Name", model.CategoryId);

            return model;
        }

        private List<BudgetItem> GetActiveBudgets()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            ViewBag.BudgetsListTitle = "All Budgets for Household: " + household.Name;
            var model = household.BudgetItems.Where(x => x.IsActive == true).ToList();

            return model;
        }


        //########################################
        //### END Budget ActionResults Section ###
        //########################################

        //######## Create Transaction #########
        //POST ACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTransaction([Bind(Include = "Id,IsActive,Date,Description,Amount,IsReconciled,ReconciledAmount,TransactionTypeId,CategoryId,EnteredById,AccountId,IsExpense")] Transaction transaction)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var transactionAccount = db.Account.Find(transaction.AccountId);

            if (ModelState.IsValid && transactionAccount.HouseholdId == household.Id)//protects againsts front end manipulation. Account/Household Comparison
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
                    return RedirectToAction("AccountManage", "Saver", new { accounts = transactionAccount.Id });
                }
                else
                {
                    ViewBag.AccountId = new SelectList(household.Accounts, "Id", "Name");
                    ViewBag.CategoryId = new SelectList(db.Category.ToList(), "Id", "Name");
                    ModelState.AddModelError("Amount", "Error With Transaction Amount!");
                    return RedirectToAction("Error", "Saver", new { error = "Error With Transaction Data, If you feel you reached this in error please contact support." });
                }

            }

            ViewBag.AccountId = new SelectList(db.Account, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name", transaction.CategoryId);

            return RedirectToAction("AccountManage", "Saver", new { accounts = transactionAccount.Id });

        }
        //######## End Account Manage Section #########
        //#############################################


        //CSV Import Testing

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult BulkUploadVerification(string textCsv)
        //{
        //    var model = new UploadVerficationVM();

        //    var csv = new CsvReader(File.OpenText("test2.csv"));


        //    return RedirectToAction("Create", "Transactions");
        //    //if(FileUpload.ContentLength > 0)
        //    //{
        //    //    var fileName = Path.GetFileName(FileUpload.FileName);
        //    //    //relative server path
        //    //    var filePath = "/Uploads/";
        //    //    // path on physical drive on server
        //    //    var absPath = Server.MapPath("~" + filePath);
        //    //    //save file to Uploads
        //    //    FileUpload.SaveAs(Path.Combine(absPath, FileUpload.FileName));

        //    //    var path = Path.Combine(absPath, FileUpload.FileName);

                
        //    //    //using (var csv = new CsvReader(new StreamReader(csvFile)))
        //    //    //{
        //    //    using (var sr = new StreamReader(path))
        //    //    {
        //    //        //string currentLine;
        //    //        //while((currentLine == sr.ReadLine()) != null)
        //    //        //{

        //    //        //}
        //    //    }
        //    //        //csvFile.Configuration.RegisterClassMap<MyClassMap>();
        //    //        //IEnumerable<Transaction> data = csv.GetRecords<Transaction>();
        //    //    //}


        //        //var transactionList = csv.GetRecords<Transaction>();
                
        //        //foreach (var item in transactionList)
        //        //{

        //        //    //model.TransactionList.Add(new Transaction { })
        //        //}
        //    //}


        //    //return PartialView("_UploadTransactionsVerification", model);
        //}


        //######## Error ActionResults ########
        //return RedirectToAction("Error", "Saver", new { error = "Error Message Here..." });
        public ActionResult Error(string error)
        {
            var currentUser = GetCurrentUser();
            var errormsg = new StringBuilder();
            errormsg.Append("Error Details: " + error + " User:" + currentUser.Email.ToString());
            ViewBag.ErrorMessage = (string)errormsg.ToString();
            return View();
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
