using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    public partial class LoginPage : ContentPage
    {
        private PersonelYonetimi _personelYonetimi = new PersonelYonetimi();
        private Personel? _secilenPersonel = null;

        public LoginPage()
        {
            InitializeComponent();
            
            // Sayfa açıldığında aktif personelleri veritabanından çekip listeye dolduruyoruz
            PersonelListesi.ItemsSource = _personelYonetimi.AktifPersonelleriGetir();
        }

        // ==============================================================================
        // PERSONEL SEÇİLDİĞİNDE (CollectionView SelectionChanged)
        // ==============================================================================
        private void OnPersonelSecildi(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                _secilenPersonel = e.CurrentSelection[0] as Personel;
                
                if (_secilenPersonel != null)
                {
                    SecimLabel.Text = $"✅ {_secilenPersonel.TamAd} ({_secilenPersonel.RolMetni}) seçildi";
                    SecimBorder.IsVisible = true;
                    GirisButonu.IsEnabled = true;
                    GirisButonu.Opacity = 1.0;
                }
            }
        }

        // ==============================================================================
        // GİRİŞ YAP BUTONUNA TIKLANDIĞINDA
        // ==============================================================================
        private async void OnGirisYapClicked(object sender, EventArgs e)
        {
            // Eğer kimse seçilmediyse uyarı ver
            if (_secilenPersonel == null)
            {
                await DisplayAlert("Uyarı", "Lütfen listeden isminizi seçin!", "Tamam");
                return;
            }

            // Giriş yapan personelin bilgilerini global değişkenlere kaydet
            App.AktifGirisYapanPersonelId = _secilenPersonel.PersonelId;
            App.AktifPersonelRolId = _secilenPersonel.RolId;
            App.AktifPersonelAdi = _secilenPersonel.TamAd;

            // ROL BAZLI YÖNLENDİRME
            // Veritabanındaki gerçek değerler: 100 = Garson, 101 = Yönetici, 102 = Barista
            switch (_secilenPersonel.RolId)
            {
                case 100: // Garson
                    Application.Current!.MainPage = new NavigationPage(new GarsonPage());
                    break;

                case 102: // Barista
                    Application.Current!.MainPage = new NavigationPage(new BaristaPage());
                    break;

                case 101: // Yönetici
                    Application.Current!.MainPage = new NavigationPage(new YoneticiPage());
                    break;

                default:
                    // Tanımlanmamış rol varsa garson ekranına yönlendir
                    Application.Current!.MainPage = new NavigationPage(new GarsonPage());
                    break;
            }
        }
    }
}