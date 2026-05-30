-- =============================================
-- CafeApp — Barista Düzeltmesi
-- Barista "Hazırlandı" dediğinde ürün silinmemeli,
-- sadece hazır olarak işaretlenmeli.
-- =============================================

USE CafeDB;

-- 1. siparis_detay tablosuna 'hazir_mi' sütunu ekle
-- 0 = Bekliyor (varsayılan), 1 = Hazırlandı
ALTER TABLE siparis_detay 
ADD COLUMN hazir_mi TINYINT NOT NULL DEFAULT 0;

-- 2. Barista: Ürünü hazır olarak işaretle (silme yerine güncelleme)
DELIMITER //

CREATE PROCEDURE sp_SiparisDetayHazirIsaretle (
    IN p_siparis_detay_id INT
)
BEGIN
    UPDATE siparis_detay 
    SET hazir_mi = 1 
    WHERE siparis_detay_id = p_siparis_detay_id;
END //

DELIMITER ;

-- 3. Mevcut sp_AcikSiparisDetaylari prosedürünü güncelle
-- Sadece henüz hazırlanMAMIŞ ürünleri göstersin
DROP PROCEDURE IF EXISTS sp_AcikSiparisDetaylari;

DELIMITER //

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
    WHERE s.durum = 'Açık' AND sd.hazir_mi = 0
    ORDER BY sd.siparis_detay_id DESC;
END //

DELIMITER ;
