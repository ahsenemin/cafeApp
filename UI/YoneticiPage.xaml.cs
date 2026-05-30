using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    // Yönetici Panelinde Personel Satış Performansını göstermek için yardımcı model
    public class PersonelPerformans
    {
        public int PersonelId { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public decimal ToplamSatis { get; set; }

        public string TamAd => $"{Ad} {Soyad}";
        public string DurumMetni => ToplamSatis > 0 ? "Aktif Satışı Var" : "Henüz Satış Yok";
    }

    public partial class YoneticiPage : ContentPage
    {
        private OdemeYonetimi _odemeYonetimi = new OdemeYonetimi();
        private MasaYonetimi _masaYonetimi = new MasaYonetimi();
        private PersonelYonetimi _personelYonetimi = new PersonelYonetimi();

        public YoneticiPage()
        {
            InitializeComponent();
            HosgeldinLabel.Text = $"Hoş geldin, {App.AktifPersonelAdi}";
        }

        // ==============================================================================
        // SAYFA HER GÖRÜNDÜĞÜNDe VERİLERİ YÜKLE
        // ==============================================================================
        protected override void OnAppearing()
        {
            base.OnAppearing();
            VerileriYukle();
        }

        private void VerileriYukle()
        {
            GunlukOzetiYukle();
            OdemeYontemiDagilimi();
            PersonelPerformansiniYukle();
            DoluMasaSayisiniGoster();
        }

        // ==============================================================================
        // GÜNLÜK ÖZET İSTATİSTİKLERİ
        // ==============================================================================
        private void GunlukOzetiYukle()
        {
            DataTable rapor = _odemeYonetimi.GunlukRapor();

            decimal toplamCiro = 0;
            int toplamIslem = 0;

            foreach (DataRow row in rapor.Rows)
            {
                toplamCiro += Convert.ToDecimal(row["toplam_tutar"]);
                toplamIslem += Convert.ToInt32(row["islem_sayisi"]);
            }

            GunlukCiroLabel.Text = $"{toplamCiro:C2}";
            GunlukIslemLabel.Text = toplamIslem.ToString();
        }

        // ==============================================================================
        // ÖDEME YÖNTEMİ DAĞILIMI
        // ==============================================================================
        private void OdemeYontemiDagilimi()
        {
            var odemeler = _odemeYonetimi.TumOdemeleriGetir();

            decimal nakitToplam = odemeler.Where(o => o.OdemeSekli == "Nakit").Sum(o => o.OdenenMiktar);
            decimal kartToplam = odemeler.Where(o => o.OdemeSekli == "Kredi Kartı").Sum(o => o.OdenenMiktar);

            NakitToplamLabel.Text = $"{nakitToplam:C2}";
            KartToplamLabel.Text = $"{kartToplam:C2}";
        }

        // ==============================================================================
        // PERSONEL PERFORMANSI
        // ==============================================================================
        private void PersonelPerformansiniYukle()
        {
            DataTable rapor = _odemeYonetimi.PersonelRaporu();

            var performansListesi = new List<PersonelPerformans>();
            foreach (DataRow row in rapor.Rows)
            {
                performansListesi.Add(new PersonelPerformans
                {
                    PersonelId = Convert.ToInt32(row["personel_id"]),
                    Ad = row["p_adi"]?.ToString(),
                    Soyad = row["p_soyadi"]?.ToString(),
                    ToplamSatis = Convert.ToDecimal(row["toplam_satis"])
                });
            }

            PersonelRaporListesi.ItemsSource = performansListesi;
        }

        // ==============================================================================
        // DOLU MASA SAYISI
        // ==============================================================================
        private void DoluMasaSayisiniGoster()
        {
            var masalar = _masaYonetimi.TumMasalariGetir();
            int doluSayisi = masalar.Count(m => m.Durum == 2);
            DoluMasaLabel.Text = doluSayisi.ToString();
        }

        // ==============================================================================
        // HIZLI ERİŞİM BUTONLARI
        // ==============================================================================
        private async void OnMasalariGorClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GarsonPage());
        }

        private async void OnMutfagiGorClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BaristaPage());
        }

        private void OnVerileriYenileClicked(object sender, EventArgs e)
        {
            VerileriYukle();
        }

        private async void OnTumSiparisleriGorClicked(object sender, EventArgs e)
        {
            // Basit bir sipariş listesi gösterimi
            var siparisYonetimi = new SiparisYonetimi();
            var acikDetaylar = siparisYonetimi.AcikSiparisDetaylariniGetir();

            if (acikDetaylar.Count == 0)
            {
                await DisplayAlert("Bilgi", "Şu anda açık sipariş bulunmamaktadır.", "Tamam");
            }
            else
            {
                string ozet = "";
                foreach (var detay in acikDetaylar)
                {
                    ozet += $"📍 {detay.MasaNo} — {detay.UrunAdi} x{detay.Adet} ({detay.ToplamTutar:C2})\n";
                }
                await DisplayAlert($"Açık Siparişler ({acikDetaylar.Count} ürün)", ozet, "Tamam");
            }
        }

        // ==============================================================================
        // YÖNETİM İŞLEMLERİ
        // ==============================================================================
        private async void OnKategoriYonetimClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new KategoriYonetimPage());
        }

        private async void OnUrunYonetimClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UrunYonetimPage());
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
