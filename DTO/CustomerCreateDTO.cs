using System.ComponentModel.DataAnnotations;

namespace EnvaTest.DTO
{
    public class CustomerCreateDTO
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

        [Required]
        [StringLength(100)]
        [MinLength(6)]
        public string Password { get; set; }

        [StringLength(20)]
        public string TaxId { get; set; }

        public bool IsAdmin { get; set; }
    }
} 