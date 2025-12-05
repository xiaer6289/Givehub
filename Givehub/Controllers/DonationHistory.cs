using Microsoft.AspNetCore.Mvc;

namespace Givehub.Controllers
{
    public class DonationHistoryController : Controller
    {
        public IActionResult DonationHistory()
        {
            // Even if you have NO DATA, return an empty list
            var donations = new List<Donation>();

            return View(donations);
        }
    }

}
