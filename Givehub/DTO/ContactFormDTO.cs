using System.ComponentModel.DataAnnotations;

namespace Givehub.DTO;

public class ContactFormDTO
{
    [Required(ErrorMessage = "Field Name is required")]
    [StringLength(100, ErrorMessage = "Name must not long than 100 characters")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Field PhoneNo is required")]
    [RegularExpression(@"^01[0-9]{8,9}$", ErrorMessage = "Phone number must start with '01' and be 10 to 11 digits long.")]
    public string PhoneNo { get; set; }

    [Required(ErrorMessage = "Field PhoneNo is required")]
    [EmailAddress(ErrorMessage ="Invalid email format")]
    public string Email { get; set; }
    public List<string>? Needs { get; set; } 

}
