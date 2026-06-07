using LibraryManagement.BusinessLogic;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Carti
{
    public partial class CarteForm : Form
    {
        private readonly CarteService _service = new();
        private readonly Carte? _existing;

        private TextBox _txtTitlu = null!;
        private TextBox _txtAutor = null!;
        private TextBox _txtGen = null!;
        private TextBox _txtAn = null!;
        private TextBox _txtPret = null!;

        public CarteForm(Carte? existing)
        {
            _existing = existing;
            InitializeComponent();
            BuildUI();
            if (_existing != null) PopulateFields();
        }

        private void BuildUI()
        {
            this.Text = _existing == null ? "Adaugă Carte Nouă" : "Editează Carte";
            this.Size = new Size(460, 360);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(20),
                AutoSize = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _txtTitlu = AddRow(layout, 0, "Titlu *");
            _txtAutor = AddRow(layout, 1, "Autor *");
            _txtGen = AddRow(layout, 2, "Gen *");
            _txtAn = AddRow(layout, 3, "An publicare *");
            _txtPret = AddRow(layout, 4, "Preț amendă/zi (MDL) *");

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

        private static TextBox AddRow(TableLayoutPanel t, int row, string label)
        {
            t.Controls.Add(new Label
            {
                Text = label,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            }, 0, row);
            var txt = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            t.Controls.Add(txt, 1, row);
            return txt;
        }

        private void PopulateFields()
        {
            _txtTitlu.Text = _existing!.Titlu;
            _txtAutor.Text = _existing.Autor;
            _txtGen.Text = _existing.Gen;
            _txtAn.Text = _existing.AnPublicare.ToString();
            _txtPret.Text = _existing.PretAmenda.ToString("F2");
        }

        private void BtnSave_Click(object? s, EventArgs e)
        {
            try
            {
                if (!int.TryParse(_txtAn.Text.Trim(), out int an))
                    throw new ValidationException("Anul publicării trebuie să fie un număr întreg.");
                if (!decimal.TryParse(_txtPret.Text.Trim().Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal pret))
                    throw new ValidationException("Prețul amenzii trebuie să fie un număr valid.");

                var carte = new Carte
                {
                    IdCarte = _existing?.IdCarte ?? 0,
                    Titlu = _txtTitlu.Text.Trim(),
                    Autor = _txtAutor.Text.Trim(),
                    Gen = _txtGen.Text.Trim(),
                    AnPublicare = an,
                    PretAmenda = pret
                };

                if (_existing == null) _service.Add(carte);
                else _service.Update(carte);

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