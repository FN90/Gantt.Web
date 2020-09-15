using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gantt.Web.DAL;

namespace Gantt.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProjetsController : Controller
    {
        private GanttDatabaseEntities db = new GanttDatabaseEntities();

        // GET: Projets
        public ActionResult Index()
        {
            return View(db.Tasks.Where(t => t.ParentId == null).ToList());
        }

        // GET: Projets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        [Authorize(Roles = "Admin")]
        // GET: Projets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Text,StartDate,Duration,Progress,SortOrder,Type,ParentId,CentreCharge,Employe")] Task task)
        {
            if (ModelState.IsValid)
            {                
                db.Tasks.Add(task);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(task);
        }

        // GET: Projets/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: Projets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Text,StartDate,Duration,Progress,SortOrder,Type,ParentId,CentreCharge,Employe")] Task task)
        {
            if (ModelState.IsValid)
            {
                task.Progress = task.Progress / 100;
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(task);
        }

        // GET: Projets/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: Projets/Delete/5
        [HttpPost, ActionName("Delete")]
        
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Tasks.Find(id);
            // get all related composants and delete them, and delete their childs
            List<Task> composants = db.Tasks.Where(t => t.ParentId == id).ToList();
            foreach(var composant in composants)
            {
                List<Task> taches = db.Tasks.Where(t => t.ParentId == composant.Id).ToList();
                foreach (var tache in taches)
                {
                    db.Tasks.Remove(tache);                    
                }
                db.Tasks.Remove(composant);                
            }
            db.SaveChanges();

            db.Tasks.Remove(task);
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
