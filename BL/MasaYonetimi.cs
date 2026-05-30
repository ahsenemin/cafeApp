using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CafeApp.DAL;
using CafeApp.Models;
using CafeApp.UI;

namespace CafeApp.BL
{
    public class MasaYonetimi
    {
        // DAL sınıfımızı örneklendiriyoruz (Tüm veritabanı iletişimi buradan geçecek)
        private CafeVeriErisim _dal = new CafeVeriErisim();

        // ==============================================================================
        // LİSTELEME İŞLEMİ (DataTable'ı Liste Nesnesine Çevirme)
        // ==============================================================================
        public List<Masa> TumMasalariGetir()
        {
            List<Masa> masaListesi = new List<Masa>();

            // 1. DAL'dan ham veriyi (DataTable) al
            DataTable dt = _dal.MasaListele();

            // 2. Ham veriyi satır satır dönerek C# Masa nesnesine (Model) dönüştür
            foreach (DataRow row in dt.Rows)
            {
                masaListesi.Add(new Masa
                {
                    MasaId = Convert.ToInt32(row["masa_id"]),
                    MasaNo = row["masa_no"]?.ToString() ?? string.Empty,
                    Durum = Convert.ToByte(row["durum"]),
                    MasaToplamFiyati = Convert.ToDecimal(row["masa_toplam_fiyati"]),
                    EnSonIslemTarihi = Convert.ToDateTime(row["en_son_islem_tarihi"])
                });
            }

            // 3. MAUI (Arayüz) tarafına temizlenmiş, nesneye dönüşmüş listeyi gönder
            return masaListesi;
        }

        // ==============================================================================
        // EKLEME İŞLEMİ (İş Kuralları ve Kontroller)
        // ==============================================================================
        public string MasaEkle(string masaNo, byte durum)
        {
            // İŞ KURALI 1: Masa numarası boş olamaz
            if (string.IsNullOrWhiteSpace(masaNo))
            {
                return "Hata: Masa numarası boş bırakılamaz!";
            }

            // İŞ KURALI 2: Durum sadece 1, 2 veya 3 olabilir
            if (durum < 1 || durum > 3)
            {
                return "Hata: Geçersiz masa durumu!";
            }

            // Kurallardan geçtik, işlemi yapması için DAL'a emir veriyoruz
            bool sonuc = _dal.MasaEkle(masaNo, durum);

            if (sonuc)
            {
                return "Başarılı: Masa sisteme eklendi.";
            }
            else
            {
                return "Hata: Sistem veritabanına bağlanırken bir sorun yaşadı.";
            }
        }

        // ==============================================================================
        // MASA DURUMU GÜNCELLEME
        // ==============================================================================
        public string MasaDurumGuncelle(int masaId, byte durum)
        {
            if (durum < 1 || durum > 3)
                return "Hata: Geçersiz masa durumu!";

            bool sonuc = _dal.MasaDurumGuncelle(masaId, durum);
            return sonuc ? "Başarılı: Masa durumu güncellendi." : "Hata: Masa durumu güncellenemedi.";
        }

        // ==============================================================================
        // MASA SIFIRLAMA (Ödeme sonrası: durum=Boş, fiyat=0)
        // ==============================================================================
        public string MasaSifirla(int masaId)
        {
            bool sonuc = _dal.MasaSifirla(masaId);
            return sonuc ? "Başarılı: Masa sıfırlandı." : "Hata: Masa sıfırlanamadı.";
        }

        // ==============================================================================
        // MASA DOLU SÜRESİ (fn_MasaDoluSuresi wrapper)
        // ==============================================================================
        public int MasaDoluSuresi(int masaId)
        {
            return _dal.MasaDoluSuresi(masaId);
        }

        // ==============================================================================
        // SİPARİŞİ (SEPETİ) VERİTABANINA KAYDETME İŞLEMİ
        // ==============================================================================
        public string YeniSiparisOlustur(int? masaId, int personelId, IEnumerable<SepetKalemi> sepetListesi)
        {
            try
            {
                // 1. İŞ KURALI: Sepet boş mu?
                if (sepetListesi == null || !sepetListesi.Any())
                {
                    return "Hata: Sepette hiç ürün yok!";
                }

                // 2. ADIM: Siparişi (Adisyon Başlığını) Oluştur
                // Not: Burada 'Açık' durumu ile siparişi başlatıyoruz.
                bool siparisEklendi = _dal.SiparisEkle(masaId, personelId, "Açık");

                if (!siparisEklendi)
                {
                    return "Hata: Sipariş başlatılamadı.";
                }

                // 3. ADIM: Son Eklenen Siparişin ID'sini SP ile al
                int sonSiparisId = _dal.SonSiparisIdGetir();

                if (sonSiparisId == 0)
                {
                    return "Hata: Oluşturulan siparişin ID'si bulunamadı.";
                }

                // 4. ADIM: Sepetteki Ürünleri Sipariş Detaylarına (siparis_detay) Ekle
                foreach (var urun in sepetListesi)
                {
                    _dal.SiparisDetayEkle(sonSiparisId, urun.UrunId, urun.Adet, urun.Fiyat, "");
                }

                // 5. ADIM: Eğer masa 'Boş' (1) ise durumunu 'Dolu' (2) yap.
                if (masaId.HasValue)
                {
                    _dal.MasaDurumGuncelle(masaId.Value, 2);
                }

                return "Başarılı: Sipariş başarıyla mutfağa iletildi!";
            }
            catch (Exception ex)
            {
                return $"Kritik Hata: {ex.Message}";
            }
        }
    }
}