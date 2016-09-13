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
    public class Post : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult MyHousehold()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var household = currentUser.Household;
            return View(household);
        }

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
