using Microsoft.AspNetCore.Mvc;

namespace Givehub.Controllers;

public class DoneeController : Controller
{

    public IActionResult DoneeHomePage()
    {
        return View();
    }

    public IActionResult CreateDonee()
    {
        return View();
    }

}