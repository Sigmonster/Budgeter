using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Collections.Generic;

namespace Budgeter.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Invitations = new HashSet<Household>();
            this.HouseholdsOwned = new HashSet<Household>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }

        //FKs
        public int? HouseholdId { get; set; }//FK

        //Virtual Properties
        public virtual Household Household { get; set; }//Holds Associated FK Household
        [InverseProperty("InvitedRegisteredUsers")]
        public virtual ICollection<Household> Invitations { get; set; }
        [InverseProperty("OwnerUser")]
        public virtual ICollection<Household> HouseholdsOwned { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Household> Household { get; set; }
        public DbSet<InvitedButNotRegisteredEmail> InvitedButNotRegisteredEmail { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Category> Category { get; set; }
        //public DbSet<TransactionType> TransactionType { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<BudgetItem> BudgetItem { get; set; }

    }
    //Add Stored Procedures
    //public class HouseholdContext : DbContext
    //{
    //    public DbSet<Household> Household { get; set; }
    //    public DbSet<InvitedButNotRegisteredEmail> InvitedButNotRegisteredEmail { get; set; }
    //    public override void OnModelCreating(DbModelBuilder model)
    //    {

    //    }
    //}


}