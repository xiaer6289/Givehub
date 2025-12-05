using System.ComponentModel.DataAnnotations;

namespace Givehub.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Please enter a Name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a Phone number.")]
        [MaxLength(11, ErrorMessage = "Phone number cannot exceed 11 digits")]
        [RegularExpression(@"^01[0-9]{8,9}$", ErrorMessage = "Phone number must start with '01' and be 10-11 digits long")]
        public string PhoneNo { get; set; }

        [Required(ErrorMessage = "Plesae enter a Password")]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "Password must be 8-20 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,20}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character (!@#$%^&*)")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
