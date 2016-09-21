using Budgeter.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class AppHelper
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        public AppHelper(ApplicationDbContext context)
        {
            this.userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            this.db = context;
        }
        public void AddNonRegisteredInvitation(string email, int householdId)
        {
            db.InvitedButNotRegisteredEmail.Add(new InvitedButNotRegisteredEmail() {Email = email, HouseholdId = householdId, DateSent = DateTime.Now });
            db.SaveChanges();
        }

        public void AddRegisteredInvitation(ApplicationUser userInvitee, int householdId)
        {
            var household = db.Household.Find(householdId);
            var user = userManager.FindById(userInvitee.Id);
            user.Invitations.Add(household);
            var result = userManager.Update(user);
        }

        internal void RemoveUserInvitation(Household household, string UserId)
        {
            var user = userManager.FindById(UserId);
            user.Invitations.Remove(household);
            var result = userManager.Update(user);
        }
        internal void RemoveNonRegisteredInvitation(int invitationId)
        {
            var invite = db.InvitedButNotRegisteredEmail.FirstOrDefault(x=>x.Id == invitationId);
            db.InvitedButNotRegisteredEmail.Remove(invite);
            db.SaveChanges();
        }

        internal void LeaveHousehold(string UserId)
        {
            var user = userManager.FindById(UserId);
            user.HouseholdId = null;
            var result = userManager.Update(user);
        }

        
    }
}