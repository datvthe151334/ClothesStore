﻿using BusinessObject.DTO;
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

        public async Task<IActionResult> Index(int? PageNum)
        {
            if (PageNum <= 0 || PageNum is null) PageNum = 1;
            int PageSize = Convert.ToInt32(configuration.GetValue<string>("AppSettings:PageSize"));

            //Get Orders
            HttpResponseMessage ordersResponse = await client.GetAsync(DefaultOrderApiUrl);
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


            return View(listOrders);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            await client.DeleteAsync(DefaultOrderApiUrl + "/" + id);

            return RedirectToAction(nameof(Index));
        }
    }
}