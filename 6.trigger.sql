-- Stok öncesi
SELECT UrunID, UrunAdi, StokMiktari FROM Urunler WHERE UrunID = 1;

-- Yeni sipariþ ekle (Trigger otomatik devreye girer)
EXEC usp_YeniSiparisEkle @MusteriID = 1, @UrunID = 1, @Miktar = 1;

-- Stok sonrasý (1 azalmýþ olmalý)
SELECT UrunID, UrunAdi, StokMiktari FROM Urunler WHERE UrunID = 1;