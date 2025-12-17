namespace Givehub.Models
{

    public class ItemManagementVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string DonorName { get; set; } = "";
        public string DoneeName { get; set; } = "";
        public string Status { get; set; } = "";
        public List<ItemDetails> Items { get; set; } = new List<ItemDetails>();

    }

    public class ItemDetails
    {
        public string ItemName { get; set; } = "";
        public int Quantity { get; set; }
    }

}
