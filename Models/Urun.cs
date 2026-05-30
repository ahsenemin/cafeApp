namespace CafeApp.Models
{
    public class Urun
    {
        public int UrunId { get; set; }
        public string? UrunAdi { get; set; }
        public decimal UrunFiyati { get; set; }
        public int KategoriId { get; set; }
    }
}