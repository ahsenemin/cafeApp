using System;

namespace CafeApp.Models
{
    public class Siparis
    {
        public int SiparisId { get; set; }
        public int? MasaId { get; set; } // Gel-Al siparişler için null olabilir
        public int PersonelId { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public string? Durum { get; set; } // Açık, Kapalı, İptal
    }
}