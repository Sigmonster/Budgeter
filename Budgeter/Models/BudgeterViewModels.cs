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

namespace Budgeter.Models
{
    //###########################################################
    //##################### Household Model #####################
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
    //##################### UserDashboardVM #####################
    public class EmailInvitationsVM
    {
        public string[] emailInvites { get; set; }
    }

}