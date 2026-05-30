using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    public partial class GarsonPage : ContentPage
    {
        // İş katmanımızı (BL) çağırıyoruz
        private MasaYonetimi _masaYonetimi = new MasaYonetimi();

        public GarsonPage()
        {
            InitializeComponent();
            HosgeldinLabel.Text = $"Hoş geldin, {App.AktifPersonelAdi}";
        }

        // ==============================================================================
        // SAYFA HER GÖRÜNDÜĞÜNDe MASALARI YENİDEN YÜKLE
        // (Sipariş sonrası geri dönüldüğünde veri güncellensin)
        // ==============================================================================
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MasalariEkranaYukle();
        }

        private void MasalariEkranaYukle()
        {
            // BL'den temiz nesne listesini alıp XAML'daki MasalarListesi'ne bağlıyoruz
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