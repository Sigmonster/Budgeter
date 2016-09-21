using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using Budgeter.Models;
using Budgeter.Helpers;
using CsvHelper;

namespace Budgeter.Models
{
    //###########################################################
    //##################### Household VMs #######################
    public class HouseholdEditVM
    {
        public int Id { get; set; }//PK
        public bool IsActive { get; set; }
        [Required]
        [MaxLength(20, ErrorMessage = "Household Name must between 5 and 20 characters"), MinLength(5, ErrorMessage = "Household Name must between 5 and 20 characters")]
        public string Name { get; set; }

        //Forigen Keys
        public string OwnerUserId { get; set; }//FK
        //Forigen Keys Tables
        public virtual ApplicationUser OwnerUser { get; set; }//Holds Associated FK OwnerUser

        [Required]
        [MaxLength(250, ErrorMessage = "Household Description must be less than 250 characters long")]
        [Column("Description", TypeName = "nvarchar")]
        public string Description { get; set; }
    }

    //###########################################################
    //##################### UserDashboardVM #####################
    public class UserDashboardVM
    {
        public UserDashboardVM()
        {

        }
        public ApplicationUser currentUser { get; set; }

    }
    //###########################################################
    //##################### EmailInvitationsVM ##################
    public class EmailInvitationsVM
    {
        public string[] emailInvites { get; set; }
    }

    //###########################################################
    //##################### Household VMs #######################
    public class AccountCreateVM
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [Required]
        [Range(double.MinValue, double.MaxValue)]
        public decimal Balance { get; set; }
    }
    //##############################################################
    //##################### AccountManage VM #######################
    public class AccountManageVM
    {
        public Household Household { get; set; }
        public Account Account { get; set; }
        public Transaction Transaction { get; set; }
    }

    public class AccountFetchVM
    {
        public Household Household { get; set; }
        public Account Account { get; set; }
        public List<Transaction> TransactionList { get; set; }
        public Transaction Transaction { get; set; }
    }
    //##############################################################
    //##################### UploadVerification VM ##################
    public class UploadVerficationVM
    {
        //public List<Transaction> TransactionList { get; set; }


    }

}