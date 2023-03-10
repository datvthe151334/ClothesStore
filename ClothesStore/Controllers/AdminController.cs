using Microsoft.AspNetCore.Mvc;

namespace ClothesStore.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult DashBoard()
        {
            return View();
        }
    }
}
