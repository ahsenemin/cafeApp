-- 1. Veritabanını Oluşturma ve Seçme
CREATE DATABASE IF NOT EXISTS CafeDB;
USE CafeDB;

-- 2. Rol Tablosu
CREATE TABLE rol (
    rol_id INT AUTO_INCREMENT PRIMARY KEY,
    rol_adi VARCHAR(50) NOT NULL,
    aktif_mi TINYINT NOT NULL CHECK (aktif_mi IN (1, 2)) -- 1: Aktif, 2: Pasif
) AUTO_INCREMENT = 100;

-- 3. Kategori Tablosu
CREATE TABLE kategori (
    kategori_id INT AUTO_INCREMENT PRIMARY KEY,
    k_adi VARCHAR(50) NOT NULL
);

-- 4. Personel Tablosu
CREATE TABLE personel (
    personel_id INT AUTO_INCREMENT PRIMARY KEY,
    p_adi VARCHAR(50) NOT NULL,
    p_soyadi VARCHAR(50) NOT NULL,
    p_eposta VARCHAR(100) NOT NULL UNIQUE, -- E-posta benzersiz olmalı
    p_adres VARCHAR(255) NOT NULL,
    p_tel VARCHAR(15) NOT NULL,
    p_tc CHAR(11) NOT NULL UNIQUE, -- TC Kimlik No benzersiz olmalı
    p_aktif_mi TINYINT NOT NULL CHECK (p_aktif_mi IN (1, 2)),
    kayit_tarihi DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL,
    rol_id INT NOT NULL,
    FOREIGN KEY (rol_id) REFERENCES rol(rol_id)
) AUTO_INCREMENT = 10;

-- 5. Masa Tablosu
CREATE TABLE masa (
    masa_id INT AUTO_INCREMENT PRIMARY KEY,
    masa_no VARCHAR(20) NOT NULL,
    durum TINYINT NOT NULL CHECK (durum IN (1, 2, 3)), -- 1: Boş, 2: Dolu, 3: Rezerve
    masa_toplam_fiyati DECIMAL(10,2) DEFAULT 0.00 NOT NULL,
    en_son_islem_tarihi DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL
);

-- 6. Urunler Tablosu
CREATE TABLE urunler (
    urun_id INT AUTO_INCREMENT PRIMARY KEY,
    urun_adi VARCHAR(100) NOT NULL,
    urun_fiyati DECIMAL(10,2) NOT NULL,
    kategori_id INT NOT NULL,
    FOREIGN KEY (kategori_id) REFERENCES kategori(kategori_id)
);

-- 7. Siparisler Tablosu
CREATE TABLE siparisler (
    siparis_id INT AUTO_INCREMENT PRIMARY KEY,
    masa_id INT NULL, -- Gel-Al siparişleri için NULL kalabilir
    personel_id INT NOT NULL,
    siparis_tarihi DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL,
    durum VARCHAR(20) NOT NULL DEFAULT 'Açık', -- Açık, Kapalı, İptal
    FOREIGN KEY (masa_id) REFERENCES masa(masa_id),
    FOREIGN KEY (personel_id) REFERENCES personel(personel_id)
);

-- 8. Siparis Detay Tablosu
CREATE TABLE siparis_detay (
    siparis_detay_id INT AUTO_INCREMENT PRIMARY KEY,
    siparis_id INT NOT NULL,
    urun_id INT NOT NULL,
    adet INT NOT NULL CHECK (adet > 0),
    satis_fiyati DECIMAL(10,2) NOT NULL,
    siparis_notu VARCHAR(255) NULL,
    FOREIGN KEY (siparis_id) REFERENCES siparisler(siparis_id),
    FOREIGN KEY (urun_id) REFERENCES urunler(urun_id)
);

-- 9. Odemeler Tablosu
CREATE TABLE odemeler (
    odeme_id INT AUTO_INCREMENT PRIMARY KEY,
    siparis_id INT NOT NULL,
    odeme_sekli VARCHAR(20) NOT NULL, -- Nakit, Kredi Kartı vb.
    odenen_miktar DECIMAL(10,2) NOT NULL CHECK (odenen_miktar > 0),
    odeme_tarih_saat DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL,
    FOREIGN KEY (siparis_id) REFERENCES siparisler(siparis_id)
);


DELIMITER // 
-- 1. KATEGORİ EKLEME (INSERT)
CREATE PROCEDURE sp_KategoriEkle (
    IN p_k_adi VARCHAR(50)
)
BEGIN
    INSERT INTO kategori (k_adi) 
    VALUES (p_k_adi);
END //

-- 2. KATEGORİ GÜNCELLEME (UPDATE)
CREATE PROCEDURE sp_KategoriGuncelle (
    IN p_kategori_id INT,
    IN p_k_adi VARCHAR(50)
)
BEGIN
    UPDATE kategori 
    SET k_adi = p_k_adi 
    WHERE kategori_id = p_kategori_id;
END //

-- 3. KATEGORİ SİLME (DELETE)
CREATE PROCEDURE sp_KategoriSil (
    IN p_kategori_id INT
)
BEGIN
    DELETE FROM kategori 
    WHERE kategori_id = p_kategori_id;
END //

-- 4. KATEGORİ LİSTELEME (SELECT)
CREATE PROCEDURE sp_KategoriListele ()
BEGIN
    SELECT * FROM kategori;
END //

DELIMITER ;

DELIMITER //

-- =============================================
-- ROL TABLOSU PROSEDÜRLERİ
-- =============================================
CREATE PROCEDURE sp_RolEkle (
    IN p_rol_adi VARCHAR(50),
    IN p_aktif_mi TINYINT
)
BEGIN
    INSERT INTO rol (rol_adi, aktif_mi) 
    VALUES (p_rol_adi, p_aktif_mi);
END //

CREATE PROCEDURE sp_RolGuncelle (
    IN p_rol_id INT,
    IN p_rol_adi VARCHAR(50),
    IN p_aktif_mi TINYINT
)
BEGIN
    UPDATE rol 
    SET rol_adi = p_rol_adi, aktif_mi = p_aktif_mi 
    WHERE rol_id = p_rol_id;
END //

CREATE PROCEDURE sp_RolSil (
    IN p_rol_id INT
)
BEGIN
    DELETE FROM rol WHERE rol_id = p_rol_id;
END //

CREATE PROCEDURE sp_RolListele ()
BEGIN
    SELECT * FROM rol;
END //

-- =============================================
-- MASA TABLOSU PROSEDÜRLERİ
-- =============================================
CREATE PROCEDURE sp_MasaEkle (
    IN p_masa_no VARCHAR(20),
    IN p_durum TINYINT
)
BEGIN
    -- Yeni masanın toplam fiyatı sıfır olarak başlar
    INSERT INTO masa (masa_no, durum, masa_toplam_fiyati) 
    VALUES (p_masa_no, p_durum, 0.00);
END //

CREATE PROCEDURE sp_MasaGuncelle (
    IN p_masa_id INT,
    IN p_masa_no VARCHAR(20),
    IN p_durum TINYINT,
    IN p_masa_toplam_fiyati DECIMAL(10,2)
)
BEGIN
    UPDATE masa 
    SET masa_no = p_masa_no, 
        durum = p_durum, 
        masa_toplam_fiyati = p_masa_toplam_fiyati,
        en_son_islem_tarihi = CURRENT_TIMESTAMP
    WHERE masa_id = p_masa_id;
END //

CREATE PROCEDURE sp_MasaSil (
    IN p_masa_id INT
)
BEGIN
    DELETE FROM masa WHERE masa_id = p_masa_id;
END //

CREATE PROCEDURE sp_MasaListele ()
BEGIN
    SELECT * FROM masa;
END //

-- =============================================
-- ÜRÜNLER TABLOSU PROSEDÜRLERİ
-- =============================================
CREATE PROCEDURE sp_UrunEkle (
    IN p_urun_adi VARCHAR(100),
    IN p_urun_fiyati DECIMAL(10,2),
    IN p_kategori_id INT
)
BEGIN
    INSERT INTO urunler (urun_adi, urun_fiyati, kategori_id) 
    VALUES (p_urun_adi, p_urun_fiyati, p_kategori_id);
END //

CREATE PROCEDURE sp_UrunGuncelle (
    IN p_urun_id INT,
    IN p_urun_adi VARCHAR(100),
    IN p_urun_fiyati DECIMAL(10,2),
    IN p_kategori_id INT
)
BEGIN
    UPDATE urunler 
    SET urun_adi = p_urun_adi, 
        urun_fiyati = p_urun_fiyati, 
        kategori_id = p_kategori_id 
    WHERE urun_id = p_urun_id;
END //

CREATE PROCEDURE sp_UrunSil (
    IN p_urun_id INT
)
BEGIN
    DELETE FROM urunler WHERE urun_id = p_urun_id;
END //

CREATE PROCEDURE sp_UrunListele ()
BEGIN
    SELECT * FROM urunler;
END //

-- =============================================
-- PERSONEL TABLOSU PROSEDÜRLERİ
-- =============================================
CREATE PROCEDURE sp_PersonelEkle (
    IN p_p_adi VARCHAR(50),
    IN p_p_soyadi VARCHAR(50),
    IN p_p_eposta VARCHAR(100),
    IN p_p_adres VARCHAR(255),
    IN p_p_tel VARCHAR(15),
    IN p_p_tc CHAR(11),
    IN p_p_aktif_mi TINYINT,
    IN p_rol_id INT
)
BEGIN
    INSERT INTO personel (p_adi, p_soyadi, p_eposta, p_adres, p_tel, p_tc, p_aktif_mi, rol_id) 
    VALUES (p_p_adi, p_p_soyadi, p_p_eposta, p_p_adres, p_p_tel, p_p_tc, p_p_aktif_mi, p_rol_id);
END //

CREATE PROCEDURE sp_PersonelGuncelle (
    IN p_personel_id INT,
    IN p_p_adi VARCHAR(50),
    IN p_p_soyadi VARCHAR(50),
    IN p_p_eposta VARCHAR(100),
    IN p_p_adres VARCHAR(255),
    IN p_p_tel VARCHAR(15),
    IN p_p_tc CHAR(11),
    IN p_p_aktif_mi TINYINT,
    IN p_rol_id INT
)
BEGIN
    UPDATE personel 
    SET p_adi = p_p_adi, 
        p_soyadi = p_p_soyadi, 
        p_eposta = p_p_eposta, 
        p_adres = p_p_adres, 
        p_tel = p_p_tel, 
        p_tc = p_p_tc, 
        p_aktif_mi = p_p_aktif_mi, 
        rol_id = p_rol_id 
    WHERE personel_id = p_personel_id;
END //

CREATE PROCEDURE sp_PersonelSil (
    IN p_personel_id INT
)
BEGIN
    DELETE FROM personel WHERE personel_id = p_personel_id;
END //

CREATE PROCEDURE sp_PersonelListele ()
BEGIN
    SELECT * FROM personel;
END //

DELIMITER ;

DELIMITER //

-- =============================================
-- 1. SİPARİŞLER (ADİSYON) TABLOSU PROSEDÜRLERİ
-- =============================================
CREATE PROCEDURE sp_SiparisEkle (
    IN p_masa_id INT,           -- Gel-Al siparişleri için dışarıdan NULL gönderilebilir
    IN p_personel_id INT,
    IN p_durum VARCHAR(20)
)
BEGIN
    INSERT INTO siparisler (masa_id, personel_id, durum) 
    VALUES (p_masa_id, p_personel_id, p_durum);
END //

CREATE PROCEDURE sp_SiparisGuncelle (
    IN p_siparis_id INT,
    IN p_masa_id INT,
    IN p_personel_id INT,
    IN p_durum VARCHAR(20)
)
BEGIN
    UPDATE siparisler 
    SET masa_id = p_masa_id, 
        personel_id = p_personel_id, 
        durum = p_durum 
    WHERE siparis_id = p_siparis_id;
END //

CREATE PROCEDURE sp_SiparisSil (
    IN p_siparis_id INT
)
BEGIN
    DELETE FROM siparisler WHERE siparis_id = p_siparis_id;
END //

CREATE PROCEDURE sp_SiparisListele ()
BEGIN
    SELECT * FROM siparisler;
END //

-- =============================================
-- 2. SİPARİŞ DETAY (ÜRÜNLER) TABLOSU PROSEDÜRLERİ
-- =============================================
CREATE PROCEDURE sp_SiparisDetayEkle (
    IN p_siparis_id INT,
    IN p_urun_id INT,
    IN p_adet INT,
    IN p_satis_fiyati DECIMAL(10,2),
    IN p_siparis_notu VARCHAR(255)
)
BEGIN
    INSERT INTO siparis_detay (siparis_id, urun_id, adet, satis_fiyati, siparis_notu) 
    VALUES (p_siparis_id, p_urun_id, p_adet, p_satis_fiyati, p_siparis_notu);
END //

CREATE PROCEDURE sp_SiparisDetayGuncelle (
    IN p_siparis_detay_id INT,
    IN p_siparis_id INT,
    IN p_urun_id INT,
    IN p_adet INT,
    IN p_satis_fiyati DECIMAL(10,2),
    IN p_siparis_notu VARCHAR(255)
)
BEGIN
    UPDATE siparis_detay 
    SET siparis_id = p_siparis_id, 
        urun_id = p_urun_id, 
        adet = p_adet, 
        satis_fiyati = p_satis_fiyati, 
        siparis_notu = p_siparis_notu 
    WHERE siparis_detay_id = p_siparis_detay_id;
END //

CREATE PROCEDURE sp_SiparisDetaySil (
    IN p_siparis_detay_id INT
)
BEGIN
    DELETE FROM siparis_detay WHERE siparis_detay_id = p_siparis_detay_id;
END //

CREATE PROCEDURE sp_SiparisDetayListele ()
BEGIN
    SELECT * FROM siparis_detay;
END //

-- =============================================
-- 3. ÖDEMELER TABLOSU PROSEDÜRLERİ
-- =============================================
CREATE PROCEDURE sp_OdemeEkle (
    IN p_siparis_id INT,
    IN p_odeme_sekli VARCHAR(20),
    IN p_odenen_miktar DECIMAL(10,2)
)
BEGIN
    INSERT INTO odemeler (siparis_id, odeme_sekli, odenen_miktar) 
    VALUES (p_siparis_id, p_odeme_sekli, p_odenen_miktar);
END //

CREATE PROCEDURE sp_OdemeGuncelle (
    IN p_odeme_id INT,
    IN p_siparis_id INT,
    IN p_odeme_sekli VARCHAR(20),
    IN p_odenen_miktar DECIMAL(10,2)
)
BEGIN
    UPDATE odemeler 
    SET siparis_id = p_siparis_id, 
        odeme_sekli = p_odeme_sekli, 
        odenen_miktar = p_odenen_miktar 
    WHERE odeme_id = p_odeme_id;
END //

CREATE PROCEDURE sp_OdemeSil (
    IN p_odeme_id INT
)
BEGIN
    DELETE FROM odemeler WHERE odeme_id = p_odeme_id;
END //

CREATE PROCEDURE sp_OdemeListele ()
BEGIN
    SELECT * FROM odemeler;
END //

DELIMITER ;

DELIMITER //

-- ==============================================================================
-- 1. TRIGGER (EKLEME): Masaya yeni bir ürün eklendiğinde hesabı artırır
-- ==============================================================================
CREATE TRIGGER trg_SiparisDetay_Ekle
AFTER INSERT ON siparis_detay
FOR EACH ROW
BEGIN
    DECLARE v_masa_id INT;
    
    -- Önce bu siparişin hangi masaya ait olduğunu bulalım
    SELECT masa_id INTO v_masa_id 
    FROM siparisler 
    WHERE siparis_id = NEW.siparis_id;

    -- Eğer bu bir masa siparişi ise (Gel-Al değilse) masanın toplam hesabını artır
    IF v_masa_id IS NOT NULL THEN
        UPDATE masa
        SET masa_toplam_fiyati = masa_toplam_fiyati + (NEW.adet * NEW.satis_fiyati)
        WHERE masa_id = v_masa_id;
    END IF;
END //

-- ==============================================================================
-- 2. TRIGGER (SİLME): Müşteri bir ürünü iptal ettiğinde hesaptan düşer
-- ==============================================================================
CREATE TRIGGER trg_SiparisDetay_Sil
AFTER DELETE ON siparis_detay
FOR EACH ROW
BEGIN
    DECLARE v_masa_id INT;
    
    -- Silinen siparişin hangi masaya ait olduğunu bulalım
    SELECT masa_id INTO v_masa_id 
    FROM siparisler 
    WHERE siparis_id = OLD.siparis_id;

    -- Masanın toplam hesabından iptal edilen ürünün tutarını çıkar
    IF v_masa_id IS NOT NULL THEN
        UPDATE masa
        SET masa_toplam_fiyati = masa_toplam_fiyati - (OLD.adet * OLD.satis_fiyati)
        WHERE masa_id = v_masa_id;
    END IF;
END //

-- ==============================================================================
-- 3. TRIGGER (GÜNCELLEME): Sipariş adedi/fiyatı değiştiğinde hesabı eşitler
-- ==============================================================================
CREATE TRIGGER trg_SiparisDetay_Guncelle
AFTER UPDATE ON siparis_detay
FOR EACH ROW
BEGIN
    DECLARE v_masa_id INT;
    DECLARE v_fiyat_farki DECIMAL(10,2);
    
    -- Siparişin hangi masaya ait olduğunu bulalım
    SELECT masa_id INTO v_masa_id 
    FROM siparisler 
    WHERE siparis_id = NEW.siparis_id;

    IF v_masa_id IS NOT NULL THEN
        -- Yeni tutar ile eski tutar arasındaki farkı hesapla
        -- Örn: 1 Çay (10TL) iken 3 Çay (30TL) olduysa fark +20TL'dir.
        SET v_fiyat_farki = (NEW.adet * NEW.satis_fiyati) - (OLD.adet * OLD.satis_fiyati);

        -- Masanın mevcut hesabına bu farkı yansıt
        UPDATE masa
        SET masa_toplam_fiyati = masa_toplam_fiyati + v_fiyat_farki
        WHERE masa_id = v_masa_id;
    END IF;
END //

DELIMITER ;

DELIMITER //

-- ==============================================================================
-- 1. FONKSİYON: Belirli bir siparişin %20 KDV eklenmiş net tutarını hesaplar
-- ==============================================================================
CREATE FUNCTION fn_SiparisKDVliTutar(p_siparis_id INT) 
RETURNS DECIMAL(10,2)
READS SQL DATA
BEGIN
    DECLARE v_toplam_tutar DECIMAL(10,2);
    DECLARE v_kdvli_tutar DECIMAL(10,2);
    
    -- Sipariş detayındaki ürünlerin (adet * fiyat) toplamını alıyoruz. 
    -- COALESCE ile eğer sipariş boşsa NULL yerine 0 dönmesini sağlıyoruz.
    SELECT COALESCE(SUM(adet * satis_fiyati), 0) INTO v_toplam_tutar
    FROM siparis_detay
    WHERE siparis_id = p_siparis_id;
    
    -- Bulunan toplama %20 KDV ekliyoruz
    SET v_kdvli_tutar = v_toplam_tutar * 1.20;
    
    RETURN v_kdvli_tutar;
END //

-- ==============================================================================
-- 2. FONKSİYON: Bir personelin bugüne kadar kapattığı (ödemesi alınan) toplam ciroyu hesaplar
-- ==============================================================================
CREATE FUNCTION fn_PersonelToplamSatis(p_personel_id INT) 
RETURNS DECIMAL(10,2)
READS SQL DATA
BEGIN
    DECLARE v_toplam_ciro DECIMAL(10,2);
    
    -- Ödemeler tablosu ile Siparişler tablosunu birleştirip,
    -- sadece dışarıdan ID'si girilen personelin aldığı ödemeleri topluyoruz.
    SELECT COALESCE(SUM(o.odenen_miktar), 0) INTO v_toplam_ciro
    FROM odemeler o
    INNER JOIN siparisler s ON o.siparis_id = s.siparis_id
    WHERE s.personel_id = p_personel_id;
    
    RETURN v_toplam_ciro;
END //

-- ==============================================================================
-- 3. FONKSİYON (BONUS): Bir masanın kaç dakikadır dolu/aktif olduğunu hesaplar
-- ==============================================================================
CREATE FUNCTION fn_MasaDoluSuresi(p_masa_id INT) 
RETURNS INT
READS SQL DATA
BEGIN
    DECLARE v_gecen_dakika INT;
    
    -- Masanın en_son_islem_tarihi ile şu anki zaman (CURRENT_TIMESTAMP) 
    -- arasındaki farkı dakika (MINUTE) cinsinden buluyoruz.
    SELECT TIMESTAMPDIFF(MINUTE, en_son_islem_tarihi, CURRENT_TIMESTAMP) INTO v_gecen_dakika
    FROM masa
    WHERE masa_id = p_masa_id AND durum = 2; -- Sadece 'Dolu' (2) durumundaysa hesapla
    
    -- Eğer masa boşsa veya bulunamadıysa 0 döndür
    RETURN COALESCE(v_gecen_dakika, 0);
END //

DELIMITER ;

-- 1 Durumu: Boş (Yeşil yanacak)
-- 2 Durumu: Dolu (Kırmızı yanacak)
-- 3 Durumu: Rezerve (Turuncu yanacak)

CALL sp_MasaEkle('Masa 1', 1);
CALL sp_MasaEkle('Masa 2', 1);
CALL sp_MasaEkle('Masa 3', 2);
CALL sp_MasaEkle('Masa 4', 1);
CALL sp_MasaEkle('Bahçe 1', 3);
CALL sp_MasaEkle('Bahçe 2', 2);

