using BusinessObject.DTO;
using ClosedXML.Excel;
using OfficeOpenXml;

namespace ClothesStoreAPI.Configuration
{
    public class ExcelConfiguration
    {
        private static readonly String error = "Can't import Excel! Error in column ";

        public static String? excelError { get; set; }

        //Export Product
        public static XLWorkbook exportProduct(List<ProductDTO> listProducts, XLWorkbook workbook)
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
            return workbook;
        }

        //Export Order
        public static XLWorkbook exportOrder(List<OrderDTO> listOrders, XLWorkbook workbook)
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
            return workbook;
        }

        //Import Product
        public static async Task<List<ProductCreateUpdateDTO>> importProduct(IFormFile? file)
        {
            var listProductsFromExcel = new List<ProductCreateUpdateDTO>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            int j = 0;
                            var productName = GetString(worksheet.Cells[i, ++j]);
                            var categoryId = GetInt(worksheet.Cells[i, ++j]);
                            var quantityPerUnit = GetString(worksheet.Cells[i, ++j]);
                            var unitPrice = GetDecimal(worksheet.Cells[i, ++j]);
                            var unitsInStock = GetShort(worksheet.Cells[i, ++j]);
                            var unitsOnOrder = GetShort(worksheet.Cells[i, ++j]);
                            var reorderLevel = GetShort(worksheet.Cells[i, ++j]);
                            var discontinued = GetBool(worksheet.Cells[i, ++j]);
                            var isActive = GetBool(worksheet.Cells[i, ++j]);
                            var picture = GetString(worksheet.Cells[i, ++j]);
                            listProductsFromExcel.Add(new ProductCreateUpdateDTO
                            {
                                ProductName = productName ?? "Blank",
                                CategoryId = categoryId,
                                QuantityPerUnit = quantityPerUnit,
                                UnitPrice = unitPrice,
                                UnitsInStock = unitsInStock,
                                UnitsOnOrder = unitsOnOrder,
                                ReorderLevel = reorderLevel,
                                Discontinued = discontinued ?? false,
                                IsActive = isActive ?? true,
                                Picture = picture
                            });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            return listProductsFromExcel;
        }

        private static string? GetString(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                return cell.ToString();
            }

            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static DateTime? GetDate(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try
                {
                    return Convert.ToDateTime(cell.ToString());
                }
                catch (FormatException e)
                {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static decimal? GetDecimal(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try
                {
                    return Convert.ToDecimal(cell);
                }
                catch (FormatException e)
                {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static short? GetShort(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try
                {
                    return (short?)Convert.ToUInt32(cell);
                }
                catch (FormatException e)
                {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static short? GetInt(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try
                {
                    return (short?)Convert.ToUInt32(cell);
                }
                catch (FormatException e)
                {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }

        private static bool? GetBool(ExcelRange excelRange)
        {
            var cell = excelRange.Value;
            if (cell is not null)
            {
                try
                {
                    return Convert.ToBoolean(cell);
                }
                catch (FormatException e)
                {
                    excelError = error + excelRange.Start.Address + "/ " + e.Message;
                }
            }
            throw new Exception(excelError = error + excelRange.Start.Address);
        }
    }
}
