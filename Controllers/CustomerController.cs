using EnvaTest.DTO;
using EnvaTest.Entities;
using EnvaTest.Result;
using EnvaTest.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnvaTest.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("GetAllCustomers")]
        public async Task<ActionResult<Result<IEnumerable<CustomerListDTO>>>> GetCustomers()
        {
            var result = await _customerService.GetAllCustomersAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetCustomerById/{id}")]
        public async Task<ActionResult<Result<Customer>>> GetCustomer(long id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("CreateCustomer")]
        public async Task<ActionResult<Result<Customer>>> CreateCustomer(CustomerCreateDTO customerDTO)
        {
            var result = await _customerService.CreateCustomerAsync(customerDTO);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("UpdateCustomer/{id}")]
        public async Task<ActionResult<Result<Customer>>> UpdateCustomer(long id, CustomerUpdateDTO customerDTO)
        {
            var result = await _customerService.UpdateCustomerAsync(id, customerDTO);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("UpdateCustomerStatus/{id}")]
        public async Task<ActionResult<Result<Customer>>> UpdateCustomerStatus(long id, [FromQuery] bool isActive)
        {
            var result = await _customerService.UpdateCustomerStatusAsync(id, isActive);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("ChangePassword/{id}")]
        public async Task<ActionResult<Result<Customer>>> ChangePassword(long id, ChangePasswordDTO changePasswordDTO)
        {
            var result = await _customerService.ChangePasswordAsync(id, changePasswordDTO);
            return StatusCode(result.StatusCode, result);
        }
    }
} 