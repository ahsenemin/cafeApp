using CafeApp.BL;
using CafeApp.Models;

namespace CafeApp.UI
{
    public partial class PersonelYonetimPage : ContentPage
    {
        private PersonelYonetimi _personelYonetimi = new PersonelYonetimi();
        private Personel? _duzenlenecekPersonel = null;
        private int _secilenRolId = -1;

        // Rol listesi sabit — veritabanındaki rol_id değerleriyle eşleşmeli
        private readonly List<(int Id, string Ad)> _roller = new()
        {
            (100, "Garson"),
            (101, "Yönetici"),
            (102, "Barista")
        };

        public PersonelYonetimPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PersonelleriYukle();
        }

        private void PersonelleriYukle()
        {
            PersonelListesi.ItemsSource = _personelYonetimi.TumPersonelleriGetir();
        }

        // ==============================================================================
        // ROL SEÇİMİ
        // ==============================================================================
        private async void OnRolSecClicked(object sender, EventArgs e)
        {
            var secenekler = _roller.Select(r => r.Ad).ToArray();
            string? secilen = await DisplayActionSheet("Rol Seçin", "İptal", null, secenekler);

            if (secilen == null || secilen == "İptal") return;

            var rol = _roller.FirstOrDefault(r => r.Ad == secilen);
            _secilenRolId = rol.Id;
            RolSecButon.Text = secilen;
            RolSecButon.TextColor = Colors.White;
        }

        // ==============================================================================
        // KAYDET
        // ==============================================================================
        private async void OnKaydetClicked(object sender, EventArgs e)
        {
            string adi    = AdiEntry.Text?.Trim() ?? "";
            string soyadi = SoyadiEntry.Text?.Trim() ?? "";
            string eposta = EpostaEntry.Text?.Trim() ?? "";
            string tel    = TelEntry.Text?.Trim() ?? "";
            string tc     = TcEntry.Text?.Trim() ?? "";
            string adres  = AdresEntry.Text?.Trim() ?? "";

            if (_secilenRolId < 0)
            {
                await DisplayAlert("Uyarı", "Lütfen bir rol seçin!", "Tamam");
                return;
            }

            string mesaj;
            if (_duzenlenecekPersonel != null)
            {
                mesaj = _personelYonetimi.PersonelGuncelle(
                    _duzenlenecekPersonel.PersonelId, adi, soyadi, eposta, adres, tel, tc, 1, _secilenRolId);
            }
            else
            {
                mesaj = _personelYonetimi.PersonelEkle(adi, soyadi, eposta, adres, tel, tc, 1, _secilenRolId);
            }

            await DisplayAlert(mesaj.StartsWith("Başarılı") ? "Başarılı" : "Hata", mesaj, "Tamam");

            if (mesaj.StartsWith("Başarılı"))
            {
                FormuTemizle();
                PersonelleriYukle();
            }
        }

        // ==============================================================================
        // DÜZENLEME
        // ==============================================================================
        private void OnDuzenleClicked(object sender, EventArgs e)
        {
            var buton = sender as Button;
            var personel = buton?.BindingContext as Personel;
            if (personel == null) return;

            _duzenlenecekPersonel = personel;
            AdiEntry.Text    = personel.Ad;
            SoyadiEntry.Text = personel.Soyad;
            EpostaEntry.Text = personel.Eposta;
            TelEntry.Text    = personel.Telefon;
            TcEntry.Text     = personel.TcKimlik;
            AdresEntry.Text  = personel.Adres;

            var rol = _roller.FirstOrDefault(r => r.Id == personel.RolId);
            _secilenRolId = rol.Id;
            RolSecButon.Text = rol.Ad ?? "Rol seçin...";
            RolSecButon.TextColor = Colors.White;

            FormBaslikLabel.Text = $"✏️ \"{personel.TamAd}\" Düzenleniyor";
            KaydetButon.Text = "💾 Güncelle";
            IptalButon.IsVisible = true;
        }

        // ==============================================================================
        // SİLME
        // ==============================================================================
        private async void OnSilClicked(object sender, EventArgs e)
        {
            var buton = sender as Button;
            var personel = buton?.BindingContext as Personel;
            if (personel == null) return;

            bool onay = await DisplayAlert("Silme Onayı",
                $"\"{personel.TamAd}\" silinecek. Devam etmek istiyor musunuz?",
                "Sil", "İptal");

            if (onay)
            {
                string mesaj = _personelYonetimi.PersonelSil(personel.PersonelId);
                await DisplayAlert(mesaj.StartsWith("Başarılı") ? "Başarılı" : "Hata", mesaj, "Tamam");
                if (mesaj.StartsWith("Başarılı")) PersonelleriYukle();
            }
        }

        // ==============================================================================
        // İPTAL
        // ==============================================================================
        private void OnIptalClicked(object sender, EventArgs e) => FormuTemizle();

        private void FormuTemizle()
        {
            _duzenlenecekPersonel = null;
            _secilenRolId = -1;
            AdiEntry.Text = SoyadiEntry.Text = EpostaEntry.Text =
            TelEntry.Text = TcEntry.Text = AdresEntry.Text = "";
            RolSecButon.Text = "Rol seçin...";
            RolSecButon.TextColor = Color.FromArgb("#533483");
            FormBaslikLabel.Text = "Yeni Personel Ekle";
            KaydetButon.Text = "💾 Kaydet";
            IptalButon.IsVisible = false;
        }
    }
}
