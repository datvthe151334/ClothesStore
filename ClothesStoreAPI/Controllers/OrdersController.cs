using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;

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
                var worksheet = workbook.Worksheets.Add("Orders");
                worksheet.Cell(1, 1).Value = "EMALL SHOP";
                worksheet.Cell(2, 1).Value = "LIST OF ORDERS";
                var currentRow = 3;
                worksheet.Cell(currentRow, 1).Value = "Order id";
                worksheet.Cell(currentRow, 2).Value = "Customer name";
                worksheet.Cell(currentRow, 3).Value = "Employee name";
                worksheet.Cell(currentRow, 4).Value = "Order date";
                worksheet.Cell(currentRow, 5).Value = "Required date";
                worksheet.Cell(currentRow, 6).Value = "Shipped date";
                worksheet.Cell(currentRow, 7).Value = "Freight";
                worksheet.Cell(currentRow, 8).Value = "Ship name";
                worksheet.Cell(currentRow, 9).Value = "Ship address";
                worksheet.Cell(currentRow, 10).Value = "Ship city";
                worksheet.Cell(currentRow, 11).Value = "Ship region";
                worksheet.Cell(currentRow, 12).Value = "Ship postal code";
                worksheet.Cell(currentRow, 13).Value = "Ship country";
                foreach (var order in listOrders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = order.OrderId;

                    if (order.CustomerName is not null)
                    {
                        worksheet.Cell(currentRow, 2).Value = order.CustomerName;
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 2).Value = "";
                    }

                    if (order.EmployeeName is not null)
                    {
                        worksheet.Cell(currentRow, 3).Value = order.EmployeeName;
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Value = "";
                    }

                    worksheet.Cell(currentRow, 4).Value = order.OrderDate;
                    worksheet.Cell(currentRow, 5).Value = order.RequiredDate;
                    worksheet.Cell(currentRow, 6).Value = order.ShippedDate;
                    worksheet.Cell(currentRow, 7).Value = order.Freight;
                    worksheet.Cell(currentRow, 8).Value = order.ShipName;
                    worksheet.Cell(currentRow, 9).Value = order.ShipAddress;
                    worksheet.Cell(currentRow, 10).Value = order.ShipCity;
                    worksheet.Cell(currentRow, 11).Value = order.ShipRegion;
                    worksheet.Cell(currentRow, 12).Value = order.ShipPostalCode;
                    worksheet.Cell(currentRow, 13).Value = order.ShipCountry;
                }

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
