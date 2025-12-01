using Microsoft.AspNetCore.Mvc;

namespace Givehub.Controllers;

public class AdminController : Controller
{

    public IActionResult AdminHomePage()
    {
        return View();
    }

}
