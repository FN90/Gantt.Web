using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gantt.Web.Models
{
    public class Projet
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public decimal Progress { get; set; }
        public int SortOrder { get; set; }
        public string Type { get; set; }
        public List<Composant> Composants { get; set; }
    }
}