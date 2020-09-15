using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gantt.Web.Controllers
{
    [Authorize(Roles = "Admin, Employé")]
    public class GanttController : Controller
    {
        // GET: Gantt
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Projet(int? id)
        {
            ViewBag.ProjetId = id;
            return View();
        }
    }
}