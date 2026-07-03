using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TechShopApp
{
    public class Form1 : Form
    {
        // ── Sol menü ────────────────────────────────────────────────
        private Panel panelMenu;
        private Label lblMenuBaslik;
        private Button btnAnaSayfa, btnMusteriler, btnUrunler,
                         btnSiparisler, btnRaporlar, btnAyarlar;

        // ── İçerik paneli ───────────────────────────────────────────
        private Panel panelIcerik;

        // ── Bağlantı göstergesi ─────────────────────────────────────
        private Label lblBaglanti;

        // ── Müşteri ekranı kontrolleri ──────────────────────────────
        private Label lblAd, lblSoyad, lblEmail, lblTelefon;
        private TextBox txtAd, txtSoyad, txtEmail, txtTelefon;
        private Button btnEkle, btnGuncelle, btnSil;
        private DataGridView dgvMusteriler;

        // ── Ürünler ekranı ──────────────────────────────────────────
        private DataGridView dgvUrunler;

        // ── Siparişler ekranı ───────────────────────────────────────
        private DataGridView dgvSiparisler;

        // ── Raporlar ekranı ─────────────────────────────────────────
        private DataGridView dgvRapor1, dgvRapor2, dgvRapor3;

        // Seçili müşteri
        private int _secilenMusteriID = -1;

        // ────────────────────────────────────────────────────────────
        public Form1()
        {
            InitializeComponent();
            BaglantiKontrol();
            AnaSayfaGoster();
        }

        // ============================================================
        // BAĞLANTI KONTROLÜ
        // ============================================================
        private void BaglantiKontrol()
        {
            bool ok = DatabaseHelper.TestConnection();
            lblBaglanti.Text = ok ? "● Veritabanı Bağlantısı: AKTİF – TechShopDB"
                                       : "● Veritabanı Bağlantısı: HATA";
            lblBaglanti.ForeColor = ok ? Color.LimeGreen : Color.Red;
        }

        // ============================================================
        // MENÜ – ekran temizle
        // ============================================================
        private void IcerikTemizle()
        {
            panelIcerik.Controls.Clear();
            // Bağlantı etiketini her zaman üstte tut
            panelIcerik.Controls.Add(lblBaglanti);
        }

        private void MenuButonRenkSifirla()
        {
            foreach (Control c in panelMenu.Controls)
                if (c is Button b) b.BackColor = Color.FromArgb(30, 30, 60);
        }

        // ============================================================
        // ANA SAYFA
        // ============================================================
        private void AnaSayfaGoster()
        {
            IcerikTemizle();
            MenuButonRenkSifirla();
            btnAnaSayfa.BackColor = Color.FromArgb(0, 122, 204);

            Label lbl = new Label
            {
                Text = "🛒  TechShop Yönetim Sistemine Hoşgeldiniz",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 80)
            };
            Label lbl2 = new Label
            {
                Text = "Sol menüden bir bölüm seçin.",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 11f),
                AutoSize = true,
                Location = new Point(30, 125)
            };
            panelIcerik.Controls.Add(lbl);
            panelIcerik.Controls.Add(lbl2);
        }

        // ============================================================
        // MÜŞTERİLER EKRANI
        // ============================================================
        private void MusterilerGoster()
        {
            IcerikTemizle();
            MenuButonRenkSifirla();
            btnMusteriler.BackColor = Color.FromArgb(0, 122, 204);

            Label baslik = MakeBaslik("👤  Müşteri Kayıt Formu", new Point(20, 55));

            lblAd = MakeLabel("Ad:", new Point(20, 95));
            txtAd = MakeTextBox(new Point(160, 92));
            lblSoyad = MakeLabel("Soyad:", new Point(20, 130));
            txtSoyad = MakeTextBox(new Point(160, 127));
            lblEmail = MakeLabel("E-posta:", new Point(20, 165));
            txtEmail = MakeTextBox(new Point(160, 162));
            lblTelefon = MakeLabel("Telefon:", new Point(20, 200));
            txtTelefon = MakeTextBox(new Point(160, 197));

            btnEkle = MakeBtn("+ Ekle", Color.FromArgb(0, 150, 0), new Point(20, 240));
            btnGuncelle = MakeBtn("Güncelle", Color.FromArgb(200, 130, 0), new Point(120, 240));
            btnSil = MakeBtn("Sil", Color.FromArgb(180, 0, 0), new Point(220, 240));

            btnEkle.Click += BtnEkle_Click;
            btnGuncelle.Click += BtnGuncelle_Click;
            btnSil.Click += BtnSil_Click;

            Label lblKayitlar = new Label
            {
                Text = "Mevcut Kayıtlar",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Location = new Point(20, 285),
                AutoSize = true
            };

            dgvMusteriler = MakeGrid(new Point(20, 310),
                new Size(panelIcerik.Width - 40, panelIcerik.Height - 330));
            dgvMusteriler.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                                   AnchorStyles.Right | AnchorStyles.Bottom;
            dgvMusteriler.SelectionChanged += DgvMusteriler_SelectionChanged;

            panelIcerik.Controls.AddRange(new Control[]
            {
                baslik, lblAd, txtAd, lblSoyad, txtSoyad,
                lblEmail, txtEmail, lblTelefon, txtTelefon,
                btnEkle, btnGuncelle, btnSil, lblKayitlar, dgvMusteriler
            });

            MusterileriYukle();
        }

        private void MusterileriYukle()
        {
            string sql = "SELECT MusteriID AS ID, Ad, Soyad, Email AS [E-posta], " +
                         "Telefon, CONVERT(VARCHAR,KayitTarihi,104) AS [Kayıt Tarihi] " +
                         "FROM Musteriler ORDER BY MusteriID";
            dgvMusteriler.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void DgvMusteriler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMusteriler.CurrentRow == null) return;
            var row = dgvMusteriler.CurrentRow;
            _secilenMusteriID = Convert.ToInt32(row.Cells["ID"].Value);
            txtAd.Text = row.Cells["Ad"].Value.ToString();
            txtSoyad.Text = row.Cells["Soyad"].Value.ToString();
            txtEmail.Text = row.Cells["E-posta"].Value.ToString();
            txtTelefon.Text = row.Cells["Telefon"].Value.ToString();
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (!FormGecerli()) return;
            string sql = "INSERT INTO Musteriler (Ad,Soyad,Email,Telefon) VALUES (@A,@S,@E,@T)";
            SqlParameter[] p = {
                new SqlParameter("@A", txtAd.Text.Trim()),
                new SqlParameter("@S", txtSoyad.Text.Trim()),
                new SqlParameter("@E", txtEmail.Text.Trim()),
                new SqlParameter("@T", txtTelefon.Text.Trim())
            };
            if (DatabaseHelper.ExecuteNonQuery(sql, p) > 0)
            {
                MessageBox.Show("✅ Müşteri eklendi!", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormTemizle(); MusterileriYukle();
            }
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (_secilenMusteriID < 0) { MessageBox.Show("Listeden müşteri seçin."); return; }
            if (!FormGecerli()) return;
            string sql = "UPDATE Musteriler SET Ad=@A,Soyad=@S,Email=@E,Telefon=@T WHERE MusteriID=@ID";
            SqlParameter[] p = {
                new SqlParameter("@A",  txtAd.Text.Trim()),
                new SqlParameter("@S",  txtSoyad.Text.Trim()),
                new SqlParameter("@E",  txtEmail.Text.Trim()),
                new SqlParameter("@T",  txtTelefon.Text.Trim()),
                new SqlParameter("@ID", _secilenMusteriID)
            };
            if (DatabaseHelper.ExecuteNonQuery(sql, p) > 0)
            {
                MessageBox.Show("✅ Güncellendi!", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormTemizle(); MusterileriYukle();
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (_secilenMusteriID < 0) { MessageBox.Show("Listeden müşteri seçin."); return; }
            if (MessageBox.Show("Silmek istediğinizden emin misiniz?", "Onay",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            string sql = "DELETE FROM Musteriler WHERE MusteriID=@ID";
            SqlParameter[] p = { new SqlParameter("@ID", _secilenMusteriID) };
            if (DatabaseHelper.ExecuteNonQuery(sql, p) > 0)
            {
                MessageBox.Show("✅ Silindi!", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormTemizle(); MusterileriYukle();
            }
        }

        private bool FormGecerli()
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text) ||
                string.IsNullOrWhiteSpace(txtSoyad.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtTelefon.Text))
            { MessageBox.Show("Tüm alanları doldurun."); return false; }
            return true;
        }

        private void FormTemizle()
        {
            txtAd.Text = txtSoyad.Text = txtEmail.Text = txtTelefon.Text = "";
            _secilenMusteriID = -1;
        }

        // ============================================================
        // ÜRÜNLER EKRANI
        // ============================================================
        private void UrunlerGoster()
        {
            IcerikTemizle();
            MenuButonRenkSifirla();
            btnUrunler.BackColor = Color.FromArgb(0, 122, 204);

            Label baslik = MakeBaslik("📦  Ürün Listesi", new Point(20, 55));
            Label not = new Label
            {
                Text = "Stok miktarı Trigger ile otomatik güncellenir.",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(20, 90)
            };

            dgvUrunler = MakeGrid(new Point(20, 115),
                new Size(panelIcerik.Width - 40, panelIcerik.Height - 135));
            dgvUrunler.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                                AnchorStyles.Right | AnchorStyles.Bottom;

            panelIcerik.Controls.AddRange(new Control[] { baslik, not, dgvUrunler });

            string sql = @"SELECT u.UrunID AS ID, u.UrunAdi AS [Ürün Adı],
                           k.KategoriAdi AS Kategori,
                           CAST(u.Fiyat AS VARCHAR)+' TL' AS Fiyat,
                           u.StokMiktari AS Stok,
                           CASE WHEN u.AktifMi=1 THEN 'Aktif' ELSE 'Pasif' END AS Durum
                           FROM Urunler u JOIN Kategoriler k ON u.KategoriID=k.KategoriID
                           ORDER BY u.UrunID";
            dgvUrunler.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        // ============================================================
        // SİPARİŞLER EKRANI
        // ============================================================
        private void SiparislerGoster()
        {
            IcerikTemizle();
            MenuButonRenkSifirla();
            btnSiparisler.BackColor = Color.FromArgb(0, 122, 204);

            Label baslik = MakeBaslik("🛍️  Siparişler", new Point(20, 55));

            dgvSiparisler = MakeGrid(new Point(20, 95),
                new Size(panelIcerik.Width - 40, panelIcerik.Height - 115));
            dgvSiparisler.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                                   AnchorStyles.Right | AnchorStyles.Bottom;

            panelIcerik.Controls.AddRange(new Control[] { baslik, dgvSiparisler });

            string sql = @"SELECT s.SiparisID AS [Sip.No],
                           m.Ad+' '+m.Soyad AS Müşteri,
                           u.UrunAdi AS Ürün,
                           sd.Miktar AS Adet,
                           CAST(s.ToplamTutar AS VARCHAR)+' TL' AS Tutar,
                           CONVERT(VARCHAR,s.SiparisTarihi,104) AS Tarih,
                           s.Durum
                           FROM Siparisler s
                           JOIN Musteriler m ON s.MusteriID=m.MusteriID
                           JOIN SiparisDetay sd ON s.SiparisID=sd.SiparisID
                           JOIN Urunler u ON sd.UrunID=u.UrunID
                           ORDER BY s.SiparisTarihi DESC";
            dgvSiparisler.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        // ============================================================
        // RAPORLAR EKRANI — 3 rapor otomatik yüklenir
        // ============================================================
        private void RaporlarGoster()
        {
            IcerikTemizle();
            MenuButonRenkSifirla();
            btnRaporlar.BackColor = Color.FromArgb(0, 122, 204);

            Label baslik = MakeBaslik("📊  Raporlar – Sayfa Açıldığında Otomatik Yüklenir", new Point(20, 55));
            Label aciklama = new Label
            {
                Text = "Aşağıdaki raporlar VIEW ve Stored Procedure üzerinden otomatik olarak yüklenmektedir.",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(20, 90)
            };

            // ── Rapor 1 ──
            Label lblR1 = MakeRaporBaslik("Rapor 1: En Çok Satan Ürünler   |   Kaynak: vw_UrunSatisRaporu", new Point(20, 115));
            dgvRapor1 = MakeGrid(new Point(20, 138), new Size(panelIcerik.Width - 40, 130));
            dgvRapor1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // ── Rapor 2 ──
            Label lblR2 = MakeRaporBaslik("Rapor 2: Aylık Ciro Özeti   |   Kaynak: usp_AylikCiroRaporu", new Point(20, 280));
            dgvRapor2 = MakeGrid(new Point(20, 303), new Size(panelIcerik.Width - 40, 130));
            dgvRapor2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // ── Rapor 3 ──
            Label lblR3 = MakeRaporBaslik("Rapor 3: Son Siparişler   |   Kaynak: vw_SonSiparisler", new Point(20, 445));
            dgvRapor3 = MakeGrid(new Point(20, 468), new Size(panelIcerik.Width - 40, 150));
            dgvRapor3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            panelIcerik.Controls.AddRange(new Control[]
            {
                baslik, aciklama,
                lblR1, dgvRapor1,
                lblR2, dgvRapor2,
                lblR3, dgvRapor3
            });

            // Verileri yükle
            dgvRapor1.DataSource = DatabaseHelper.ExecuteQuery(
                "SELECT UrunAdi AS [Ürün Adı], KategoriAdi AS Kategori, " +
                "ToplamSatisMiktar AS [Satış Adedi], " +
                "CAST(ToplamSatisTutar AS DECIMAL(10,2)) AS [Toplam Ciro (TL)] " +
                "FROM vw_UrunSatisRaporu ORDER BY ToplamSatisMiktar DESC");

            dgvRapor2.DataSource = DatabaseHelper.ExecuteStoredProcedure(
                "usp_AylikCiroRaporu",
                new SqlParameter[] { new SqlParameter("@Yil", DateTime.Now.Year) });

            DataTable dt3 = DatabaseHelper.ExecuteQuery(
                "SELECT SiparisID AS [Sip.No], MusteriAdSoyad AS Müşteri, " +
                "UrunAdi AS Ürün, Miktar AS Adet, " +
                "CAST(BirimFiyat AS DECIMAL(10,2)) AS [Birim Fiyat], " +
                "CONVERT(VARCHAR,SiparisTarihi,104) AS Tarih, Durum " +
                "FROM vw_SonSiparisler");

            // Durum sütununa renk
            dgvRapor3.DataSource = dt3;
            dgvRapor3.CellFormatting += (s, ev) =>
            {
                if (ev.ColumnIndex < 0 || ev.RowIndex < 0) return;
                if (dgvRapor3.Columns[ev.ColumnIndex].Name == "Durum")
                {
                    string val = ev.Value?.ToString() ?? "";
                    if (val == "Teslim") ev.CellStyle.ForeColor = Color.LimeGreen;
                    else if (val == "Kargoda") ev.CellStyle.ForeColor = Color.Orange;
                    else if (val == "Hazirlaniyor") ev.CellStyle.ForeColor = Color.Cyan;
                    else if (val == "Beklemede") ev.CellStyle.ForeColor = Color.Yellow;
                    else if (val == "Iptal") ev.CellStyle.ForeColor = Color.Red;
                    else ev.CellStyle.ForeColor = Color.White;
                }
            };
        }

        // ============================================================
        // AYARLAR EKRANI
        // ============================================================
        private void AyarlarGoster()
        {
            IcerikTemizle();
            MenuButonRenkSifirla();
            btnAyarlar.BackColor = Color.FromArgb(0, 122, 204);

            Label baslik = MakeBaslik("⚙️  Ayarlar", new Point(20, 55));
            Label bilgi = new Label
            {
                Text = "Veritabanı: TechShopDB\nSunucu: (local)\nUygulama: TechShop v1.0",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10f),
                AutoSize = true,
                Location = new Point(20, 100)
            };
            panelIcerik.Controls.AddRange(new Control[] { baslik, bilgi });
        }

        // ============================================================
        // INITIALIZECOMPONENT
        // ============================================================
        private void InitializeComponent()
        {
            this.Text = "TechShop Yönetim Sistemi";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 40);
            this.Font = new Font("Segoe UI", 9f);
            this.MinimumSize = new Size(900, 600);

            // ── Sol menü ────────────────────────────────────────────
            panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 180,
                BackColor = Color.FromArgb(20, 20, 50)
            };

            lblMenuBaslik = new Label
            {
                Text = "MENU",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            btnAnaSayfa = MakeMenuBtn("Ana Sayfa", new Point(0, 55));
            btnMusteriler = MakeMenuBtn("Müşteriler", new Point(0, 100));
            btnUrunler = MakeMenuBtn("Ürünler", new Point(0, 145));
            btnSiparisler = MakeMenuBtn("Siparişler", new Point(0, 190));
            btnRaporlar = MakeMenuBtn("Raporlar", new Point(0, 235));
            btnAyarlar = MakeMenuBtn("Ayarlar", new Point(0, 280));

            btnAnaSayfa.Click += (s, e) => AnaSayfaGoster();
            btnMusteriler.Click += (s, e) => MusterilerGoster();
            btnUrunler.Click += (s, e) => UrunlerGoster();
            btnSiparisler.Click += (s, e) => SiparislerGoster();
            btnRaporlar.Click += (s, e) => RaporlarGoster();
            btnAyarlar.Click += (s, e) => AyarlarGoster();

            panelMenu.Controls.AddRange(new Control[]
            {
                lblMenuBaslik,
                btnAnaSayfa, btnMusteriler, btnUrunler,
                btnSiparisler, btnRaporlar, btnAyarlar
            });

            // ── İçerik paneli ────────────────────────────────────────
            panelIcerik = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 55),
                AutoScroll = true
            };

            // ── Bağlantı etiketi ─────────────────────────────────────
            lblBaglanti = new Label
            {
                Text = "● Bağlanıyor...",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9f),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            panelIcerik.Controls.Add(lblBaglanti);

            this.Controls.Add(panelIcerik);
            this.Controls.Add(panelMenu);
        }

        // ============================================================
        // UI YARDIMCILARI
        // ============================================================
        private Button MakeMenuBtn(string text, Point loc)
        {
            Button b = new Button
            {
                Text = text,
                Size = new Size(180, 40),
                Location = loc,
                BackColor = Color.FromArgb(30, 30, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private Label MakeBaslik(string text, Point loc)
        {
            return new Label
            {
                Text = text,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = loc
            };
        }

        private Label MakeRaporBaslik(string text, Point loc)
        {
            return new Label
            {
                Text = text,
                ForeColor = Color.FromArgb(0, 200, 255),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                AutoSize = true,
                Location = loc
            };
        }

        private Label MakeLabel(string text, Point loc)
        {
            return new Label
            {
                Text = text,
                ForeColor = Color.White,
                AutoSize = true,
                Location = loc
            };
        }

        private TextBox MakeTextBox(Point loc)
        {
            return new TextBox
            {
                Location = loc,
                Size = new Size(220, 25),
                BackColor = Color.FromArgb(50, 50, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Button MakeBtn(string text, Color color, Point loc)
        {
            Button b = new Button
            {
                Text = text,
                Size = new Size(95, 30),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = loc,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private DataGridView MakeGrid(Point loc, Size size)
        {
            return new DataGridView
            {
                Location = loc,
                Size = size,
                BackgroundColor = Color.FromArgb(40, 40, 70),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(60, 60, 100),
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(0, 90, 160),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(40, 40, 70),
                    ForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(0, 100, 180)
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false
            };
        }
    }
}