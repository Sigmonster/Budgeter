using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
            this.Budgets = new HashSet<Budget>();
            //this.BudgetItems = new HashSet<BudgetItem>();
            //this.Transactions = new HashSet<Transaction>();
            //this.TransactionType = new HashSet<TransactionType>();
            this.InvitedNotRegisteredEmail = new HashSet<InvitedButNotRegisteredEmail>();
        }
        public int Id { get; set; }//PK
        public bool IsActive { get; set; }
        [Required]
        [MaxLength(20, ErrorMessage ="Household Name must between 5 and 20 characters"), MinLength(5,ErrorMessage = "Household Name must between 5 and 20 characters")]
        public string Name { get; set; }
        public HouseholdDetails HouseholdDetail { get; set; }

        //Forigen Keys
        public string OwnerUserId { get; set; }//FK


        //Forigen Keys Tables
        [InverseProperty("HouseholdsOwned")]
        public virtual ApplicationUser OwnerUser { get; set; }//Holds Associated FK OwnerUser


        //Holds Multiple Associated Records
        [InverseProperty("Household")]
        public virtual ICollection<ApplicationUser> Members { get; set; }
        [InverseProperty("Invitations")]
        public virtual ICollection<ApplicationUser> InvitedRegisteredUsers { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        //public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        //public virtual ICollection<Transaction> Transactions { get; set; }
        //public virtual ICollection<TransactionType> TransactionType { get; set; }
        public virtual ICollection<InvitedButNotRegisteredEmail> InvitedNotRegisteredEmail { get; set; }
    }
    //######Household Details######
    [ComplexType]
    public class HouseholdDetails
    {
        [Column("Created")]
        public DateTimeOffset? Created { get; set; }
        [Column("Updated")]
        public DateTimeOffset? Updated { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Household Description must be less than 30 characters long")]
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
        public bool IsActive { get; set; }
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Date { get; set; }
        [StringLength(150, ErrorMessage ="Description cannot exceed 150 characters.")]
        public string Description { get; set; }
        [Range(double.MinValue, double.MaxValue)]
        public decimal Amount { get; set; }
        public bool IsReconciled { get; set; }
        public bool IsExpense { get; set; }
        public bool IsVoid { get; set; }
        [Range(double.MinValue, double.MaxValue)]
        //public decimal ReconciledAmount { get; set; }

        //FKs
        //public int TransactionTypeId { get; set; }//Remove
        public int CategoryId { get; set; }
        public string EnteredById { get; set; }
        public int AccountId { get; set; }

        //Virtual Properties
        //public virtual TransactionType TransactionType { get; set; }//Remove
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
        }

        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

    }

    //###########################################################
    //################### TransactionType Model #################
    //###########################################################
    //public class TransactionType
    //{
    //    public TransactionType()
    //    {
    //        this.Transactions = new HashSet<Transaction>();
    //    }

    //    public int Id { get; set; }
    //    [StringLength(25, ErrorMessage = "Description cannot exceed 25 characters.")]
    //    public string Name { get; set; }

    //    //FKs
    //    //public int HouseholdId { get; set; }

    //    //Virtual Properties
    //    //public virtual Household Household { get; set; }// One to one
    //    public ICollection<Transaction> Transactions { get; private set; }
    //}

    //###########################################################
    //###################### Budget Model #######################
    //###########################################################
    public class Budget
    {
        public Budget()
        {
            this.BudgetItems = new HashSet<BudgetItem>();
        }

        public int Id { get; set; }
        public bool IsActive { get; set; }
        [StringLength(25, ErrorMessage = "Description cannot exceed 25 characters.")]
        public string Name { get; set; }

        //FKs
        public int HouseholdId { get; set; }

        //Virtual Properties
        public virtual Household Household { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; private set; }
    }

    //###########################################################
    //##################### BudgetItem Model #####################
    //###########################################################
    public class BudgetItem
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        [Range(double.MinValue, double.MaxValue)]
        public decimal Amount { get; set; }

        //FKs
        public int CategoryId { get; set; }
        public int BudgetId { get; set; }
        //public int HouseholdId { get; set; }

        //Virtual Properties
        //public virtual Household Household { get; set; }// One to one
        public virtual Category Category { get; set; }
        public virtual Budget Budget { get; set; }
    }
}