using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gantt.Web.DAL;
using Gantt.Web.Models;

namespace Gantt.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComposantsController : Controller
    {
        private GanttDatabaseEntities db = new GanttDatabaseEntities();

        // GET: Composants
        public ActionResult Index()
        {
            // get projects and get composants for each project
            List<Task> projects = db.Tasks.Where(t => t.ParentId == null).ToList();
            List<Composant> composants = new List<Composant>();
            foreach (var project in projects)
            {
                var projectComposants = db.Tasks.Where(t => t.ParentId == project.Id).ToList();
                foreach (var comp in projectComposants)
                {
                    var composant = new Composant(comp);
                    composant.Projet = project.Text;
                    composants.Add(composant);
                }
            }
            return View(composants);
        }

        // GET: Composants/Details/5
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

        // GET: Composants/Create
        public ActionResult Create()
        {

            List<Task> projects = db.Tasks.Where(t => t.ParentId == null).ToList();
            ViewBag.Projects = projects;
            return View();
        }

        // POST: Composants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Create([Bind(Include = "Id,Text,StartDate,Duration,Progress,SortOrder,Type,ParentId,CentreCharge,Employe")] Task task)
        {

            List<Task> projects = db.Tasks.Where(t => t.ParentId == null).ToList();
            ViewBag.Projects = projects;
            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(task);
        }

        // GET: Composants/Edit/5
        public ActionResult Edit(int? id)
        {
            List<Task> projects = db.Tasks.Where(t => t.ParentId == null).ToList();
            ViewBag.Projects = projects;
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

        // POST: Composants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Edit([Bind(Include = "Id,Text,StartDate,Duration,Progress,SortOrder,Type,ParentId,CentreCharge,Employe")] Task task)
        {
            List<Task> projects = db.Tasks.Where(t => t.ParentId == null).ToList();
            ViewBag.Projects = projects;
            if (ModelState.IsValid)
            {
                task.Progress = task.Progress / 100;
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(task);
        }

        // GET: Composants/Delete/5
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

        // POST: Composants/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Tasks.Find(id);

            List<Task> taches = db.Tasks.Where(t => t.ParentId == id).ToList();
            foreach (var tache in taches)
            {
                db.Tasks.Remove(tache);
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
