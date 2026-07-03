using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TechShopApp
{
    public class RaporlarForm : Form
    {
        private Label lblBaslik;
        private Label lblAciklama;
        private TabControl tabRaporlar;

        // Rapor 1 — En Çok Satan Ürünler (VIEW)
        private TabPage tabRapor1;
        private DataGridView dgvRapor1;
        private Label lblRapor1Kaynak;

        // Rapor 2 — Aylık Ciro (SP)
        private TabPage tabRapor2;
        private DataGridView dgvRapor2;
        private Label lblRapor2Kaynak;
        private NumericUpDown nudYil;
        private Button btnRapor2Yenile;

        // Rapor 3 — Son Siparişler (VIEW)
        private TabPage tabRapor3;
        private DataGridView dgvRapor3;
        private Label lblRapor3Kaynak;

        public RaporlarForm()
        {
            InitializeComponent();
            // ✅ Sayfa açılır açılmaz 3 rapor otomatik yüklenir
            Rapor1Yukle();
            Rapor2Yukle();
            Rapor3Yukle();
        }

        // ============================================================
        // RAPOR 1 — En Çok Satan Ürünler → vw_UrunSatisRaporu (VIEW)
        // ============================================================
        private void Rapor1Yukle()
        {
            string sql = "SELECT UrunAdi AS [Ürün Adı], " +
                         "KategoriAdi AS [Kategori], " +
                         "ToplamSatisMiktar AS [Satış Adedi], " +
                         "CAST(ToplamSatisTutar AS DECIMAL(10,2)) AS [Toplam Ciro (TL)] " +
                         "FROM vw_UrunSatisRaporu " +
                         "ORDER BY ToplamSatisMiktar DESC";
            dgvRapor1.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        // ============================================================
        // RAPOR 2 — Aylık Ciro → usp_AylikCiroRaporu (SP)
        // ============================================================
        private void Rapor2Yukle()
        {
            int yil = (int)nudYil.Value;
            SqlParameter[] prms = { new SqlParameter("@Yil", yil) };
            DataTable dt = DatabaseHelper.ExecuteStoredProcedure("usp_AylikCiroRaporu", prms);

            // Kolon başlıklarını Türkçeleştir
            if (dt.Columns.Contains("Ay")) dt.Columns["Ay"].ColumnName = "Ay No";
            if (dt.Columns.Contains("AyAdi")) dt.Columns["AyAdi"].ColumnName = "Ay";
            if (dt.Columns.Contains("SiparisSayisi")) dt.Columns["SiparisSayisi"].ColumnName = "Sipariş Sayısı";
            if (dt.Columns.Contains("ToplamCiro")) dt.Columns["ToplamCiro"].ColumnName = "Ciro (TL)";

            dgvRapor2.DataSource = dt;
        }

        // ============================================================
        // RAPOR 3 — Son Siparişler → vw_SonSiparisler (VIEW)
        // ============================================================
        private void Rapor3Yukle()
        {
            string sql = "SELECT SiparisID AS [Sip. No], " +
                         "MusteriAdSoyad AS [Müşteri], " +
                         "UrunAdi AS [Ürün], " +
                         "Miktar AS [Adet], " +
                         "CAST(BirimFiyat AS DECIMAL(10,2)) AS [Birim Fiyat], " +
                         "CONVERT(VARCHAR,SiparisTarihi,104) AS [Tarih], " +
                         "Durum " +
                         "FROM vw_SonSiparisler";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            dgvRapor3.DataSource = dt;

            // Durum sütununa renk ver
            dgvRapor3.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
                if (dgvRapor3.Columns[e.ColumnIndex].Name == "Durum")
                {
                    string val = e.Value?.ToString() ?? "";
                    if (val == "Teslim") e.CellStyle.ForeColor = Color.LimeGreen;
                    else if (val == "Kargoda") e.CellStyle.ForeColor = Color.Orange;
                    else if (val == "Hazirlaniyor") e.CellStyle.ForeColor = Color.Cyan;
                    else if (val == "Beklemede") e.CellStyle.ForeColor = Color.Yellow;
                    else if (val == "Iptal") e.CellStyle.ForeColor = Color.Red;
                    else e.CellStyle.ForeColor = Color.White;
                }
            };
        }

        private void btnRapor2Yenile_Click(object sender, EventArgs e) => Rapor2Yukle();

        // ============================================================
        // INITIALIZECOMPONENT
        // ============================================================
        private void InitializeComponent()
        {
            this.Text = "TechShop — Raporlar";
            this.Size = new Size(1000, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.Font = new Font("Segoe UI", 9f);

            // ── Başlık ───────────────────────────────────────────────
            lblBaslik = new Label
            {
                Text = "📊  Raporlar",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Location = new Point(15, 12),
                AutoSize = true
            };

            lblAciklama = new Label
            {
                Text = "Aşağıdaki raporlar VIEW ve Stored Procedure üzerinden sayfa açılışında otomatik yüklenmektedir.",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 8.5f),
                Location = new Point(15, 40),
                AutoSize = true
            };

            // ── TabControl ───────────────────────────────────────────
            tabRaporlar = new TabControl
            {
                Location = new Point(10, 65),
                Size = new Size(965, 570),
                Font = new Font("Segoe UI", 9f),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // ── RAPOR 1 ──────────────────────────────────────────────
            tabRapor1 = new TabPage("📦  En Çok Satan Ürünler");
            tabRapor1.BackColor = Color.FromArgb(40, 40, 40);

            lblRapor1Kaynak = MakeKaynakLabel("Kaynak: vw_UrunSatisRaporu (VIEW)", new Point(10, 10));
            dgvRapor1 = MakeGrid(new Point(10, 35), new Size(935, 490));

            tabRapor1.Controls.Add(lblRapor1Kaynak);
            tabRapor1.Controls.Add(dgvRapor1);

            // ── RAPOR 2 ──────────────────────────────────────────────
            tabRapor2 = new TabPage("📅  Aylık Ciro Özeti");
            tabRapor2.BackColor = Color.FromArgb(40, 40, 40);

            lblRapor2Kaynak = MakeKaynakLabel("Kaynak: usp_AylikCiroRaporu (Stored Procedure)", new Point(10, 10));

            Label lblYil = new Label
            {
                Text = "Yıl:",
                ForeColor = Color.White,
                Location = new Point(10, 38),
                AutoSize = true
            };

            nudYil = new NumericUpDown
            {
                Location = new Point(40, 35),
                Size = new Size(80, 25),
                Minimum = 2020,
                Maximum = 2030,
                Value = DateTime.Now.Year,
                BackColor = Color.FromArgb(55, 55, 55),
                ForeColor = Color.White
            };

            btnRapor2Yenile = new Button
            {
                Text = "🔄 Yenile",
                Size = new Size(90, 28),
                Location = new Point(130, 33),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnRapor2Yenile.FlatAppearance.BorderSize = 0;
            btnRapor2Yenile.Click += btnRapor2Yenile_Click;

            dgvRapor2 = MakeGrid(new Point(10, 70), new Size(935, 455));

            tabRapor2.Controls.AddRange(new Control[]
            {
                lblRapor2Kaynak, lblYil, nudYil, btnRapor2Yenile, dgvRapor2
            });

            // ── RAPOR 3 ──────────────────────────────────────────────
            tabRapor3 = new TabPage("🛍️  Son Siparişler");
            tabRapor3.BackColor = Color.FromArgb(40, 40, 40);

            lblRapor3Kaynak = MakeKaynakLabel("Kaynak: vw_SonSiparisler (VIEW)", new Point(10, 10));
            dgvRapor3 = MakeGrid(new Point(10, 35), new Size(935, 490));

            tabRapor3.Controls.Add(lblRapor3Kaynak);
            tabRapor3.Controls.Add(dgvRapor3);

            // ── Ekle ─────────────────────────────────────────────────
            tabRaporlar.TabPages.Add(tabRapor1);
            tabRaporlar.TabPages.Add(tabRapor2);
            tabRaporlar.TabPages.Add(tabRapor3);

            this.Controls.Add(lblBaslik);
            this.Controls.Add(lblAciklama);
            this.Controls.Add(tabRaporlar);
        }

        private Label MakeKaynakLabel(string text, Point loc)
        {
            return new Label
            {
                Text = text,
                ForeColor = Color.FromArgb(0, 200, 255),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                Location = loc,
                AutoSize = true
            };
        }

        private DataGridView MakeGrid(Point loc, Size size)
        {
            DataGridView dgv = new DataGridView
            {
                Location = loc,
                Size = size,
                BackgroundColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(70, 70, 70),
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold) },
                DefaultCellStyle = { BackColor = Color.FromArgb(45, 45, 45), ForeColor = Color.White, SelectionBackColor = Color.FromArgb(0, 100, 160) },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            return dgv;
        }
    }
}
