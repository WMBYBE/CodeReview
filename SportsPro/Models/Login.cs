using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsPro.Models
{
    public class Login
    {
        [Key]

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}