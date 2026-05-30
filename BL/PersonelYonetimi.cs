using System;
using System.Collections.Generic;
using System.Data;
using CafeApp.DAL;
using CafeApp.Models;

namespace CafeApp.BL
{
    public class PersonelYonetimi
    {
        private CafeVeriErisim _dal = new CafeVeriErisim();

        // ==============================================================================
        // AKTİF PERSONELLERİ GETİRME (Giriş ekranı için)
        // ==============================================================================
        public List<Personel> AktifPersonelleriGetir()
        {
            List<Personel> personeller = new List<Personel>();
            DataTable dt = _dal.PersonelListele();

            foreach (DataRow row in dt.Rows)
            {
                // Sadece aktif (çalışan) personelleri listeye ekliyoruz (AktifMi == 1)
                if (Convert.ToByte(row["p_aktif_mi"]) == 1)
                {
                    personeller.Add(new Personel
                    {
                        PersonelId = Convert.ToInt32(row["personel_id"]),
                        Ad = row["p_adi"].ToString(),
                        Soyad = row["p_soyadi"].ToString(),
                        Eposta = row["p_eposta"]?.ToString(),
                        Adres = row["p_adres"]?.ToString(),
                        Telefon = row["p_tel"]?.ToString(),
                        TcKimlik = row["p_tc"]?.ToString(),
                        AktifMi = Convert.ToByte(row["p_aktif_mi"]),
                        KayitTarihi = Convert.ToDateTime(row["kayit_tarihi"]),
                        RolId = Convert.ToInt32(row["rol_id"])
                    });
                }
            }
            return personeller;
        }

        // ==============================================================================
        // TÜM PERSONELLERİ GETİRME (Yönetici ekranı için)
        // ==============================================================================
        public List<Personel> TumPersonelleriGetir()
        {
            List<Personel> personeller = new List<Personel>();
            DataTable dt = _dal.PersonelListele();

            foreach (DataRow row in dt.Rows)
            {
                personeller.Add(new Personel
                {
                    PersonelId = Convert.ToInt32(row["personel_id"]),
                    Ad = row["p_adi"].ToString(),
                    Soyad = row["p_soyadi"].ToString(),
                    Eposta = row["p_eposta"]?.ToString(),
                    Adres = row["p_adres"]?.ToString(),
                    Telefon = row["p_tel"]?.ToString(),
                    TcKimlik = row["p_tc"]?.ToString(),
                    AktifMi = Convert.ToByte(row["p_aktif_mi"]),
                    KayitTarihi = Convert.ToDateTime(row["kayit_tarihi"]),
                    RolId = Convert.ToInt32(row["rol_id"])
                });
            }
            return personeller;
        }

        // ==============================================================================
        // PERSONEL EKLEME
        // ==============================================================================
        public string PersonelEkle(string adi, string soyadi, string eposta, string adres, string tel, string tc, byte aktifMi, int rolId)
        {
            if (string.IsNullOrWhiteSpace(adi) || string.IsNullOrWhiteSpace(soyadi))
                return "Hata: Ad ve soyad boş bırakılamaz!";
            if (string.IsNullOrWhiteSpace(tc) || tc.Length != 11)
                return "Hata: TC Kimlik No 11 haneli olmalıdır!";

            bool sonuc = _dal.PersonelEkle(adi, soyadi, eposta, adres, tel, tc, aktifMi, rolId);
            return sonuc ? "Başarılı: Personel eklendi." : "Hata: Personel eklenemedi. TC veya e-posta zaten kayıtlı olabilir.";
        }

        // ==============================================================================
        // PERSONEL GÜNCELLEME
        // ==============================================================================
        public string PersonelGuncelle(int personelId, string adi, string soyadi, string eposta, string adres, string tel, string tc, byte aktifMi, int rolId)
        {
            if (string.IsNullOrWhiteSpace(adi) || string.IsNullOrWhiteSpace(soyadi))
                return "Hata: Ad ve soyad boş bırakılamaz!";

            bool sonuc = _dal.PersonelGuncelle(personelId, adi, soyadi, eposta, adres, tel, tc, aktifMi, rolId);
            return sonuc ? "Başarılı: Personel güncellendi." : "Hata: Personel güncellenemedi.";
        }

        // ==============================================================================
        // PERSONEL SİLME
        // ==============================================================================
        public string PersonelSil(int personelId)
        {
            bool sonuc = _dal.PersonelSil(personelId);
            return sonuc ? "Başarılı: Personel silindi." : "Hata: Personel silinemedi.";
        }

        // ==============================================================================
        // PERSONEL TOPLAM SATIŞ (fn_PersonelToplamSatis wrapper)
        // ==============================================================================
        public decimal PersonelCiroGetir(int personelId)
        {
            return _dal.PersonelToplamSatis(personelId);
        }
    }
}