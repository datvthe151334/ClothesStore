using Microsoft.AspNetCore.Mvc;

namespace ClothesStore.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
