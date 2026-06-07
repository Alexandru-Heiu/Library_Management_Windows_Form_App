using LibraryManagement.BusinessLogic;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Imprumuturi
{
    public partial class ImprumuturiPanel : UserControl
    {
        private readonly ImprumutService _service = new();
        private readonly CititorService _cititorService = new();
        private DataGridView _grid = null!;
        private ComboBox _cboCititor = null!;

        public ImprumuturiPanel()
        {
            InitializeComponent();
            BuildUI();
            LoadCititori();
            LoadData();
        }

        private void BuildUI()
        {
            this.BackColor = Color.FromArgb(245, 245, 250);

            var lblTitle = new Label
            {
                Text = "🔄  Gestionare Împrumuturi",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Dock = DockStyle.Top,
                Height = 45,
                Padding = new Padding(5, 10, 0, 0)
            };

            var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White, Padding = new Padding(5) };

            var btnAdd = Btn("➕ Adaugă", Color.FromArgb(40, 167, 69), BtnAdd_Click);
            var btnEdit = Btn("✏️ Editează", Color.FromArgb(0, 123, 255), BtnEdit_Click);
            var btnDelete = Btn("🗑️ Anulează", Color.FromArgb(220, 53, 69), BtnDelete_Click);
            var btnAll = Btn("📋 Toate", Color.FromArgb(108, 117, 125), (_, _) => { _cboCititor.SelectedIndex = 0; LoadData(); });

            var lblF = new Label { Text = "Cititor:", Width = 55, Top = 14, Font = new Font("Segoe UI", 10) };
            _cboCititor = new ComboBox { Width = 220, Top = 9, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            _cboCititor.SelectedIndexChanged += (_, _) =>
            {
                if (_cboCititor.SelectedIndex > 0 && _cboCititor.SelectedItem is Cititor c)
                    BindGrid(_service.GetByCititor(c.IdCititor));
                else
                    LoadData();
            };

            toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnAll, lblF, _cboCititor });
            int x = 5;
            foreach (Control c in new Control[] { btnAdd, btnEdit, btnDelete, btnAll })
            { c.Left = x; c.Top = 9; x += c.Width + 5; }
            lblF.Left = x + 10; _cboCititor.Left = lblF.Left + lblF.Width + 3;

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
            _grid.DoubleClick += BtnEdit_Click;

            this.Controls.Add(_grid);
            this.Controls.Add(toolbar);
            this.Controls.Add(lblTitle);
        }

        private void LoadCititori()
        {
            _cboCititor.Items.Clear();
            _cboCititor.Items.Add("(Toți cititorii)");
            foreach (var c in _cititorService.GetAll())
                _cboCititor.Items.Add(c);
            _cboCititor.SelectedIndex = 0;
        }

        private void LoadData() => BindGrid(_service.GetAll());

        private void BindGrid(List<Imprumut> data)
        {
            _grid.DataSource = null;
            _grid.Columns.Clear();
            var display = data.Select(i => new
            {
                Cititor = i.NumeCititor,
                Carte = i.TitluCarte,
                Data = i.DataImprumut.ToShortDateString(),
                ZileIntarziere = i.ZileIntarziere,
                Amenda = i.AmendaTotal.ToString("F2") + " MDL",
                _Id = i.IdImprumut
            }).ToList();
            _grid.DataSource = display;

            if (_grid.Columns.Count > 0)
            {
                _grid.Columns["Cititor"].HeaderText = "Cititor";
                _grid.Columns["Carte"].HeaderText = "Carte";
                _grid.Columns["Data"].HeaderText = "Data Împrumutului";
                _grid.Columns["ZileIntarziere"].HeaderText = "Zile Întârziere";
                _grid.Columns["Amenda"].HeaderText = "Amendă Totală";
                _grid.Columns["_Id"].Visible = false;
            }
        }

        private Imprumut? GetSelected()
        {
            if (_grid.CurrentRow == null) return null;
            var id = (int)_grid.CurrentRow.Cells["_Id"].Value;
            return _service.GetAll().FirstOrDefault(i => i.IdImprumut == id);
        }

        private void BtnAdd_Click(object? s, EventArgs e)
        {
            using var f = new ImprumutForm(null);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnEdit_Click(object? s, EventArgs e)
        {
            var imp = GetSelected();
            if (imp == null) { MessageBox.Show("Selectați un împrumut.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            using var f = new ImprumutForm(imp);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object? s, EventArgs e)
        {
            var imp = GetSelected();
            if (imp == null) { MessageBox.Show("Selectați un împrumut.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var r = MessageBox.Show(
                $"Anulați împrumutul cărții \"{imp.TitluCarte}\" pentru \"{imp.NumeCititor}\"?",
                "Confirmare anulare", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
            {
                try { _service.Delete(imp.IdImprumut); LoadData(); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private static Button Btn(string text, Color color, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 110,
                Height = 34,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += onClick;
            return b;
        }
    }
}