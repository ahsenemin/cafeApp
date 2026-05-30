using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    public partial class KategoriYonetimPage : ContentPage
    {
        private KategoriYonetimi _kategoriYonetimi = new KategoriYonetimi();
        private Kategori? _duzenlenecekKategori = null; // Düzenleme modunda ise bu dolu olur

        public KategoriYonetimPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            KategorileriYukle();
        }

        // ==============================================================================
        // KATEGORİLERİ YÜKLEME
        // ==============================================================================
        private void KategorileriYukle()
        {
            KategoriListesi.ItemsSource = _kategoriYonetimi.TumKategorileriGetir();
        }

        // ==============================================================================
        // KAYDET (Ekleme veya Güncelleme)
        // ==============================================================================
        private async void OnKaydetClicked(object sender, EventArgs e)
        {
            string kategoriAdi = KategoriAdiEntry.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(kategoriAdi))
            {
                await DisplayAlert("Uyarı", "Kategori adı boş bırakılamaz!", "Tamam");
                return;
            }

            string mesaj;

            if (_duzenlenecekKategori != null)
            {
                // GÜNCELLEME MODU
                mesaj = _kategoriYonetimi.KategoriGuncelle(_duzenlenecekKategori.KategoriId, kategoriAdi);
            }
            else
            {
                // EKLEME MODU
                mesaj = _kategoriYonetimi.KategoriEkle(kategoriAdi);
            }

            await DisplayAlert(mesaj.StartsWith("Başarılı") ? "Başarılı" : "Hata", mesaj, "Tamam");

            if (mesaj.StartsWith("Başarılı"))
            {
                FormuTemizle();
                KategorileriYukle();
            }
        }

        // ==============================================================================
        // DÜZENLEME MODUNA GEÇ
        // ==============================================================================
        private void OnDuzenleClicked(object sender, EventArgs e)
        {
            var buton = sender as Button;
            if (buton == null) return;

            var kategori = buton.BindingContext as Kategori;
            if (kategori == null) return;

            _duzenlenecekKategori = kategori;
            KategoriAdiEntry.Text = kategori.KAdi;
            FormBaslikLabel.Text = $"✏️ \"{kategori.KAdi}\" Düzenleniyor";
            KaydetButon.Text = "💾 Güncelle";
            IptalButon.IsVisible = true;
        }

        // ==============================================================================
        // SİLME
        // ==============================================================================
        private async void OnSilClicked(object sender, EventArgs e)
        {
            var buton = sender as Button;
            if (buton == null) return;

            var kategori = buton.BindingContext as Kategori;
            if (kategori == null) return;

            bool onay = await DisplayAlert("Silme Onayı",
                $"\"{kategori.KAdi}\" kategorisi silinecek.\nBu kategorideki ürünler de etkilenebilir.\n\nDevam etmek istiyor musunuz?",
                "Sil", "İptal");

            if (onay)
            {
                string mesaj = _kategoriYonetimi.KategoriSil(kategori.KategoriId);
                await DisplayAlert(mesaj.StartsWith("Başarılı") ? "Başarılı" : "Hata", mesaj, "Tamam");

                if (mesaj.StartsWith("Başarılı"))
                {
                    KategorileriYukle();
                }
            }
        }

        // ==============================================================================
        // İPTAL (Düzenleme modundan çık)
        // ==============================================================================
        private void OnIptalClicked(object sender, EventArgs e)
        {
            FormuTemizle();
        }

        // ==============================================================================
        // FORMU TEMİZLE
        // ==============================================================================
        private void FormuTemizle()
        {
            _duzenlenecekKategori = null;
            KategoriAdiEntry.Text = "";
            FormBaslikLabel.Text = "Yeni Kategori Ekle";
            KaydetButon.Text = "💾 Kaydet";
            IptalButon.IsVisible = false;
        }
    }
}
