using Givehub.Models;
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

    public async Task<IActionResult> ItemManagement(string? search, int page = 1)
    {
        int pageSize = 5;

        var donations = await _db.Donations
            .Include(d => d.Donors)
            .Include(d => d.Donees)
            .Where(d => d.Status == "Pending")
            .ToListAsync();

        // Apply search if needed
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            donations = donations.Where(d =>
                d.Id.ToString().Contains(search) ||
                (d.Donors != null && d.Donors.Name.ToLower().Contains(search)) ||
                (d.Donees != null && d.Donees.Name.ToLower().Contains(search)) ||
                (d.Items != null && d.Items.Keys.Any(i => i.ToLower().Contains(search))) ||
                d.Status.ToLower().Contains(search)
            ).ToList();
        }

        ViewBag.Search = search;

        int totalRecords = donations.Count; // important: use filtered count
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.CurrentPage = page;

        var pagedDonations = donations
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new ItemManagementVM
            {
                Id = d.Id,
                DonorName = d.Donors?.Name ?? "-",
                DoneeName = d.Donees?.Name ?? "-",
                Date = d.Date,
                Status = d.Status,
                Items = d.Items?.Select(i => new ItemDetails
                {
                    ItemName = i.Key,
                    Quantity = i.Value
                }).ToList() ?? new List<ItemDetails>()
            }).ToList();

        return View(pagedDonations);
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

    public IActionResult ManageDonor()
    {
        return View();
    }

    public IActionResult ViewDonor()
    {
        var donors = _db.Donors
                        .Include(d => d.Donations) 
                        .ToList();

        return View(donors);
    }



    public IActionResult DonationHistory(int donorId, string search)
    {
        var donor = _db.Donors.FirstOrDefault(d => d.Id == donorId);
        if (donor == null) return NotFound();

        
        var donations = _db.Donations
            .Include(d => d.Donees)
            .Where(d => d.DonorId == donorId && d.Status == "Completed")
            .AsEnumerable(); 

        
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            donations = donations.Where(d =>
                (d.Donees != null && d.Donees.Name.ToLower().Contains(search)) ||
                (d.Method == "Item"
                    && d.Items != null
                    && d.Items.Any(i => i.Key.Contains(search)))
            );
        }

        var vm = new ReportVM
        {
            DonorName = donor.Name,
            DonorEmail = donor.Email,
            Donations = donations
                .OrderByDescending(d => d.Date)
                .ToList()
        };

        ViewBag.SearchQuery = search;
        ViewBag.DonorId = donorId;

        return View(vm);
    }


}
