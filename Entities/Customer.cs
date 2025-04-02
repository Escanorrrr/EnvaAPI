using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvaTest.Entities
{
    public class Customer : BaseEntity
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

        public DateTime RegistrationDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(60)]
        public string Password { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool IsAdmin { get; set; }

        [StringLength(20)]
        public string TaxId { get; set; }

        public bool Active { get; set; }

        // Navigation property
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
} 