using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using project.Models;
using project.packages;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorController : ControllerBase
    {
        IPKG_TO_DO package;
        private readonly IPKG_TO_DO _package;
        private IConfiguration _configuration;
        public OperatorController(IPKG_TO_DO package, IConfiguration configuration)
        {
            _package = package;
            _configuration = configuration;
        }


        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductUpdate productDto)
        {
            if (productDto == null)
            {
                return BadRequest(new { message = "Invalid product data." });
            }

            try
            {

                if (string.IsNullOrWhiteSpace(productDto.warehouse))
                {
                    return BadRequest(new { message = "Warehouse name is required." });
                }


                await _package.AddProductToWarehouse(
                    productDto.ProductName,
                    productDto.quantity,
                    productDto.userId,
                    productDto.warehouse,
                    productDto.Location
                );

                return Ok(new { message = "Product added successfully." });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("FetchProductbyuserId/{userId}")]
        public async Task<IActionResult> FetchProductbyuserId(int userId)
        {
            Console.WriteLine(userId);
            try
            {
                var products = await _package.FetchProductbyuserId(userId);

                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "No products found for the specified user." });
                }

                return Ok(products);
            }
            catch (OracleException oracleEx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Database error: {oracleEx.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost("EditProduct")]
        public async Task<IActionResult> EditProduct([FromBody] Product productDto)
        {
            if (productDto == null)
            {
                return BadRequest(new { message = "Invalid product data." });
            }

            try
            {
                await _package.EditProductInWarehouse(
                    productDto.ProductName,
                    productDto.barcode,
                    productDto.ProductId,
                    productDto.quantity,
                    productDto.userId
                );

                return Ok(new { message = "Product updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost("GetRemovedProductsByUserId")]
        public async Task<IActionResult> GetRemovedProductsByUserId([FromBody] int userId)
        {

            try
            {
                var products = await _package.GetRemovedProductsByUserId(userId);

                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "No products found for the specified user." });
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost]
        [Route("RemoveProduct")]
        public async Task<IActionResult> RemoveProductAsync([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product data is required.");
            }

            try
            {
                await _package.RemoveProductAsync(product.userId, product.barcode, product.quantity);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest($"Error: {ex.Message}");
            }
        }


    }
}
