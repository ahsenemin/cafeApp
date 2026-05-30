using System;

namespace CafeApp.Models
{
    public class Personel
    {
        public int PersonelId { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Eposta { get; set; }
        public string? Adres { get; set; }
        public string? Telefon { get; set; }
        public string? TcKimlik { get; set; }
        public byte AktifMi { get; set; } // 1: Aktif, 2: Pasif
        public DateTime KayitTarihi { get; set; }
        public int RolId { get; set; }

        // MAUI Arayüzünde (Örn: Giriş yapan kişinin adını gösterirken) kullanmak için yardımcı özellik
        public string TamAd
        {
            get { return $"{Ad} {Soyad}"; }
        }

        // Aktiflik durumunu metin olarak göstermek için
        public string DurumMetni
        {
            get { return AktifMi == 1 ? "Aktif" : "Pasif"; }
        }

        // Rol ismini göstermek için (Login ekranında kullanılıyor)
        public string RolMetni
        {
            get
            {
                return RolId switch
                {
                    100 => "Garson",
                    101 => "Yönetici",
                    102 => "Barista",
                    _ => "Personel"
                };
            }
        }
    }
}