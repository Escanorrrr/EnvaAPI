namespace EnvaTest.Entities
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }  // int yerine long kullanıyoruz çünkü mevcut Customer entity'miz long kullanıyor

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 