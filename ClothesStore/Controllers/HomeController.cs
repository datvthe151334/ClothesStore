using BusinessObject.DTO;
using ClothesStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;


namespace ClothesStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client = null;
        private string BaseUrl = "";
        private string DefaultProductApiUrl = "";
        private string DefaultCategoryApiUrl = "";
        private string DefaultCustomerApiUrl = "";
        private string DefaultAccountsApiUrl = "";

        public HomeController(IConfiguration configuration)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultProductApiUrl = "https://localhost:7059/api/Products";
            DefaultCategoryApiUrl = "https://localhost:7059/api/Categories";
            DefaultCustomerApiUrl = "https://localhost:7059/api/Customers";
            DefaultAccountsApiUrl = "https://localhost:7059/api/Accounts";
            BaseUrl = "https://localhost:7059";
            this.configuration = configuration;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["accessToken"]))
            {
                return Redirect("/Home/Index");
            }
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["accessToken"]) && !string.IsNullOrEmpty(HttpContext.Request.Cookies["refreshToken"]))
            {
                AccountInfoTokenDTO u = new AccountInfoTokenDTO();
                u.RefreshToken = HttpContext.Request.Cookies["refreshToken"];
                var conn = $"api/Accounts/refresh-token";
                var Res = await PostData(conn, JsonConvert.SerializeObject(u));
                if (!Res.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete("refreshToken");
                    return RedirectToAction("Index", "Home");
                }

                var user = JsonConvert.DeserializeObject<AccountInfoTokenDTO>(Res.Content.ReadAsStringAsync().Result);

                HttpContext.Session.SetString("user", Res.Content.ReadAsStringAsync().Result);

                validateToken(user!.AccessToken!.Replace("\"", ""));

                Response.Cookies.Append("refreshToken", user.RefreshToken!, new CookieOptions { Expires = user.TokenExpires, HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict });

                if (user.Account!.Role == 1) return RedirectToAction("Index", "AdminAccount");

                return RedirectToAction("Index", "Home");
            }
            ViewBag.ErrMsg = TempData["ErrorMessage"] as string;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string? email)
        {
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize<string>(email), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(DefaultAccountsApiUrl + "/ResetPassword?email=" + email, stringContent);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("NotFound", "Accounts");
            }
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home", new { @alertMessage = "Send mail successfully!" });
            }
            return RedirectToAction("Index", "Home", new { @alertMessage = "Send mail fail, Re-enter your email!" });
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(String email,String password)
        {            
                LoginDTO us = new LoginDTO();
                us.Email = email;
                us.Password = password;
                var Res = PostData("api/Accounts/signin", JsonConvert.SerializeObject(us));
        
                /*var name = "";*/

            if (!Res.Result.IsSuccessStatusCode)
            {
                /*return StatusCode(StatusCodes.Status500InternalServerError);*/
                return RedirectToAction("Index", "Home", new { @alertMessage = "Login fail!" });

            }
            else
            {

                var user = JsonConvert.DeserializeObject<AccountInfoTokenDTO>(Res.Result.Content.ReadAsStringAsync().Result);
                
                 /*name = user.Name;*/

                HttpContext.Session.SetString("user", Res.Result.Content.ReadAsStringAsync().Result);
                
                validateToken(user!.AccessToken!.Replace("\"", ""));

                Response.Cookies.Append("refreshToken", user.RefreshToken!, new CookieOptions
                { Expires = user.TokenExpires, HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict });

                if(user.Account.EmployeeId != null)
                {
                    return RedirectToAction("Index", "AdminAccount", new { @alertMessage = "Login successfully!" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { @alertMessage = "Login successfully!" });
                }


            }
            

        }

        
        public async Task<IActionResult> Index([FromQuery] string? CategoryGeneral, string? alertMessage)
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

            List<ProductDTO>? listProducts = JsonConvert.DeserializeObject<List<ProductDTO>>(strProducts);
            List<ProductDTO>? listMenProducts = JsonConvert.DeserializeObject<List<ProductDTO>>(strMenProducts);
            List<ProductDTO>? listWomenProducts = JsonConvert.DeserializeObject<List<ProductDTO>>(strWomenProducts);
            List<ProductDTO>? listBabyProducts = JsonConvert.DeserializeObject<List<ProductDTO>>(strBabyProducts);
            
            List<string>? listCategoryGeneral = JsonConvert.DeserializeObject<List<string>>(strCategoryGeneral);
            List<CategoryDTO>? listCategories = JsonConvert.DeserializeObject<List<CategoryDTO>>(strCategories);
            List<CustomerDTO>? listCustomers = JsonConvert.DeserializeObject<List<CustomerDTO>>(strCustomers);

            ViewBag.listMenProducts = listMenProducts.OrderByDescending(x => x.ProductId).Take(12).ToList();
            ViewBag.listWomenProducts = listWomenProducts.OrderByDescending(x => x.ProductId).Take(12).ToList();
            ViewBag.listBabyProducts = listBabyProducts.OrderByDescending(x => x.ProductId).Take(12).ToList();
            
            
            ViewBag.listCategories = listCategories;
            ViewBag.listCategoryGeneral = listCategoryGeneral;
            ViewBag.listProducts = listProducts.OrderByDescending(x => x.ProductId).Take(12).ToList();
            ViewData["CurCatGeneral"] = CategoryGeneral;
            /* ViewData["TotalCustomer"] = listCustomers.Count;*/
            ViewData["AlertMessage"] = alertMessage;
            return View();
        }

        private void validateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var expires = jwtToken.ValidTo;
                var role = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role);
                var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email);
                List<ClaimsIdentity> identities = new List<ClaimsIdentity>
                {
                    new ClaimsIdentity(new[] { role }),
                    new ClaimsIdentity(new[] { email })
                };

                User.AddIdentities(identities);

                Response.Cookies.Append("accessToken", token, new CookieOptions
                { Expires = expires, HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<HttpResponseMessage> PostData(string targerAddress, string content)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["accessToken"]))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies["accessToken"]);
            }
            else
            {
                client.DefaultRequestHeaders.Add("refreshToken", HttpContext.Request.Cookies["refreshToken"]);
            }
            var Response = await client.PostAsync(targerAddress, new StringContent(content, Encoding.UTF8, "application/json"));
            return Response;
        }

        [HttpGet]
        public IActionResult signout()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            HttpContext.Session.Remove("CartSession");
            return RedirectToAction("index");
        }
        

        [HttpPost]
        public IActionResult SignUp(SignUpDTO req)
        {
            var conn = "api/Accounts/signup";
            var Res = PostData(conn, JsonConvert.SerializeObject(req));
            if (!Res.Result.IsSuccessStatusCode) return RedirectToAction("Index", "Home", new { @alertMessage = "Signup fail!" });
            return RedirectToAction("Index", "Home", new { @alertMessage = "Signup successfully!" });
        }

        
    }
}