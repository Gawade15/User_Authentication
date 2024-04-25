using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserProject1.Models;

namespace UserProject1.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private ProjectDataEntities1 dbContext = new ProjectDataEntities1();
        public ActionResult Index()
        {
            using
                (ProjectDataEntities1 db = new ProjectDataEntities1())
            {
                return View(db.UserDetails.ToList());
            }
        }

            public ActionResult Activate(int id)
        {
            var user = dbContext.UserDetails.Find(id);
            if (user != null)
            {
                user.IsActive = true;
                dbContext.SaveChanges();
            }

            // Redirect back to the Index page after activation
            return RedirectToAction("Index");
        }

        // Deactivate Action
        public ActionResult DeActivate(int id)
        {
            var user = dbContext.UserDetails.Find(id);
            if (user != null)
            {
                user.IsActive = false;
                dbContext.SaveChanges();
            }

            // Redirect back to the Index page after deactivation
            return RedirectToAction("Index");
        }

        // Delete Action
        public ActionResult Delete(int id)
        {
            var user = dbContext.UserDetails.Find(id);
            if (user != null)
            {
                dbContext.UserDetails.Remove(user);
                dbContext.SaveChanges();
            }

            // Redirect back to the Index page after deletion
            return RedirectToAction("Index");
        }
    

    }
}