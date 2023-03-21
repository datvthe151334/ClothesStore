﻿using BusinessObject.DTO;
using BusinessObject.Models;
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
        public async Task<IActionResult> GetProducts(int? categoryId , string? text)
        {
            try
            {
                
                return StatusCode(200, await repository.GetProducts(categoryId, text));
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
    }
}
