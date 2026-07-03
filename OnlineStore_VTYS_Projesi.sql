-- ============================================================
--  VTYS DÖNEM PROJESİ 2025-2026 BAHAR
--  Hayali Şirket: "TechShop" Online Mağaza
--  Veritabanı: MS SQL Server
-- ============================================================

-- ============================================================
-- 0. VERİTABANI OLUŞTURMA
-- ============================================================
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'TechShopDB')
    DROP DATABASE TechShopDB;
GO

CREATE DATABASE TechShopDB;
GO

USE TechShopDB;
GO

-- ============================================================
-- 1. TABLOLAR (CREATE TABLE) - 6 TABLO
-- ============================================================

-- 1.1 Kategoriler
CREATE TABLE Kategoriler (
    KategoriID   INT          IDENTITY(1,1) PRIMARY KEY,
    KategoriAdi  VARCHAR(100) NOT NULL,
    Aciklama     VARCHAR(255) DEFAULT 'Açıklama girilmedi'
);
GO

-- 1.2 Musteriler
CREATE TABLE Musteriler (
    MusteriID    INT          IDENTITY(1,1) PRIMARY KEY,
    Ad           VARCHAR(50)  NOT NULL,
    Soyad        VARCHAR(50)  NOT NULL,
    Email        VARCHAR(100) NOT NULL UNIQUE,
    Telefon      VARCHAR(20)  NOT NULL,
    KayitTarihi  DATETIME     DEFAULT GETDATE(),
    CONSTRAINT CHK_Email CHECK (Email LIKE '%@%.%')
);
GO

-- 1.3 Urunler
CREATE TABLE Urunler (
    UrunID       INT           IDENTITY(1,1) PRIMARY KEY,
    KategoriID   INT           NOT NULL,
    UrunAdi      VARCHAR(150)  NOT NULL,
    Fiyat        DECIMAL(10,2) NOT NULL,
    StokMiktari  INT           NOT NULL DEFAULT 0,
    AktifMi      BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FK_Urunler_Kategoriler FOREIGN KEY (KategoriID)
        REFERENCES Kategoriler(KategoriID),
    CONSTRAINT CHK_Fiyat    CHECK (Fiyat > 0),
    CONSTRAINT CHK_Stok     CHECK (StokMiktari >= 0)
);
GO

-- 1.4 Siparisler
CREATE TABLE Siparisler (
    SiparisID    INT           IDENTITY(1,1) PRIMARY KEY,
    MusteriID    INT           NOT NULL,
    SiparisTarihi DATETIME     DEFAULT GETDATE(),
    Durum        VARCHAR(30)   NOT NULL DEFAULT 'Beklemede',
    ToplamTutar  DECIMAL(10,2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_Siparisler_Musteriler FOREIGN KEY (MusteriID)
        REFERENCES Musteriler(MusteriID),
    CONSTRAINT CHK_Durum CHECK (Durum IN ('Beklemede','Hazirlaniyor','Kargoda','Teslim','Iptal'))
);
GO

-- 1.5 SiparisDetay
CREATE TABLE SiparisDetay (
    DetayID     INT           IDENTITY(1,1) PRIMARY KEY,
    SiparisID   INT           NOT NULL,
    UrunID      INT           NOT NULL,
    Miktar      INT           NOT NULL,
    BirimFiyat  DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_SiparisDetay_Siparisler FOREIGN KEY (SiparisID)
        REFERENCES Siparisler(SiparisID),
    CONSTRAINT FK_SiparisDetay_Urunler FOREIGN KEY (UrunID)
        REFERENCES Urunler(UrunID),
    CONSTRAINT CHK_Miktar     CHECK (Miktar > 0),
    CONSTRAINT CHK_BirimFiyat CHECK (BirimFiyat > 0)
);
GO

-- 1.6 Odemeler
CREATE TABLE Odemeler (
    OdemeID       INT           IDENTITY(1,1) PRIMARY KEY,
    SiparisID     INT           NOT NULL UNIQUE,
    OdemeMiktari  DECIMAL(10,2) NOT NULL,
    OdemeYontemi  VARCHAR(30)   NOT NULL DEFAULT 'Kredi Karti',
    OdemeTarihi   DATETIME      DEFAULT GETDATE(),
    CONSTRAINT FK_Odemeler_Siparisler FOREIGN KEY (SiparisID)
        REFERENCES Siparisler(SiparisID),
    CONSTRAINT CHK_OdemeYontemi CHECK (OdemeYontemi IN ('Kredi Karti','Havale','Kapida Odeme')),
    CONSTRAINT CHK_OdemeMiktari CHECK (OdemeMiktari > 0)
);
GO

-- ============================================================
-- 2. DML İŞLEMLERİ
-- ============================================================

-- --------------------
-- 2.1 INSERT
-- --------------------

INSERT INTO Kategoriler (KategoriAdi, Aciklama) VALUES
('Bilgisayar',   'Laptop ve masaüstü bilgisayarlar'),
('Telefon',      'Akıllı telefonlar ve aksesuar'),
('Ses Sistemleri','Kulaklık ve hoparlörler'),
('Çevre Birimleri','Mouse, klavye ve diğer ekipmanlar');
GO

INSERT INTO Musteriler (Ad, Soyad, Email, Telefon) VALUES
('Ahmet',    'Yılmaz',   'ahmet@mail.com',  '0532-1112233'),
('Selin',    'Kaya',     'selin@mail.com',  '0533-2223344'),
('Mert',     'Demir',    'mert@mail.com',   '0535-3334455'),
('Fatma',    'Arslan',   'fatma@mail.com',  '0542-4445566'),
('Hüseyin',  'Demirci',  'huseyin@mail.com','0545-5556677');
GO

INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, StokMiktari) VALUES
(1, 'Laptop Pro X',         24999.99,  15),
(1, 'Gaming Laptop Z',      34999.99,   8),
(2, 'Akıllı Telefon S20',   12999.99,  30),
(2, 'Akıllı Telefon A10',    7999.99,  25),
(3, 'Kablosuz Kulaklık',     2499.99,  50),
(3, 'Bluetooth Hoparlör',    1299.99,  40),
(4, 'Gaming Mouse',           899.99,  60),
(4, 'Mekanik Klavye',        1499.99,  35);
GO

INSERT INTO Siparisler (MusteriID, Durum) VALUES
(1, 'Teslim'),
(2, 'Hazirlaniyor'),
(3, 'Kargoda'),
(4, 'Beklemede'),
(1, 'Teslim');
GO

INSERT INTO SiparisDetay (SiparisID, UrunID, Miktar, BirimFiyat) VALUES
(1, 1, 1, 24999.99),
(1, 7, 2,   899.99),
(2, 5, 1,  2499.99),
(3, 3, 1, 12999.99),
(4, 8, 1,  1499.99),
(4, 6, 1,  1299.99),
(5, 2, 1, 34999.99);
GO

-- ToplamTutar güncelle
UPDATE Siparisler SET ToplamTutar = 24999.99 + (2 * 899.99) WHERE SiparisID = 1;
UPDATE Siparisler SET ToplamTutar = 2499.99  WHERE SiparisID = 2;
UPDATE Siparisler SET ToplamTutar = 12999.99 WHERE SiparisID = 3;
UPDATE Siparisler SET ToplamTutar = 1499.99 + 1299.99 WHERE SiparisID = 4;
UPDATE Siparisler SET ToplamTutar = 34999.99 WHERE SiparisID = 5;
GO

INSERT INTO Odemeler (SiparisID, OdemeMiktari, OdemeYontemi) VALUES
(1, 26799.97, 'Kredi Karti'),
(3, 12999.99, 'Havale'),
(5, 34999.99, 'Kredi Karti');
GO

-- --------------------
-- 2.2 UPDATE (Canlı Demo için)
-- --------------------
-- Sipariş durumu güncelleme
UPDATE Siparisler
SET Durum = 'Teslim'
WHERE SiparisID = 3;
GO

-- Ürün fiyatı güncelleme
UPDATE Urunler
SET Fiyat = 11999.99
WHERE UrunAdi = 'Akıllı Telefon S20';
GO

-- --------------------
-- 2.3 DELETE (Canlı Demo için)
-- --------------------
-- Önce detayı sil, sonra siparişi sil (FK kısıtı)
DELETE FROM SiparisDetay WHERE SiparisID = 4;
DELETE FROM Siparisler    WHERE SiparisID = 4;
GO

-- ============================================================
-- 3. PROGRAMLANABILIR NESNELER
-- ============================================================

-- --------------------
-- 3.1 VIEW - En Çok Satan Ürünler (birden fazla tablo JOIN)
-- --------------------
CREATE VIEW vw_UrunSatisRaporu AS
SELECT
    u.UrunID,
    u.UrunAdi,
    k.KategoriAdi,
    SUM(sd.Miktar)                        AS ToplamSatisMiktar,
    SUM(sd.Miktar * sd.BirimFiyat)        AS ToplamSatisTutar
FROM SiparisDetay sd
JOIN Urunler   u ON sd.UrunID    = u.UrunID
JOIN Kategoriler k ON u.KategoriID = k.KategoriID
JOIN Siparisler s  ON sd.SiparisID = s.SiparisID
WHERE s.Durum != 'Iptal'
GROUP BY u.UrunID, u.UrunAdi, k.KategoriAdi;
GO

-- VIEW - Son Siparişler
CREATE VIEW vw_SonSiparisler AS
SELECT TOP 20
    s.SiparisID,
    m.Ad + ' ' + m.Soyad   AS MusteriAdSoyad,
    u.UrunAdi,
    sd.Miktar,
    sd.BirimFiyat,
    s.SiparisTarihi,
    s.Durum
FROM Siparisler s
JOIN Musteriler  m  ON s.MusteriID  = m.MusteriID
JOIN SiparisDetay sd ON s.SiparisID = sd.SiparisID
JOIN Urunler u       ON sd.UrunID   = u.UrunID
ORDER BY s.SiparisTarihi DESC;
GO

-- VIEW - Müşteri Özet Raporu
CREATE VIEW vw_MusteriOzet AS
SELECT
    m.MusteriID,
    m.Ad + ' ' + m.Soyad AS MusteriAdSoyad,
    m.Email,
    COUNT(s.SiparisID)    AS ToplamSiparis,
    SUM(s.ToplamTutar)    AS ToplamHarcama
FROM Musteriler m
LEFT JOIN Siparisler s ON m.MusteriID = s.MusteriID
GROUP BY m.MusteriID, m.Ad, m.Soyad, m.Email;
GO

-- --------------------
-- 3.2 STORED PROCEDURE - Parametre alan
-- --------------------
-- SP 1: Müşteri siparişlerini getir
CREATE PROCEDURE usp_MusteriSiparisler
    @MusteriID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.SiparisID,
        s.SiparisTarihi,
        s.Durum,
        s.ToplamTutar,
        COUNT(sd.DetayID) AS UrunSayisi
    FROM Siparisler s
    LEFT JOIN SiparisDetay sd ON s.SiparisID = sd.SiparisID
    WHERE s.MusteriID = @MusteriID
    GROUP BY s.SiparisID, s.SiparisTarihi, s.Durum, s.ToplamTutar
    ORDER BY s.SiparisTarihi DESC;
END;
GO

-- SP 2: Aylık ciro raporu
CREATE PROCEDURE usp_AylikCiroRaporu
    @Yil INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Yil IS NULL SET @Yil = YEAR(GETDATE());

    SELECT
        MONTH(s.SiparisTarihi)          AS Ay,
        DATENAME(MONTH, s.SiparisTarihi) AS AyAdi,
        COUNT(DISTINCT s.SiparisID)      AS SiparisSayisi,
        SUM(s.ToplamTutar)               AS ToplamCiro
    FROM Siparisler s
    WHERE YEAR(s.SiparisTarihi) = @Yil
      AND s.Durum != 'Iptal'
    GROUP BY MONTH(s.SiparisTarihi), DATENAME(MONTH, s.SiparisTarihi)
    ORDER BY Ay;
END;
GO

-- SP 3: Yeni sipariş ekle
CREATE PROCEDURE usp_YeniSiparisEkle
    @MusteriID INT,
    @UrunID    INT,
    @Miktar    INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Fiyat      DECIMAL(10,2);
    DECLARE @SiparisID  INT;
    DECLARE @Toplam     DECIMAL(10,2);

    -- Fiyat al
    SELECT @Fiyat = Fiyat FROM Urunler WHERE UrunID = @UrunID;
    SET @Toplam = @Fiyat * @Miktar;

    -- Sipariş oluştur
    INSERT INTO Siparisler (MusteriID, Durum, ToplamTutar)
    VALUES (@MusteriID, 'Beklemede', @Toplam);
    SET @SiparisID = SCOPE_IDENTITY();

    -- Detay ekle
    INSERT INTO SiparisDetay (SiparisID, UrunID, Miktar, BirimFiyat)
    VALUES (@SiparisID, @UrunID, @Miktar, @Fiyat);

    SELECT @SiparisID AS YeniSiparisID, @Toplam AS ToplamTutar;
END;
GO

-- --------------------
-- 3.3 TRIGGER - DML sonrası otomatik devreye giren
-- --------------------
-- TRIGGER 1: Sipariş eklendiğinde stok düş
CREATE TRIGGER trg_StokDusur
ON SiparisDetay
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Urunler
    SET StokMiktari = StokMiktari - i.Miktar
    FROM Urunler u
    JOIN inserted i ON u.UrunID = i.UrunID;
END;
GO

-- TRIGGER 2: Sipariş silindiğinde stok geri ekle
CREATE TRIGGER trg_StokGeriEkle
ON SiparisDetay
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Urunler
    SET StokMiktari = StokMiktari + d.Miktar
    FROM Urunler u
    JOIN deleted d ON u.UrunID = d.UrunID;
END;
GO

-- TRIGGER 3: Stok sıfırlandığında ürünü pasife al
CREATE TRIGGER trg_StokBitince_Pasif
ON Urunler
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Urunler
    SET AktifMi = 0
    WHERE UrunID IN (SELECT UrunID FROM inserted WHERE StokMiktari = 0);
END;
GO

-- ============================================================
-- 4. KONTROL SORGULARI
-- ============================================================

-- VIEW sorgulama
SELECT * FROM vw_UrunSatisRaporu  ORDER BY ToplamSatisMiktar DESC;
SELECT * FROM vw_SonSiparisler;
SELECT * FROM vw_MusteriOzet;

-- SP çalıştırma
EXEC usp_MusteriSiparisler @MusteriID = 1;
EXEC usp_AylikCiroRaporu   @Yil = 2025;
EXEC usp_YeniSiparisEkle   @MusteriID = 2, @UrunID = 4, @Miktar = 1;

-- Trigger test (stok değişimini gözlemle)
SELECT UrunID, UrunAdi, StokMiktari FROM Urunler;
