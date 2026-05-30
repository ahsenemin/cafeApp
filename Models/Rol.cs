namespace CafeApp.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string? RolAdi { get; set; }
        public byte AktifMi { get; set; }

        public string DurumMetni
        {
            get { return AktifMi == 1 ? "Aktif" : "Pasif"; }
        }
    }
}
