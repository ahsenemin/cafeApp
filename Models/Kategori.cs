namespace CafeApp.Models
{
    public class Kategori
    {
        public int KategoriId { get; set; }
        public string? KAdi { get; set; }

        public override string ToString() => KAdi ?? "";
    }
}
