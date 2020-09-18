using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Gantt.Web.Models;

namespace Gantt.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext context;
        public UsersController()
        {
            context = new ApplicationDbContext();
        }
        // GET: Users
        public ActionResult Index()
        {
            var dbUsers = context.Users;

            List<UserViewModel> users = new List<UserViewModel>();

            foreach (var user in dbUsers)
            {
                users.Add(new UserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Nom = user.FirstName + " " + user.LastName,
                    Role = GetRoleName(user.Id)
                });
            }
            return View(users);
        }

        private string GetRoleName(string id)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userManager.GetRoles(id);

            return roles.Count() > 0 ? roles.ElementAt(0) : "s.a";

        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.Roles = new SelectList(context.Roles.ToList(), "Name", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {

            ViewBag.Roles = new SelectList(context.Roles.ToList(), "Name", "Name");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                };
                context.Users.Add(user);
                context.SaveChanges();
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                await userManager.AddToRoleAsync(user.Id, model.UserRoles);
                TempData["Message"] = "Le nouveau utilisateur a été ajouté avec succès";
                return RedirectToAction("Index");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id = null)
        {

            //ViewBag.Roles = context.Roles.ToList();
            var user = context.Users.First(u => u.Id == id);
            var model = new EditUserViewModel(user);
            model.UserRoles = GetRoleName(user.Id);
            ViewBag.Roles = new SelectList(context.Roles.ToList(), "Name", "Name", GetRoleName(user.Id));
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            ViewBag.Roles = new SelectList(context.Roles.ToList(), "Name", "Name");

            model.UserName = model.Email;
            if (ModelState.IsValid)
            {
                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var manager = new UserManager<ApplicationUser>(store);
                var user = manager.FindById(model.Id);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.Email;
                user.Email = model.Email;
                user.UserName = model.Email;

                await manager.UpdateAsync(user);
                var ctx = store.Context;
                ctx.SaveChanges();
                //await context.SaveChangesAsync();


                // THIS LINE IS IMPORTANT
                var oldUser = userManager.FindById(user.Id);
                var oldRoleId = oldUser.Roles.SingleOrDefault().RoleId;
                var oldRoleName = context.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;

                if (oldRoleName != model.UserRoles)
                {
                    userManager.RemoveFromRole(user.Id, oldRoleName);
                    userManager.AddToRole(user.Id, model.UserRoles);
                }
                context.Entry(user).State = EntityState.Modified;


                TempData["Message"] = "L'utilisateur a été modifié avec succès";
                return RedirectToAction("Index");
            }

            ViewBag.Roles = new SelectList(context.Roles.ToList(), "Name", "Name", GetRoleName(model.Id));
                

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id = null)
        {
            var user = context.Users.First(u => u.Id == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(string id = null)
        {
            var user = context.Users.First(u => u.Id == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult Profile(string id = null)
        {
            var user = context.Users.First(u => u.Id == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = context.Users.First(u => u.Id == id);
            context.Users.Remove(user);
            context.SaveChanges();
            TempData["Message"] = "L'utilisateur a été supprimé avec succès";
            return RedirectToAction("Index");
        }


        [AllowAnonymous]
        public ActionResult NotAuthorized(string id)
        {
            return View();
        }
    }
}