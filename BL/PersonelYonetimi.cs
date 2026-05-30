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
        // PERSONEL TOPLAM SATIŞ (fn_PersonelToplamSatis wrapper)
        // ==============================================================================
        public decimal PersonelCiroGetir(int personelId)
        {
            return _dal.PersonelToplamSatis(personelId);
        }
    }
}