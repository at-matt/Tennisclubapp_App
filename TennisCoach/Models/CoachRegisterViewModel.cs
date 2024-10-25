﻿using System.ComponentModel.DataAnnotations;

namespace TennisCoach.Models
{
    public class CoachRegisterViewModel
    {
        public int CoachId { get; set; } // Primary Key (will auto-increment)

        public string? Biography { get; set; } // Optional field for coach biography

        [Required]
        [EmailAddress] // Validates the email format
        public string Email { get; set; } // For email login

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
