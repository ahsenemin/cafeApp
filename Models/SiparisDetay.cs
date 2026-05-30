namespace CafeApp.Models
{
    public class SiparisDetay
    {
        public int SiparisDetayId { get; set; }
        public int SiparisId { get; set; }
        public int UrunId { get; set; }
        public int Adet { get; set; }
        public decimal SatisFiyati { get; set; }
        public string? SiparisNotu { get; set; }

        // JOIN sorgularından gelecek yardımcı alanlar (Barista ve Ödeme ekranında kullanılacak)
        public string? UrunAdi { get; set; }
        public string? MasaNo { get; set; }

        // Arayüzde toplam tutarı göstermek için (Adet x Fiyat)
        public decimal ToplamTutar => Adet * SatisFiyati;
    }
}
