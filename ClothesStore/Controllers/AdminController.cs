using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClothesStore.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult DashBoard()
        {
            var mySessionValue = HttpContext.Session.GetString("user");
            if (mySessionValue == null)
            {
                return RedirectToAction("NotFound", "Accounts");
            }
            else
            {
                var userObject = JsonConvert.DeserializeObject<dynamic>(mySessionValue);
                var customerId = userObject.account.customerId;
                if (customerId != null || mySessionValue == null)
                {
                    return RedirectToAction("NotFound", "Accounts");
                }
            }
            return View();
        }
    }
}
