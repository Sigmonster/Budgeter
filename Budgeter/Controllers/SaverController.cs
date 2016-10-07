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
{   [Authorize]
    public class SaverController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult MyHousehold()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            if (currentUser.Household != null)
            {
                var household = currentUser.Household;
                ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name");
                var transactionList = new List<Transaction>();

                  foreach (var item in household.Accounts)
                {
                    var top5 = item.Transactions.Where(t=>t.IsActive == true).OrderByDescending(x => x.Date).Take(5).ToList();
                   transactionList.AddRange(top5);
                }

                ViewData["TransactionList"] = (List<Transaction>)transactionList.OrderByDescending(x=>x.Date).ToList();
                return View(household);
            }
            else
            {
               return RedirectToAction("Create", "Households");
            }
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
        //######## MyManager Section #########
        public ActionResult AccountManage(int? accounts)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            
            //var accountManageVM = new AccountManageVM();
            //accountManageVM.Household = household;
            ViewBag.Accounts = new SelectList(household.Accounts.Where(x=>x.IsActive == true), "Id", "Name");
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
                    ViewBag.AccountId = new SelectList(household.Accounts.Where(x => x.IsActive == true), "Id", "Name");
                    ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name");
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
            ViewBag.AccountId = new SelectList(household.Accounts.Where(x => x.IsActive == true), "Id", "Name");
            ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name");

            return accountFetchVM;
        }
        private List<Transaction> GetActiveTransactions(int accountId)
        {
            var account = db.Account.Find(accountId);
            var transactionList = account.Transactions.Where(t => t.IsActive == true).OrderByDescending(t => t.Date).ToList();
            ViewBag.TransactionsListTitle = "Active Transactions";
            return transactionList;
        }
        private List<Transaction> GetInActiveTransactions(int accountId)
        {
            var account = db.Account.Find(accountId);
            var transactionList = account.Transactions.Where(t => t.IsActive == false).OrderByDescending(t => t.Date).ToList();
            ViewBag.TransactionsListTitle = "Deleted Transactions";
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
            if(transaction.IsVoid == true)
            {
                return Content("True");
            }
            else if(transaction.IsVoid == false)
            {
                return Content("False");
            }
            else
            {
                return Content("Error");
            }
            //return PartialView("_DisplayTransactionsPartial", model);
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
            if (transaction.IsReconciled == true)
            {
                return Content("True");
            }
            else if (transaction.IsReconciled == false)
            {
                return Content("False");
            }
            else
            {
                return Content("Error");
            }
            //return PartialView("_DisplayTransactionsPartial", model);
        }

        [HttpPost]
        public ActionResult ActiveStatusToggleTransaction(string id, string isActiveAction)
        {
            int transactionID = 0;
            int.TryParse(id, out transactionID);
            bool isActiveActionRequest = (isActiveAction == "true") ? true : false;
            var currentUser = GetCurrentUser();
            var household = currentUser.Household;
            var transaction = db.Transaction.Find(transactionID);

            if (household.Accounts.Any(x => x.Transactions.Any(t => t.Id == transaction.Id)))
            { 
            transaction.IsActive = isActiveActionRequest;
            db.Entry(transaction).State = EntityState.Modified;
            db.SaveChanges();
            }
            else
            {
                return Content("Error");
            }

            if (transaction.IsActive == true)
            {
                return Content("Success");
            }
            else if(transaction.IsActive == false)
            {
                return Content("Success");
            }

            //If we got this far something went wrong.
            return Content("Error");
        }

        //AccountManage AJAX GET Action - returns partial view.
        public PartialViewResult _DisplayTransactionsPartial(int accounts)
        {
            var model = GetActiveTransactions(accounts);
            return PartialView("_DisplayTransactionsPartial", model);
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
        public PartialViewResult _AccountBalancesPartial(string id)
        {
            int accountID = 0;
            int.TryParse(id, out accountID);
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var model = household.Accounts.First(x => x.Id == accountID);
            return PartialView("_AccountBalancesPartial", model);
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
            ViewBag.CategoryId = new SelectList(account.Household.Categories.Where(x => x.IsActive == true), "Id", "Name", model.CategoryId);
            return model;
        }


        public PartialViewResult EditTransaction(string id)
        {
            int transactionID = 0;
            int.TryParse(id, out transactionID);

            var model = GetTransactionModel(transactionID);

            return PartialView("_EditTransactionPartial", model);
        }

        public ActionResult QuickAddTransaction()
        {
            var currentUser = GetCurrentUser();
            
            if(currentUser.Household != null && currentUser.Household.Accounts != null)
            {
                var model = GetQuickAddTransaction();
                return PartialView("_QuickAddPartial", model);
            }
            else
            {
                return Content("Error");
            }
        }
        public ActionResult UpdateTransactionManagerHead()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            if (currentUser.HouseholdId != null)
            {
                ViewBag.Accounts = new SelectList(household.Accounts.Where(x => x.IsActive == true), "Id", "Name");
                return PartialView("_TransactionsManagerDropdownPartial");
            }
            else
            {
                return Content("Error");
            }
        }


        //POST : Transactions ####################
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickAddTransaction([Bind(Include = "Id, Date, Description, Amount, IsReconciled, IsExpense, CategoryId, AccountId")]Transaction transaction)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var transactionAccount = db.Account.Find(transaction.AccountId);
            ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name");
            ViewBag.AccountId = new SelectList(household.Accounts.Where(x => x.IsActive == true), "Id", "Name");

            if (ModelState.IsValid && transactionAccount.HouseholdId == household.Id)//protects againsts front end manipulation. Account/Household Comparison
            {
                if (transaction.Amount != 0)
                {
                    var newTransaction = new Transaction();
                    if (transaction.IsExpense == true && transaction.Amount == Math.Abs(transaction.Amount) || transaction.Amount != Math.Abs(transaction.Amount))//checks for user input selection errors.
                    {
                        newTransaction.Amount = Math.Abs(transaction.Amount) * -1;
                        newTransaction.IsExpense = true;
                    }
                    else
                    {
                        newTransaction.Amount = transaction.Amount;
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
                    return Content("Success");

                }
                else
                {
                    ModelState.AddModelError("Amount", "Error With Transaction Amount!");
                    return PartialView("_QuickAddPartial", transaction);
                }
            }
            return PartialView("_QuickAddPartial", transaction);
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
                currentTransaction.Amount = (currentTransaction.IsExpense != false || editedTransaction.Amount < 0) ? Math.Abs(editedTransaction.Amount)*-1 : Math.Abs(editedTransaction.Amount);
                currentTransaction.IsReconciled = editedTransaction.IsReconciled;
                currentTransaction.IsExpense = editedTransaction.IsExpense;
                currentTransaction.CategoryId = editedTransaction.CategoryId;
                db.SaveChanges();
                return Content("Success");
            }
            ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name", transaction.CategoryId);
            return PartialView("_EditTransactionPartial", transaction);
        }

        private Transaction GetTransactionModel(int transactionID)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var model = db.Transaction.Find(transactionID);
            ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name", model.CategoryId);

            return model;
        }
        private Transaction GetQuickAddTransaction()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var model = new Transaction();
            ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name");
            ViewBag.AccountId = new SelectList(household.Accounts.Where(x => x.IsActive == true), "Id", "Name");
            return model;
        }
        //##############################################
        //##### START Budget ActionResults Section######
        //##############################################

        public ActionResult MyManager()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            if(currentUser.HouseholdId != null)
            {
                ViewBag.Accounts = new SelectList(household.Accounts.Where(x => x.IsActive == true), "Id", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("Create", "Households");
            }
        }
        public ActionResult DisplayActiveBudgets()
        {
            var model = GetActiveBudgets();

            return PartialView("_DisplayBudgetsPartial", model);
        }

        public ActionResult DisplayInactiveBudgets()
        {
            var model = GetInactiveBudgets();

            return PartialView("_DisplayBudgetsPartial", model);
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
            if(!household.BudgetItems.Any(x=>x.CategoryId == budget.CategoryId) && ModelState.IsValid)
            {
                db.BudgetItem.Add(budget);
                await db.SaveChangesAsync();
                return Content("Success");
            }
            else if(household.BudgetItems.Any(x => x.CategoryId == budget.CategoryId) && ModelState.IsValid)
            {
                return Content("ErrorDuplicate");
            }
            else
            {
                GetNewBudgetItem();//gets select list needed for partial.
                return PartialView("_BudgetAddPartial", budget);
            }

        }

        [HttpPost]
        public ActionResult ActiveStatusToggleBudget(string id, string isActiveAction)
        {
            int budgetId = 0;
            int.TryParse(id, out budgetId);
            var isActiveActionRequest = (isActiveAction == "true") ? true : false;
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var budgetItem = db.BudgetItem.Find(budgetId);

            if (household.BudgetItems.Contains(budgetItem))
            {
                db.Entry(budgetItem).State = EntityState.Modified;
                budgetItem.IsActive = isActiveActionRequest;
                db.SaveChanges();
                return Content("Success");
            }

            return Content("Error");
        }

        public PartialViewResult EditBudget(string id)
        {
            int budgetId = 0;
            int.TryParse(id, out budgetId);

            var model = GetBudgetModel(budgetId);

            return PartialView("_EditBudgetPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBudget([Bind(Include = "Id, Amount, Name, CategoryId")]BudgetItem budgetItemForm)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var editedBudgetItem = budgetItemForm;
            var Result = household.BudgetItems.Any(x=>x.Id == editedBudgetItem.Id);

            if (ModelState.IsValid && Result)
            {
                var currentBudget = db.BudgetItem.Find(editedBudgetItem.Id);
                db.Entry(currentBudget).State = EntityState.Modified;
                currentBudget.Amount = editedBudgetItem.Amount;
                currentBudget.Name = editedBudgetItem.Name;
                currentBudget.CategoryId = editedBudgetItem.CategoryId;
                db.SaveChanges();
                return Content("Success");
            }
            ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name");
            // if we got this far something went wrong
            return PartialView("_EditBudgetPartial", budgetItemForm);
        }

        //########################################
        //Budget Helpers START
        private BudgetItem GetNewBudgetItem()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);

            var model = new BudgetItem();
            model.HouseholdId = household.Id;
            ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name", model.CategoryId);

            return model;
        }
        private BudgetItem GetBudgetModel(int budgetId)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var model = db.BudgetItem.FirstOrDefault(x => x.Id == budgetId);
            ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name", model.CategoryId);
            return model;
        }
        private List<BudgetItem> GetActiveBudgets()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            ViewBag.BudgetsListTitle = "Active Budgets";
            var model = household.BudgetItems.Where(x => x.IsActive == true).ToList();

            var categories = household.Categories;
            //foreach (var item in model)
            //{
            //    item.SpentAmount
            //}

            //ViewData["BudgetData"] = 
            return model;
        }
        private List<BudgetItem> GetInactiveBudgets()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            ViewBag.BudgetsListTitle = "Archived/Inactive Budgets";
            var model = household.BudgetItems.Where(x => x.IsActive == false).ToList();

            return model;
        }

        //########################################
        //### END Budget ActionResults Section ###
        //########################################

        //############################################
        //### START Category ActionResults Section ###
        //############################################

        public ActionResult GetAddCategoryPartial()
        {

            return PartialView("_CategoryAddPartial", new Category());
        }

        public ActionResult GetHouseholdCategoriesPartial()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            ViewBag.CategoriesListTitle = "All Budget/Transaction Categories";
            return PartialView("_DisplayCategories", household.Categories.OrderBy(x=>x.IsDefault).ToList());
        }
        public ActionResult EditCategory(int id)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var model = household.Categories.First(x => x.Id == id);
            return PartialView("_EditCategoryPartial", model);
        }

        //Category Posts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory([Bind(Include ="Id, Name")] Category categoryForm)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var editedCategory = categoryForm;
            var Result = household.Categories.Any(x => x.Id == editedCategory.Id);
            var category = db.Category.Find(editedCategory.Id);

            if (ModelState.IsValid && Result && category.IsDefault == false)
            {
                db.Entry(category).State = EntityState.Modified;
                category.Name = editedCategory.Name;
                db.SaveChanges();
                return Content("Success");
            }
            else if (category.IsDefault == true)
            {
                return Content("Error:DefaultCategory");
            }

            return PartialView("_EditCategoryPartial", categoryForm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory([Bind(Include = "Name")] Category categoryAddForm)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var categoryToAdd = categoryAddForm;
            
            if (ModelState.IsValid)
            {
                Category newCategory = new Category();
                newCategory.Name = categoryToAdd.Name;
                newCategory.Households.Add(household);
                newCategory.IsActive = true;
                newCategory.IsExpense = true;
                newCategory.IsDefault = false;
                db.Category.Add(newCategory);
                db.SaveChanges();
                return Content("Success");
            }

            return PartialView("_CategoryAddPartial", categoryAddForm);
        }
        [HttpPost]
        public ActionResult ActiveStatusToggleCategory(string id, string isActiveAction)
        {
            int categoryId = 0;
            int.TryParse(id, out categoryId);
            var isActiveActionRequest = (isActiveAction == "true") ? true : false;
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var category = db.Category.First(x=>x.Id == categoryId);

            if (household.Categories.Contains(category) && category.IsDefault == false)
            {
                db.Entry(category).State = EntityState.Modified;
                category.IsActive = isActiveActionRequest;
                db.SaveChanges();
                return Content("Success");
            }
            else if(category.IsDefault == true)
            {
                return Content("Error:DefaultCategory");
            }
            return Content("Error");
        }
        //############################################
        //###  END Category ActionResults Section  ###
        //############################################

        //############################################
        //########  Start Accounts Section  ##########
        //############################################

        //Get Add Acount Partial
        public ActionResult GetAddAccountPartial()
        {
            return PartialView("_AccountAddPartial", new AccountCreateVM());
        }

        //Get Active Accounts List Partial
        public ActionResult GetAccountsListPartial()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var accounts = household.Accounts.Where(x=>x.IsActive == true).ToList();
            @ViewBag.AccountsListTitle = "Active Accounts";
            return PartialView("_DisplayAccountsPartial", accounts);
        }
        //Get Inactive Accounts List Partial
        public ActionResult GetInactiveAccountsListPartial()
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var accounts = household.Accounts.Where(x => x.IsActive == false).ToList();
            @ViewBag.AccountsListTitle = "Inactive Accounts";
            return PartialView("_DisplayAccountsPartial", accounts);
        }
        //Get Edit Account Modal
        public ActionResult EditAccount(string id)
        {
            int accountId = 0;
            int.TryParse(id, out accountId);
            var model = GetAccountModel(accountId);
            return PartialView("_EditAccountPartial", model);
        }

        //Post Add Account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitAddAccountForm([Bind(Include ="Name, Balance")]AccountCreateVM account)
        {
            if (ModelState.IsValid)
            {
                var currentUser = GetCurrentUser();
                var currentHousehold = db.Household.Find(currentUser.HouseholdId);
                var newAccount = new Account();
                newAccount.Name = account.Name;
                newAccount.IsActive = true;
                newAccount.HouseholdId = currentHousehold.Id;
                db.Account.Add(newAccount);
                db.SaveChanges();
                if (account.Balance > 0)
                {
                    var newTransaction = new Transaction();
                    newTransaction.AccountId = newAccount.Id;
                    newTransaction.Amount = account.Balance;
                    newTransaction.CategoryId = db.Category.FirstOrDefault(x => x.Name == "Income").Id;
                    newTransaction.Date = DateTimeOffset.UtcNow;
                    newTransaction.Description = "Account Starting Amount";
                    newTransaction.EnteredById = currentUser.Id;
                    newTransaction.IsActive = true;
                    newTransaction.IsExpense = (account.Balance == Math.Abs(account.Balance) * -1) ? true : false;
                    newTransaction.IsReconciled = true;
                    newTransaction.IsVoid = false;
                    db.Transaction.Add(newTransaction);
                    db.SaveChanges();
                }
                return Content("Success");
            }

            //If we got this far something went wrong.
            return PartialView("_AccountAddPartial", account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount([Bind(Include = "Id,IsActive,Name")] Account account)
        {
            if (ModelState.IsValid)
            {
                var currentUser = GetCurrentUser();
                var currentHousehold = db.Household.Find(currentUser.HouseholdId);
                var currentAccount = db.Account.Find(account.Id);
                if (currentHousehold.Accounts.Contains(currentAccount))
                {
                    currentAccount.Name = account.Name;
                    db.Entry(currentAccount).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Content("Success");
            }
            return PartialView("_EditAccountPartial", account);
        }
        [HttpPost]
        public ActionResult ActiveStatusToggleAccount(string id, string isActiveAction)
        {
            int accountId = 0;
            int.TryParse(id, out accountId);
            var isActiveActionRequest = (isActiveAction == "true") ? true : false;
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var account = db.Account.First(x => x.Id == accountId);

            if (household.Accounts.Contains(account))
            {
                db.Entry(account).State = EntityState.Modified;
                account.IsActive = isActiveActionRequest;
                db.SaveChanges();
                return Content("Success");
            }

            return Content("Error");
        }
        //####### Accounts Helpers
        private Account GetAccountModel(int accountId)
        {
            var currentUser = GetCurrentUser();
            var currentHousehold = db.Household.Find(currentUser.HouseholdId);
            var model = currentHousehold.Accounts.First(x=>x.Id == accountId);
            return model;
        }

        //############################################
        //#########  END Accounts Section  ###########
        //############################################

        //######## Create Transaction #########
        //POST ACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTransaction([Bind(Include = "Id,IsActive,Date,Description,Amount,IsReconciled,ReconciledAmount,TransactionTypeId,CategoryId,EnteredById,AccountId,IsExpense")] Transaction transaction)
        {
            var currentUser = GetCurrentUser();
            var household = db.Household.Find(currentUser.HouseholdId);
            var transactionAccount = db.Account.Find(transaction.AccountId);
            ViewBag.CategoryId = new SelectList(household.Categories.Where(x => x.IsActive == true), "Id", "Name");

            if (ModelState.IsValid && transactionAccount.HouseholdId == household.Id)//protects againsts front end manipulation. Account/Household Comparison
            {
                if (transaction.Amount != 0)
                {
                    var newTransaction = new Transaction();
                    if (transaction.IsExpense == true && transaction.Amount == Math.Abs(transaction.Amount) || transaction.Amount != Math.Abs(transaction.Amount))//checks for user input selection errors.
                    {
                        newTransaction.Amount = Math.Abs(transaction.Amount) * -1;
                        newTransaction.IsExpense = true;
                    }
                    else
                    {
                        newTransaction.Amount = transaction.Amount;
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
                    return Content("Success");

                }
                else
                {
                    ModelState.AddModelError("Amount", "Error With Transaction Amount!");
                    return PartialView("_AddTransactionPartial", transaction);
                }
            }
            return PartialView("_AddTransactionPartial", transaction);
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
