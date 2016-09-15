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
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        public ActionResult Index()
        {
            var household = db.Household.Include(h => h.OwnerUser);
            return View(household.ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Household.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,HouseholdDetail")] Household household)
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                if (currentUser.HouseholdId == null)
                {
                    var timestamp = DateTimeOffset.UtcNow;
                    household.OwnerUserId = currentUser.Id;
                    household.Members.Add(currentUser);
                    household.HouseholdDetail.Created = timestamp;
                    household.IsActive = true;
                    household.Members.Add(currentUser);
                    db.Household.Add(household);
                    db.SaveChanges();
                    return RedirectToAction("MyHousehold", "Saver");

                }
                else
                {
                    return RedirectToAction("Error", "Saver", new { error = "Create Household Action, User is already in a household." });
                }
            }

            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", household.OwnerUserId);
            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Household.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", household.OwnerUserId);
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IsActive,TimeStamp,Name,HouseholdDetail,OwnerUserId")] Household model, string[] emailInvites)
        {
            if (ModelState.IsValid)
            {
                var invitations = emailInvites;
                var timestamp = DateTimeOffset.UtcNow;
                var currentUser = db.Users.Find(User.Identity.GetUserId());
                var household = db.Household.Find(currentUser.HouseholdId);
                var appHelper = new AppHelper(db);
                household.HouseholdDetail.Description = model.HouseholdDetail.Description;
                household.HouseholdDetail.Updated = timestamp;
                household.Name = model.Name;
                household.IsActive = model.IsActive;

                db.Entry(household).State = EntityState.Modified;
                
                for(int i=0; i < invitations.Length; i++)
                {
                    var emailInvitee = invitations[i].ToString();
                    if (household.Members.Where(u => u.Email.ToLower() == emailInvitee.ToLower()).Count() == 0)
                    {
                        var result1 = household.InvitedRegisteredUsers.Where(u => u.Email.ToLower() == emailInvitee.ToLower()).Count();
                        var result2 = db.Users.Where(u => u.Email.ToLower() == emailInvitee.ToLower()).Count();
                        var result3 = household.InvitedNotRegisteredEmail.Where(u => u.Email.ToLower() == emailInvitee.ToLower() && u.HouseholdId == household.Id).Count();
                        if (result1 == 0  && result2 == 1)
                        {
                            var userInvitee = db.Users.FirstOrDefault(u => u.Email == emailInvitee);
                            appHelper.AddRegisteredInvitation(userInvitee, household.Id);
                        }
                        else if(result3 == 0 && result2 == 0)
                        {
                            appHelper.AddNonRegisteredInvitation(emailInvitee, household.Id);
                        }

                    }
                }

                db.SaveChanges();

                return RedirectToAction("MyHousehold", "Saver");
            }
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", model.OwnerUserId);
            return View(model);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Household.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Household.Find(id);
            db.Household.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
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
