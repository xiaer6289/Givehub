using System.ComponentModel.DataAnnotations;

namespace Givehub.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = " Please enter a Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a Password")]
        public string Password { get; set; }
    }
}
