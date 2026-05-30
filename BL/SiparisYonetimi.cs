using System;
using System.Collections.Generic;
using System.Data;
using CafeApp.DAL;
using CafeApp.Models;

namespace CafeApp.BL
{
    public class SiparisYonetimi
    {
        private CafeVeriErisim _dal = new CafeVeriErisim();

        // ==============================================================================
        // BARİSTA EKRANI: AÇIK SİPARİŞ DETAYLARINI GETİRME (JOIN İLE)
        // ==============================================================================
        public List<SiparisDetay> AcikSiparisDetaylariniGetir()
        {
            List<SiparisDetay> detaylar = new List<SiparisDetay>();
            DataTable dt = _dal.AcikSiparisDetaylari();

            foreach (DataRow row in dt.Rows)
            {
                detaylar.Add(new SiparisDetay
                {
                    SiparisDetayId = Convert.ToInt32(row["siparis_detay_id"]),
                    SiparisId = Convert.ToInt32(row["siparis_id"]),
                    UrunId = Convert.ToInt32(row["urun_id"]),
                    Adet = Convert.ToInt32(row["adet"]),
                    SatisFiyati = Convert.ToDecimal(row["satis_fiyati"]),
                    SiparisNotu = row["siparis_notu"] == DBNull.Value ? "" : row["siparis_notu"]?.ToString(),
                    UrunAdi = row["urun_adi"]?.ToString() ?? string.Empty,
                    MasaNo = row["masa_no"]?.ToString() ?? "Gel-Al"
                });
            }
            return detaylar;
        }

        // ==============================================================================
        // BİR SİPARİŞİN DETAYLARINI GETİRME (ÖDEME EKRANI İÇİN)
        // ==============================================================================
        public List<SiparisDetay> SiparisDetayGetir(int siparisId)
        {
            List<SiparisDetay> detaylar = new List<SiparisDetay>();
            DataTable dt = _dal.SiparisDetayBySiparis(siparisId);

            foreach (DataRow row in dt.Rows)
            {
                detaylar.Add(new SiparisDetay
                {
                    SiparisDetayId = Convert.ToInt32(row["siparis_detay_id"]),
                    SiparisId = Convert.ToInt32(row["siparis_id"]),
                    UrunId = Convert.ToInt32(row["urun_id"]),
                    Adet = Convert.ToInt32(row["adet"]),
                    SatisFiyati = Convert.ToDecimal(row["satis_fiyati"]),
                    SiparisNotu = row["siparis_notu"] == DBNull.Value ? "" : row["siparis_notu"]?.ToString(),
                    UrunAdi = row["urun_adi"]?.ToString() ?? string.Empty
                });
            }
            return detaylar;
        }

        // ==============================================================================
        // BİR MASANIN AÇIK SİPARİŞLERİNİ GETİRME
        // ==============================================================================
        public List<Siparis> MasaninAcikSiparisleriniGetir(int masaId)
        {
            List<Siparis> siparisler = new List<Siparis>();
            DataTable dt = _dal.SiparislerByMasa(masaId);

            foreach (DataRow row in dt.Rows)
            {
                siparisler.Add(new Siparis
                {
                    SiparisId = Convert.ToInt32(row["siparis_id"]),
                    MasaId = row["masa_id"] == DBNull.Value ? null : Convert.ToInt32(row["masa_id"]),
                    PersonelId = Convert.ToInt32(row["personel_id"]),
                    SiparisTarihi = Convert.ToDateTime(row["siparis_tarihi"]),
                    Durum = row["durum"]?.ToString()
                });
            }
            return siparisler;
        }

        // ==============================================================================
        // SİPARİŞ KAPATMA
        // ==============================================================================
        public string SiparisKapat(int siparisId)
        {
            bool sonuc = _dal.SiparisKapat(siparisId);
            return sonuc ? "Başarılı: Sipariş kapatıldı." : "Hata: Sipariş kapatılamadı.";
        }

        // ==============================================================================
        // KDV'Lİ TUTAR HESAPLAMA (Fonksiyon Wrapper)
        // ==============================================================================
        public decimal KDVliTutarGetir(int siparisId)
        {
            return _dal.SiparisKDVliTutar(siparisId);
        }

        // ==============================================================================
        // MASA DOLU SÜRESİ (Fonksiyon Wrapper)
        // ==============================================================================
        public int MasaDoluSuresiGetir(int masaId)
        {
            return _dal.MasaDoluSuresi(masaId);
        }

        // ==============================================================================
        // BARİSTA: ÜRÜNÜ HAZIR OLARAK İŞARETLE (Silme yerine güncelleme)
        // Böylece DELETE trigger tetiklenmez ve masa fiyatı korunur
        // ==============================================================================
        public string SiparisDetayHazirIsaretle(int siparisDetayId)
        {
            bool sonuc = _dal.SiparisDetayHazirIsaretle(siparisDetayId);
            return sonuc ? "Başarılı: Ürün hazırlandı olarak işaretlendi." : "Hata: İşlem yapılamadı.";
        }
    }
}
