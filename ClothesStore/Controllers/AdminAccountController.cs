using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class AdminAccountController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string DefaultAccountApiUrl = "";

        public AdminAccountController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultAccountApiUrl = "https://localhost:7059/api/Accounts";
        }

        public async Task<IActionResult> Index(int? PageNum)
        {
            if (PageNum <= 0 || PageNum is null) PageNum = 1;
            int PageSize = Convert.ToInt32(configuration.GetValue<string>("AppSettings:PageSize"));

            //Get Accounts
            HttpResponseMessage accountsResponse = await client.GetAsync(DefaultAccountApiUrl);
            string strAccounts = await accountsResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<AccountDTO>? listAccounts = JsonSerializer.Deserialize<List<AccountDTO>>(strAccounts, options);
            int Total = listAccounts.Count;

            //Lay thong tin cho Pager
            int TotalPage = Total / PageSize;
            if (Total % PageSize != 0) TotalPage++;
            ViewData["TotalPage"] = TotalPage;
            ViewData["PageNum"] = PageNum;
            ViewData["Total"] = listAccounts.Count;
            ViewData["StartIndex"] = (PageNum - 1) * PageSize + 1;

            listAccounts = listAccounts.Skip((int)(((PageNum - 1) * PageSize + 1) - 1)).Take(PageSize).ToList();

            ViewData["TotalOnPage"] = listAccounts.Count;
            ViewBag.listAccounts = listAccounts;


            return View(listAccounts);
        }
    }
}
