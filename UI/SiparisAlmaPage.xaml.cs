using System.Collections.ObjectModel;
using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    public partial class SiparisAlmaPage : ContentPage
    {
        // 1. Sepetteki ürünleri tutacağımız ve ekranı otomatik güncelleyen listemiz
        public ObservableCollection<SepetKalemi> Sepet { get; set; } = new ObservableCollection<SepetKalemi>();

        private int _seciliMasaId; // Hangi masaya sipariş alıyoruz?
        private decimal _genelToplam = 0;

        // İş Katmanı Sınıfları
        private UrunYonetimi _urunYonetimi = new UrunYonetimi();
        private KategoriYonetimi _kategoriYonetimi = new KategoriYonetimi();

        // Sayfa açılırken hangi masa için açıldığını parametre olarak alıyoruz
        public SiparisAlmaPage(int masaId)
        {
            InitializeComponent();
            _seciliMasaId = masaId;

            // XAML'daki SepetListesi'nin veri kaynağını bizim Gözlemlenebilir Listemize bağlıyoruz
            SepetListesi.ItemsSource = Sepet;

            // Kategorileri veritabanından yükle
            KategorileriYukle();

            // Tüm ürünleri veritabanından yükle
            UrunleriVeritabanindanYukle();
        }

        // ==============================================================================
        // KATEGORİLERİ VERİTABANINDAN YÜKLEME (Dinamik Butonlar)
        // ==============================================================================
        private void KategorileriYukle()
        {
            var kategoriler = _kategoriYonetimi.TumKategorileriGetir();

            // Önce "Tümü" butonu ekle
            var tumuButon = new Button
            {
                Text = "Tümü",
                BackgroundColor = Color.FromArgb("#E94560"),
                TextColor = Colors.White,
                CornerRadius = 20,
                FontSize = 13,
                Padding = new Thickness(16, 0),
                HeightRequest = 38
            };
            tumuButon.Clicked += (s, e) =>
            {
                UrunleriVeritabanindanYukle();
                KategoriButonRengiGuncelle(tumuButon);
            };
            KategoriListesi.Children.Add(tumuButon);

            // Veritabanından gelen kategorileri buton olarak ekle
            foreach (var kategori in kategoriler)
            {
                var buton = new Button
                {
                    Text = kategori.KAdi,
                    BackgroundColor = Color.FromArgb("#16213E"),
                    TextColor = Colors.White,
                    CornerRadius = 20,
                    FontSize = 13,
                    Padding = new Thickness(16, 0),
                    HeightRequest = 38
                };

                // Her butona kendi kategorisinin ID'sini bind ediyoruz
                int kategoriId = kategori.KategoriId;
                buton.Clicked += (s, e) =>
                {
                    KategoriyeGoreFiltrele(kategoriId);
                    KategoriButonRengiGuncelle(buton);
                };

                KategoriListesi.Children.Add(buton);
            }
        }

        // ==============================================================================
        // AKTİF KATEGORİ BUTONUNU VURGULAMA
        // ==============================================================================
        private void KategoriButonRengiGuncelle(Button aktifButon)
        {
            foreach (var child in KategoriListesi.Children)
            {
                if (child is Button btn)
                {
                    btn.BackgroundColor = Color.FromArgb("#16213E");
                }
            }
            aktifButon.BackgroundColor = Color.FromArgb("#E94560");
        }

        // ==============================================================================
        // TÜM ÜRÜNLERİ VERİTABANINDAN YÜKLEME
        // ==============================================================================
        private void UrunleriVeritabanindanYukle()
        {
            UrunlerListesi.ItemsSource = _urunYonetimi.TumUrunleriGetir();
        }

        // ==============================================================================
        // KATEGORİYE GÖRE FİLTRELEME
        // ==============================================================================
        private void KategoriyeGoreFiltrele(int kategoriId)
        {
            UrunlerListesi.ItemsSource = _urunYonetimi.KategoriyeGoreUrunGetir(kategoriId);
        }

        // ==============================================================================
        // "EKLE +" BUTONUNA TIKLANDIĞINDA ÇALIŞACAK KOD
        // ==============================================================================
        private void OnUrunEkleClicked(object sender, EventArgs e)
        {
            // Tıklanan butonu yakalıyoruz
            var buton = sender as Button;
            if (buton == null) return;

            // Butonun içindeki (BindingContext) Urun verisini alıyoruz
            var secilenUrun = buton.BindingContext as Urun;

            if (secilenUrun != null)
            {
                // Sepette bu ürün daha önce eklenmiş mi diye bakıyoruz
                var sepettekiUrun = Sepet.FirstOrDefault(u => u.UrunId == secilenUrun.UrunId);

                if (sepettekiUrun != null)
                {
                    // Varsa sadece adedini 1 artırıyoruz
                    sepettekiUrun.Adet++;
                }
                else
                {
                    // Yoksa sepete yeni bir satır olarak ekliyoruz
                    Sepet.Add(new SepetKalemi
                    {
                        UrunId = secilenUrun.UrunId,
                        UrunAdi = secilenUrun.UrunAdi,
                        Fiyat = secilenUrun.UrunFiyati,
                        Adet = 1
                    });
                }

                // Toplam fiyatı güncelle
                ToplamFiyatiHesapla();
            }
        }

        // ==============================================================================
        // SEPETTEN ÜRÜN ÇIKARMA
        // ==============================================================================
        private void OnSepettenCikarClicked(object sender, EventArgs e)
        {
            var buton = sender as Button;
            if (buton == null) return;

            var sepetKalemi = buton.BindingContext as SepetKalemi;
            if (sepetKalemi != null)
            {
                if (sepetKalemi.Adet > 1)
                {
                    sepetKalemi.Adet--;
                }
                else
                {
                    Sepet.Remove(sepetKalemi);
                }
                ToplamFiyatiHesapla();
            }
        }

        // ==============================================================================
        // TOPLAM FİYATI HESAPLAMA VE EKRANA YAZDIRMA
        // ==============================================================================
        private void ToplamFiyatiHesapla()
        {
            _genelToplam = 0;
            foreach (var item in Sepet)
            {
                _genelToplam += item.ToplamTutar;
            }

            // XAML'daki Label'ın metnini güncelliyoruz
            SepetToplamLabel.Text = $"{_genelToplam:C2}";
        }

        // ==============================================================================
        // "SİPARİŞİ ONAYLA" BUTONUNA TIKLANDIĞINDA ÇALIŞACAK KOD
        // ==============================================================================
        private async void OnSiparisiOnaylaClicked(object sender, EventArgs e)
        {
            if (Sepet.Count == 0)
            {
                await DisplayAlert("Uyarı", "Sepet boş, onaylanacak bir şey yok!", "Tamam");
                return;
            }

            // İş Katmanını çağırıyoruz
            MasaYonetimi masaYonetimi = new MasaYonetimi();
            
            // Giriş yapan personelin ID'sini kullanıyoruz
            int personelId = App.AktifGirisYapanPersonelId;

            // Siparişi kaydetmesi için BL'ye gönderiyoruz
            string mesaj = masaYonetimi.YeniSiparisOlustur(_seciliMasaId, personelId, Sepet);

            if (mesaj.StartsWith("Başarılı"))
            {
                await DisplayAlert("Başarılı", mesaj, "Harika");
                
                // Sipariş alındıktan sonra sepeti temizleyip garsonu önceki sayfaya (Masalar) geri gönderiyoruz
                Sepet.Clear();
                _genelToplam = 0;
                SepetToplamLabel.Text = "0,00 ₺";
                await Navigation.PopAsync();
            }
            else
            {
                // Eğer BL'den "Hata" ile başlayan bir mesaj dönerse ekranda gösteriyoruz
                await DisplayAlert("Hata Oluştu", mesaj, "Tamam");
            }
        }
    }

    // ==============================================================================
    // SEPET İÇİN GEÇİCİ MODEL SINIFI (Arayüzde kullanmak için)
    // INotifyPropertyChanged arayüzü sayesinde "Adet" değiştiğinde XAML otomatik haberdar olur
    // ==============================================================================
    public class SepetKalemi : BindableObject
    {
        public int UrunId { get; set; }
        public string? UrunAdi { get; set; }
        public decimal Fiyat { get; set; }

        private int _adet;
        public int Adet
        {
            get => _adet;
            set
            {
                _adet = value;
                OnPropertyChanged(nameof(Adet));
                OnPropertyChanged(nameof(ToplamTutar)); // Adet değişirse Toplam Tutar da değişir
            }
        }

        public decimal ToplamTutar => Adet * Fiyat;
    }
}