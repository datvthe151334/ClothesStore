using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class AdminEmployeeController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string DefaultEmployeeApiUrl = "";

        public AdminEmployeeController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultEmployeeApiUrl = "https://localhost:7059/api/Employees";
        }

        public async Task<IActionResult> Index(int? PageNum, string? searchString)
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
            if (PageNum <= 0 || PageNum is null) PageNum = 1;
            int PageSize = Convert.ToInt32(configuration.GetValue<string>("AppSettings:PageSize"));

            //Get Categories
            HttpResponseMessage employeesResponse = await client.GetAsync(DefaultEmployeeApiUrl + "?searchString=" + searchString);
            string strEmployees = await employeesResponse.Content.ReadAsStringAsync();

            

            List<EmployeeDTO>? listEmployees = JsonConvert.DeserializeObject<List<EmployeeDTO>>(strEmployees);
            int Total = listEmployees.Count;

            //Lay thong tin cho Pager
            int TotalPage = Total / PageSize;
            if (Total % PageSize != 0) TotalPage++;
            ViewData["TotalPage"] = TotalPage;
            ViewData["PageNum"] = PageNum;
            ViewData["Total"] = listEmployees.Count;
            ViewData["StartIndex"] = (PageNum - 1) * PageSize + 1;

            listEmployees = listEmployees.Skip((int)(((PageNum - 1) * PageSize + 1) - 1)).Take(PageSize).ToList();

            ViewData["TotalOnPage"] = listEmployees.Count;
            ViewBag.listEmployees = listEmployees;

            ViewData["SearchString"] = searchString;

            return View(listEmployees);
        }
    }
}
