using Aspose.Pdf;
using BusinessObject.DTO;
using ClothesStoreAPI.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string BaseUrl = "";
        private string DefaultCategoryApiUrl = "";
        private string DefaultOrderApiUrl = "";

        public OrderController(IConfiguration configuration)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultCategoryApiUrl = "https://localhost:7059/api/Categories";
            DefaultOrderApiUrl = "https://localhost:7059/api/Orders";
            BaseUrl = "https://localhost:7059";
            this.configuration = configuration;
        }
        public async Task<IActionResult> Index(string? alertMessage)
        {
            var mySessionValue = HttpContext.Session.GetString("user");
            var userObject = JsonConvert.DeserializeObject<dynamic>(mySessionValue);
            var customerId = userObject.account.customerId;

            //Get list order
            HttpResponseMessage ordersResponse = await client.GetAsync(DefaultOrderApiUrl + "/getOrdersByCustomer?customerId=" + customerId);
            string strOrders = await ordersResponse.Content.ReadAsStringAsync();

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
            List<OrderDictionaryDTO>? listOrders = JsonConvert.DeserializeObject<List<OrderDictionaryDTO>>(strOrders);

            ViewData["AlertMessage"] = alertMessage;
            ViewBag.listCategories = listCategories;
            ViewBag.listCategoryGeneral = listCategoryGeneral;
            return View(listOrders);
        }

        public async Task<IActionResult> GetOrderReport(int id)
        {
            var mySessionValue = HttpContext.Session.GetString("user");
            var userObject = JsonConvert.DeserializeObject<dynamic>(mySessionValue);
            var email = userObject.account.Email;

            //Get list order
            HttpResponseMessage ordersResponse = await client.GetAsync(DefaultOrderApiUrl + "/" + id);
            string strOrders = await ordersResponse.Content.ReadAsStringAsync();


            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            OrderDictionaryDTO? Order = JsonConvert.DeserializeObject<OrderDictionaryDTO>(strOrders);

            string body = PdfConfiguration.GetBody(Order, email);
            {
                HtmlLoadOptions objLoadOptions = new HtmlLoadOptions();
                objLoadOptions.PageInfo.Margin.Bottom = 10;
                objLoadOptions.PageInfo.Margin.Top = 20;

                Document document = new Document(new MemoryStream(Encoding.UTF8.GetBytes(body)), objLoadOptions);
                FileContentResult pdf;

                using (var stream = new MemoryStream())
                {
                    document.Save(stream);
                    pdf = new FileContentResult(stream.ToArray(), "application/pdf")
                    {
                        FileDownloadName = "Order.pdf"
                    };

                }
                return pdf;
            }


            return RedirectToAction("Index", "Order", new { @alertMessage = "Get report successfully!" });
        }
    }
}
