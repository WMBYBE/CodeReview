using System.Collections.Generic;

namespace SportsPro.Models {
    public class TechIncidentViewModel {
        public List<Incident>? Incidents { get; set; }
        public List<Technician>? Technicians { get; set; }
        public Technician? SelectedTech { get; set; }
    }
}
