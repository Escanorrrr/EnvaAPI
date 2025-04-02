using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EnvaTest.DTO
{
    public class InvoiceRequestDTO
    {
        [Required(ErrorMessage = "Lütfen bir fatura dosyası seçiniz")]
        [Display(Name = "Fatura Dosyası")]
        public IFormFile InvoiceFile { get; set; }

        [Required(ErrorMessage = "Lütfen fatura tipini seçiniz")]
        [Display(Name = "Fatura Tipi")]
        public long InvoiceTypeId { get; set; }

        [Required(ErrorMessage = "Lütfen fatura tutarını giriniz")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fatura tutarı 0'dan büyük olmalıdır")]
        [Display(Name = "Fatura Tutarı")]
        public decimal Amount { get; set; }

        // Dosya validasyonu için özel bir metot
        public bool IsValidFileType()
        {
            if (InvoiceFile == null || InvoiceFile.Length == 0)
                return false;

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(InvoiceFile.FileName).ToLowerInvariant();
            
            return allowedExtensions.Contains(fileExtension);
        }

        // Dosya boyutu kontrolü (örneğin max 10MB)
        public bool IsValidFileSize()
        {
            const int maxFileSize = 10 * 1024 * 1024; // 10MB
            return InvoiceFile != null && InvoiceFile.Length <= maxFileSize;
        }
    }
} 