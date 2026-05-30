using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    public partial class OdemePage : ContentPage
    {
        private SiparisYonetimi _siparisYonetimi = new SiparisYonetimi();
        private OdemeYonetimi _odemeYonetimi = new OdemeYonetimi();
        private MasaYonetimi _masaYonetimi = new MasaYonetimi();

        private int _masaId;
        private string _masaNo;
        private decimal _masaToplamFiyati;
        private string _secilenOdemeYontemi = "";

        public OdemePage(int masaId, string masaNo, decimal masaToplamFiyati)
        {
            InitializeComponent();
            _masaId = masaId;
            _masaNo = masaNo;
            _masaToplamFiyati = masaToplamFiyati;

            MasaBaslikLabel.Text = $"💳 {masaNo} — Ödeme";

            SiparisDetaylariniYukle();
            FiyatBilgileriniHesapla();
            DoluSuresiniGoster();
        }

        // ==============================================================================
        // SİPARİŞ DETAYLARINI YÜKLEME
        // ==============================================================================
        private void SiparisDetaylariniYukle()
        {
            // Masanın açık siparişlerini bul
            var acikSiparisler = _siparisYonetimi.MasaninAcikSiparisleriniGetir(_masaId);

            // Tüm açık siparişlerin detaylarını tek listede topla
            List<SiparisDetay> tumDetaylar = new List<SiparisDetay>();
            foreach (var siparis in acikSiparisler)
            {
                var detaylar = _siparisYonetimi.SiparisDetayGetir(siparis.SiparisId);
                tumDetaylar.AddRange(detaylar);
            }

            SiparisDetayListesi.ItemsSource = tumDetaylar;
        }

        // ==============================================================================
        // FİYAT BİLGİLERİNİ HESAPLAMA (Ara Toplam, KDV, Genel Toplam)
        // ==============================================================================
        private void FiyatBilgileriniHesapla()
        {
            decimal araToplam = _masaToplamFiyati;
            decimal kdv = araToplam * 0.20m;
            decimal genelToplam = araToplam + kdv;

            AraToplamLabel.Text = $"{araToplam:C2}";
            KDVLabel.Text = $"{kdv:C2}";
            GenelToplamLabel.Text = $"{genelToplam:C2}";
        }

        // ==============================================================================
        // MASA DOLU SÜRESİNİ GÖSTERME
        // ==============================================================================
        private void DoluSuresiniGoster()
        {
            int dakika = _siparisYonetimi.MasaDoluSuresiGetir(_masaId);
            if (dakika > 0)
            {
                int saat = dakika / 60;
                int kalanDakika = dakika % 60;
                DoluSuresiLabel.Text = saat > 0 
                    ? $"⏱ Masa {saat} saat {kalanDakika} dakikadır dolu" 
                    : $"⏱ Masa {kalanDakika} dakikadır dolu";
            }
            else
            {
                DoluSuresiLabel.Text = "";
            }
        }

        // ==============================================================================
        // ÖDEME YÖNTEMİ SEÇİMİ
        // ==============================================================================
        private void OnNakitSecildi(object sender, EventArgs e)
        {
            _secilenOdemeYontemi = "Nakit";
            NakitButon.BackgroundColor = Color.FromArgb("#E94560");
            KartButon.BackgroundColor = Color.FromArgb("#0F3460");
            OdemeyiTamamlaButon.IsEnabled = true;
        }

        private void OnKartSecildi(object sender, EventArgs e)
        {
            _secilenOdemeYontemi = "Kredi Kartı";
            KartButon.BackgroundColor = Color.FromArgb("#E94560");
            NakitButon.BackgroundColor = Color.FromArgb("#0F3460");
            OdemeyiTamamlaButon.IsEnabled = true;
        }

        // ==============================================================================
        // ÖDEMEYİ TAMAMLA
        // ==============================================================================
        private async void OnOdemeyiTamamlaClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_secilenOdemeYontemi))
            {
                await DisplayAlert("Uyarı", "Lütfen bir ödeme yöntemi seçin!", "Tamam");
                return;
            }

            decimal genelToplam = _masaToplamFiyati * 1.20m; // KDV dahil

            bool onay = await DisplayAlert("Ödeme Onayı",
                $"Toplam: {genelToplam:C2}\nYöntem: {_secilenOdemeYontemi}\n\nÖdemeyi onaylıyor musunuz?",
                "Onayla", "İptal");

            if (!onay) return;

            // İş Katmanına gönder: Ödeme al + Sipariş kapat + Masa sıfırla
            string mesaj = _odemeYonetimi.MasaOdemesiAl(_masaId, _secilenOdemeYontemi, genelToplam);

            if (mesaj.StartsWith("Başarılı"))
            {
                await DisplayAlert("Başarılı", $"✅ {_masaNo} ödemesi alındı!\n\n{genelToplam:C2} — {_secilenOdemeYontemi}", "Tamam");
                await Navigation.PopAsync(); // Garson sayfasına dön
            }
            else
            {
                await DisplayAlert("Hata", mesaj, "Tamam");
            }
        }
    }
}
