namespace Givehub.Models;

public class ReportVM
{
    public string DonorName { get; set; }
    public string DonorEmail { get; set; }
    public List<Donation> Donations { get; set; } = new();
}
