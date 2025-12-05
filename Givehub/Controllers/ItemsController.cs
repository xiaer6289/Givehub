using Microsoft.AspNetCore.Mvc;

namespace Givehub.Controllers
{
    public class ItemsController : Controller
    {
        public IActionResult Items()
        {
            return View();
        }
    }
}
