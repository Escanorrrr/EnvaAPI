using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvaTest.Entities
{
    public class Invoice : BaseEntity
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        public long InvoiceTypeId { get; set; }

        [Required]
        [StringLength(500)]
        public string InvoicePath { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "double")]
        public double? GHG { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("InvoiceTypeId")]
        public virtual InvoiceType InvoiceType { get; set; }
    }
} 