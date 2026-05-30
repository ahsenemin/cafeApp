using System;

namespace CafeApp.Models
{
    public class Masa
    {
        // Veritabanındaki sütunların C# karşılıkları
        public int MasaId { get; set; }
        public string? MasaNo { get; set; }
        public byte Durum { get; set; } // 1: Boş, 2: Dolu, 3: Rezerve
        public decimal MasaToplamFiyati { get; set; }
        public DateTime EnSonIslemTarihi { get; set; }

        // MAUI Arayüzünde durumu metin olarak göstermek için yardımcı özellik (Sadece Okunabilir)
        public string DurumMetni
        {
            get
            {
                return Durum switch
                {
                    1 => "Boş",
                    2 => "Dolu",
                    3 => "Rezerve",
                    _ => "Bilinmiyor"
                };
            }
        }
    }
}