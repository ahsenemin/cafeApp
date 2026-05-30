using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    public partial class UrunYonetimPage : ContentPage
    {
        private UrunYonetimi _urunYonetimi = new UrunYonetimi();
        private KategoriYonetimi _kategoriYonetimi = new KategoriYonetimi();
        private Urun? _duzenlenecekUrun = null;

        public UrunYonetimPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            KategorileriPickeraDoldur();
            UrunleriYukle();
        }

        // ==============================================================================
        // KATEGORİLERİ PİCKER'A DOLDUR
        // ==============================================================================
        private void KategorileriPickeraDoldur()
        {
            KategoriPicker.ItemsSource = _kategoriYonetimi.TumKategorileriGetir();
        }

        // ==============================================================================
        // ÜRÜNLERİ YÜKLEME
        // ==============================================================================
        private void UrunleriYukle()
        {
            UrunlerListesi.ItemsSource = _urunYonetimi.TumUrunleriGetir();
        }

        // ==============================================================================
        // KAYDET (Ekleme veya Güncelleme)
        // ==============================================================================
        private async void OnKaydetClicked(object sender, EventArgs e)
        {
            string urunAdi = UrunAdiEntry.Text?.Trim() ?? "";
            string fiyatText = UrunFiyatEntry.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(urunAdi))
            {
                await DisplayAlert("Uyarı", "Ürün adı boş bırakılamaz!", "Tamam");
                return;
            }

            if (!decimal.TryParse(fiyatText, out decimal fiyat) || fiyat <= 0)
            {
                await DisplayAlert("Uyarı", "Geçerli bir fiyat girin! (Örn: 75.00)", "Tamam");
                return;
            }

            if (KategoriPicker.SelectedItem == null)
            {
                await DisplayAlert("Uyarı", "Lütfen bir kategori seçin!", "Tamam");
                return;
            }

            var secilenKategori = (Kategori)KategoriPicker.SelectedItem;
            string mesaj;

            if (_duzenlenecekUrun != null)
            {
                // GÜNCELLEME MODU
                mesaj = _urunYonetimi.UrunGuncelle(_duzenlenecekUrun.UrunId, urunAdi, fiyat, secilenKategori.KategoriId);
            }
            else
            {
                // EKLEME MODU
                mesaj = _urunYonetimi.UrunEkle(urunAdi, fiyat, secilenKategori.KategoriId);
            }

            await DisplayAlert(mesaj.StartsWith("Başarılı") ? "Başarılı" : "Hata", mesaj, "Tamam");

            if (mesaj.StartsWith("Başarılı"))
            {
                FormuTemizle();
                UrunleriYukle();
            }
        }

        // ==============================================================================
        // DÜZENLEME MODUNA GEÇ
        // ==============================================================================
        private void OnDuzenleClicked(object sender, EventArgs e)
        {
            var buton = sender as Button;
            if (buton == null) return;

            var urun = buton.BindingContext as Urun;
            if (urun == null) return;

            _duzenlenecekUrun = urun;
            UrunAdiEntry.Text = urun.UrunAdi;
            UrunFiyatEntry.Text = urun.UrunFiyati.ToString("F2");

            // Picker'da doğru kategoriyi seç
            var kategoriler = KategoriPicker.ItemsSource as List<Kategori>;
            if (kategoriler != null)
            {
                var kategori = kategoriler.FirstOrDefault(k => k.KategoriId == urun.KategoriId);
                if (kategori != null)
                {
                    KategoriPicker.SelectedItem = kategori;
                }
            }

            FormBaslikLabel.Text = $"✏️ \"{urun.UrunAdi}\" Düzenleniyor";
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

            var urun = buton.BindingContext as Urun;
            if (urun == null) return;

            bool onay = await DisplayAlert("Silme Onayı",
                $"\"{urun.UrunAdi}\" ({urun.UrunFiyati:C2}) silinecek.\n\nDevam etmek istiyor musunuz?",
                "Sil", "İptal");

            if (onay)
            {
                string mesaj = _urunYonetimi.UrunSil(urun.UrunId);
                await DisplayAlert(mesaj.StartsWith("Başarılı") ? "Başarılı" : "Hata", mesaj, "Tamam");

                if (mesaj.StartsWith("Başarılı"))
                {
                    UrunleriYukle();
                }
            }
        }

        // ==============================================================================
        // İPTAL
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
            _duzenlenecekUrun = null;
            UrunAdiEntry.Text = "";
            UrunFiyatEntry.Text = "";
            KategoriPicker.SelectedItem = null;
            FormBaslikLabel.Text = "Yeni Ürün Ekle";
            KaydetButon.Text = "💾 Kaydet";
            IptalButon.IsVisible = false;
        }
    }
}
