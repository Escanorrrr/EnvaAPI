namespace EnvaTest.DTO
{
    public class YanginTupuDto
    {
        public int CustomerId { get; set; }
        public double FaaliyetVerisiKg { get; set; } // Tüp başına ağırlık (kg)
        public int TupSayisi { get; set; }
        public double ToplamUretimTon { get; set; }
        public long InvoiceId { get; set; }
    }
}
