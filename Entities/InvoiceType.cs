using System.ComponentModel.DataAnnotations;

namespace EnvaTest.Entities
{
    public class InvoiceType
    {
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string InvoiceName { get; set; }

        // Navigation property
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
} 