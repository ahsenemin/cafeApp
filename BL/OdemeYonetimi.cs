using System;
using System.Collections.Generic;
using System.Data;
using CafeApp.DAL;
using CafeApp.Models;

namespace CafeApp.BL
{
    public class OdemeYonetimi
    {
        private CafeVeriErisim _dal = new CafeVeriErisim();

        // ==============================================================================
        // ÖDEME ALMA (İş Kuralları ile)
        // ==============================================================================
        public string OdemeAl(int siparisId, string odemeSekli, decimal miktar)
        {
            // İŞ KURALI 1: Ödeme tutarı sıfırdan büyük olmalı
            if (miktar <= 0)
                return "Hata: Ödeme tutarı sıfırdan büyük olmalıdır!";

            // İŞ KURALI 2: Ödeme şekli seçilmiş olmalı
            if (string.IsNullOrWhiteSpace(odemeSekli))
                return "Hata: Lütfen bir ödeme yöntemi seçin!";

            // İŞ KURALI 3: Geçerli ödeme yöntemleri
            if (odemeSekli != "Nakit" && odemeSekli != "Kredi Kartı")
                return "Hata: Geçersiz ödeme yöntemi!";

            // Ödeme kaydı oluştur
            bool sonuc = _dal.OdemeEkle(siparisId, odemeSekli, miktar);

            if (sonuc)
            {
                // Ödeme başarılıysa siparişi kapat
                _dal.SiparisKapat(siparisId);
                return "Başarılı: Ödeme alındı ve sipariş kapatıldı.";
            }
            else
            {
                return "Hata: Ödeme kaydedilemedi.";
            }
        }

        // ==============================================================================
        // MASANIN TÜM AÇIK SİPARİŞLERİNİN ÖDEMESİNİ AL VE MASAYI SIFIRLA
        // ==============================================================================
        public string MasaOdemesiAl(int masaId, string odemeSekli, decimal toplamTutar)
        {
            try
            {
                // İŞ KURALI
                if (toplamTutar <= 0)
                    return "Hata: Ödeme tutarı sıfırdan büyük olmalıdır!";

                if (string.IsNullOrWhiteSpace(odemeSekli))
                    return "Hata: Lütfen bir ödeme yöntemi seçin!";

                // 1. Masanın açık siparişlerini bul
                DataTable acikSiparisler = _dal.SiparislerByMasa(masaId);

                if (acikSiparisler.Rows.Count == 0)
                    return "Hata: Bu masada açık sipariş bulunamadı!";

                // 2. Her açık sipariş için ödeme kaydı oluştur ve siparişi kapat
                foreach (DataRow row in acikSiparisler.Rows)
                {
                    int siparisId = Convert.ToInt32(row["siparis_id"]);
                    
                    // Sipariş detaylarının toplam tutarını hesapla (KDV'li)
                    decimal kdvliTutar = _dal.SiparisKDVliTutar(siparisId);

                    // Ödeme kaydı oluştur
                    _dal.OdemeEkle(siparisId, odemeSekli, kdvliTutar);

                    // Siparişi kapat
                    _dal.SiparisKapat(siparisId);
                }

                // 3. Masayı sıfırla (durum=Boş, fiyat=0)
                _dal.MasaSifirla(masaId);

                return "Başarılı: Ödeme alındı, masa sıfırlandı!";
            }
            catch (Exception ex)
            {
                return $"Kritik Hata: {ex.Message}";
            }
        }

        // ==============================================================================
        // TÜM ÖDEMELERİ LİSTELEME (Raporlama)
        // ==============================================================================
        public List<Odeme> TumOdemeleriGetir()
        {
            List<Odeme> odemeler = new List<Odeme>();
            DataTable dt = _dal.OdemeListele();

            foreach (DataRow row in dt.Rows)
            {
                odemeler.Add(new Odeme
                {
                    OdemeId = Convert.ToInt32(row["odeme_id"]),
                    SiparisId = Convert.ToInt32(row["siparis_id"]),
                    OdemeSekli = row["odeme_sekli"]?.ToString(),
                    OdenenMiktar = Convert.ToDecimal(row["odenen_miktar"]),
                    OdemeTarihSaat = Convert.ToDateTime(row["odeme_tarih_saat"])
                });
            }
            return odemeler;
        }

        // ==============================================================================
        // GÜNLÜK SATIŞ RAPORU
        // ==============================================================================
        public DataTable GunlukRapor()
        {
            return _dal.GunlukSatisRaporu();
        }

        // ==============================================================================
        // PERSONEL SATIŞ RAPORU
        // ==============================================================================
        public DataTable PersonelRaporu()
        {
            return _dal.PersonelSatisRaporu();
        }
    }
}
