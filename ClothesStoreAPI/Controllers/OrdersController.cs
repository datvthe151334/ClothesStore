using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using ClothesStoreAPI.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;
using BusinessObject.DTO;

namespace ClothesStoreAPI.Controllersorder
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IOrderRepository repository;

        public OrdersController(IOrderRepository repo)
        {
            repository = repo;
        }

        //GET: api/Orders
        [HttpGet]
        public async Task<IActionResult> GetOrders(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                return StatusCode(200, await repository.GetOrders(startDate, endDate));
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

        //GET: api/Orders/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                return StatusCode(200, await repository.GetOrderById(id));
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

        //GET: api/Orders/getDictionaryOrders
        [HttpGet("getOrdersByCustomer")]
        public async Task<IActionResult> GetDictionaryOrderByCustomer(string customerId)
        {
            try
            {
                return StatusCode(200, await repository.GetOrdersByCustomer(customerId));
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
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateUpdateDTO orderDTO)
        {
            try
            {
                return StatusCode(204, await repository.CreateOrder(orderDTO));
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
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await repository.DeleteOrder(id);
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
        public async Task<IActionResult> Export(DateTime? startDate, DateTime? endDate)
        {
            var listOrders = await repository.GetOrders(startDate, endDate);

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
        }
    }
}
