using LibraryManagement.BusinessLogic;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Cititori
{
    public partial class CititorForm : Form
    {
        private readonly CititorService _service = new();
        private readonly Cititor? _existing;

        private TextBox _txtNume = null!;
        private TextBox _txtPrenume = null!;
        private TextBox _txtIDNP = null!;
        private TextBox _txtTelefon = null!;
        private TextBox _txtEmail = null!;
        private DateTimePicker _dtpData = null!;

        public CititorForm(Cititor? existing)
        {
            _existing = existing;
            InitializeComponent();
            BuildUI();
            if (_existing != null) PopulateFields();
        }

        private void BuildUI()
        {
            this.Text = _existing == null ? "Adaugă Cititor Nou" : "Editează Cititor";
            this.Size = new Size(480, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(20),
                AutoSize = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _txtNume = AddRow(layout, 0, "Nume *", "");
            _txtPrenume = AddRow(layout, 1, "Prenume *", "");
            _txtIDNP = AddRow(layout, 2, "IDNP * (13 cifre)", "");
            _txtTelefon = AddRow(layout, 3, "Telefon *", "");
            _txtEmail = AddRow(layout, 4, "Email", "");

            // Data înregistrare
            var lblData = new Label
            {
                Text = "Data înregistrării *",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            _dtpData = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            layout.Controls.Add(lblData, 0, 5);
            layout.Controls.Add(_dtpData, 1, 5);

            // Butoane
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
                DialogResult = DialogResult.None,
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

            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnSave);

            this.Controls.Add(layout);
            this.Controls.Add(btnPanel);
        }

        private static TextBox AddRow(TableLayoutPanel layout, int row, string label, string value)
        {
            var lbl = new Label
            {
                Text = label,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            var txt = new TextBox
            {
                Text = value,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Height = 30
            };
            layout.Controls.Add(lbl, 0, row);
            layout.Controls.Add(txt, 1, row);
            return txt;
        }

        private void PopulateFields()
        {
            _txtNume.Text = _existing!.Nume;
            _txtPrenume.Text = _existing.Prenume;
            _txtIDNP.Text = _existing.IDNP;
            _txtTelefon.Text = _existing.Telefon;
            _txtEmail.Text = _existing.Email ?? "";
            _dtpData.Value = _existing.DataInregistrare;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var cititor = new Cititor
            {
                IdCititor = _existing?.IdCititor ?? 0,
                Nume = _txtNume.Text.Trim(),
                Prenume = _txtPrenume.Text.Trim(),
                IDNP = _txtIDNP.Text.Trim(),
                Telefon = _txtTelefon.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(_txtEmail.Text) ? null : _txtEmail.Text.Trim(),
                DataInregistrare = _dtpData.Value.Date
            };

            try
            {
                if (_existing == null)
                    _service.Add(cititor);
                else
                    _service.Update(cititor);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ValidationException vex)
            {
                MessageBox.Show(vex.Message, "Date invalide",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvare:\n{ex.Message}", "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}