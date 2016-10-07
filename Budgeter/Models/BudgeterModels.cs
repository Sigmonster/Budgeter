using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Models
{
    //###########################################################
    //##################### Household Model #####################
    //###########################################################
    public class Household
    {
        public Household()
        {
            this.Members = new HashSet<ApplicationUser>();
            this.InvitedRegisteredUsers = new HashSet<ApplicationUser>();
            this.Accounts = new HashSet<Account>();
            this.BudgetItems = new HashSet<BudgetItem>();
            //this.Budgets = new HashSet<Budget>();
            this.Categories = new HashSet<Category>();
            this.InvitedNotRegisteredEmail = new HashSet<InvitedButNotRegisteredEmail>();
        }
        public int Id { get; set; }//PK
        public bool IsActive { get; set; }
        [Required]
        [StringLength(20, ErrorMessage ="Household Name must between 5 and 20 characters"), MinLength(5,ErrorMessage = "Household Name must between 5 and 20 characters")]
        public string Name { get; set; }
        public HouseholdDetails HouseholdDetail { get; set; }

        //Forigen Keys
        public string OwnerUserId { get; set; }//FK

        //Virtual Properties
        [InverseProperty("HouseholdsOwned")]
        public virtual ApplicationUser OwnerUser { get; set; }//Holds Associated FK OwnerUser
        [InverseProperty("Household")]
        public virtual ICollection<ApplicationUser> Members { get; set; }
        [InverseProperty("Invitations")]
        public virtual ICollection<ApplicationUser> InvitedRegisteredUsers { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        //public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<InvitedButNotRegisteredEmail> InvitedNotRegisteredEmail { get; set; }
    }
    //######Household Details######
    [ComplexType]
    public class HouseholdDetails
    {
        [Column("Created")]
        [DisplayFormat(DataFormatString = "{0:o}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? Created { get; set; }
        [Column("Updated")]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? Updated { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Household Description must between 5 and 20 characters"), MinLength(5, ErrorMessage = "Household Description must between 5 and 20 characters")]
        [Column("Description", TypeName="nvarchar")]
        public string Description { get; set; }
    }

    //###########################################################
    //######### Invited But Not Registered Email Model ##########
    //###########################################################
    public class InvitedButNotRegisteredEmail
    {
        public InvitedButNotRegisteredEmail()
        {
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime DateSent { get; set; }

        //FKs
        public int HouseholdId { get; set; }

        //Virtual Property
        public virtual Household Household { get; set; }
    }

    //###########################################################
    //##################### Accounts Model ######################
    //###########################################################
    public class Account
    {
        public Account()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public bool IsActive { get; set; }
        [StringLength(20, ErrorMessage = "Account Name must between 5 and 20 characters"), MinLength(5, ErrorMessage = "Account Name must between 5 and 20 characters")]
        public string Name { get; set; }
        //[Range(double.MinValue, double.MaxValue)]
        [NotMapped]
        public decimal Balance
        {
            get
            {
                return Transactions.Where(t => t.IsActive == true && t.IsVoid == false).Sum(x => x.Amount);
            }
            private set
            {
            }
        }
        [NotMapped]
        [Range(double.MinValue, double.MaxValue)]
        public decimal ReconciledBalance
        {
            get
            {
            return Transactions.Where(t => t.IsActive == true && t.IsReconciled == true && t.IsVoid == false).Sum(x => x.Amount);
            }
            private set
            {
            }

        }

        //FKs
        public int HouseholdId { get; set; }

        //Virtual Properties
        public virtual Household Household { get; set; }// One to one
        public virtual ICollection<Transaction> Transactions { get; set; }
    }

    //###########################################################
    //##################### Transaction Model ###################
    //###########################################################
    public class Transaction
    {
        public Transaction()
        {
        }

        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Date { get; set; }
        [StringLength(25, ErrorMessage = "Transaction Description must between 5 and 20 characters"), MinLength(2, ErrorMessage = "Transaction Description must between 2 and 20 characters")]
        [MaxLength(25, ErrorMessage = "Transaction Description must between 5 and 20 characters")]
        public string Description { get; set; }
        [Range(double.MinValue, double.MaxValue)]
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public bool IsReconciled { get; set; }
        public bool IsExpense { get; set; }
        public bool IsVoid { get; set; }


        //FKs
        public int CategoryId { get; set; }
        public string EnteredById { get; set; }
        public int AccountId { get; set; }

        //Virtual Properties
        public virtual Category Category { get; set; }
        public virtual ApplicationUser EnteredBy { get; set; }
        public virtual Account Account { get; set; }

    }

    //###########################################################
    //##################### Category Model ######################
    //###########################################################
    public class Category
    {
        public Category()
        {
            this.Transactions = new HashSet<Transaction>();
            this.BudgetItems = new HashSet<BudgetItem>();
            this.Households = new HashSet<Household>();
        }
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public bool IsExpense { get; set; }
        //public int? BudgetItemId { get; set; }//FK
        //[NotMapped]
        //public decimal BudgetAmount
        //{
        //    get
        //    {
        //        return BudgetItems.First().Amount;
        //    }
        //    private set { }
        //}
        //[NotMapped]
        //public decimal AmountSpentThisMonth
        //{
        //    get
        //    {
        //        return Transactions.Where(x => x.Date.Month == DateTime.Now.Month && x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount);
        //    }
        //    private set { }
        //}

        [Required]
        [StringLength(25, ErrorMessage = "Category Name must between 4 and 25 characters"), MinLength(4, ErrorMessage = "Category Name must between 4 and 25 characters")]
        public string Name { get; set; }

        //public virtual BudgetItem BudgetItem { get; set; }
        public virtual ICollection<Household> Households { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

    }

  
    //###########################################################
    //###################### Budget Model #######################
    //###########################################################
    //public class Budget
    //{
    //    public Budget()
    //    {
    //        this.BudgetItems = new HashSet<BudgetItem>();
    //        this.Transactions = new HashSet<Transaction>();
    //    }

    //    public int Id { get; set; }
    //    public bool IsActive { get; set; }
    //    [Required]
    //    [StringLength(20, ErrorMessage = "Budget Name must between 5 and 20 characters"), MinLength(5, ErrorMessage = "Budget Name must between 5 and 20 characters")]
    //    public string Name { get; set; }
    //    [NotMapped]
    //    public decimal BudgetItemsTotal
    //    {
    //        get
    //        {
    //            return BudgetItems.Where(t => t.IsActive == true ).Sum(x => x.Amount);
    //        }
    //        private set
    //        {

    //        }
    //    }
    //    [NotMapped]
    //    public decimal TransactionsItemsTotal
    //    {
    //        get
    //        {
    //            return Transactions.Where(t => t.IsActive == true && t.IsVoid == false).Sum(x => x.Amount);
    //        }
    //        private set
    //        {

    //        }
    //    }

    //    //FKs
    //    public int HouseholdId { get; set; }

    //    //Virtual Properties
    //    public virtual Household Household { get; set; }
    //    public virtual ICollection<Transaction> Transactions { get; private set; }
    //    public virtual ICollection<BudgetItem> BudgetItems { get; private set; }
    //}

    //###########################################################
    //##################### BudgetItem Model #####################
    //###########################################################
    public class BudgetItem
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        [Range(double.MinValue, double.MaxValue)]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Budget Item Name must between 5 and 20 characters"), MinLength(5, ErrorMessage = "Budget Item Name must between 5 and 20 characters")]
        public string Name { get; set; }
        [NotMapped]
        public decimal SpentPercentage
        {
            get
            {
                if (Category != null)
                {
                    var TransactionTotal = Category.Transactions.Where(x => x.Date.Month == DateTime.Now.Month && x.Date.Year == DateTime.Now.Year && x.Account.HouseholdId == HouseholdId && x.IsActive == true && x.IsVoid == false).Sum(x => x.Amount);
                    var percentage = Math.Abs(TransactionTotal / Amount) * 100;
                    return Math.Round(percentage, 2);
                }
                else
                {
                    return 0;
                }
                
            }
            private set { }
        }

        //FKs
        public int CategoryId { get; set; }
        public int HouseholdId { get; set; }
        //public int BudgetId { get; set; }

        //Virtual Properties
        public virtual Household Household { get; set; }
        public virtual Category Category { get; set; }
        //public virtual Budget Budget { get; set; }
    }
}