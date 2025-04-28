using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvaTest.Entities
{
    public class Formul
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        [StringLength(100)]
        public string Operation { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public double Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}   
