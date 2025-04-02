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

        [HttpGet("MyInvoices")]
        public async Task<ActionResult<Result<IEnumerable<InvoiceResponseDTO>>>> GetMyInvoices()
        {
            var customerId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _invoiceService.GetCustomerInvoicesAsync(customerId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<InvoiceResponseDTO>>> GetInvoice(long id)
        {
            var result = await _invoiceService.GetInvoiceByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
} 