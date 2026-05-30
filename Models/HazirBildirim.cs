namespace CafeApp.Models
{
    public class HazirBildirim
    {
        public int SiparisDetayId { get; set; }
        public int SiparisId { get; set; }
        public int Adet { get; set; }
        public string UrunAdi { get; set; } = "";
        public string MasaNo { get; set; } = "";
    }
}
