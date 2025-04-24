using System.ComponentModel.DataAnnotations;

namespace EnvaTest.DTO
{
    public class InvoiceResponseDTO
    {
        public long Id { get; set; }
        public string InvoicePath { get; set; }
        public decimal Amount { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public long InvoiceTypeId { get; set; }
        public string InvoiceTypeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public double? GHG { get; set; }
    }
} 