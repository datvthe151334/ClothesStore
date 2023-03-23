using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    
    public class AdminProductController : Controller
    {

        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string DefaultProductApiUrl = "";
        private string DefaultCategoryApiUrl = "";

        public AdminProductController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultProductApiUrl = "https://localhost:7059/api/Products";
            DefaultCategoryApiUrl = "https://localhost:7059/api/Categories";
        }

        public async Task<IActionResult> Index(int? PageNum, string? searchString, decimal? startPrice, decimal? endPrice)
        {
            if (PageNum <= 0 || PageNum is null) PageNum = 1;
            int PageSize = Convert.ToInt32(configuration.GetValue<string>("AppSettings:PageSize"));

            //Get Products
            HttpResponseMessage productsResponse = await client.GetAsync(DefaultProductApiUrl + "/FilterProduct?text=" + searchString + "&startPrice=" + startPrice + "&endPrice=" + endPrice);
            string strProducts = await productsResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<ProductDTO>? listProducts = JsonSerializer.Deserialize<List<ProductDTO>>(strProducts, options);
            int TotalProduct = listProducts.Count;

            //Lay thong tin cho Pager
            int TotalPage = TotalProduct / PageSize;
            if (TotalProduct % PageSize != 0) TotalPage++;
            ViewData["TotalPage"] = TotalPage;
            ViewData["PageNum"] = PageNum;
            ViewData["Total"] = listProducts.Count;
            ViewData["StartIndex"] = (PageNum - 1) * PageSize + 1;

            listProducts = listProducts.Skip((int)(((PageNum - 1) * PageSize + 1) - 1)).Take(PageSize).ToList();

            ViewData["TotalOnPage"] = listProducts.Count;
            ViewBag.listProducts = listProducts;

            ViewData["StartPrice"] = startPrice;
            ViewData["EndPrice"] = endPrice;
            ViewData["SearchString"] = searchString;


            return View(listProducts);
        }

        // GET
        public async Task<ActionResult> Create()
        {
            List<CategoryDTO> listCategories = await GetCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(listCategories, "CategoryId", "CategoryDetails");

            return View();
        }

        //POST: product/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductCreateUpdateDTO productDTO)
        {
            var stringContent = new StringContent(JsonSerializer.Serialize<ProductCreateUpdateDTO>(productDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(DefaultProductApiUrl, stringContent);

            List<CategoryDTO> listCategories = await GetCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(listCategories, "CategoryId", "CategoryDetails");

            if (response.IsSuccessStatusCode)
            {
                ViewData["Message"] = "Create successfully!";
                return View(productDTO);
            }

            ViewData["Message"] = "Create fail, try again!";
            return View(productDTO);
        }

        //GET
        public async Task<ActionResult> Edit(int id)
        {
            HttpResponseMessage productResponse = await client.GetAsync(DefaultProductApiUrl + "/" + id);
            string strProduct = await productResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ProductCreateUpdateDTO? productDTO = JsonSerializer.Deserialize<ProductCreateUpdateDTO>(strProduct, options);

            List<CategoryDTO> listCategories = await GetCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(listCategories, "CategoryId", "CategoryDetails");

            return View(productDTO);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductCreateUpdateDTO productDTO)
        {
            var stringContent = new StringContent(JsonSerializer.Serialize<ProductCreateUpdateDTO>(productDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(DefaultProductApiUrl, stringContent);

            List<CategoryDTO> listCategories = await GetCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(listCategories, "CategoryId", "CategoryDetails");

            if (response.IsSuccessStatusCode)
            {
                ViewData["Message"] = "Edit successfully!";
                return View(productDTO);
            }

            ViewData["Message"] = "Edit fail, try again!";
            return View(productDTO);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            await client.DeleteAsync(DefaultProductApiUrl + "/" + id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<CategoryDTO>> GetCategoriesAsync()
        {
            //Get Categories
            HttpResponseMessage categoriesResponse = await client.GetAsync(DefaultCategoryApiUrl);
            string strCategories = await categoriesResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<CategoryDTO>>(strCategories, options);
        }
    }
}
