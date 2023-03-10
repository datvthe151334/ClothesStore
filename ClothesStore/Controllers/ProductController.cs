using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
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

            ProductDTO? product = JsonSerializer.Deserialize<ProductDTO>(strProduct, options);
            List<string>? listCategoryGeneral = JsonSerializer.Deserialize<List<string>>(strCategoryGeneral, options);
            List<CategoryDTO>? listCategories = JsonSerializer.Deserialize<List<CategoryDTO>>(strCategories, options);

            //Get related product
            HttpResponseMessage relatedProductsResponse = await client.GetAsync(DefaultProductApiUrl + "/filterByCatId/" + product.CategoryId);
            string strRelatedProducts = await relatedProductsResponse.Content.ReadAsStringAsync();

            List<ProductDTO>? listRelatedProducts = JsonSerializer.Deserialize<List<ProductDTO>>(strRelatedProducts, options);

            ViewBag.listRelatedProducts = listRelatedProducts.OrderBy(x => Guid.NewGuid()).Where(x => x.ProductId != product.ProductId).Take(4).ToList();
            ViewBag.listCategories = listCategories;
            ViewBag.listCategoryGeneral = listCategoryGeneral;

            return View(product);
        }
    }
}
