using System.ComponentModel.DataAnnotations;

namespace EnvaTest.DTO
{
    public class FormulCreateDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Operation { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double Amount { get; set; }
    }
} 