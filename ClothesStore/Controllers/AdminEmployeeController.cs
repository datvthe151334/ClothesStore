using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
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
            if (PageNum <= 0 || PageNum is null) PageNum = 1;
            int PageSize = Convert.ToInt32(configuration.GetValue<string>("AppSettings:PageSize"));

            //Get Categories
            HttpResponseMessage employeesResponse = await client.GetAsync(DefaultEmployeeApiUrl + "?searchString=" + searchString);
            string strEmployees = await employeesResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<EmployeeDTO>? listEmployees = JsonSerializer.Deserialize<List<EmployeeDTO>>(strEmployees, options);
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
