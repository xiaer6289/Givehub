using Givehub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Givehub.Helpers;

namespace Givehub.Controllers;

public class DonorController : Controller
{
    private readonly DB db;
    private readonly Helpers.Helper _helper;
    public DonorController(DB db, Helpers.Helper helper)
    {
        this.db = db;
        this._helper = helper;
    }

    public IActionResult MoneyReport()
    {
        try
        {
            int donorId = _helper.GetLoggedDonorId();
            var donation = db.Donations.Include(d => d.Donees)
                .Where(d => d.DonorId == donorId && d.Amount != null)
                .OrderByDescending(d => d.Date)
                .ToList();
            return View(new DonationVM
            {
                Donation = donation
            });
        }
        catch (NotLoggedInException)
        {
            return RedirectToAction("Login", "Account");
        }

    }

    public IActionResult ItemReport()
    {
        try
        {
            int donorId = _helper.GetLoggedDonorId();
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
        catch (NotLoggedInException)
        {
            return RedirectToAction("Login", "Account");
        }

    }
}
