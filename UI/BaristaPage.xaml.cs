using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    public partial class BaristaPage : ContentPage
    {
        private SiparisYonetimi _siparisYonetimi = new SiparisYonetimi();

        // Otomatik yenileme için zamanlayıcı
        private IDispatcherTimer? _yenilemeTimer;

        public BaristaPage()
        {
            InitializeComponent();
        }

        // ==============================================================================
        // SAYFA HER GÖRÜNDÜĞÜNDe VERİLERİ YÜKLE VE TIMER BAŞLAT
        // ==============================================================================
        protected override void OnAppearing()
        {
            base.OnAppearing();
            BekleyenSiparisleriYukle();
            TimerBaslat();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            TimerDurdur();
        }

        // ==============================================================================
        // OTOMATİK YENİLEME TIMER'I (Her 15 saniyede bir)
        // ==============================================================================
        private void TimerBaslat()
        {
            _yenilemeTimer = Dispatcher.CreateTimer();
            _yenilemeTimer.Interval = TimeSpan.FromSeconds(15);
            _yenilemeTimer.Tick += (s, e) => BekleyenSiparisleriYukle();
            _yenilemeTimer.Start();
        }

        private void TimerDurdur()
        {
            _yenilemeTimer?.Stop();
            _yenilemeTimer = null;
        }

        // ==============================================================================
        // BEKLEYEN SİPARİŞLERİ VERİTABANINDAN YÜKLEME
        // ==============================================================================
        private void BekleyenSiparisleriYukle()
        {
            var detaylar = _siparisYonetimi.AcikSiparisDetaylariniGetir();
            BekleyenUrunlerListesi.ItemsSource = detaylar;
            BekleyenSayisiLabel.Text = $"Bekleyen: {detaylar.Count} ürün";

            // Yeni sipariş göstergesi
            YeniSiparisIndicator.IsVisible = detaylar.Count > 0;
        }

        // ==============================================================================
        // "HAZIRLANDI" BUTONUNA TIKLANDIĞINDA
        // ==============================================================================
        private async void OnHazirlandiClicked(object sender, EventArgs e)
        {
            var buton = sender as Button;
            if (buton == null) return;

            var detay = buton.BindingContext as SiparisDetay;
            if (detay == null) return;

            bool onay = await DisplayAlert("Onay", 
                $"{detay.UrunAdi} ({detay.Adet}x) hazırlandı olarak işaretlenecek.\nOnaylıyor musunuz?", 
                "Evet", "Hayır");

            if (onay)
            {
                // Ürünü hazır olarak işaretle (silmiyoruz, masa fiyatı korunsun)
                string mesaj = _siparisYonetimi.SiparisDetayHazirIsaretle(detay.SiparisDetayId);

                if (mesaj.StartsWith("Başarılı"))
                {
                    // Listeyi yenile
                    BekleyenSiparisleriYukle();
                }
                else
                {
                    await DisplayAlert("Hata", mesaj, "Tamam");
                }
            }
        }

        // ==============================================================================
        // MANUEL YENİLEME
        // ==============================================================================
        private void OnYenileClicked(object sender, EventArgs e)
        {
            BekleyenSiparisleriYukle();
        }

        // ==============================================================================
        // ÇIKIŞ YAP
        // ==============================================================================
        private async void OnCikisClicked(object sender, EventArgs e)
        {
            bool cikisOnay = await DisplayAlert("Çıkış", "Oturumu kapatmak istediğinize emin misiniz?", "Evet", "Hayır");
            if (cikisOnay)
            {
                TimerDurdur();
                Application.Current!.MainPage = new LoginPage();
            }
        }
    }
}
