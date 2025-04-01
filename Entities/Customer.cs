using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvaTest.Entities
{
    public class Customer
    {
        [Key]
        public long Id { get; set; }

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
        public DateTime RegistrationDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(60)] // Bcrypt hash için yeterli uzunluk
        public string Password { get; set; }

        public DateTime? LastLoginDate { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        [StringLength(20)]
        public string TaxId { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
} 