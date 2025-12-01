using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Givehub.Models.ViewModels
{
    public class DoneeVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a  Name.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please select a Category.")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Please select a Address.")]
        public string? Address { get; set; }

        public string? Requirements { get; set; }

        public string? Description { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
