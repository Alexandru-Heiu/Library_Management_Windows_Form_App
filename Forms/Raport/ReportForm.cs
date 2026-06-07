using LibraryManagement.BusinessLogic;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Raport
{
    public partial class ReportForm : Form
    {
        private readonly ReportService _reportService = new();

        public ReportForm()
        {
            InitializeComponent();
            BuildUI();
            LoadReport();
        }

        private DataGridView _grid = null!;
        private Label _lblTotal = null!, _lblMedia = null!, _lblTop = null!;

        private void BuildUI()
        {
            this.Text = "📊 Raport Sumar — Amenzi Cititori";
            this.Size = new Size(800, 620);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Header
            var hdr = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(33, 37, 41) };
            hdr.Controls.Add(new Label
            {
                Text = "📊  Raport Sumar — Amenzi per Cititor",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });

            // Statistici
            var statsPanel = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = Color.FromArgb(248, 249, 250), Padding = new Padding(15) };

            _lblTotal = StatLabel("💰 Total general: —");
            _lblMedia = StatLabel("📈 Media per cititor: —");
            _lblTop = StatLabel("📖 Cea mai împrumutată carte: —");

            _lblTotal.Location = new Point(15, 15);
            _lblMedia.Location = new Point(15, 45);
            _lblTop.Location = new Point(15, 72);

            statsPanel.Controls.AddRange(new Control[] { _lblTotal, _lblMedia, _lblTop });

            // Grid
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 249, 250) },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(52, 58, 64),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                EnableHeadersVisualStyles = false
            };

            var btnClose = new Button
            {
                Text = "✖  Închide",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                DialogResult = DialogResult.OK
            };
            btnClose.FlatAppearance.BorderSize = 0;

            this.Controls.Add(_grid);
            this.Controls.Add(statsPanel);
            this.Controls.Add(btnClose);
            this.Controls.Add(hdr);
        }

        private void LoadReport()
        {
            try
            {
                var report = _reportService.GenerateReport();

                _grid.DataSource = null;
                _grid.Columns.Clear();

                var display = report.Rows.Select(r => new
                {
                    Cititor = r.NumeCititor,
                    NrImprumuturi = r.NrImprumuturi,
                    TotalZileIntarziere = r.TotalZileIntarziere,
                    TotalAmenda = r.TotalAmenda.ToString("F2") + " MDL"
                }).ToList();

                _grid.DataSource = display;

                if (_grid.Columns.Count > 0)
                {
                    _grid.Columns["Cititor"].HeaderText = "Cititor";
                    _grid.Columns["NrImprumuturi"].HeaderText = "Nr. Împrumuturi";
                    _grid.Columns["TotalZileIntarziere"].HeaderText = "Total Zile Întârziere";
                    _grid.Columns["TotalAmenda"].HeaderText = "Total Amendă";
                }

                _lblTotal.Text = $"💰  Total general încasat: {report.TotalGeneral:F2} MDL";
                _lblMedia.Text = $"📈  Media amenzii per cititor: {report.MediaPlati:F2} MDL";
                _lblTop.Text = $"📖  Cea mai împrumutată carte: {report.CarteCeaMaiImprumutata} ({report.NrImprumuturiCarte} împrumuturi)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la generarea raportului:\n{ex.Message}",
                    "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Label StatLabel(string text) => new()
        {
            Text = text,
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41)
        };
    }
}