-- ============================================================
-- TechnoCafe — CafeDB Veritabanı Kurulum Scripti
-- ============================================================

-- ============================================================
-- 1. VERİTABANI OLUŞTURMA
-- ============================================================

CREATE DATABASE IF NOT EXISTS CafeDB;
USE CafeDB;

-- ============================================================
-- 2. TABLOLARI OLUŞTURMA
-- ============================================================

-- Rol Tablosu
CREATE TABLE rol (
    rol_id   INT         AUTO_INCREMENT PRIMARY KEY,
    rol_adi  VARCHAR(50) NOT NULL,
    aktif_mi TINYINT     NOT NULL CHECK (aktif_mi IN (1, 2))  -- 1: Aktif, 2: Pasif
) AUTO_INCREMENT = 100;

-- Kategori Tablosu
CREATE TABLE kategori (
    kategori_id INT         AUTO_INCREMENT PRIMARY KEY,
    k_adi       VARCHAR(50) NOT NULL
);

-- Personel Tablosu
CREATE TABLE personel (
    personel_id  INT          AUTO_INCREMENT PRIMARY KEY,
    p_adi        VARCHAR(50)  NOT NULL,
    p_soyadi     VARCHAR(50)  NOT NULL,
    p_eposta     VARCHAR(100) NOT NULL UNIQUE,
    p_adres      VARCHAR(255) NOT NULL,
    p_tel        VARCHAR(15)  NOT NULL,
    p_tc         CHAR(11)     NOT NULL UNIQUE,
    p_aktif_mi   TINYINT      NOT NULL CHECK (p_aktif_mi IN (1, 2)),
    kayit_tarihi DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    rol_id       INT          NOT NULL,
    FOREIGN KEY (rol_id) REFERENCES rol(rol_id)
) AUTO_INCREMENT = 10;

-- Masa Tablosu
CREATE TABLE masa (
    masa_id              INT           AUTO_INCREMENT PRIMARY KEY,
    masa_no              VARCHAR(20)   NOT NULL,
    durum                TINYINT       NOT NULL CHECK (durum IN (1, 2, 3)),  -- 1: Boş, 2: Dolu, 3: Rezerve
    masa_toplam_fiyati   DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    en_son_islem_tarihi  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Ürünler Tablosu
CREATE TABLE urunler (
    urun_id    INT           AUTO_INCREMENT PRIMARY KEY,
    urun_adi   VARCHAR(100)  NOT NULL,
    urun_fiyati DECIMAL(10,2) NOT NULL,
    kategori_id INT          NOT NULL,
    FOREIGN KEY (kategori_id) REFERENCES kategori(kategori_id)
);

-- Siparişler Tablosu
CREATE TABLE siparisler (
    siparis_id     INT         AUTO_INCREMENT PRIMARY KEY,
    masa_id        INT         NULL,  -- Gel-Al siparişleri için NULL kalabilir
    personel_id    INT         NOT NULL,
    siparis_tarihi DATETIME    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    durum          VARCHAR(20) NOT NULL DEFAULT 'Açık',  -- Açık, Kapalı, İptal
    FOREIGN KEY (masa_id)     REFERENCES masa(masa_id),
    FOREIGN KEY (personel_id) REFERENCES personel(personel_id)
);

-- Sipariş Detay Tablosu
CREATE TABLE siparis_detay (
    siparis_detay_id INT           AUTO_INCREMENT PRIMARY KEY,
    siparis_id       INT           NOT NULL,
    urun_id          INT           NOT NULL,
    adet             INT           NOT NULL CHECK (adet > 0),
    satis_fiyati     DECIMAL(10,2) NOT NULL,
    siparis_notu     VARCHAR(255)  NULL,
    hazir_mi         TINYINT       NOT NULL DEFAULT 0,  -- 0: Bekliyor, 1: Hazırlandı
    FOREIGN KEY (siparis_id) REFERENCES siparisler(siparis_id),
    FOREIGN KEY (urun_id)    REFERENCES urunler(urun_id)
);

-- Ödemeler Tablosu
CREATE TABLE odemeler (
    odeme_id         INT           AUTO_INCREMENT PRIMARY KEY,
    siparis_id       INT           NOT NULL,
    odeme_sekli      VARCHAR(20)   NOT NULL,  -- Nakit, Kredi Kartı vb.
    odenen_miktar    DECIMAL(10,2) NOT NULL CHECK (odenen_miktar > 0),
    odeme_tarih_saat DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (siparis_id) REFERENCES siparisler(siparis_id)
);


-- ============================================================
-- 3. STORED PROCEDURE'LAR
-- ============================================================

DELIMITER //

-- -------------------------
-- ROL PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_RolEkle (
    IN p_rol_adi  VARCHAR(50),
    IN p_aktif_mi TINYINT
)
BEGIN
    INSERT INTO rol (rol_adi, aktif_mi)
    VALUES (p_rol_adi, p_aktif_mi);
END //

CREATE PROCEDURE sp_RolGuncelle (
    IN p_rol_id   INT,
    IN p_rol_adi  VARCHAR(50),
    IN p_aktif_mi TINYINT
)
BEGIN
    UPDATE rol
    SET rol_adi  = p_rol_adi,
        aktif_mi = p_aktif_mi
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

-- -------------------------
-- KATEGORİ PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_KategoriEkle (
    IN p_k_adi VARCHAR(50)
)
BEGIN
    INSERT INTO kategori (k_adi)
    VALUES (p_k_adi);
END //

CREATE PROCEDURE sp_KategoriGuncelle (
    IN p_kategori_id INT,
    IN p_k_adi       VARCHAR(50)
)
BEGIN
    UPDATE kategori
    SET k_adi = p_k_adi
    WHERE kategori_id = p_kategori_id;
END //

CREATE PROCEDURE sp_KategoriSil (
    IN p_kategori_id INT
)
BEGIN
    DELETE FROM kategori WHERE kategori_id = p_kategori_id;
END //

CREATE PROCEDURE sp_KategoriListele ()
BEGIN
    SELECT * FROM kategori;
END //

-- -------------------------
-- PERSONEL PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_PersonelEkle (
    IN p_p_adi      VARCHAR(50),
    IN p_p_soyadi   VARCHAR(50),
    IN p_p_eposta   VARCHAR(100),
    IN p_p_adres    VARCHAR(255),
    IN p_p_tel      VARCHAR(15),
    IN p_p_tc       CHAR(11),
    IN p_p_aktif_mi TINYINT,
    IN p_rol_id     INT
)
BEGIN
    INSERT INTO personel (p_adi, p_soyadi, p_eposta, p_adres, p_tel, p_tc, p_aktif_mi, rol_id)
    VALUES (p_p_adi, p_p_soyadi, p_p_eposta, p_p_adres, p_p_tel, p_p_tc, p_p_aktif_mi, p_rol_id);
END //

CREATE PROCEDURE sp_PersonelGuncelle (
    IN p_personel_id INT,
    IN p_p_adi       VARCHAR(50),
    IN p_p_soyadi    VARCHAR(50),
    IN p_p_eposta    VARCHAR(100),
    IN p_p_adres     VARCHAR(255),
    IN p_p_tel       VARCHAR(15),
    IN p_p_tc        CHAR(11),
    IN p_p_aktif_mi  TINYINT,
    IN p_rol_id      INT
)
BEGIN
    UPDATE personel
    SET p_adi      = p_p_adi,
        p_soyadi   = p_p_soyadi,
        p_eposta   = p_p_eposta,
        p_adres    = p_p_adres,
        p_tel      = p_p_tel,
        p_tc       = p_p_tc,
        p_aktif_mi = p_p_aktif_mi,
        rol_id     = p_rol_id
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

-- -------------------------
-- MASA PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_MasaEkle (
    IN p_masa_no VARCHAR(20),
    IN p_durum   TINYINT
)
BEGIN
    INSERT INTO masa (masa_no, durum, masa_toplam_fiyati)
    VALUES (p_masa_no, p_durum, 0.00);
END //

CREATE PROCEDURE sp_MasaGuncelle (
    IN p_masa_id             INT,
    IN p_masa_no             VARCHAR(20),
    IN p_durum               TINYINT,
    IN p_masa_toplam_fiyati  DECIMAL(10,2)
)
BEGIN
    UPDATE masa
    SET masa_no             = p_masa_no,
        durum               = p_durum,
        masa_toplam_fiyati  = p_masa_toplam_fiyati,
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

CREATE PROCEDURE sp_MasaDurumGuncelle (
    IN p_masa_id INT,
    IN p_durum   TINYINT
)
BEGIN
    UPDATE masa
    SET durum               = p_durum,
        en_son_islem_tarihi = CURRENT_TIMESTAMP
    WHERE masa_id = p_masa_id;
END //

CREATE PROCEDURE sp_MasaSifirla (
    IN p_masa_id INT
)
BEGIN
    UPDATE masa
    SET durum               = 1,
        masa_toplam_fiyati  = 0.00,
        en_son_islem_tarihi = CURRENT_TIMESTAMP
    WHERE masa_id = p_masa_id;
END //

-- -------------------------
-- ÜRÜN PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_UrunEkle (
    IN p_urun_adi    VARCHAR(100),
    IN p_urun_fiyati DECIMAL(10,2),
    IN p_kategori_id INT
)
BEGIN
    INSERT INTO urunler (urun_adi, urun_fiyati, kategori_id)
    VALUES (p_urun_adi, p_urun_fiyati, p_kategori_id);
END //

CREATE PROCEDURE sp_UrunGuncelle (
    IN p_urun_id     INT,
    IN p_urun_adi    VARCHAR(100),
    IN p_urun_fiyati DECIMAL(10,2),
    IN p_kategori_id INT
)
BEGIN
    UPDATE urunler
    SET urun_adi    = p_urun_adi,
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

CREATE PROCEDURE sp_UrunListeleByKategori (
    IN p_kategori_id INT
)
BEGIN
    SELECT * FROM urunler WHERE kategori_id = p_kategori_id;
END //

-- -------------------------
-- SİPARİŞ PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_SiparisEkle (
    IN p_masa_id     INT,
    IN p_personel_id INT,
    IN p_durum       VARCHAR(20)
)
BEGIN
    INSERT INTO siparisler (masa_id, personel_id, durum)
    VALUES (p_masa_id, p_personel_id, p_durum);
END //

CREATE PROCEDURE sp_SiparisGuncelle (
    IN p_siparis_id  INT,
    IN p_masa_id     INT,
    IN p_personel_id INT,
    IN p_durum       VARCHAR(20)
)
BEGIN
    UPDATE siparisler
    SET masa_id     = p_masa_id,
        personel_id = p_personel_id,
        durum       = p_durum
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

CREATE PROCEDURE sp_SiparislerByMasa (
    IN p_masa_id INT
)
BEGIN
    SELECT * FROM siparisler
    WHERE masa_id = p_masa_id AND durum = 'Açık';
END //

CREATE PROCEDURE sp_SonSiparisIdGetir ()
BEGIN
    SELECT MAX(siparis_id) AS son_id FROM siparisler;
END //

CREATE PROCEDURE sp_SiparisKapat (
    IN p_siparis_id INT
)
BEGIN
    UPDATE siparisler SET durum = 'Kapalı' WHERE siparis_id = p_siparis_id;
END //

-- -------------------------
-- SİPARİŞ DETAY PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_SiparisDetayEkle (
    IN p_siparis_id   INT,
    IN p_urun_id      INT,
    IN p_adet         INT,
    IN p_satis_fiyati DECIMAL(10,2),
    IN p_siparis_notu VARCHAR(255)
)
BEGIN
    INSERT INTO siparis_detay (siparis_id, urun_id, adet, satis_fiyati, siparis_notu)
    VALUES (p_siparis_id, p_urun_id, p_adet, p_satis_fiyati, p_siparis_notu);
END //

CREATE PROCEDURE sp_SiparisDetayGuncelle (
    IN p_siparis_detay_id INT,
    IN p_siparis_id       INT,
    IN p_urun_id          INT,
    IN p_adet             INT,
    IN p_satis_fiyati     DECIMAL(10,2),
    IN p_siparis_notu     VARCHAR(255)
)
BEGIN
    UPDATE siparis_detay
    SET siparis_id   = p_siparis_id,
        urun_id      = p_urun_id,
        adet         = p_adet,
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

CREATE PROCEDURE sp_SiparisDetayBySiparis (
    IN p_siparis_id INT
)
BEGIN
    SELECT sd.siparis_detay_id, sd.siparis_id, sd.urun_id, sd.adet,
           sd.satis_fiyati, sd.siparis_notu, u.urun_adi
    FROM siparis_detay sd
    INNER JOIN urunler u ON sd.urun_id = u.urun_id
    WHERE sd.siparis_id = p_siparis_id;
END //

CREATE PROCEDURE sp_AcikSiparisDetaylari ()
BEGIN
    SELECT sd.siparis_detay_id, sd.siparis_id, sd.urun_id, sd.adet,
           sd.satis_fiyati, sd.siparis_notu,
           u.urun_adi,
           COALESCE(m.masa_no, 'Gel-Al') AS masa_no
    FROM siparis_detay sd
    INNER JOIN siparisler s ON sd.siparis_id = s.siparis_id
    INNER JOIN urunler    u ON sd.urun_id    = u.urun_id
    LEFT  JOIN masa       m ON s.masa_id     = m.masa_id
    WHERE s.durum = 'Açık' AND sd.hazir_mi = 0
    ORDER BY sd.siparis_detay_id DESC;
END //

CREATE PROCEDURE sp_SiparisDetayHazirIsaretle (
    IN p_siparis_detay_id INT
)
BEGIN
    UPDATE siparis_detay
    SET hazir_mi = 1
    WHERE siparis_detay_id = p_siparis_detay_id;
END //

-- -------------------------
-- ÖDEME PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_OdemeEkle (
    IN p_siparis_id    INT,
    IN p_odeme_sekli   VARCHAR(20),
    IN p_odenen_miktar DECIMAL(10,2)
)
BEGIN
    INSERT INTO odemeler (siparis_id, odeme_sekli, odenen_miktar)
    VALUES (p_siparis_id, p_odeme_sekli, p_odenen_miktar);
END //

CREATE PROCEDURE sp_OdemeGuncelle (
    IN p_odeme_id      INT,
    IN p_siparis_id    INT,
    IN p_odeme_sekli   VARCHAR(20),
    IN p_odenen_miktar DECIMAL(10,2)
)
BEGIN
    UPDATE odemeler
    SET siparis_id    = p_siparis_id,
        odeme_sekli   = p_odeme_sekli,
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

-- -------------------------
-- FONKSİYON SARMALAYICI (WRAPPER) PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_SiparisKDVliTutarGetir (
    IN p_siparis_id INT
)
BEGIN
    SELECT fn_SiparisKDVliTutar(p_siparis_id) AS kdvli_tutar;
END //

CREATE PROCEDURE sp_PersonelToplamSatisGetir (
    IN p_personel_id INT
)
BEGIN
    SELECT fn_PersonelToplamSatis(p_personel_id) AS toplam_satis;
END //

CREATE PROCEDURE sp_MasaDoluSuresiGetir (
    IN p_masa_id INT
)
BEGIN
    SELECT fn_MasaDoluSuresi(p_masa_id) AS dolu_suresi;
END //

-- -------------------------
-- RAPORLAMA PROSEDÜRLERİ
-- -------------------------

CREATE PROCEDURE sp_GunlukSatisRaporu ()
BEGIN
    SELECT DATE(o.odeme_tarih_saat) AS tarih,
           o.odeme_sekli,
           COUNT(*)                 AS islem_sayisi,
           SUM(o.odenen_miktar)     AS toplam_tutar
    FROM odemeler o
    WHERE DATE(o.odeme_tarih_saat) = CURDATE()
    GROUP BY DATE(o.odeme_tarih_saat), o.odeme_sekli;
END //

CREATE PROCEDURE sp_PersonelSatisRaporu ()
BEGIN
    SELECT p.personel_id, p.p_adi, p.p_soyadi,
           fn_PersonelToplamSatis(p.personel_id) AS toplam_satis
    FROM personel p
    WHERE p.p_aktif_mi = 1
    ORDER BY toplam_satis DESC;
END //

DELIMITER ;


-- ============================================================
-- 4. TRIGGER'LAR
-- ============================================================

DELIMITER //

-- Masaya yeni ürün eklendiğinde masa hesabını artırır
CREATE TRIGGER trg_SiparisDetay_Ekle
AFTER INSERT ON siparis_detay
FOR EACH ROW
BEGIN
    DECLARE v_masa_id INT;

    SELECT masa_id INTO v_masa_id
    FROM siparisler
    WHERE siparis_id = NEW.siparis_id;

    IF v_masa_id IS NOT NULL THEN
        UPDATE masa
        SET masa_toplam_fiyati = masa_toplam_fiyati + (NEW.adet * NEW.satis_fiyati)
        WHERE masa_id = v_masa_id;
    END IF;
END //

-- Ürün iptal edildiğinde masa hesabından düşer
CREATE TRIGGER trg_SiparisDetay_Sil
AFTER DELETE ON siparis_detay
FOR EACH ROW
BEGIN
    DECLARE v_masa_id INT;

    SELECT masa_id INTO v_masa_id
    FROM siparisler
    WHERE siparis_id = OLD.siparis_id;

    IF v_masa_id IS NOT NULL THEN
        UPDATE masa
        SET masa_toplam_fiyati = masa_toplam_fiyati - (OLD.adet * OLD.satis_fiyati)
        WHERE masa_id = v_masa_id;
    END IF;
END //

-- Sipariş adedi veya fiyatı değiştiğinde masa hesabını günceller
CREATE TRIGGER trg_SiparisDetay_Guncelle
AFTER UPDATE ON siparis_detay
FOR EACH ROW
BEGIN
    DECLARE v_masa_id      INT;
    DECLARE v_fiyat_farki  DECIMAL(10,2);

    SELECT masa_id INTO v_masa_id
    FROM siparisler
    WHERE siparis_id = NEW.siparis_id;

    IF v_masa_id IS NOT NULL THEN
        SET v_fiyat_farki = (NEW.adet * NEW.satis_fiyati) - (OLD.adet * OLD.satis_fiyati);

        UPDATE masa
        SET masa_toplam_fiyati = masa_toplam_fiyati + v_fiyat_farki
        WHERE masa_id = v_masa_id;
    END IF;
END //

DELIMITER ;


-- ============================================================
-- 5. FONKSİYONLAR
-- ============================================================

DELIMITER //

-- Belirli bir siparişin %20 KDV eklenmiş tutarını hesaplar
CREATE FUNCTION fn_SiparisKDVliTutar (p_siparis_id INT)
RETURNS DECIMAL(10,2)
READS SQL DATA
BEGIN
    DECLARE v_toplam  DECIMAL(10,2);

    SELECT COALESCE(SUM(adet * satis_fiyati), 0) INTO v_toplam
    FROM siparis_detay
    WHERE siparis_id = p_siparis_id;

    RETURN v_toplam * 1.20;
END //

-- Bir personelin tüm zamanlarda aldığı toplam ödeme tutarını hesaplar
CREATE FUNCTION fn_PersonelToplamSatis (p_personel_id INT)
RETURNS DECIMAL(10,2)
READS SQL DATA
BEGIN
    DECLARE v_toplam DECIMAL(10,2);

    SELECT COALESCE(SUM(o.odenen_miktar), 0) INTO v_toplam
    FROM odemeler o
    INNER JOIN siparisler s ON o.siparis_id = s.siparis_id
    WHERE s.personel_id = p_personel_id;

    RETURN v_toplam;
END //

-- Bir masanın kaç dakikadır dolu olduğunu hesaplar
CREATE FUNCTION fn_MasaDoluSuresi (p_masa_id INT)
RETURNS INT
READS SQL DATA
BEGIN
    DECLARE v_dakika INT;

    SELECT TIMESTAMPDIFF(MINUTE, en_son_islem_tarihi, CURRENT_TIMESTAMP) INTO v_dakika
    FROM masa
    WHERE masa_id = p_masa_id AND durum = 2;

    RETURN COALESCE(v_dakika, 0);
END //

DELIMITER ;


-- ============================================================
-- 6. ÖRNEK VERİLER
-- ============================================================

-- Roller
CALL sp_RolEkle('Yönetici', 1);  -- rol_id: 100
CALL sp_RolEkle('Garson',   1);  -- rol_id: 101
CALL sp_RolEkle('Barista',  1);  -- rol_id: 102

-- Personeller
CALL sp_PersonelEkle('Ahmet', 'Yılmaz', 'ahmet@technocafe.com', 'İstanbul Kadıköy',  '5551234567', '12345678901', 1, 100);
CALL sp_PersonelEkle('Mehmet', 'Kaya',  'mehmet@technocafe.com','İstanbul Üsküdar',  '5559876543', '98765432109', 1, 101);
CALL sp_PersonelEkle('Ayşe',  'Demir', 'ayse@technocafe.com',  'İstanbul Beşiktaş', '5554567890', '45678901234', 1, 102);
CALL sp_PersonelEkle('Fatma', 'Çelik', 'fatma@technocafe.com', 'İstanbul Şişli',    '5553214567', '56789012345', 1, 101);

-- Kategoriler
CALL sp_KategoriEkle('Sıcak İçecekler');   -- kategori_id: 1
CALL sp_KategoriEkle('Soğuk İçecekler');   -- kategori_id: 2
CALL sp_KategoriEkle('Tatlılar');          -- kategori_id: 3
CALL sp_KategoriEkle('Atıştırmalıklar');   -- kategori_id: 4

-- Ürünler
CALL sp_UrunEkle('Filtre Kahve',   45.00, 1);
CALL sp_UrunEkle('Türk Kahvesi',   50.00, 1);
CALL sp_UrunEkle('Latte',          75.00, 1);
CALL sp_UrunEkle('Cappuccino',     70.00, 1);
CALL sp_UrunEkle('Americano',      55.00, 1);
CALL sp_UrunEkle('Çay',            25.00, 1);
CALL sp_UrunEkle('Sıcak Çikolata', 65.00, 1);
CALL sp_UrunEkle('Ice Latte',      85.00, 2);
CALL sp_UrunEkle('Ice Americano',  65.00, 2);
CALL sp_UrunEkle('Limonata',       55.00, 2);
CALL sp_UrunEkle('Smoothie',       80.00, 2);
CALL sp_UrunEkle('Soğuk Çay',      35.00, 2);
CALL sp_UrunEkle('Cheesecake',     95.00, 3);
CALL sp_UrunEkle('Brownie',        75.00, 3);
CALL sp_UrunEkle('Tiramisu',       90.00, 3);
CALL sp_UrunEkle('Sufle',          85.00, 3);
CALL sp_UrunEkle('Sandviç',        90.00, 4);
CALL sp_UrunEkle('Tost',           65.00, 4);
CALL sp_UrunEkle('Kruvasan',       55.00, 4);
CALL sp_UrunEkle('Kurabiye',       35.00, 4);

-- Masalar
CALL sp_MasaEkle('Masa 1',   1);
CALL sp_MasaEkle('Masa 2',   1);
CALL sp_MasaEkle('Masa 3',   1);
CALL sp_MasaEkle('Masa 4',   1);
CALL sp_MasaEkle('Bahçe 1',  1);
CALL sp_MasaEkle('Bahçe 2',  1);
