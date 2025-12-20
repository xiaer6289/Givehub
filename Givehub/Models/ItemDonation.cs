using System.ComponentModel.DataAnnotations;
using Givehub.Models;
using Microsoft.AspNetCore.Mvc;

namespace Givehub.Models
{
    public class ItemDonation 
    {

        [Required(ErrorMessage = "Delivery Date is required")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        public string DonateAddress { get; set; } = "";
        public string DoneeName { get; set; } = "";
        public int DoneeId { get; set; } 

        [Required(ErrorMessage = "At least one item is required")]
        [MinLength(1, ErrorMessage = "You must add at least one item")]
        public List<ItemEntry>? Items { get; set; } = new List<ItemEntry>();

    }

    public class ItemEntry
    {
        public string ItemName { get; set; } = "";

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 50, ErrorMessage = "Quantity must be between 1 and 50")]
        public int Quantity { get; set; }
    }

    public class ItemDonationViewModel
    {

        public int DoneeId { get; set; }
        public ItemDonation Donation { get; set; } = new ItemDonation();

        // Items allowed by the Donee (for dropdown)
        public List<string> AvailableItems { get; set; } = new();
    }

}
