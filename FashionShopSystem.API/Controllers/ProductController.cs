using FashionShopSystem.Service.Services;
using FashionShopSystem.Service;

﻿using FashionShopSystem.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int? categoryId,[FromQuery] string? brand, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,[FromQuery] string? keyword) 
        {
            var products = await _productService.SearchProductsAsync(categoryId, brand, minPrice, maxPrice, keyword);
            return Ok(products);
        }

        [HttpGet("Product")]
        public async Task<IActionResult> GetAllProductsAsync(int? categoryid, string? keyword,string? sort, string? brand, decimal? price)
        {
            try
            {
                var products = await _productService.GetAllProductsAsync(categoryid, keyword,sort,brand,price);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving products: {ex.Message}");
            }
        }
        [HttpGet("Product/{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving product: {ex.Message}");
            }
        }
        [HttpPost("Product")]
        public async Task<IActionResult> CreatProduct(CreateProductDto dto)
        {

            var result = await _productService.CreateProduct(dto);
            return Ok(result);
        }
        [HttpPut("Product")]
        public async Task<IActionResult> updateProduct(UpdateProductDto dto)
        {
            var result = await _productService.UpdateProduct(dto);
            return Ok(result);
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var result = await _productService.deteletProduct(Id);
            return Ok(result);
        }
        [HttpGet("Brand")]
        public async Task<IActionResult> GetAllBrand()
        {
            try
            {
                var brands = await _productService.getAllBrand();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving brands: {ex.Message}");
            }
        }
    }
}
