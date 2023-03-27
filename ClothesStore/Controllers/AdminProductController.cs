using BusinessObject.DTO;
using ClosedXML.Excel;
using ClothesStoreAPI.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class AdminProductController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnv;
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string DefaultProductApiUrl = "";
        private string DefaultCategoryApiUrl = "";

        public AdminProductController(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            this.hostingEnv = env;
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultProductApiUrl = "https://localhost:7059/api/Products";
            DefaultCategoryApiUrl = "https://localhost:7059/api/Categories";
        }

        public async Task<IActionResult> Index(int? PageNum, string? searchString, decimal? startPrice, decimal? endPrice)
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

            //Get Products
            HttpResponseMessage productsResponse = await client.GetAsync(DefaultProductApiUrl + "/FilterProduct?text=" + searchString + "&startPrice=" + startPrice + "&endPrice=" + endPrice + "&isAdmin=true");
            string strProducts = await productsResponse.Content.ReadAsStringAsync();

          

            List<ProductDTO>? listProducts = JsonConvert.DeserializeObject<List<ProductDTO>>(strProducts);
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
            List<CategoryDTO> listCategories = await GetCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(listCategories, "CategoryId", "CategoryDetails");

            return View();
        }

        //POST: product/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductCreateUpdateDTO productDTO, IFormFile? imgFile)
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
            if (imgFile != null)
            {
                string fileName = imgFile.FileName;
                string fileDir = "images";
                string filePath = Path.Combine(hostingEnv.WebRootPath, fileDir + "\\" + fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                        imgFile.CopyTo(fs);
                }
                productDTO.Picture = "images/" + imgFile.FileName;
            } else
            {
                productDTO.Picture = "images/default.png";
            }

            var stringContent = new StringContent(JsonConvert.SerializeObject(productDTO), Encoding.UTF8, "application/json");
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
            HttpResponseMessage productResponse = await client.GetAsync(DefaultProductApiUrl + "/" + id);
            string strProduct = await productResponse.Content.ReadAsStringAsync();

           

            ProductCreateUpdateDTO? productDTO = JsonConvert.DeserializeObject<ProductCreateUpdateDTO>(strProduct);
            List<CategoryDTO> listCategories = await GetCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(listCategories, "CategoryId", "CategoryDetails");

            return View(productDTO);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductCreateUpdateDTO productDTO, IFormFile? imgFile)
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
            if (imgFile == null) productDTO.Picture = "" + productDTO.Picture;
            else productDTO.Picture = "images/" + imgFile.FileName;
            var stringContent = new StringContent(JsonConvert.SerializeObject(productDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(DefaultProductApiUrl, stringContent);

            List<CategoryDTO> listCategories = await GetCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(listCategories, "CategoryId", "CategoryDetails");

            if (response.IsSuccessStatusCode)
            {
                if (imgFile != null)
                {
                    string fileName = imgFile.FileName;
                    string fileDir = "images";
                    string filePath = Path.Combine(hostingEnv.WebRootPath, fileDir + "\\" + fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        imgFile.CopyTo(fs);
                    }

                }
                ViewData["Message"] = "Edit successfully!";
                return View(productDTO);
            }

            ViewData["Message"] = "Edit fail, try again!";
            return View(productDTO);
        }

        public async Task<IActionResult> Delete(int? id)
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
            await client.DeleteAsync(DefaultProductApiUrl + "/" + id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> exportExcel(int? PageNum, string? searchString, decimal? startPrice, decimal? endPrice)
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
            HttpResponseMessage productListResponse = await client.GetAsync(DefaultProductApiUrl + "/exportExcel?searchString=" + searchString + "&endPrice=" + startPrice + "&endPrice=" + endPrice);
            string strListProduct = await productListResponse.Content.ReadAsStringAsync();

            List<ProductDTO>? listProducts = JsonConvert.DeserializeObject<List<ProductDTO>>(strListProduct);
            using (var workbook = new XLWorkbook())
            {
                ExcelConfiguration.exportProduct(listProducts, workbook);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "products.xlsx");
                }
            }

            return RedirectToAction("Index", "AdminProduct", new { @PageNum = PageNum, @searchString = searchString, @startPrice = startPrice, @endPrice = endPrice });
        }

        public async Task<IActionResult> importExcel(IFormFile? file)
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
            if (file == null) return RedirectToAction(nameof(Index));
            var bytes = new byte[file.OpenReadStream().Length + 1];
            file.OpenReadStream().Read(bytes, 0, bytes.Length);
            var multiPartFromDataContent = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(bytes);
            multiPartFromDataContent.Add(fileContent, "file", file.FileName);
            var Response = await client.PostAsync(DefaultProductApiUrl + "/importExcel", multiPartFromDataContent);
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<CategoryDTO>> GetCategoriesAsync()
        {
            //Get Categories
            HttpResponseMessage categoriesResponse = await client.GetAsync(DefaultCategoryApiUrl);
            string strCategories = await categoriesResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CategoryDTO>>(strCategories);
        }
    }
}
