# ☕ TechnoCafe POS — Kafe Otomasyon Sistemi

> BTS304 Veritabanı Yönetim Sistemleri II | Final Ödevi  
> .NET MAUI (iOS / Android / macOS / Windows) + MySQL

---

## 📌 Proje Hakkında

TechnoCafe, şehir merkezinde faaliyet gösteren teknoloji temalı bir kafenin sipariş, ödeme ve personel yönetimini dijitalleştiren bir POS (Satış Noktası) uygulamasıdır.

Uygulama **N-Katmanlı Mimari** ile geliştirilmiş olup tüm veritabanı işlemleri yalnızca **Stored Procedure** üzerinden yapılmaktadır.

---

## 🏗️ Mimari

```
UI (Presentation Layer)
    ↓
BL (Business Layer)
    ↓
DAL (Data Access Layer)
    ↓
MySQL — Stored Procedures
```

---

## 🗂️ Proje Yapısı

```
CafeApp/
├── UI/                   # Ekranlar (XAML + code-behind)
│   ├── LoginPage
│   ├── GarsonPage
│   ├── BaristaPage
│   ├── YoneticiPage
│   ├── SiparisAlmaPage
│   ├── OdemePage
│   ├── KategoriYonetimPage
│   ├── UrunYonetimPage
│   └── PersonelYonetimPage
├── BL/                   # İş Katmanı
├── DAL/                  # Veri Erişim Katmanı
├── Models/               # Veri modelleri
├── Converters/           # XAML dönüştürücüler
└── Script_Son.sql        # Veritabanı kurulum scripti
```

---

## 🗄️ Veritabanı

### Tablolar
| Tablo | Açıklama |
|-------|----------|
| `rol` | Yönetici, Garson, Barista rolleri |
| `personel` | Çalışan bilgileri |
| `kategori` | Ürün kategorileri |
| `urunler` | Menü ürünleri |
| `masa` | Masa durumları ve toplam tutarlar |
| `siparisler` | Adisyon kayıtları |
| `siparis_detay` | Sipariş kalemleri |
| `odemeler` | Ödeme kayıtları |

### Stored Procedure'lar
Her tablo için **INSERT / UPDATE / DELETE / SELECT** işlemleri SP ile yapılmaktadır.

### Trigger'lar
| Trigger | Olay | İşlev |
|---------|------|-------|
| `trg_SiparisDetay_Ekle` | AFTER INSERT | Masanın toplam tutarını artırır |
| `trg_SiparisDetay_Sil` | AFTER DELETE | Masanın toplam tutarını düşürür |
| `trg_SiparisDetay_Guncelle` | AFTER UPDATE | Masanın toplam tutarını günceller |

### Fonksiyonlar
| Fonksiyon | İşlev |
|-----------|-------|
| `fn_SiparisKDVliTutar` | Siparişe %20 KDV eklenmiş tutarı döndürür |
| `fn_PersonelToplamSatis` | Personelin toplam satış cirosunu döndürür |
| `fn_MasaDoluSuresi` | Masanın kaç dakikadır dolu olduğunu döndürür |

---

## 👥 Kullanıcı Rolleri

| Rol | Yapabilecekleri |
|-----|----------------|
| **Garson** | Sipariş alma, ödeme alma, masa yönetimi |
| **Barista** | Açık siparişleri görme, ürünleri hazır işaretleme |
| **Yönetici** | Kategori/ürün/personel yönetimi, raporlar (şifreli giriş) |

---

## ⚙️ Kurulum

### Gereksinimler
- .NET 9 SDK
- XAMPP (MySQL 3307 portu)
- Visual Studio 2022 veya Rider

### Veritabanı Kurulumu
1. XAMPP'ı başlatın (MySQL aktif olsun)
2. phpMyAdmin'i açın
3. `Script_Son.sql` dosyasını import edin
4. `CafeDB` veritabanı ve örnek veriler otomatik oluşur

### Bağlantı Ayarı
`DAL/CafeVeriErisim.cs` dosyasında bağlantı bilgilerini güncelleyin:
```csharp
_connString = $"Server={server};Port=3307;Database=CafeDB;Uid=root;Pwd=;";
```

### Uygulamayı Çalıştırma
```bash
dotnet build
dotnet run
```
Ya da Visual Studio'da hedef platform seçerek (iOS / Android / Windows / macOS) çalıştırın.

---

## 🔔 Özellikler

- ✅ N-Katmanlı mimari (UI → BL → DAL)
- ✅ Tüm DB işlemleri Stored Procedure üzerinden
- ✅ Trigger ile otomatik masa tutarı güncelleme
- ✅ KDV hesaplama ve personel satış fonksiyonları
- ✅ Barista → Garson hazır sipariş bildirim sistemi (10 sn polling)
- ✅ Yönetici girişi şifre korumalı
- ✅ iOS, Android, macOS ve Windows desteği
