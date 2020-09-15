using Gantt.Web.DAL;
using Gantt.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gantt.Web.Controllers
{
    public class ApiController : Controller
    {
        private GanttDatabaseEntities db = new GanttDatabaseEntities();

        [HttpGet]
        public JsonResult Data(int? id)
        {
            List<Task> tasks = db.Tasks.ToList();
            List<Task> ganttTasks = new List<Task>();
            if (id != null)
            {
                Task task = db.Tasks.Where(t=>t.Id == id).FirstOrDefault();
                ganttTasks.Add(task);
                tasks = db.Tasks.Where(t=>t.ParentId == id).ToList();
                ganttTasks.AddRange(tasks);

                // add Composants tasks
                foreach(var composant in tasks)
                {
                    List<Task> taches = db.Tasks.Where(t => t.ParentId == composant.Id).ToList();
                    ganttTasks.AddRange(taches);
                }

            }
            else
            {
                ganttTasks = db.Tasks.ToList();
            }
            List<GanttDataViewModel> data = new List<GanttDataViewModel>();
            foreach (var task in ganttTasks)
            {
                data.Add(new GanttDataViewModel(task));
            }

            List<Link> dbLinks = db.Links.ToList();
            List<GanttLinkViewModel> links = new List<GanttLinkViewModel>();
            foreach (var link in dbLinks)
            {
                links.Add(new GanttLinkViewModel(link));
            }


            var jsonData = new
            {
                // create tasks array
                data = data.ToArray(),
                // create links array
                links = links.ToArray()
            };

            return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        [HttpGet]
        public JsonResult GetComposantTasks(int id)
        {
            List<Task> tasks = db.Tasks.Where(t => t.ParentId == id).ToList();
            Task task = db.Tasks.Where(t => t.Id == id).FirstOrDefault();
            Task parent = db.Tasks.Where(t => t.Id == task.ParentId).FirstOrDefault();
            //List<GanttDataViewModel> data = new List<GanttDataViewModel>();
            //foreach (var item in tasks)
            //{
            //    data.Add(new GanttDataViewModel(item));
            //}

            var jsonData = new
            {
                task,
                parent,
                // create tasks array
                tasks,
            };

            return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetTaskParent(int id)
        {
            Task task = db.Tasks.Where(t => t.Id == id).FirstOrDefault();
            Task parent = db.Tasks.Where(t => t.Id == task.ParentId).FirstOrDefault();
            var jsonData = parent;

            return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetTask(int id)
        {
            Task task = db.Tasks.Where(t => t.Id == id).FirstOrDefault();
            var jsonData = task;

            return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [HttpGet]
        public JsonResult TaskType(int id)
        {
            Task task = db.Tasks.Where(t => t.Id == id).FirstOrDefault();
            string type = "task";
            if (task.ParentId == null)
            {
                type = "project";
            }
            else
            {
                // if task parent is a project: the current task is composant
                Task parent = db.Tasks.Where(t => t.Id == task.ParentId).FirstOrDefault();
                if (parent.ParentId == null)
                {
                    type = "composant";
                }
            }

            var jsonData = new
            {
                type
            };

            return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}