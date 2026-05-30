using MySqlConnector;
using System.Data;
using System;

namespace CafeApp.DAL
{
    public class CafeVeriErisim
    {
        // Android emülatörde 127.0.0.1 emülatörün kendisini gösterir.
        // Mac'teki MySQL'e ulaşmak için 10.0.2.2 kullanılır.
        private readonly string _connString;

        public CafeVeriErisim()
        {
#if ANDROID
            string server = "10.0.2.2";
#else
            string server = "127.0.0.1";
#endif
            _connString = $"Server={server};Port=3307;Database=CafeDB;Uid=root;Pwd=;";
        }

        #region MERKEZİ YARDIMCI METOTLAR (Kod Tekrarını Önler)

        // Ekleme, Güncelleme ve Silme işlemleri için ortak çalıştırıcı metot
        private bool ExecuteProcedure(string procName, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(_connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null) cmd.Parameters.AddRange(parameters);

                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DAL Hatası ({procName}): {ex.Message}");
                        return false;
                    }
                }
            }
        }

        // Listeleme (Select) işlemleri için ortak çalıştırıcı metot
        private DataTable GetDataTable(string procName, params MySqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(_connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null) cmd.Parameters.AddRange(parameters);

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DAL Hatası ({procName}): {ex.Message}");
                    }
                }
            }
            return dt;
        }

        // Tek değer döndüren sorgular için ortak çalıştırıcı metot (Fonksiyon wrapper'ları için)
        private object? ExecuteScalarProcedure(string procName, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(_connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null) cmd.Parameters.AddRange(parameters);

                    try
                    {
                        conn.Open();
                        return cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DAL Hatası ({procName}): {ex.Message}");
                        return null;
                    }
                }
            }
        }
        #endregion

        #region 1. ROL İŞLEMLERİ
        public bool RolEkle(string rolAdi, byte aktifMi) =>
            ExecuteProcedure("sp_RolEkle", new MySqlParameter("p_rol_adi", rolAdi), new MySqlParameter("p_aktif_mi", aktifMi));

        public bool RolGuncelle(int rolId, string rolAdi, byte aktifMi) =>
            ExecuteProcedure("sp_RolGuncelle", new MySqlParameter("p_rol_id", rolId), new MySqlParameter("p_rol_adi", rolAdi), new MySqlParameter("p_aktif_mi", aktifMi));

        public bool RolSil(int rolId) =>
            ExecuteProcedure("sp_RolSil", new MySqlParameter("p_rol_id", rolId));

        public DataTable RolListele() => GetDataTable("sp_RolListele");
        #endregion

        #region 2. KATEGORİ İŞLEMLERİ
        public bool KategoriEkle(string kAdi) =>
            ExecuteProcedure("sp_KategoriEkle", new MySqlParameter("p_k_adi", kAdi));

        public bool KategoriGuncelle(int kategoriId, string kAdi) =>
            ExecuteProcedure("sp_KategoriGuncelle", new MySqlParameter("p_kategori_id", kategoriId), new MySqlParameter("p_k_adi", kAdi));

        public bool KategoriSil(int kategoriId) =>
            ExecuteProcedure("sp_KategoriSil", new MySqlParameter("p_kategori_id", kategoriId));

        public DataTable KategoriListele() => GetDataTable("sp_KategoriListele");
        #endregion

        #region 3. MASA İŞLEMLERİ
        public bool MasaEkle(string masaNo, byte durum) =>
            ExecuteProcedure("sp_MasaEkle", new MySqlParameter("p_masa_no", masaNo), new MySqlParameter("p_durum", durum));

        public bool MasaGuncelle(int masaId, string masaNo, byte durum, decimal masaToplamFiyati) =>
            ExecuteProcedure("sp_MasaGuncelle", new MySqlParameter("p_masa_id", masaId), new MySqlParameter("p_masa_no", masaNo), new MySqlParameter("p_durum", durum), new MySqlParameter("p_masa_toplam_fiyati", masaToplamFiyati));

        public bool MasaSil(int masaId) =>
            ExecuteProcedure("sp_MasaSil", new MySqlParameter("p_masa_id", masaId));

        public DataTable MasaListele() => GetDataTable("sp_MasaListele");

        // Sadece masa durumunu güncelle
        public bool MasaDurumGuncelle(int masaId, byte durum) =>
            ExecuteProcedure("sp_MasaDurumGuncelle", new MySqlParameter("p_masa_id", masaId), new MySqlParameter("p_durum", durum));

        // Masa sıfırla (durum=Boş, fiyat=0)
        public bool MasaSifirla(int masaId) =>
            ExecuteProcedure("sp_MasaSifirla", new MySqlParameter("p_masa_id", masaId));

        // Masa dolu süresi (dakika) — fn_MasaDoluSuresi wrapper
        public int MasaDoluSuresi(int masaId)
        {
            var result = ExecuteScalarProcedure("sp_MasaDoluSuresiGetir", new MySqlParameter("p_masa_id", masaId));
            return result != null ? Convert.ToInt32(result) : 0;
        }
        #endregion

        #region 4. ÜRÜN İŞLEMLERİ
        public bool UrunEkle(string urunAdi, decimal urunFiyati, int kategoriId) =>
            ExecuteProcedure("sp_UrunEkle", new MySqlParameter("p_urun_adi", urunAdi), new MySqlParameter("p_urun_fiyati", urunFiyati), new MySqlParameter("p_kategori_id", kategoriId));

        public bool UrunGuncelle(int urunId, string urunAdi, decimal urunFiyati, int kategoriId) =>
            ExecuteProcedure("sp_UrunGuncelle", new MySqlParameter("p_urun_id", urunId), new MySqlParameter("p_urun_adi", urunAdi), new MySqlParameter("p_urun_fiyati", urunFiyati), new MySqlParameter("p_kategori_id", kategoriId));

        public bool UrunSil(int urunId) =>
            ExecuteProcedure("sp_UrunSil", new MySqlParameter("p_urun_id", urunId));

        public DataTable UrunListele() => GetDataTable("sp_UrunListele");

        // Kategoriye göre ürün listesi
        public DataTable UrunListeleByKategori(int kategoriId) =>
            GetDataTable("sp_UrunListeleByKategori", new MySqlParameter("p_kategori_id", kategoriId));
        #endregion

        #region 5. PERSONEL İŞLEMLERİ
        public bool PersonelEkle(string adi, string soyadi, string eposta, string adres, string tel, string tc, byte aktifMi, int rolId) =>
            ExecuteProcedure("sp_PersonelEkle", new MySqlParameter("p_p_adi", adi), new MySqlParameter("p_p_soyadi", soyadi), new MySqlParameter("p_p_eposta", eposta), new MySqlParameter("p_p_adres", adres), new MySqlParameter("p_p_tel", tel), new MySqlParameter("p_p_tc", tc), new MySqlParameter("p_p_aktif_mi", aktifMi), new MySqlParameter("p_rol_id", rolId));

        public bool PersonelGuncelle(int personelId, string adi, string soyadi, string eposta, string adres, string tel, string tc, byte aktifMi, int rolId) =>
            ExecuteProcedure("sp_PersonelGuncelle", new MySqlParameter("p_personel_id", personelId), new MySqlParameter("p_p_adi", adi), new MySqlParameter("p_p_soyadi", soyadi), new MySqlParameter("p_p_eposta", eposta), new MySqlParameter("p_p_adres", adres), new MySqlParameter("p_p_tel", tel), new MySqlParameter("p_p_tc", tc), new MySqlParameter("p_p_aktif_mi", aktifMi), new MySqlParameter("p_rol_id", rolId));

        public bool PersonelSil(int personelId) =>
            ExecuteProcedure("sp_PersonelSil", new MySqlParameter("p_personel_id", personelId));

        public DataTable PersonelListele() => GetDataTable("sp_PersonelListele");

        // Personel toplam satış — fn_PersonelToplamSatis wrapper
        public decimal PersonelToplamSatis(int personelId)
        {
            var result = ExecuteScalarProcedure("sp_PersonelToplamSatisGetir", new MySqlParameter("p_personel_id", personelId));
            return result != null ? Convert.ToDecimal(result) : 0;
        }
        #endregion

        #region 6. SİPARİŞLER (ADİSYON) İŞLEMLERİ
        public bool SiparisEkle(int? masaId, int personelId, string durum) =>
            ExecuteProcedure("sp_SiparisEkle", new MySqlParameter("p_masa_id", masaId ?? (object)DBNull.Value), new MySqlParameter("p_personel_id", personelId), new MySqlParameter("p_durum", durum));

        public bool SiparisGuncelle(int siparisId, int? masaId, int personelId, string durum) =>
            ExecuteProcedure("sp_SiparisGuncelle", new MySqlParameter("p_siparis_id", siparisId), new MySqlParameter("p_masa_id", masaId ?? (object)DBNull.Value), new MySqlParameter("p_personel_id", personelId), new MySqlParameter("p_durum", durum));

        public bool SiparisSil(int siparisId) =>
            ExecuteProcedure("sp_SiparisSil", new MySqlParameter("p_siparis_id", siparisId));

        public DataTable SiparisListele() => GetDataTable("sp_SiparisListele");

        // Bir masanın açık siparişleri
        public DataTable SiparislerByMasa(int masaId) =>
            GetDataTable("sp_SiparislerByMasa", new MySqlParameter("p_masa_id", masaId));

        // Son eklenen sipariş ID'si
        public int SonSiparisIdGetir()
        {
            var result = ExecuteScalarProcedure("sp_SonSiparisIdGetir");
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // Sipariş kapatma
        public bool SiparisKapat(int siparisId) =>
            ExecuteProcedure("sp_SiparisKapat", new MySqlParameter("p_siparis_id", siparisId));

        // KDV'li tutar — fn_SiparisKDVliTutar wrapper
        public decimal SiparisKDVliTutar(int siparisId)
        {
            var result = ExecuteScalarProcedure("sp_SiparisKDVliTutarGetir", new MySqlParameter("p_siparis_id", siparisId));
            return result != null ? Convert.ToDecimal(result) : 0;
        }
        #endregion

        #region 7. SİPARİŞ DETAY (ÜRÜNLER) İŞLEMLERİ
        public bool SiparisDetayEkle(int siparisId, int urunId, int adet, decimal satisFiyati, string siparisNotu) =>
            ExecuteProcedure("sp_SiparisDetayEkle", new MySqlParameter("p_siparis_id", siparisId), new MySqlParameter("p_urun_id", urunId), new MySqlParameter("p_adet", adet), new MySqlParameter("p_satis_fiyati", satisFiyati), new MySqlParameter("p_siparis_notu", string.IsNullOrEmpty(siparisNotu) ? (object)DBNull.Value : siparisNotu));

        public bool SiparisDetayGuncelle(int siparisDetayId, int siparisId, int urunId, int adet, decimal satisFiyati, string siparisNotu) =>
            ExecuteProcedure("sp_SiparisDetayGuncelle", new MySqlParameter("p_siparis_detay_id", siparisDetayId), new MySqlParameter("p_siparis_id", siparisId), new MySqlParameter("p_urun_id", urunId), new MySqlParameter("p_adet", adet), new MySqlParameter("p_satis_fiyati", satisFiyati), new MySqlParameter("p_siparis_notu", string.IsNullOrEmpty(siparisNotu) ? (object)DBNull.Value : siparisNotu));

        public bool SiparisDetaySil(int siparisDetayId) =>
            ExecuteProcedure("sp_SiparisDetaySil", new MySqlParameter("p_siparis_detay_id", siparisDetayId));

        public DataTable SiparisDetayListele() => GetDataTable("sp_SiparisDetayListele");

        // Barista: Açık siparişlerin detayları (ürün adı + masa no JOIN)
        public DataTable AcikSiparisDetaylari() => GetDataTable("sp_AcikSiparisDetaylari");

        // Bir siparişin detayları (ürün adı ile)
        public DataTable SiparisDetayBySiparis(int siparisId) =>
            GetDataTable("sp_SiparisDetayBySiparis", new MySqlParameter("p_siparis_id", siparisId));

        // Barista: Ürünü hazır olarak işaretle (silmek yerine güncelleme — trigger tetiklenmez)
        public bool SiparisDetayHazirIsaretle(int siparisDetayId) =>
            ExecuteProcedure("sp_SiparisDetayHazirIsaretle", new MySqlParameter("p_siparis_detay_id", siparisDetayId));
        #endregion

        #region 8. ÖDEMELER İŞLEMLERİ
        public bool OdemeEkle(int siparisId, string odemeSekli, decimal odenenMiktar) =>
            ExecuteProcedure("sp_OdemeEkle", new MySqlParameter("p_siparis_id", siparisId), new MySqlParameter("p_odeme_sekli", odemeSekli), new MySqlParameter("p_odenen_miktar", odenenMiktar));

        public bool OdemeGuncelle(int odemeId, int siparisId, string odemeSekli, decimal odenenMiktar) =>
            ExecuteProcedure("sp_OdemeGuncelle", new MySqlParameter("p_odeme_id", odemeId), new MySqlParameter("p_siparis_id", siparisId), new MySqlParameter("p_odeme_sekli", odemeSekli), new MySqlParameter("p_odenen_miktar", odenenMiktar));

        public bool OdemeSil(int odemeId) =>
            ExecuteProcedure("sp_OdemeSil", new MySqlParameter("p_odeme_id", odemeId));

        public DataTable OdemeListele() => GetDataTable("sp_OdemeListele");
        #endregion

        #region 9. RAPORLAMA İŞLEMLERİ
        // Günlük satış raporu
        public DataTable GunlukSatisRaporu() => GetDataTable("sp_GunlukSatisRaporu");

        // Personel bazlı satış raporu
        public DataTable PersonelSatisRaporu() => GetDataTable("sp_PersonelSatisRaporu");
        #endregion
    }
}