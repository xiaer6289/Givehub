using Microsoft.AspNetCore.Mvc;

namespace Givehub.Models
{
    public class ItemDonationVM 
    {
        public DateTime DeliveryDate { get; set; }
        public List<ItemEntry> Items { get; set; } = new List<ItemEntry>();

    }

    public class ItemEntry
    {
        public string ItemName { get; set; } = "";
        public int Quantity { get; set; }
    }


}
