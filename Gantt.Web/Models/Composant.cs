using Gantt.Web.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gantt.Web.Models
{
    public class Composant
    {

        public Composant() { }
        public Composant(Task task)
        {
            Id = task.Id;
            Duration = task.Duration;
            Progress = task.Progress;
            SortOrder = task.SortOrder;
            StartDate = task.StartDate;
            Type = task.Type;
            Text = task.Text;
        }
        public string Projet { get; set; }
        public int Id { get; set; }
        [MaxLength(255)]
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public decimal Progress { get; set; }
        public int SortOrder { get; set; }
        public string Type { get; set; }
        public List<Task> Tasks { get; set; }
    }
}