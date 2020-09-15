using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gantt.Web.DAL;

namespace Gantt.Web.Models
{
    public class GanttDataViewModel
    {

        public GanttDataViewModel() { }
        public GanttDataViewModel(Task task)
        {
            id = task.Id;
            text = task.Text;
            start_date = task.StartDate.ToString("u");
            duration = task.Duration;
            order = task.SortOrder;
            progress = task.Progress;
            open = true;
            parent = task.ParentId;
            type = (task.Type != null) ? task.Type : String.Empty;
        }
        public int id { get; set; }
        public string text { get; set; }
        public string start_date { get; set; }
        public int duration { get; set; }
        public int order { get; set; }
        public int? parent { get; set; }
        public string type { get; set; }
        public bool open { get; set; }
        public decimal progress { get; set; }

    }


    public class GanttLinkViewModel
    {
        public GanttLinkViewModel() { }
        public GanttLinkViewModel(Link link)
        {
            id = link.Id;
            source = link.SourceTaskId;
            target = link.TargetTaskId;
            type = link.Type;
        }

        public int id { get; set; }
        public int source { get; set; }
        public int target { get; set; }
        public string type { get; set; }

    }
}