using System.Web.Mvc;
using Budgeter.Models;
using System.Text;
using Microsoft.AspNet.Identity;
using System;
using Budgeter.Helpers;
using System.Linq;
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
using Budgeter.Helpers;

namespace Budgeter.Controllers
{
    public class SaverController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult MyHousehold()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var household = currentUser.Household;
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
