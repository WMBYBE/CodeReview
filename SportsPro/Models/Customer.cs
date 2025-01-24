using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SportsPro.Models
{
    public class Customer
    {
		public int CustomerID { get; set; }

		[Required(ErrorMessage = "Not a name")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Not a name")]
		public string LastName { get; set; }

		[Required]
		public string Address { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string State { get; set; }

		[Required]
		public string PostalCode { get; set; }

		[Required (ErrorMessage ="Not a valid country")]
        public string CountryID { get; set; }
		public Country Country { get; set; }

		[RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$", ErrorMessage ="Not a valid Phone Number") ]
		public string Phone { get; set; }

		public string Email { get; set; }

		public string FullName => FirstName + " " + LastName;   // read-only property
	}
}