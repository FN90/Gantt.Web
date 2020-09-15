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
    public class TachesController : Controller
    {
        private GanttDatabaseEntities db = new GanttDatabaseEntities();

        // GET: Taches
        public ActionResult Index()
        {
            // get projects and get composants for each project
            List<Task> projects = db.Tasks.Where(t => t.ParentId == null).ToList();
            List<Tache> tasks = new List<Tache>();
            foreach (var project in projects)
            {
                var projectComposants = db.Tasks.Where(t => t.ParentId == project.Id).ToList();
                foreach (var comp in projectComposants)
                {
                    var composantTasks = db.Tasks.Where(t => t.ParentId == comp.Id).ToList();
                    foreach (var task in composantTasks)
                    {
                        Tache tache = new Tache(task);
                        tache.Composant = comp.Text;
                        tache.Project = project.Text;
                        tasks.Add(tache);
                    }
                }
            }
            return View(tasks);
        }

        // GET: Taches/Details/5
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

        // GET: Taches/Create
        public ActionResult Create()
        {
            // get projects and send them to the create form
            List<Task> dbProjects = db.Tasks.Where(t => t.ParentId == null).ToList();
            List<Projet> projects = new List<Projet>();
            foreach (var project in dbProjects)
            {
                List<Task> composants = db.Tasks.Where(t => t.ParentId == project.Id).ToList();
                List<Composant> projectComposants = new List<Composant>();
                foreach (var item in composants)
                {
                    // get project tasks
                    List<Task> composantTasks = db.Tasks.Where(t => t.ParentId == item.Id).ToList();
                    projectComposants.Add(new Composant()
                    {
                        Id = item.Id,
                        Duration = item.Duration,
                        Progress = item.Progress,
                        SortOrder = item.SortOrder,
                        StartDate = item.StartDate,
                        Type = item.Type,
                        Text = item.Text,
                        Tasks = composantTasks
                    });
                }

                projects.Add(new Projet()
                {
                    Id = project.Id,
                    Duration = project.Duration,
                    Progress = project.Progress,
                    SortOrder = project.SortOrder,
                    StartDate = project.StartDate,
                    Type = project.Type,
                    Text = project.Text,
                    Composants = projectComposants
                });
            }

            ViewBag.Projects = projects;
            return View();
        }

        // POST: Taches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Create([Bind(Include = "Id,Text,StartDate,Duration,Progress,SortOrder,Type,ParentId,CentreCharge,Employe")] Task task)
        {
            task.Progress = 0;
            task.Type = "task";            
            // add task's link
            List<Task> currentProjectTasks = db.Tasks.Where(t => t.ParentId == task.ParentId).ToList();

            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                if (currentProjectTasks.Count() > 0)
                {
                    var precedentTaskId = Request.Form["PredecessorId"];
                    Link link = new Link()
                    {
                        SourceTaskId = int.Parse(precedentTaskId),
                        TargetTaskId = task.Id,
                        Type = "0"
                    };
                    db.Links.Add(link);
                    db.SaveChanges();
                }

                return Redirect("/gantt/index");
            }

            return View(task);
        }

        // GET: Taches/Edit/5
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

        // POST: Taches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Edit([Bind(Include = "Id,Text,StartDate,Duration,Progress,SortOrder,Type,ParentId,CentreCharge,Employe")] Task task)
        {
            if (ModelState.IsValid)
            {
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(task);
        }

        // GET: Taches/Delete/5
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

        // POST: Taches/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Tasks.Find(id);
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
