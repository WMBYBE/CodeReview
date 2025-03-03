﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SportsPro.Models
{
    public class Technician
    {
        internal object Character;

        public int TechnicianID { get; set; }	   

		[Required]
		public string Name { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string Phone { get; set; }
	}
}
