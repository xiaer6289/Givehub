using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Givehub.Controllers;

[Authorize (Roles = "Admin")]
public class AdminController : Controller
{
    private readonly DB _db;

    public IActionResult AdminHomePage()
    {
        return View();
    }

   
    public AdminController(DB db)
    {
        _db = db;
    }

    public async Task<IActionResult> ItemManagement()
    {
        var donations = await _db.Donations
            .Include(d => d.Donors)  //navigate to related table 
            .Include(d => d.Donees)  //navigate to related table 
            .Where(d => d.Status == "Pending")

            .ToListAsync();

        var vm = donations.Select(d => new ItemManagementVM
        {
            Id = d.Id,
            DonorName = d.Donors?.Name ?? "-", //check if the donee is null,if yes return "-"
            DoneeName = d.Donees?.Name ?? "-", //check if the donee is null,if yes return "-"
            Date = d.Date,
            Status = d.Status,
            Items = d.Items?.Select(i => new ItemDetails
            {
                ItemName = i.Key,
                Quantity = i.Value
            }).ToList() ?? new List<ItemDetails>()
        }).ToList();

        return View(vm);
    }

    [HttpPost]
    public IActionResult UpdateStatus(int id,string status)
    {

        if (status == "Pending")
        {
            TempData["AlertMessage"] = "Cannot update the status to Pending.";
            return RedirectToAction("ItemManagement");
        }

        var donation = _db.Donations.FirstOrDefault(d => d.Id == id);

        if(donation !=null)
        {
            donation.Status = status;
            _db.SaveChanges();
        }

        return RedirectToAction("ItemManagement", "Admin");
    }
}
