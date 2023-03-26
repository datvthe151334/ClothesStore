using BusinessObject.DTO;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultProductApiUrl = "";
        private string DefaultCategoryApiUrl = "";

        public ProductController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultProductApiUrl = "https://localhost:7059/api/Products";
            DefaultCategoryApiUrl = "https://localhost:7059/api/Categories";
        }

        public async Task<IActionResult> Detail(int id)
        {
            
            //Get Products
            HttpResponseMessage productsResponse = await client.GetAsync(DefaultProductApiUrl + "/" + id);
            string strProduct = await productsResponse.Content.ReadAsStringAsync();

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
            

            ProductDTO? product = JsonConvert.DeserializeObject<ProductDTO>(strProduct);
            List<string>? listCategoryGeneral = JsonConvert.DeserializeObject<List<string>>(strCategoryGeneral);
            List<CategoryDTO>? listCategories = JsonConvert.DeserializeObject<List<CategoryDTO>>(strCategories);

            //Get related product
            HttpResponseMessage relatedProductsResponse = await client.GetAsync(DefaultProductApiUrl + "/filterByCatId/" + product.CategoryId);
            string strRelatedProducts = await relatedProductsResponse.Content.ReadAsStringAsync();

            List<ProductDTO>? listRelatedProducts = JsonConvert.DeserializeObject<List<ProductDTO>>(strRelatedProducts);

            ViewBag.listRelatedProducts = listRelatedProducts.OrderBy(x => Guid.NewGuid()).Where(x => x.ProductId != product.ProductId).Take(4).ToList();
            ViewBag.listCategories = listCategories;
            ViewBag.listCategoryGeneral = listCategoryGeneral;
            return View(product);
        }

        public async Task<IActionResult> Index(int? categoryId, string? text, string? sortType)
        {
            
            //Get Products By Name t nghị sau ni chỗ search/filterbyid chỉ cần dùng cái api ni là đc : DefaultProductApiUrl + "/FilterProduct?categoryId=" + categoryId + "&text="+ text phbair ko
            HttpResponseMessage productsResponse = await client.GetAsync(DefaultProductApiUrl + "/FilterProduct?categoryId=" + categoryId + "&text="+ text + "&sortType=" + sortType);
            string strProduct = await productsResponse.Content.ReadAsStringAsync();

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
            List<ProductDTO>? product = JsonConvert.DeserializeObject<List<ProductDTO>>(strProduct);
            List<string>? listCategoryGeneral = JsonConvert.DeserializeObject<List<string>>(strCategoryGeneral);
            List<CategoryDTO>? listCategories = JsonConvert.DeserializeObject<List<CategoryDTO>>(strCategories);
            
            ViewBag.listCategories = listCategories;
            ViewBag.listCategoryGeneral = listCategoryGeneral;
            ViewData["text"] = text;
            ViewData["categoryId"] = categoryId;
            ViewData["sortType"] = sortType;

            ViewBag.SortType = sortType;
            return View(product);
        }
    }
}
