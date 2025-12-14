using System.ComponentModel.DataAnnotations;

public class ResetPasswordVM
{
    [Required]
    public string Token { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Plesae enter a Password")]
    [StringLength(200, MinimumLength = 8, ErrorMessage = "Password must be 8-20 characters long")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,20}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character (!@#$%^&*)")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}