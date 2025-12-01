using Microsoft.AspNetCore.Mvc;

namespace Givehub.Controllers;

public class DonorController : Controller
{
    public IActionResult MoneyReport()
    {
        return View();
    }

    public IActionResult ItemReport()
    {
        return View();
    }
}
