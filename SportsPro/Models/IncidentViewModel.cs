using System;

namespace SportsPro.Models
{
    public class IncidentViewModel
    {
        public int IncidentID { get; set; }
        public string Title { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public DateTime DateOpened { get; set; }
    }
}
