using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text.Json;
using Givehub.Models;
using Givehub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Givehub.Controllers
{
    public class ItemsController : Controller
    {
        //readonly ensure it can only assigned once
        private readonly DB _db;

        public ItemsController(DB db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult Items(ItemDonationViewModel model)
        {
            var donorIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);//take the current userid

            if (donorIdClaim == null)
            {
                // Not logged in, redirect to login
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)  //verify is user had enter data before make donate 
            {
                var donee = _db.Donees.FirstOrDefault(d => d.Id == 1);
                model.AvailableItems = donee.Requirements.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(x => x.Trim())
                                                         .ToList();//seperate the item from the list by comma
                return View(model);
            }

            TempData["Donation"]= JsonSerializer.Serialize(model.Donation);  //this one will convert the C# object into json file
            TempData["DoneeId"] = model.DoneeId;

            return RedirectToAction("Summary");
        }
            
        public IActionResult Summary()
        {
            if (TempData["Donation"] == null  || TempData["DoneeId"] == null)
                return RedirectToAction("Items");
            
            var donation = JsonSerializer.
                Deserialize<ItemDonation>(TempData["Donation"].ToString());  //convert json to object back

            int doneeId = (int)TempData["DoneeId"];
            var donee = _db.Donees.FirstOrDefault(d => d.Id == doneeId);

            TempData.Keep("Donation"); //keep the data alive coz tempdata will delete automatically after it read once
            TempData.Keep("DoneeId");

            if (donee != null)
            {
                donation.DonateAddress = donee.Address;  // populate address
                donation.DoneeName = donee.Name;         // populate name if needed
                donation.DoneeId = donee.Id;
            }

           
            donation.Items = donation.Items
                .GroupBy(i => i.ItemName)
                .Select(g => new ItemEntry
                {
                    ItemName = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

           
            TempData.Keep("Donation"); //keep the data alive coz tempdata will delete automatically after it read once
            TempData.Keep("DoneeId");
            return View(donation);

        }


        [HttpPost]
        public async Task<IActionResult> Confirm()

        {
            if (TempData["Donation"] == null)
                return RedirectToAction("Items");

            var donationData = JsonSerializer.Deserialize<ItemDonation>(TempData["Donation"].ToString());


            var donorIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (donorIdClaim == null)
            {
                // Not logged in, redirect to login
                return RedirectToAction("Login", "Account");
            }

            int donorId = int.Parse(donorIdClaim);
            int doneeId = int.Parse(TempData["DoneeId"].ToString());


            var donation = new Donation
            {
                Method = "Item",
                Date = donationData.DeliveryDate,
                Amount = 0,
                Status = "Pending",
                DonorId = donorId,
                DoneeId = doneeId,

                Items = donationData.Items
                .GroupBy(x => x.ItemName) //group all the same item together
                .ToDictionary(
                    //put this as key
                    g => g.Key,
                    //put this as value
                    g => g.Sum(x => x.Quantity)
                )

            };

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync();

            TempData.Keep("DoneeId");
            TempData.Remove("Donation"); // clear temporary data
            return RedirectToAction("Success", "Items");
        }

        [HttpPost]
        public IActionResult Cancel()
        {
            
            int doneeId = int.Parse(TempData["DoneeId"].ToString());
            return RedirectToAction("Items", new { doneeId });
        }

        public IActionResult Items(int doneeId)
        {
            var donee = _db.Donees.FirstOrDefault(d => d.Id == doneeId);

            if (donee == null)
                return NotFound();

            var availableItems = donee.Requirements
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            var vm = new ItemDonationViewModel
            {

                DoneeId = doneeId,
                AvailableItems = availableItems
            };


            if (TempData["Donation"] != null)
            {
                vm.Donation = JsonSerializer.Deserialize<ItemDonation>(TempData["Donation"].ToString());
                TempData.Keep("Donation"); // keep TempData alive for next round
            }
            else
            {
                vm.Donation = new ItemDonation
                {
                    Items = new List<ItemEntry> { new ItemEntry() } // default
                };
            }

            return View(vm);
        }

        public IActionResult Success()
        {
            return View();
        }

    }
}
