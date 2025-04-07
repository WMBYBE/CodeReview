using System.Collections.Generic;

namespace SportsPro.Models
{
    public class RegistrationViewModel
    {
        public Customer Customer { get; set; }
        public List<Product> RegisteredProducts { get; set; }
        public List<Product> AvailableProducts { get; set; }
    }
}
