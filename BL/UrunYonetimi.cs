using System;
using System.Collections.Generic;
using System.Data;
using CafeApp.DAL;
using CafeApp.Models;

namespace CafeApp.BL
{
    public class UrunYonetimi
    {
        private CafeVeriErisim _dal = new CafeVeriErisim();

        // ==============================================================================
        // TÜM ÜRÜNLERİ LİSTELEME
        // ==============================================================================
        public List<Urun> TumUrunleriGetir()
        {
            List<Urun> urunler = new List<Urun>();
            DataTable dt = _dal.UrunListele();

            foreach (DataRow row in dt.Rows)
            {
                urunler.Add(new Urun
                {
                    UrunId = Convert.ToInt32(row["urun_id"]),
                    UrunAdi = row["urun_adi"]?.ToString() ?? string.Empty,
                    UrunFiyati = Convert.ToDecimal(row["urun_fiyati"]),
                    KategoriId = Convert.ToInt32(row["kategori_id"])
                });
            }
            return urunler;
        }

        // ==============================================================================
        // KATEGORİYE GÖRE ÜRÜN LİSTELEME
        // ==============================================================================
        public List<Urun> KategoriyeGoreUrunGetir(int kategoriId)
        {
            List<Urun> urunler = new List<Urun>();
            DataTable dt = _dal.UrunListeleByKategori(kategoriId);

            foreach (DataRow row in dt.Rows)
            {
                urunler.Add(new Urun
                {
                    UrunId = Convert.ToInt32(row["urun_id"]),
                    UrunAdi = row["urun_adi"]?.ToString() ?? string.Empty,
                    UrunFiyati = Convert.ToDecimal(row["urun_fiyati"]),
                    KategoriId = Convert.ToInt32(row["kategori_id"])
                });
            }
            return urunler;
        }

        // ==============================================================================
        // ÜRÜN EKLEME
        // ==============================================================================
        public string UrunEkle(string urunAdi, decimal urunFiyati, int kategoriId)
        {
            if (string.IsNullOrWhiteSpace(urunAdi))
                return "Hata: Ürün adı boş bırakılamaz!";

            if (urunFiyati <= 0)
                return "Hata: Ürün fiyatı sıfırdan büyük olmalıdır!";

            bool sonuc = _dal.UrunEkle(urunAdi, urunFiyati, kategoriId);
            return sonuc ? "Başarılı: Ürün eklendi." : "Hata: Ürün eklenemedi.";
        }

        // ==============================================================================
        // ÜRÜN GÜNCELLEME
        // ==============================================================================
        public string UrunGuncelle(int urunId, string urunAdi, decimal urunFiyati, int kategoriId)
        {
            if (string.IsNullOrWhiteSpace(urunAdi))
                return "Hata: Ürün adı boş bırakılamaz!";

            if (urunFiyati <= 0)
                return "Hata: Ürün fiyatı sıfırdan büyük olmalıdır!";

            bool sonuc = _dal.UrunGuncelle(urunId, urunAdi, urunFiyati, kategoriId);
            return sonuc ? "Başarılı: Ürün güncellendi." : "Hata: Ürün güncellenemedi.";
        }

        // ==============================================================================
        // ÜRÜN SİLME
        // ==============================================================================
        public string UrunSil(int urunId)
        {
            bool sonuc = _dal.UrunSil(urunId);
            return sonuc ? "Başarılı: Ürün silindi." : "Hata: Ürün silinemedi.";
        }
    }
}
