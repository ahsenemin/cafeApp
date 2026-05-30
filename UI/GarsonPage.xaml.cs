using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    public partial class GarsonPage : ContentPage
    {
        private MasaYonetimi _masaYonetimi = new MasaYonetimi();
        private BildirimYonetimi _bildirimYonetimi = new BildirimYonetimi();
        private IDispatcherTimer? _timer;

        public GarsonPage()
        {
            InitializeComponent();
            HosgeldinLabel.Text = $"Hoş geldin, {App.AktifPersonelAdi}";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MasalariEkranaYukle();
            TimerBaslat();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _timer?.Stop();
        }

        private void TimerBaslat()
        {
            _timer?.Stop();
            _timer = Application.Current!.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(10);
            _timer.Tick += (s, e) => BildirimleriKontrolEt();
            _timer.Start();
        }

        private void BildirimleriKontrolEt()
        {
            try
            {
                var hazirlar = _bildirimYonetimi.HazirBekleyenleriGetir();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (hazirlar.Count > 0)
                    {
                        BildirimLabel.Text = $"🔔 {hazirlar.Count} ürün hazır! Tıkla →";
                        BildirimFrame.IsVisible = true;
                        MasalariEkranaYukle();
                    }
                    else
                    {
                        BildirimFrame.IsVisible = false;
                    }
                });
            }
            catch { }
        }

        private async void OnBildirimTapped(object sender, TappedEventArgs e)
        {
            var hazirlar = _bildirimYonetimi.HazirBekleyenleriGetir();
            if (hazirlar.Count == 0) return;

            // Tüm hazır ürünleri listele
            string mesaj = string.Join("\n", hazirlar.Select(h => $"• {h.MasaNo} — {h.Adet}x {h.UrunAdi}"));

            bool tamam = await DisplayAlert("🔔 Hazır Siparişler", mesaj, "Gördüm ✓", "Kapat");

            if (tamam)
            {
                foreach (var b in hazirlar)
                    _bildirimYonetimi.GorulduIsaretle(b.SiparisDetayId);

                BildirimFrame.IsVisible = false;
            }
        }

        private void MasalariEkranaYukle()
        {
            MasalarListesi.ItemsSource = _masaYonetimi.TumMasalariGetir();
        }

        // ==============================================================================
        // MASAYA TIKLANDIĞINDA ÇALIŞACAK KOD
        // ==============================================================================
        private async void OnMasaTapped(object sender, TappedEventArgs e)
        {
            // Tıklanan kutuyu (Frame) yakalıyoruz
            var frame = sender as Frame;
            if (frame == null) return;

            // Kutunun içindeki Masa modelini (Verisini) alıyoruz
            var secilenMasa = frame.BindingContext as Masa;
            if (secilenMasa == null) return;

            switch (secilenMasa.Durum)
            {
                case 1: // BOŞ MASA → Direkt sipariş alma sayfasına git
                    await Navigation.PushAsync(new SiparisAlmaPage(secilenMasa.MasaId));
                    break;

                case 2: // DOLU MASA → Seçenek göster
                    string secim = await DisplayActionSheet(
                        $"{secilenMasa.MasaNo} — {secilenMasa.MasaToplamFiyati:C2}",
                        "İptal", null,
                        "➕ Yeni Sipariş Ekle",
                        "💳 Ödeme Al");

                    if (secim == "➕ Yeni Sipariş Ekle")
                    {
                        await Navigation.PushAsync(new SiparisAlmaPage(secilenMasa.MasaId));
                    }
                    else if (secim == "💳 Ödeme Al")
                    {
                        await Navigation.PushAsync(new OdemePage(secilenMasa.MasaId, secilenMasa.MasaNo ?? "Masa", secilenMasa.MasaToplamFiyati));
                    }
                    break;

                case 3: // REZERVE MASA → Seçenek göster
                    string rezSecim = await DisplayActionSheet(
                        $"{secilenMasa.MasaNo} — Rezerve",
                        "İptal", null,
                        "➕ Sipariş Al (Masa Aç)",
                        "🟢 Boş Olarak İşaretle");

                    if (rezSecim == "➕ Sipariş Al (Masa Aç)")
                    {
                        await Navigation.PushAsync(new SiparisAlmaPage(secilenMasa.MasaId));
                    }
                    else if (rezSecim == "🟢 Boş Olarak İşaretle")
                    {
                        _masaYonetimi.MasaDurumGuncelle(secilenMasa.MasaId, 1);
                        MasalariEkranaYukle();
                    }
                    break;
            }
        }

        // ==============================================================================
        // ÇIKIŞ YAP
        // ==============================================================================
        private async void OnCikisClicked(object sender, EventArgs e)
        {
            bool cikisOnay = await DisplayAlert("Çıkış", "Oturumu kapatmak istediğinize emin misiniz?", "Evet", "Hayır");
            if (cikisOnay)
            {
                Application.Current!.MainPage = new LoginPage();
            }
        }
    }
}