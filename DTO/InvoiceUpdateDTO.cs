using Microsoft.AspNetCore.Http;

namespace EnvaTest.DTO
{
    public class InvoiceUpdateDTO
    {
        public IFormFile? InvoiceFile { get; set; }
        public long? InvoiceTypeId { get; set; }
        public decimal? Amount { get; set; }
    }
} 