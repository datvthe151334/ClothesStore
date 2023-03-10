using Microsoft.AspNetCore.Mvc;

namespace ClothesStore.Controllers
{
    
    public class AdminProductController : Controller
    {
      
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
