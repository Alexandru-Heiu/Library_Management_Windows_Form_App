using LibraryManagement.BusinessLogic;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Carti
{
    public partial class CartiPanel : UserControl
    {
        private readonly CarteService _service = new();
        private DataGridView _grid = null!;
        private TextBox _txtSearch = null!;
        private ComboBox _cboGen = null!;

        public CartiPanel()
        {
            InitializeComponent();
            BuildUI();
            LoadGenuri();
            LoadData();
        }

        private void BuildUI()
        {
            this.BackColor = Color.FromArgb(245, 245, 250);

            var lblTitle = new Label
            {
                Text = "📖  Gestionare Cărți",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Dock = DockStyle.Top,
                Height = 45,
                Padding = new Padding(5, 10, 0, 0)
            };

            var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White, Padding = new Padding(5) };

            var btnAdd = CreateBtn("➕ Adaugă", Color.FromArgb(40, 167, 69), BtnAdd_Click);
            var btnEdit = CreateBtn("✏️ Editează", Color.FromArgb(0, 123, 255), BtnEdit_Click);
            var btnDelete = CreateBtn("🗑️ Șterge", Color.FromArgb(220, 53, 69), BtnDelete_Click);
            var btnRefresh = CreateBtn("🔄 Reîncarcă", Color.FromArgb(108, 117, 125), (_, _) => LoadData());

            _txtSearch = new TextBox { PlaceholderText = "🔍  Titlu sau autor...", Width = 220, Height = 32, Font = new Font("Segoe UI", 10), Top = 9 };
            _txtSearch.TextChanged += (_, _) => SearchData();

            var lblGen = new Label { Text = "Gen:", Width = 35, Top = 14, Font = new Font("Segoe UI", 10) };
            _cboGen = new ComboBox { Width = 140, Top = 9, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            _cboGen.SelectedIndexChanged += (_, _) => SearchData();

            toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh, _txtSearch, lblGen, _cboGen });

            int x = 5;
            foreach (Control c in new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh })
            { c.Left = x; c.Top = 9; x += c.Width + 5; }
            _txtSearch.Left = x + 5;
            lblGen.Left = _txtSearch.Left + _txtSearch.Width + 10;
            _cboGen.Left = lblGen.Left + lblGen.Width + 3;

            _grid = CreateGrid();
            _grid.DoubleClick += BtnEdit_Click;

            this.Controls.Add(_grid);
            this.Controls.Add(toolbar);
            this.Controls.Add(lblTitle);
        }

        private void LoadGenuri()
        {
            _cboGen.Items.Clear();
            _cboGen.Items.Add("(Toate genurile)");
            foreach (var g in _service.GetGenuri())
                _cboGen.Items.Add(g);
            _cboGen.SelectedIndex = 0;
        }

        private void LoadData() => BindGrid(_service.GetAll());

        private void SearchData()
        {
            var term = _txtSearch.Text.Trim();
            var gen = _cboGen.SelectedIndex > 0 ? _cboGen.SelectedItem!.ToString()! : null;

            List<Carte> data;
            if (!string.IsNullOrEmpty(term))
                data = _service.Search(term);
            else if (gen != null)
                data = _service.FilterByGen(gen);
            else
                data = _service.GetAll();

            BindGrid(data);
        }

        private void BindGrid(List<Carte> data)
        {
            _grid.DataSource = null;
            _grid.Columns.Clear();
            var display = data.Select(c => new
            {
                c.Titlu,
                c.Autor,
                c.Gen,
                AnPublicare = c.AnPublicare.ToString(),
                PretAmenda = c.PretAmenda.ToString("F2") + " MDL/zi",
                _Hidden_Id = c.IdCarte
            }).ToList();

            _grid.DataSource = display;
            if (_grid.Columns.Count > 0)
            {
                _grid.Columns["Titlu"].HeaderText = "Titlu";
                _grid.Columns["Autor"].HeaderText = "Autor";
                _grid.Columns["Gen"].HeaderText = "Gen";
                _grid.Columns["AnPublicare"].HeaderText = "An Publicare";
                _grid.Columns["PretAmenda"].HeaderText = "Preț Amendă";
                _grid.Columns["_Hidden_Id"].Visible = false;
            }
        }

        private Carte? GetSelected()
        {
            if (_grid.CurrentRow == null) return null;
            var id = (int)_grid.CurrentRow.Cells["_Hidden_Id"].Value;
            return _service.GetById(id);
        }

        private void BtnAdd_Click(object? s, EventArgs e)
        {
            using var f = new CarteForm(null);
            if (f.ShowDialog() == DialogResult.OK) { LoadGenuri(); LoadData(); }
        }

        private void BtnEdit_Click(object? s, EventArgs e)
        {
            var carte = GetSelected();
            if (carte == null) { MessageBox.Show("Selectați o carte.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            using var f = new CarteForm(carte);
            if (f.ShowDialog() == DialogResult.OK) { LoadGenuri(); LoadData(); }
        }

        private void BtnDelete_Click(object? s, EventArgs e)
        {
            var carte = GetSelected();
            if (carte == null) { MessageBox.Show("Selectați o carte.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var r = MessageBox.Show($"Ștergeți cartea \"{carte.Titlu}\"?", "Confirmare",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
            {
                try { _service.Delete(carte.IdCarte); LoadData(); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private static DataGridView CreateGrid() => new()
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

        private static Button CreateBtn(string text, Color color, EventHandler onClick)
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