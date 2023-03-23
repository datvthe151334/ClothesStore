using BusinessObject.DTO;
using BusinessObject.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace ClothesStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private IProductRepository repository;

        public ProductsController(IProductRepository repo)
        {
            repository = repo;
        }

        //GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                return StatusCode(200, await repository.GetProducts());
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET: api/Products/text
        [HttpGet("FilterProduct")] 
        public async Task<IActionResult> GetProducts(int? categoryId , string? text, string? sortType, decimal? startPrice, decimal? endPrice)
        {
            try
            {
                
                return StatusCode(200, await repository.GetProducts(categoryId, text, sortType, startPrice, endPrice));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET: api/Products/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                return StatusCode(200, await repository.GetProductById(id));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET: api/Products/filterByCatId/categoryId
        [HttpGet("filterByCatId/{categoryId}")]
        public async Task<IActionResult> GetProductByCategory(int categoryId)
        {
            try
            {
                return StatusCode(200, await repository.GetProductsByCategory(categoryId));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        //GET: api/Products/filterByCatGeneral/categoryGeneral
        [HttpGet("filterByCatGeneral/{categoryGeneral}")]
        public async Task<IActionResult> GetProductByCategoryGeneral(string categoryGeneral)
        {
            try
            {
                return StatusCode(200, await repository.GetProductsByCategoryGeneral(categoryGeneral));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateUpdateDTO productDTO)
        {
            try
            {
                return StatusCode(200, await repository.CreateProduct(productDTO));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //PUT
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductCreateUpdateDTO productDTO)
        {
            try
            {
                return StatusCode(200, await repository.UpdateProduct(productDTO));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await repository.DeleteProduct(id);
                return StatusCode(204, "Delete successfully!");
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("exportExcel")]
        public async Task<IActionResult> Export(string? searchString, decimal? startPrice, decimal? endPrice)
        {
            var listProducts = await repository.GetProducts(null, searchString, null, startPrice, endPrice);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Products");
                worksheet.Cell(1, 1).Value = "EMALL SHOP";
                worksheet.Cell(2, 1).Value = "LIST OF PRODUCTS";
                var currentRow = 3;
                worksheet.Cell(currentRow, 1).Value = "Product id";
                worksheet.Cell(currentRow, 2).Value = "Product name";
                worksheet.Cell(currentRow, 3).Value = "Category general";
                worksheet.Cell(currentRow, 4).Value = "Category name";
                worksheet.Cell(currentRow, 5).Value = "Quantity per unit";
                worksheet.Cell(currentRow, 6).Value = "Unit price";
                worksheet.Cell(currentRow, 7).Value = "Units in stock";
                worksheet.Cell(currentRow, 8).Value = "Units on order";
                worksheet.Cell(currentRow, 9).Value = "Reorder level";
                worksheet.Cell(currentRow, 10).Value = "Discontinued";
                worksheet.Cell(currentRow, 11).Value = "Is Active";
                worksheet.Cell(currentRow, 12).Value = "Picture";
                foreach (var product in listProducts)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = product.ProductId;
                    worksheet.Cell(currentRow, 2).Value = product.ProductName;
                    worksheet.Cell(currentRow, 3).Value = product.CategoryId;
                    worksheet.Cell(currentRow, 4).Value = product.CategoryGeneral;
                    worksheet.Cell(currentRow, 5).Value = product.QuantityPerUnit;
                    worksheet.Cell(currentRow, 6).Value = product.UnitPrice;
                    worksheet.Cell(currentRow, 7).Value = product.UnitsInStock;
                    worksheet.Cell(currentRow, 8).Value = product.UnitsOnOrder;
                    worksheet.Cell(currentRow, 9).Value = product.ReorderLevel;
                    worksheet.Cell(currentRow, 10).Value = (product.Discontinued == true) ? "true" : "false";
                    worksheet.Cell(currentRow, 11).Value = (product.IsActive == true) ? "true" : "false";
                    worksheet.Cell(currentRow, 12).Value = product.Picture;
                }

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
        }
    }
}
