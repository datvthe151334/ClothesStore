using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class AdminCustomerController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string DefaultCustomerApiUrl = "";

        public AdminCustomerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultCustomerApiUrl = "https://localhost:7059/api/Customers";
        }

        public async Task<IActionResult> Index(int? PageNum, string? searchString)
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
            HttpResponseMessage customersResponse = await client.GetAsync(DefaultCustomerApiUrl + "?searchString=" + searchString);
            string strCustomers = await customersResponse.Content.ReadAsStringAsync();


            List<CustomerDTO>? listCustomers = JsonConvert.DeserializeObject<List<CustomerDTO>>(strCustomers);
            int Total = listCustomers.Count;

            //Lay thong tin cho Pager
            int TotalPage = Total / PageSize;
            if (Total % PageSize != 0) TotalPage++;
            ViewData["TotalPage"] = TotalPage;
            ViewData["PageNum"] = PageNum;
            ViewData["Total"] = listCustomers.Count;
            ViewData["StartIndex"] = (PageNum - 1) * PageSize + 1;

            listCustomers = listCustomers.Skip((int)(((PageNum - 1) * PageSize + 1) - 1)).Take(PageSize).ToList();

            ViewData["TotalOnPage"] = listCustomers.Count;
            ViewBag.listCustomers = listCustomers;

            ViewData["SearchString"] = searchString;

            return View(listCustomers);
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
            return View();
        }

        //POST: customer/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerDTO customerDTO)
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
            var stringContent = new StringContent(JsonConvert.SerializeObject(customerDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(DefaultCustomerApiUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                ViewData["Message"] = "Create successfully!";
                return View(customerDTO);
            }

            ViewData["Message"] = "Create fail, try again!";
            return View(customerDTO);
        }

        //GET
        public async Task<ActionResult> Edit(int id)
        {
            HttpResponseMessage customerResponse = await client.GetAsync(DefaultCustomerApiUrl + "/" + id);
            string strCustomer = await customerResponse.Content.ReadAsStringAsync();

           

            CustomerDTO? customerDTO = JsonConvert.DeserializeObject<CustomerDTO>(strCustomer);

            return View(customerDTO);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CustomerDTO customerDTO)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(customerDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(DefaultCustomerApiUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                ViewData["Message"] = "Edit successfully!";
                return View(customerDTO);
            }

            ViewData["Message"] = "Edit fail, try again!";
            return View(customerDTO);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            await client.DeleteAsync(DefaultCustomerApiUrl + "/" + id);

            return RedirectToAction(nameof(Index));
        }
    }
}
