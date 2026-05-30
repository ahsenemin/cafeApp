using CafeApp.UI; // UI klasörünü tanıttık

namespace CafeApp
{
    public partial class App : Application
    {
        // Tüm sayfalardan (Sipariş alırken vb.) ulaşabileceğimiz Global Değişkenler
        public static int AktifGirisYapanPersonelId { get; set; }
        public static int AktifPersonelRolId { get; set; }
        public static string AktifPersonelAdi { get; set; } = "";

        public App()
        {
            InitializeComponent();

            // Varsayılan MainPage.xaml yerine, uygulamayı BİZİM yazdığımız LoginPage ile başlatıyoruz!
            MainPage = new LoginPage(); 
        }
    }
}