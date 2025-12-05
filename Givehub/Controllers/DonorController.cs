using Givehub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Givehub.Controllers;

public class DonorController : Controller
{
    private readonly DB db;

    public DonorController(DB db)
    {
        this.db = db;
    }

    public IActionResult MoneyReport()
    {
        int donorId = GetLoggedDonorId();
        var donation = db.Donations.Include(d => d.Donees)
            .Where(d => d.DonorId == donorId && d.Amount != null)
            .OrderByDescending(d => d.Date)
            .ToList();
        return View(new DonationVM
        {
            Donation = donation
        });
    }

    public IActionResult ItemReport()
    {
        int donorId = GetLoggedDonorId();
        var donation = db.Donations
            .Include(d => d.Donees)
            .Where(d => d.DonorId == donorId && d.ItemsJson != null)
            .OrderByDescending(d => d.Date)
            .ToList();

        return View(new DonationVM
        {
            Donation = donation
        });
    }
}
