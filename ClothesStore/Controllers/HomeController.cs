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

        public async Task<IActionResult> Index([FromQuery] string? CategoryGeneral)
        {
            if (CategoryGeneral == null) CategoryGeneral = "men";

            //Get Products
            HttpResponseMessage productsResponse = await client.GetAsync(DefaultProductApiUrl);
            string strProducts = await productsResponse.Content.ReadAsStringAsync();

            HttpResponseMessage menProductsResponse = await client.GetAsync(DefaultProductApiUrl + "/filterByCatGeneral/men");
            string strMenProducts = await menProductsResponse.Content.ReadAsStringAsync();

            HttpResponseMessage womenProductsResponse = await client.GetAsync(DefaultProductApiUrl + "/filterByCatGeneral/woman");
            string strWomenProducts = await womenProductsResponse.Content.ReadAsStringAsync();

            HttpResponseMessage babyProductsResponse = await client.GetAsync(DefaultProductApiUrl + "/filterByCatGeneral/baby");
            string strBabyProducts = await babyProductsResponse.Content.ReadAsStringAsync();

            //Get CategoryGeneral
            HttpResponseMessage categoryGeneralResponse = await client.GetAsync(DefaultCategoryApiUrl + "/getCategoryGeneral");
            string strCategoryGeneral = await categoryGeneralResponse.Content.ReadAsStringAsync();

            //Get Categories
            HttpResponseMessage categoriesResponse = await client.GetAsync(DefaultCategoryApiUrl);
            string strCategories = await categoriesResponse.Content.ReadAsStringAsync();

            //Get Customers
            HttpResponseMessage customersResponse = await client.GetAsync(DefaultCustomerApiUrl);
            string strCustomers = await customersResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<ProductDTO>? listProducts = JsonSerializer.Deserialize<List<ProductDTO>>(strProducts, options);
            List<ProductDTO>? listMenProducts = JsonSerializer.Deserialize<List<ProductDTO>>(strMenProducts, options);
            List<ProductDTO>? listWomenProducts = JsonSerializer.Deserialize<List<ProductDTO>>(strWomenProducts, options);
            List<ProductDTO>? listBabyProducts = JsonSerializer.Deserialize<List<ProductDTO>>(strBabyProducts, options);
            
            List<string>? listCategoryGeneral = JsonSerializer.Deserialize<List<string>>(strCategoryGeneral, options);
            List<CategoryDTO>? listCategories = JsonSerializer.Deserialize<List<CategoryDTO>>(strCategories, options);
            List<CustomerDTO>? listCustomers = JsonSerializer.Deserialize<List<CustomerDTO>>(strCustomers, options);

            ViewBag.listMenProducts = listMenProducts.OrderByDescending(x => x.ProductId).Take(12).ToList();
            ViewBag.listWomenProducts = listWomenProducts.OrderByDescending(x => x.ProductId).Take(12).ToList();
            ViewBag.listBabyProducts = listBabyProducts.OrderByDescending(x => x.ProductId).Take(12).ToList();

            ViewBag.listCategories = listCategories;
            ViewBag.listCategoryGeneral = listCategoryGeneral;

            ViewData["CurCatGeneral"] = CategoryGeneral;
            ViewData["TotalCustomer"] = listCustomers.Count;

            return View(listProducts.OrderByDescending(x => x.ProductId).Take(12).ToList());
        }
    }
}