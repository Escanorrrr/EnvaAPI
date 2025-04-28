namespace EnvaTest.DTO
{
    public class FormulResponseDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public string Operation { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 