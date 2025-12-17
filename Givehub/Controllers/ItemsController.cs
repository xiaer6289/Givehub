using Microsoft.AspNetCore.Mvc;
using Givehub.Models;

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
        public async Task<IActionResult> Items(ItemDonation model)
        {
            var donation = new Donation
            {
                Method = "Item",
                Date = model.DeliveryDate,
                Amount = 0,
                DonorId = 4,
                DoneeId = 1,

                Items = model.Items
                .GroupBy(x =>x.ItemName) //group all the same item together
                .ToDictionary(
                    //put this as key
                    g => g.Key,
                    //put this as value
                    g => g.Sum(x => x.Quantity)
                )

            };

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Items()
        {
            return View();
        }

    }
}
