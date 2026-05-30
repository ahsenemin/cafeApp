using System;
using System.Collections.Generic;
using System.Data;
using CafeApp.DAL;
using CafeApp.Models;

namespace CafeApp.BL
{
    public class KategoriYonetimi
    {
        private CafeVeriErisim _dal = new CafeVeriErisim();

        // ==============================================================================
        // KATEGORİ LİSTELEME
        // ==============================================================================
        public List<Kategori> TumKategorileriGetir()
        {
            List<Kategori> kategoriler = new List<Kategori>();
            DataTable dt = _dal.KategoriListele();

            foreach (DataRow row in dt.Rows)
            {
                kategoriler.Add(new Kategori
                {
                    KategoriId = Convert.ToInt32(row["kategori_id"]),
                    KAdi = row["k_adi"]?.ToString() ?? string.Empty
                });
            }
            return kategoriler;
        }

        // ==============================================================================
        // KATEGORİ EKLEME
        // ==============================================================================
        public string KategoriEkle(string kAdi)
        {
            if (string.IsNullOrWhiteSpace(kAdi))
                return "Hata: Kategori adı boş bırakılamaz!";

            bool sonuc = _dal.KategoriEkle(kAdi);
            return sonuc ? "Başarılı: Kategori eklendi." : "Hata: Kategori eklenemedi.";
        }

        // ==============================================================================
        // KATEGORİ GÜNCELLEME
        // ==============================================================================
        public string KategoriGuncelle(int kategoriId, string kAdi)
        {
            if (string.IsNullOrWhiteSpace(kAdi))
                return "Hata: Kategori adı boş bırakılamaz!";

            bool sonuc = _dal.KategoriGuncelle(kategoriId, kAdi);
            return sonuc ? "Başarılı: Kategori güncellendi." : "Hata: Kategori güncellenemedi.";
        }

        // ==============================================================================
        // KATEGORİ SİLME
        // ==============================================================================
        public string KategoriSil(int kategoriId)
        {
            bool sonuc = _dal.KategoriSil(kategoriId);
            return sonuc ? "Başarılı: Kategori silindi." : "Hata: Kategori silinemedi. Bu kategoriye ait ürünler olabilir.";
        }
    }
}
