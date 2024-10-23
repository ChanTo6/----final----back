using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using project.packages;
using project.Model;
using System.Data;
using project.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        IPKG_TO_DO package;
        private readonly IPKG_TO_DO _package;
        private IConfiguration _configuration;

        public HomeController(IPKG_TO_DO package, IConfiguration configuration)
        {
            _package = package;
            _configuration = configuration;
        }


        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(UserData userData)
        {
            try
            {

                await _package.CreateUser(userData.Email, userData.EmployeeLastName, userData.EmployeeName, userData.OrganizationAddress, userData.OrganizationName, userData.Password, userData.personId, userData.PhoneNumber, userData.Role);


                return Ok(new { message = "User created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost("UpdateUserByPersonId")]
        public async Task<IActionResult> UpdateUserByPersonId([FromBody] Update request)
        {
            Console.WriteLine(request);
            try
            {
                await _package.UpdateUserByPersonId(
            request.PersonId,
            request.EmployeeName,
            request.EmployeeSurname,
            request.Password,
            request.Role,
            request.Telephone,
            request.OrgName,
            request.Warehouse
                );
                return Ok(new { message = "User updated successfully" });
            }
            catch (OracleException ex)
            {
                return BadRequest(new { message = $"Oracle error occurred: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] int userId)
        {
            try
            {
                Console.WriteLine(userId);
                string result = await _package.DeleteUser(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while deleting the user."); 
            }
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Invalid request." });
            }

            try
            {
                var loginResponse = await _package.LoginUser(request.UserName, request.Password);

                if (loginResponse != null)
                {
                    return Ok(new
                    {
                        token = loginResponse.Token,
                        role = loginResponse.Role,
                        userId = loginResponse.UserId
                    });
                }
                else
                {
                    return Unauthorized(new { message = "Invalid username or password." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        public static UserData user = new UserData();


        //   [Authorize(Roles = "admin")]
        [HttpGet("GetAllProjectUsersAsync")]
        public async Task<IActionResult> GetAllProjectUsersAsync()
        {


            var users = await _package.GetAllProjectUsersAsync();


            return Ok(users);
        }


        [HttpPost("UpdateUserStatus")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusRequest request)
        {
            try
            {
                await _package.UpdateUserStatus(request.UserId, request.Status);
                return Ok(new { message = "User status updated successfully." });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        //    [Authorize(Roles = "admin")]
        [HttpGet("GetAllOrganizationNamesAsync")]
        public async Task<IActionResult> GetAllOrganizationNamesAsync()
        {
            try
            {
                var orgNames = await _package.GetAllOrganizationNamesAsync();
                return Ok(orgNames);
            }
            catch (Exception ex)
            {


                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("check-free-seats/{userId}")]
        public async Task<IActionResult> CheckFreeSeatsByUserIdAsync(int userId)
        {
            try
            {
                var warehouseList = await _package.CheckFreeSeatsByUserIdAsync(userId);

                if (warehouseList == null || !warehouseList.Any())
                {
                    return NotFound($"No warehouses found for user ID: {userId}");
                }

                return Ok(warehouseList);
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework is recommended)
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("check-free-seats")]
        public async Task<IActionResult> CheckFreeSeatsAllWarehousesAsync()
        {
            try
            {
                var warehouseList = await _package.CheckFreeSeatsAllWarehousesAsync();

                if (warehouseList == null || !warehouseList.Any())
                {
                    return NotFound("No warehouses found.");
                }

                return Ok(warehouseList);
            }
            catch (Exception ex)
            {
                // Log the exception (using a logging framework is recommended)
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("FetchNameAndLocation")]
        public async Task<IActionResult> FetchNameAndLocation()
        {
            try
            {
                var warehouses = await _package.FetchNameAndLocation();

                if (warehouses == null || !warehouses.Any())
                {
                    return NotFound(new { message = "No warehouses found." });
                }

                return Ok(warehouses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        
    }
}
