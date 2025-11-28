using Microsoft.AspNetCore.Mvc;

namespace Givehub.Controllers;

public class DonorController : Controller
{
    public IActionResult MoneyReport()
    {
        return View();
    }
}
