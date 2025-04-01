using System.ComponentModel.DataAnnotations;

namespace EnvaTest.DTO
{
    public class CustomerUpdateDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(20)]
        public string TaxId { get; set; }

        // Şifre değişikliği ayrı bir endpoint'te yapılacağı için buraya eklemedik
        // IsAdmin değişikliği sadece admin tarafından yapılabilmeli, ayrı bir endpoint'te olmalı
        // Active durumu zaten ayrı bir endpoint'te
    }
} 