using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace FYP.Models
{
    public class Member
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Invalid, must be at least 4 characters long")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Invalid, must be at least 6 characters long")]
        public string Password { get; set; }

        [NotMapped]
       // [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != ConfirmPassword)
            {
                yield return new ValidationResult("Passwords mismatch", new[] { "Member" });
            }
        }
    }

}