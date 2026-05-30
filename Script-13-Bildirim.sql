-- ============================================================
-- Bildirim Sistemi: Garson hazır sipariş bildirimi
-- ============================================================

USE CafeDB;

-- siparis_detay tablosuna garson_gordu_mu kolonu ekle
ALTER TABLE siparis_detay
ADD COLUMN garson_gordu_mu TINYINT NOT NULL DEFAULT 0;

DELIMITER //

-- Hazır ama garson henüz görmemiş siparişleri getirir
CREATE PROCEDURE sp_HazirBekleyenler()
BEGIN
    SELECT sd.siparis_detay_id, sd.siparis_id, sd.adet,
           u.urun_adi,
           COALESCE(m.masa_no, 'Gel-Al') AS masa_no
    FROM siparis_detay sd
    INNER JOIN siparisler s ON sd.siparis_id = s.siparis_id
    INNER JOIN urunler    u ON sd.urun_id    = u.urun_id
    LEFT  JOIN masa       m ON s.masa_id     = m.masa_id
    WHERE sd.hazir_mi = 1
      AND sd.garson_gordu_mu = 0
      AND s.durum = 'Açık';
END //

-- Garson bildirimi gördü olarak işaretle
CREATE PROCEDURE sp_GarsonGorduIsaretle(
    IN p_siparis_detay_id INT
)
BEGIN
    UPDATE siparis_detay
    SET garson_gordu_mu = 1
    WHERE siparis_detay_id = p_siparis_detay_id;
END //

DELIMITER ;
