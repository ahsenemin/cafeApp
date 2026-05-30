-- =============================================
-- CafeApp — Ek Stored Procedure'lar ve Test Verileri
-- Bu dosya, Script-10.sql çalıştırıldıktan SONRA çalıştırılmalıdır.
-- =============================================

USE CafeDB;

DELIMITER //

-- =============================================
-- EK STORED PROCEDURE'LAR (Uygulama İhtiyaçları)
-- =============================================

-- 1. Kategoriye göre ürün listeleme
CREATE PROCEDURE sp_UrunListeleByKategori (
    IN p_kategori_id INT
)
BEGIN
    SELECT * FROM urunler WHERE kategori_id = p_kategori_id;
END //

-- 2. Barista Ekranı: Açık siparişlerin detaylı listesi (JOIN sorgusu)
CREATE PROCEDURE sp_AcikSiparisDetaylari ()
BEGIN
    SELECT sd.siparis_detay_id, sd.siparis_id, sd.urun_id, sd.adet, 
           sd.satis_fiyati, sd.siparis_notu,
           u.urun_adi, 
           COALESCE(m.masa_no, 'Gel-Al') AS masa_no
    FROM siparis_detay sd
    INNER JOIN siparisler s ON sd.siparis_id = s.siparis_id
    INNER JOIN urunler u ON sd.urun_id = u.urun_id
    LEFT JOIN masa m ON s.masa_id = m.masa_id
    WHERE s.durum = 'Açık'
    ORDER BY sd.siparis_detay_id DESC;
END //

-- 3. Bir siparişin detay satırlarını getirme (ürün adı ile birlikte)
CREATE PROCEDURE sp_SiparisDetayBySiparis (
    IN p_siparis_id INT
)
BEGIN
    SELECT sd.siparis_detay_id, sd.siparis_id, sd.urun_id, sd.adet, 
           sd.satis_fiyati, sd.siparis_notu,
           u.urun_adi
    FROM siparis_detay sd
    INNER JOIN urunler u ON sd.urun_id = u.urun_id
    WHERE sd.siparis_id = p_siparis_id;
END //

-- 4. Bir masanın açık siparişlerini getirme
CREATE PROCEDURE sp_SiparislerByMasa (
    IN p_masa_id INT
)
BEGIN
    SELECT * FROM siparisler 
    WHERE masa_id = p_masa_id AND durum = 'Açık';
END //

-- 5. Son eklenen sipariş ID'sini getirme
CREATE PROCEDURE sp_SonSiparisIdGetir ()
BEGIN
    SELECT MAX(siparis_id) AS son_id FROM siparisler;
END //

-- 6. Sipariş kapatma (durumu 'Kapalı' yapma)
CREATE PROCEDURE sp_SiparisKapat (
    IN p_siparis_id INT
)
BEGIN
    UPDATE siparisler SET durum = 'Kapalı' WHERE siparis_id = p_siparis_id;
END //

-- 7. Masa sıfırlama (durum=Boş, fiyat=0)
CREATE PROCEDURE sp_MasaSifirla (
    IN p_masa_id INT
)
BEGIN
    UPDATE masa 
    SET durum = 1, 
        masa_toplam_fiyati = 0.00, 
        en_son_islem_tarihi = CURRENT_TIMESTAMP 
    WHERE masa_id = p_masa_id;
END //

-- 8. Sadece masa durumunu güncelleme
CREATE PROCEDURE sp_MasaDurumGuncelle (
    IN p_masa_id INT,
    IN p_durum TINYINT
)
BEGIN
    UPDATE masa 
    SET durum = p_durum, 
        en_son_islem_tarihi = CURRENT_TIMESTAMP 
    WHERE masa_id = p_masa_id;
END //

-- =============================================
-- FONKSİYON SARMALAYICI (WRAPPER) PROCEDURE'LAR
-- (Ödev kuralı: Doğrudan SQL kullanılamaz, 
--  fonksiyonları da SP üzerinden çağırıyoruz)
-- =============================================

-- 9. KDV'li tutar hesaplama (fn_SiparisKDVliTutar wrapper)
CREATE PROCEDURE sp_SiparisKDVliTutarGetir (
    IN p_siparis_id INT
)
BEGIN
    SELECT fn_SiparisKDVliTutar(p_siparis_id) AS kdvli_tutar;
END //

-- 10. Personel toplam satış (fn_PersonelToplamSatis wrapper)
CREATE PROCEDURE sp_PersonelToplamSatisGetir (
    IN p_personel_id INT
)
BEGIN
    SELECT fn_PersonelToplamSatis(p_personel_id) AS toplam_satis;
END //

-- 11. Masa dolu süresi (fn_MasaDoluSuresi wrapper)
CREATE PROCEDURE sp_MasaDoluSuresiGetir (
    IN p_masa_id INT
)
BEGIN
    SELECT fn_MasaDoluSuresi(p_masa_id) AS dolu_suresi;
END //

-- =============================================
-- RAPORLAMA PROCEDURE'LARI
-- =============================================

-- 12. Günlük satış raporu
CREATE PROCEDURE sp_GunlukSatisRaporu ()
BEGIN
    SELECT DATE(o.odeme_tarih_saat) AS tarih, 
           o.odeme_sekli, 
           COUNT(*) AS islem_sayisi, 
           SUM(o.odenen_miktar) AS toplam_tutar
    FROM odemeler o
    WHERE DATE(o.odeme_tarih_saat) = CURDATE()
    GROUP BY DATE(o.odeme_tarih_saat), o.odeme_sekli;
END //

-- 13. Personel bazlı satış raporu
CREATE PROCEDURE sp_PersonelSatisRaporu ()
BEGIN
    SELECT p.personel_id, p.p_adi, p.p_soyadi, 
           fn_PersonelToplamSatis(p.personel_id) AS toplam_satis
    FROM personel p
    WHERE p.p_aktif_mi = 1
    ORDER BY toplam_satis DESC;
END //

DELIMITER ;

-- =============================================
-- TEST VERİLERİ
-- =============================================

-- 1. Roller — ZATEN MEVCUT:
-- rol_id 100 = Garson
-- rol_id 101 = Yönetici  
-- rol_id 102 = Barista
-- Tekrar eklemeye gerek yok, aşağıdaki personel kayıtları bu ID'leri kullanıyor.

-- 2. Personeller (AUTO_INCREMENT = 10 başlar)
CALL sp_PersonelEkle('Ahmet', 'Yılmaz', 'ahmet@cafe.com', 'İstanbul Kadıköy', '5551234567', '12345678901', 1, 101);   -- Yönetici (rol_id: 101)
CALL sp_PersonelEkle('Mehmet', 'Kaya', 'mehmet@cafe.com', 'İstanbul Üsküdar', '5559876543', '98765432109', 1, 100);    -- Garson (rol_id: 100)
CALL sp_PersonelEkle('Ayşe', 'Demir', 'ayse@cafe.com', 'İstanbul Beşiktaş', '5554567890', '45678901234', 1, 102);     -- Barista (rol_id: 102)
CALL sp_PersonelEkle('Fatma', 'Çelik', 'fatma@cafe.com', 'İstanbul Şişli', '5553214567', '56789012345', 1, 100);       -- Garson 2 (rol_id: 100)

-- 3. Kategoriler
CALL sp_KategoriEkle('Sıcak İçecekler');     -- kategori_id: 1
CALL sp_KategoriEkle('Soğuk İçecekler');      -- kategori_id: 2
CALL sp_KategoriEkle('Tatlılar');             -- kategori_id: 3
CALL sp_KategoriEkle('Atıştırmalıklar');      -- kategori_id: 4

-- 4. Ürünler
-- Sıcak İçecekler (kategori_id: 1)
CALL sp_UrunEkle('Filtre Kahve', 45.00, 1);
CALL sp_UrunEkle('Türk Kahvesi', 50.00, 1);
CALL sp_UrunEkle('Latte', 75.00, 1);
CALL sp_UrunEkle('Cappuccino', 70.00, 1);
CALL sp_UrunEkle('Americano', 55.00, 1);
CALL sp_UrunEkle('Çay', 25.00, 1);
CALL sp_UrunEkle('Sıcak Çikolata', 65.00, 1);

-- Soğuk İçecekler (kategori_id: 2)
CALL sp_UrunEkle('Ice Latte', 85.00, 2);
CALL sp_UrunEkle('Ice Americano', 65.00, 2);
CALL sp_UrunEkle('Limonata', 55.00, 2);
CALL sp_UrunEkle('Smoothie', 80.00, 2);
CALL sp_UrunEkle('Soğuk Çay', 35.00, 2);

-- Tatlılar (kategori_id: 3)
CALL sp_UrunEkle('Cheesecake', 95.00, 3);
CALL sp_UrunEkle('Brownie', 75.00, 3);
CALL sp_UrunEkle('Tiramisu', 90.00, 3);
CALL sp_UrunEkle('Sufle', 85.00, 3);

-- Atıştırmalıklar (kategori_id: 4)
CALL sp_UrunEkle('Sandviç', 90.00, 4);
CALL sp_UrunEkle('Tost', 65.00, 4);
CALL sp_UrunEkle('Kruvasan', 55.00, 4);
CALL sp_UrunEkle('Kurabiye', 35.00, 4);
