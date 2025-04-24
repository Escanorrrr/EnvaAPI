using EnvaTest.DTO;
using EnvaTest.Entities;
using EnvaTest.Result;
using EnvaTest.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnvaTest.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("Upload")]
        public async Task<ActionResult<Result<InvoiceResponseDTO>>> UploadInvoice([FromForm] InvoiceRequestDTO requestDTO)
        {
            var customerId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _invoiceService.UploadInvoiceAsync(customerId, requestDTO);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<Result<IEnumerable<InvoiceResponseDTO>>>> GetInvoiceByDateRange([FromQuery] InvoiceDateDTO dateRange)
        {
            var isAdmin = User.IsInRole("Admin");
            var currentUserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var result = await _invoiceService.GetInvoiceByDateRangeAsync(dateRange, isAdmin, currentUserId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<InvoiceResponseDTO>>> GetInvoice(long id)
        {
            var result = await _invoiceService.GetCustomerInvoicesAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Result<InvoiceResponseDTO>>> UpdateInvoice(long id, [FromForm] InvoiceUpdateDTO updateDTO)
        {
            var result = await _invoiceService.UpdateInvoiceAsync(id, updateDTO);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("types")]
        public async Task<ActionResult<Result<IEnumerable<InvoiceTypeResponseDTO>>>> GetInvoiceTypes()
        {
            var result = await _invoiceService.GetInvoiceTypesAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("yearly-ghg/{year}")]
        public async Task<ActionResult<Result<YearlyGHGResponseDTO>>> GetYearlyGHGData(int year)
        {
            var customerId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _invoiceService.GetYearlyGHGDataAsync(year, customerId);
            return StatusCode(result.StatusCode, result);
        }
    }
} 