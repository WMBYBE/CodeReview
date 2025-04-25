using Microsoft.VisualBasic;
using System;

namespace SportsPro.Models
{
    public class CustomerProduct
    {
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
