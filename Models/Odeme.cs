using System;

namespace CafeApp.Models
{
    public class Odeme
    {
        public int OdemeId { get; set; }
        public int SiparisId { get; set; }
        public string? OdemeSekli { get; set; }
        public decimal OdenenMiktar { get; set; }
        public DateTime OdemeTarihSaat { get; set; }
    }
}
