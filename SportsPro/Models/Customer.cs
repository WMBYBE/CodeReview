using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Mvc;

namespace SportsPro.Models
{
    public class Customer
    {
		public int CustomerID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Address must be between 1 and 50 characters.")]
        public string Address { get; set; }


        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "City must be between 1 and 50 characters.")]
        public string City { get; set; }


        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "State must be between 1 and 50 characters.")]
        public string State { get; set; }


        [Required(ErrorMessage = "Postal Code is required.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Postal Code must be between 1 and 20 characters.")]
        public string PostalCode { get; set; }

		[Required(ErrorMessage = "Not a valid country")]
		public string CountryID { get; set; } 
		public Country Country { get; set; }

        [StringLength(20, ErrorMessage = "Not a valid phone number.")]
        [RegularExpression(@"\(\d\d\d\)\s\d\d\d-\d\d\d\d", ErrorMessage = "Phone number must be in the format (999) 999-9999.")]
        public string Phone { get; set; }

        [StringLength(50)]
        [Remote("CheckEmail", "Customer", ErrorMessage = "This Email is already in use")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

		public string FullName => FirstName + " " + LastName;   // read-only property
	}
}