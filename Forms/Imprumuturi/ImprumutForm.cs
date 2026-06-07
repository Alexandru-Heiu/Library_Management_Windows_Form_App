using LibraryManagement.BusinessLogic;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Imprumuturi
{
    public partial class ImprumutForm : Form
    {
        private readonly ImprumutService _service = new();
        private readonly CititorService _cititorService = new();
        private readonly CarteService _carteService = new();
        private readonly Imprumut? _existing;

        private ComboBox _cboCititor = null!;
        private ComboBox _cboCarte = null!;
        private DateTimePicker _dtpData = null!;
        private NumericUpDown _nudZile = null!;

        public ImprumutForm(Imprumut? existing)
        {
            _existing = existing;
            InitializeComponent();
            BuildUI();
            LoadComboBoxes();
            if (_existing != null) PopulateFields();
        }

        private void BuildUI()
        {
            this.Text = _existing == null ? "Adaugă Împrumut" : "Editează Împrumut";
            this.Size = new Size(480, 340);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(20),
                AutoSize = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            layout.Controls.Add(Lbl("Cititor *"), 0, 0);
            _cboCititor = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            layout.Controls.Add(_cboCititor, 1, 0);

            layout.Controls.Add(Lbl("Carte *"), 0, 1);
            _cboCarte = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            layout.Controls.Add(_cboCarte, 1, 1);

            layout.Controls.Add(Lbl("Data împrumutului *"), 0, 2);
            _dtpData = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(_dtpData, 1, 2);

            layout.Controls.Add(Lbl("Zile întârziere"), 0, 3);
            _nudZile = new NumericUpDown { Minimum = 0, Maximum = 9999, Value = 0, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(_nudZile, 1, 3);

            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 55,
                Padding = new Padding(10)
            };
            var btnSave = new Button
            {
                Text = "💾 Salvează",
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 120,
                Height = 36,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            var btnCancel = new Button
            {
                Text = "Anulează",
                DialogResult = DialogResult.Cancel,
                Width = 100,
                Height = 36,
                Font = new Font("Segoe UI", 10)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            btnPanel.Controls.AddRange(new Control[] { btnCancel, btnSave });

            this.Controls.Add(layout);
            this.Controls.Add(btnPanel);
        }

        private static Label Lbl(string text) => new()
        {
            Text = text,
            TextAlign = ContentAlignment.MiddleRight,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10)
        };

        private void LoadComboBoxes()
        {
            _cboCititor.DataSource = _cititorService.GetAll();
            _cboCititor.DisplayMember = "NumeComplet";
            _cboCititor.ValueMember = "IdCititor";

            _cboCarte.DataSource = _carteService.GetAll();
            _cboCarte.DisplayMember = "Titlu";
            _cboCarte.ValueMember = "IdCarte";
        }

        private void PopulateFields()
        {
            _cboCititor.SelectedValue = _existing!.IdCititor;
            _cboCarte.SelectedValue = _existing.IdCarte;
            _dtpData.Value = _existing.DataImprumut;
            _nudZile.Value = _existing.ZileIntarziere;
        }

        private void BtnSave_Click(object? s, EventArgs e)
        {
            try
            {
                var imp = new Imprumut
                {
                    IdImprumut = _existing?.IdImprumut ?? 0,
                    IdCititor = (int)_cboCititor.SelectedValue!,
                    IdCarte = (int)_cboCarte.SelectedValue!,
                    DataImprumut = _dtpData.Value.Date,
                    ZileIntarziere = (int)_nudZile.Value
                };

                if (_existing == null) _service.Add(imp);
                else _service.Update(imp);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ValidationException vex)
            {
                MessageBox.Show(vex.Message, "Date invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare:\n{ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}