using Gantt.Web.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gantt.Web.Models
{
  
    public class Tache
    {
        public Tache() { }
        public Tache(Task task)
        {
            Id = task.Id;
            Duration = task.Duration;
            Progress = task.Progress;
            SortOrder = task.SortOrder;
            StartDate = task.StartDate;
            Type = task.Type;
            Text = task.Text;            
        }

        public string Project { get; set; }
        public string Composant { get; set; }
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public decimal Progress { get; set; }
        public int SortOrder { get; set; }
        public string Type { get; set; }
        public List<Task> Tasks { get; set; }
    }
}