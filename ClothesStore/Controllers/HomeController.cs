using BusinessObject.DTO;
using ClothesStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultProductApiUrl = "";
        private string DefaultCategoryApiUrl = "";
        private string DefaultCustomerApiUrl = "";

        public HomeController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultProductApiUrl = "https://localhost:7059/api/Products";
            DefaultCategoryApiUrl = "https://localhost:7059/api/Categories";
            DefaultCustomerApiUrl = "https://localhost:7059/api/Customers";
        }

        public async Task<IActionResult> Index([FromQuery] int? CategoryId)
        {

            //Get Products
            if (CategoryId == null) CategoryId = 1;
            DefaultProductApiUrl = DefaultProductApiUrl + "/filter/" + CategoryId;
            HttpResponseMessage productsResponse = await client.GetAsync(DefaultProductApiUrl);
            string strProducts = await productsResponse.Content.ReadAsStringAsync();

            //Get Categories
            HttpResponseMessage categoriesResponse = await client.GetAsync(DefaultCategoryApiUrl);
            string strCategories = await categoriesResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            //Get Customers
            HttpResponseMessage customersResponse = await client.GetAsync(DefaultCustomerApiUrl);
            string strCustomers = await customersResponse.Content.ReadAsStringAsync();

            List<ProductDTO>? listProducts = JsonSerializer.Deserialize<List<ProductDTO>>(strProducts, options);
            List<CategoryDTO>? listCategories = JsonSerializer.Deserialize<List<CategoryDTO>>(strCategories, options);
            List<CustomerDTO>? listCustomers = JsonSerializer.Deserialize<List<CustomerDTO>>(strCustomers, options);

            ViewBag.listCategories = listCategories;

            ViewData["CurCatId"] = CategoryId;
            ViewData["TotalCustomer"] = listCustomers.Count;

            return View(listProducts);
        }
    }
}