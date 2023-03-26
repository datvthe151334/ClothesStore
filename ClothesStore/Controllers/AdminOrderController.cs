using BusinessObject.DTO;
using ClosedXML.Excel;
using ClothesStoreAPI.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class AdminOrderController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string DefaultOrderApiUrl = "";

        public AdminOrderController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultOrderApiUrl = "https://localhost:7059/api/Orders";
        }

        public async Task<IActionResult> Index(int? PageNum, DateTime? startDate, DateTime? endDate)
        {
            if (PageNum <= 0 || PageNum is null) PageNum = 1;
            int PageSize = Convert.ToInt32(configuration.GetValue<string>("AppSettings:PageSize"));

            //Get Orders
            HttpResponseMessage ordersResponse = await client.GetAsync(DefaultOrderApiUrl + "?startDate=" + startDate + "&endDate=" + endDate);
            string strOrders = await ordersResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<OrderDTO>? listOrders = JsonSerializer.Deserialize<List<OrderDTO>>(strOrders, options);
            int Total = listOrders.Count;

            //Lay thong tin cho Pager
            int TotalPage = Total / PageSize;
            if (Total % PageSize != 0) TotalPage++;

            ViewData["TotalPage"] = TotalPage;
            ViewData["PageNum"] = PageNum;
            ViewData["Total"] = listOrders.Count;
            ViewData["StartIndex"] = (PageNum - 1) * PageSize + 1;

            listOrders = listOrders.Skip((int)(((PageNum - 1) * PageSize + 1) - 1)).Take(PageSize).ToList();

            ViewData["TotalOnPage"] = listOrders.Count;
            ViewBag.listOrders = listOrders;

            ViewData["StartDate"] = String.Format("{0:yyyy-MM-dd}", startDate);
            ViewData["EndDate"] = String.Format("{0:yyyy-MM-dd}", endDate);

            return View(listOrders);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            await client.DeleteAsync(DefaultOrderApiUrl + "/" + id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> exportExcel(int? PageNum, DateTime? startDate, DateTime? endDate)
        {
            HttpResponseMessage listOrdersResponse = await client.GetAsync(DefaultOrderApiUrl + "/exportExcel?startDate=" + startDate + "&endDate=" + endDate);
            string strListOrders = await listOrdersResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<OrderDTO>? listOrders = JsonSerializer.Deserialize<List<OrderDTO>>(strListOrders, options);
            using (var workbook = new XLWorkbook())
            {
                ExcelConfiguration.exportOrder(listOrders, workbook);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "orders.xlsx");
                }
            }
            return RedirectToAction("Index", "AdminOrder" , new { @PageNum = PageNum, @startDate = startDate, @endDate = endDate });
        }
    }
}
