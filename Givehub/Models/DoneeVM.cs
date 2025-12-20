using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Givehub.Models.ViewModels
{
    public class DoneeVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a  Name.")]
        [Remote(action: "CheckNameExists", controller: "Donee")]
        public string? Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        [Required]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please select a Category.")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Please enter an Address.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please enter at least one Requirement.")]
        public List<string>? Requirements { get; set; }

        public string? Description { get; set; }
        public string? Image { get; set; }
        [Required(ErrorMessage = "Please select an image")]
        public IFormFile? ImageFile { get; set; }
    }
}
