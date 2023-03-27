using BusinessObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class AdminCategoryController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string DefaultCategoryApiUrl = "";

        public AdminCategoryController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultCategoryApiUrl = "https://localhost:7059/api/Categories";
        }

        public async Task<IActionResult> Index(int? PageNum)
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

            //Get Categories
            HttpResponseMessage categoriesResponse = await client.GetAsync(DefaultCategoryApiUrl);
            string strCategories = await categoriesResponse.Content.ReadAsStringAsync();

            

            List<CategoryDTO>? listCategories = JsonConvert.DeserializeObject<List<CategoryDTO>>(strCategories);
            int Total = listCategories.Count;

            //Lay thong tin cho Pager
            int TotalPage = Total / PageSize;
            if (Total % PageSize != 0) TotalPage++;
            ViewData["TotalPage"] = TotalPage;
            ViewData["PageNum"] = PageNum;
            ViewData["Total"] = listCategories.Count;
            ViewData["StartIndex"] = (PageNum - 1) * PageSize + 1;

            listCategories = listCategories.Skip((int)(((PageNum - 1) * PageSize + 1) - 1)).Take(PageSize).ToList();

            ViewData["TotalOnPage"] = listCategories.Count;
            ViewBag.listCategories = listCategories;


            return View(listCategories);
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
            List<string> listCategoryGeneral = await GetCategoryGeneralAsync();
            var sl = listCategoryGeneral.Select(s => new SelectListItem { Text = s, Value = s }).ToList();

            ViewBag.sl = sl;
            
            return View();
        }

		//POST: product/create
		[HttpPost]
		[ValidateAntiForgeryToken]
        
		public async Task<ActionResult> Create(CategoryDTO categoryDTO)
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
            var stringContent = new StringContent(JsonConvert.SerializeObject(categoryDTO), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync(DefaultCategoryApiUrl, stringContent);

            List<string> listCategoryGeneral = await GetCategoryGeneralAsync();
            var sl = listCategoryGeneral.Select(s => new SelectListItem { Text = s, Value = s }).ToList();
            ViewBag.sl = sl;

            if (response.IsSuccessStatusCode)
			{
                ViewData["Message"] = "Create successfully!";
				return View(categoryDTO);
			}

            ViewData["Message"] = "Create fail, try again!";
            return View(categoryDTO);
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
            HttpResponseMessage categoryResponse = await client.GetAsync(DefaultCategoryApiUrl + "/" + id);
            string strCategory = await categoryResponse.Content.ReadAsStringAsync();          
            CategoryDTO? categoryDTO = JsonConvert.DeserializeObject<CategoryDTO>(strCategory);
            List<string> listCategoryGeneral = await GetCategoryGeneralAsync();
            var sl = listCategoryGeneral.Select(s => new SelectListItem { Text = s, Value = s }).ToList();
            ViewBag.sl = sl;

            return View(categoryDTO);
        }

		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(CategoryDTO categoryDTO)
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
            var stringContent = new StringContent(JsonConvert.SerializeObject(categoryDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(DefaultCategoryApiUrl, stringContent);

            List<string> listCategoryGeneral = await GetCategoryGeneralAsync();
            var sl = listCategoryGeneral.Select(s => new SelectListItem { Text = s, Value = s }).ToList();
            ViewBag.sl = sl;

            if (response.IsSuccessStatusCode)
            {
                ViewData["Message"] = "Edit successfully!";
                return View(categoryDTO);
            }

            ViewData["Message"] = "Edit fail, try again!";
            return View(categoryDTO);
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
            await client.DeleteAsync(DefaultCategoryApiUrl + "/" + id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<string>> GetCategoryGeneralAsync()
        {
            //Get CategoryGeneral
            HttpResponseMessage categoryGeneralResponse = await client.GetAsync(DefaultCategoryApiUrl + "/getCategoryGeneral");
            string strCategoryGeneral = await categoryGeneralResponse.Content.ReadAsStringAsync();

           

            return JsonConvert.DeserializeObject<List<string>>(strCategoryGeneral);
        }
    }
}
