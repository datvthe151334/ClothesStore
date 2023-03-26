using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClothesStore.Controllers
{
    public class CartController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string BaseUrl = "";
        private string DefaultCategoryApiUrl = "";
        private string DefaultOrderApiUrl = "";
        private string DefaultProductApiUrl = "";
        public static decimal? total = 0;

        public CartController(IConfiguration configuration)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultCategoryApiUrl = "https://localhost:7059/api/Categories";
            DefaultOrderApiUrl = "https://localhost:7059/api/Orders";
            DefaultProductApiUrl = "https://localhost:7059/api/Products";
            BaseUrl = "https://localhost:7059";
            this.configuration = configuration;
        }
        public async Task<IActionResult> Index(string? alertMessage)
        {
            ViewData["alertMessage"] = alertMessage;

            List<CartDTO> CartItems = new List<CartDTO>();
            //Get CategoryGeneral
            HttpResponseMessage categoryGeneralResponse = await client.GetAsync(DefaultCategoryApiUrl + "/getCategoryGeneral");
            string strCategoryGeneral = await categoryGeneralResponse.Content.ReadAsStringAsync();

            //Get Categories
            HttpResponseMessage categoriesResponse = await client.GetAsync(DefaultCategoryApiUrl);
            string strCategories = await categoriesResponse.Content.ReadAsStringAsync();

            //Get cart from session
            string? cart = HttpContext.Session.GetString("CartSession");

            //Get Account/Customer form session
            var mySessionValue = HttpContext.Session.GetString("user");
            if (mySessionValue != null)
            {
                var userObject = JsonConvert.DeserializeObject<dynamic>(mySessionValue);
                ViewBag.customer = userObject.account.customer;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<string>? listCategoryGeneral = JsonConvert.DeserializeObject<List<string>>(strCategoryGeneral);
            List<CategoryDTO>? listCategories = JsonConvert.DeserializeObject<List<CategoryDTO>>(strCategories);

            if (cart == null)
            {
                ViewData["alertMessage"] = "Cart empty!";
            }
            else
            {
                CartItems = JsonConvert.DeserializeObject<List<CartDTO>>(cart);
                total = GetTotal(CartItems);
                ViewData["total"] = total;
            }


            ViewBag.listCategories = listCategories;
            ViewBag.listCategoryGeneral = listCategoryGeneral;

            return View(CartItems);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            string? cart = HttpContext.Session.GetString("CartSession");
            List<CartDTO> CartItems = new List<CartDTO>();

            //Get Products
            HttpResponseMessage productsResponse = await client.GetAsync(DefaultProductApiUrl + "/" + id);
            string strProduct = await productsResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ProductDTO? Product = JsonConvert.DeserializeObject<ProductDTO>(strProduct);

            total = 0;
            if (cart == null)
            {
                CartItems.Add(new CartDTO(Product, 1));
                total += Product.UnitPrice;
            }
            else
            {
                //deserialize Dictionary from string got from session
                CartItems = JsonConvert.DeserializeObject<List<CartDTO>>(cart);
                total = GetTotal(CartItems);

                int index = getIndexOfProductInCart(Product, CartItems);
                if (index != -1)
                    CartItems[index].Quantity++;
                else
                    CartItems.Add(new CartDTO(Product, 1));

                total += Product.UnitPrice;
            }

            //serialize dictionary to string to store in session
            string cartsserialize = JsonConvert.SerializeObject(CartItems);
            HttpContext.Session.SetString("CartSession", cartsserialize);

            //string totalserialize = JsonConvert.SerializeObject(total);
            //HttpContext.Session.SetString("total", totalserialize);

            ViewData["total"] = total;
            ViewData["AlertMessage"] = "Add to cart successful!";
            return RedirectToAction("Index", "Cart");
        }

        public async Task<IActionResult> OnClickAction(int productId, string? actionType)
        {
            string? cart = HttpContext.Session.GetString("CartSession");
            List<CartDTO> CartItems = JsonConvert.DeserializeObject<List<CartDTO>>(cart);

            //Get Products
            HttpResponseMessage productsResponse = await client.GetAsync(DefaultProductApiUrl + "/" + productId);
            string strProduct = await productsResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ProductDTO? Product = JsonConvert.DeserializeObject<ProductDTO>(strProduct);
            int index;
            switch (actionType)
            {
                case "plus":
                    index = getIndexOfProductInCart(Product, CartItems);
                    CartItems[index].Quantity++;
                    break;
                case "minus":
                    index = getIndexOfProductInCart(Product, CartItems);
                    CartItems[index].Quantity--;
                    if (CartItems[index].Quantity == 0) CartItems.Remove(CartItems[index]);
                    break;
                case "remove":
                    index = getIndexOfProductInCart(Product, CartItems);
                    CartItems.RemoveAt(index);
                    break;
                default:
                    break;
            }
            //serialize dictionary to string to store in session
            string cartsserialize = JsonConvert.SerializeObject(CartItems);
            HttpContext.Session.SetString("CartSession", cartsserialize);

            if (CartItems.Count == 0)
            {
                HttpContext.Session.Remove("CartSession");
            }

            //string totalserialize = JsonConvert.SerializeObject(total);
            //HttpContext.Session.SetString("total", totalserialize);

            ViewData["total"] = total;
            return RedirectToAction("Index", "Cart");
        }

        public async Task<ActionResult> Order()
        {
            //Get Account/Customer form session
            var mySessionValue = HttpContext.Session.GetString("user");
            
            if (mySessionValue == null) return RedirectToAction("Index", "Cart", new { @alertMessage = "Login to order!" });

            var userObject = JsonConvert.DeserializeObject<dynamic>(mySessionValue);
            var customer = userObject.account.customer;

            string? cartsdeserialize = HttpContext.Session.GetString("CartSession");
            List<CartDTO> CartItems = JsonConvert.DeserializeObject<List<CartDTO>>(cartsdeserialize);

            //Get list orderdetails
            List<OrderDetailCreateUpdateDTO> orderDetails = new List<OrderDetailCreateUpdateDTO>();
            foreach (var item in CartItems)
            {
                var od = new OrderDetailCreateUpdateDTO()
                {
                    ProductId = item.Product.ProductId,
                    UnitPrice = (decimal)item.Product.UnitPrice,
                    Quantity = (short)item.Quantity,
                    Discount = 0
                };
                orderDetails.Add(od);
            }


            OrderCreateUpdateDTO o = new OrderCreateUpdateDTO
            {
                CustomerId = customer.customerId,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now.AddDays(7),
                ShipName = customer.companyName,
                ShipAddress = customer.address,
                OrderDetails = orderDetails
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(DefaultOrderApiUrl, stringContent);

            if (!response.IsSuccessStatusCode)
            {
                RedirectToAction("Index", "Cart", new { @alertMessage = "Order failed!" });
            }

            HttpContext.Session.Remove("CartSession");
            return RedirectToAction("Index", "Order", new { @alertMessage = "Order successfully!" });
        }

        public int getIndexOfProductInCart(ProductDTO product, List<CartDTO> carts)
        {

            for (int i = 0; i < carts.Count; i++)
            {
                if (carts[i].Product.ProductId == product.ProductId) return i;
            }
            return -1;
        }

        public decimal? GetTotal(List<CartDTO> carts)
        {
            total = 0;
            foreach (CartDTO cart in carts)
            {
                total += cart.Product.UnitPrice * cart.Quantity;
            }
            return total;
        }
    }
}
