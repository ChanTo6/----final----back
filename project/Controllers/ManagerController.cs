using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.packages;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        IPKG_TO_DO package;
        private readonly IPKG_TO_DO _package;
        private IConfiguration _configuration;
        public ManagerController(IPKG_TO_DO package, IConfiguration configuration)
        {
            _package = package;
            _configuration = configuration;
        }

        [HttpGet("FetchProducts")]
        public async Task<IActionResult> FetchProducts()
        {
            try
            {
                var products = await _package.FetchProducts();
                Console.WriteLine(products);

                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "No products found." });
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("GetRemovedProducts")]
        public async Task<IActionResult> GetRemovedProducts()
        {
            try
            {
                var products = await _package.GetRemovedProducts();

                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "No products found." });
                }

                return Ok(products);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost("AddWarehouse")]
        public async Task<IActionResult> AddWarehouse([FromBody] WarehouseInfo warehouseData)
        {
            if (warehouseData == null)
            {
                return BadRequest(new { message = "Warehouse data is required" });
            }

            try
            {
                await _package.AddWarehouse(
                    warehouseData.UserId,
                    warehouseData.WarehouseName,
                    warehouseData.Location
                );

                return Ok(new { message = "Warehouse added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
