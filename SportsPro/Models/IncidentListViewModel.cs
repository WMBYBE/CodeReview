using System.Collections.Generic;

namespace SportsPro.Models
{
    public class IncidentListViewModel
    {
        public List<IncidentViewModel> Incidents { get; set; }
        public string FilterResult { get; set; }
    }
}
