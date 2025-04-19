using System;
using System.ComponentModel.DataAnnotations;

namespace EnvaTest.DTO
{
    public class InvoiceDateDTO
    {
        [Required(ErrorMessage = "Müşteri ID zorunludur.")]
        public long CustomerId { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
        public DateTime BaslangicTarihi { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
        public DateTime BitisTarihi { get; set; }
    }
} 