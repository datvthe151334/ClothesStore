using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClothesStore.Controllers
{

    public class AccountsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string DefaultCategoryApiUrl = "";
        private string DefaultAccountsApiUrl = "";
        private string BaseUrl = "";
        public AccountsController(IConfiguration configuration)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultCategoryApiUrl = "https://localhost:7059/api/Categories";
            DefaultAccountsApiUrl = "https://localhost:7059/api/Accounts";
            BaseUrl = "https://localhost:7059";
            this.configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> Profile(string? alertMessage)       
        {
            
            var mySessionValue = HttpContext.Session.GetString("user");
            var userObject = JsonConvert.DeserializeObject<dynamic>(mySessionValue);

            // Truy cập customerId của đối tượng JSON
            var customerId = userObject.account.customerId;
            //Get InfomationAccountGeneral
            HttpResponseMessage infoAccountsResponse = await client.GetAsync(DefaultAccountsApiUrl + "/GetInfoCustomerById/" + customerId);
            string strinfoAccountsGeneral = await infoAccountsResponse.Content.ReadAsStringAsync();

            //Get CategoryGeneral
            HttpResponseMessage categoryGeneralResponse = await client.GetAsync(DefaultCategoryApiUrl + "/getCategoryGeneral");
            string strCategoryGeneral = await categoryGeneralResponse.Content.ReadAsStringAsync();

            //Get Categories
            HttpResponseMessage categoriesResponse = await client.GetAsync(DefaultCategoryApiUrl);
            string strCategories = await categoriesResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            List<string>? listCategoryGeneral = JsonConvert.DeserializeObject<List<string>>(strCategoryGeneral);
            List<CategoryDTO>? listCategories = JsonConvert.DeserializeObject<List<CategoryDTO>>(strCategories);
            AccountUpdateDTO? accInfo = JsonConvert.DeserializeObject<AccountUpdateDTO>(strinfoAccountsGeneral);
            @ViewData["AlertMessage"] = alertMessage;
            ViewBag.listCategories = listCategories;
            ViewBag.listCategoryGeneral = listCategoryGeneral;
            return View(accInfo);
        }
        [HttpPost]
        public IActionResult UpdateCustomer(AccountUpdateDTO req)
        {

            var conn = "api/Accounts/updateProfile";
            var Res = PostData(conn, JsonConvert.SerializeObject(req));
            if (!Res.Result.IsSuccessStatusCode) return RedirectToAction("Profile", "Accounts", new { @alertMessage = "Update failed!" });
            return RedirectToAction("Profile", "Accounts", new { @signUpMessage = "Update successfully" });
        }

        public async Task<HttpResponseMessage> PostData(string targerAddress, string content)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["accessToken"]))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies["accessToken"]);
            }
            else
            {
                client.DefaultRequestHeaders.Add("refreshToken", HttpContext.Request.Cookies["refreshToken"]);
            }
            var Response = await client.PostAsync(targerAddress, new StringContent(content, Encoding.UTF8, "application/json"));
            return Response;
        }

        public async Task<IActionResult> NotFound()
        {
           

            //Get CategoryGeneral
            HttpResponseMessage categoryGeneralResponse = await client.GetAsync(DefaultCategoryApiUrl + "/getCategoryGeneral");
            string strCategoryGeneral = await categoryGeneralResponse.Content.ReadAsStringAsync();

            //Get Categories
            HttpResponseMessage categoriesResponse = await client.GetAsync(DefaultCategoryApiUrl);
            string strCategories = await categoriesResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<string>? listCategoryGeneral = JsonConvert.DeserializeObject<List<string>>(strCategoryGeneral);
            List<CategoryDTO>? listCategories = JsonConvert.DeserializeObject<List<CategoryDTO>>(strCategories);
            
            
            ViewBag.listCategories = listCategories;
            ViewBag.listCategoryGeneral = listCategoryGeneral;
            return View();
        }

       
    }
}
