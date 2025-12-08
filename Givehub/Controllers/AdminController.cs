using Microsoft.AspNetCore.Mvc;

namespace Givehub.Controllers;

public class AdminController : Controller
{

    public IActionResult AdminHomePage()
    {
        return View();
    }

     public IActionResult DonationHistory()
      {
          // Even if you have NO DATA, return an empty list
          var donations = new List<Donation>();

          return View(donations);
      }
    

}
