using Microsoft.AspNetCore.Mvc;

namespace ClothesStore.Controllers
{
    public class MenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
