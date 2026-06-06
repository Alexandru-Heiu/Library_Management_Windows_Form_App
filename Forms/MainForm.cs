using LibraryManagement.Forms.Cititori;
using LibraryManagement.Forms.Carti;
using LibraryManagement.Forms.Imprumuturi;
using LibraryManagement.Forms.Raport;

namespace LibraryManagement.Forms
{
    public partial class MainForm : Form
    {
        private Panel _contentPanel = null!;

        public MainForm()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "📚 Sistem de Evidență Bibliotecă";
            this.Size = new Size(1280, 800);
            this.MinimumSize = new Size(1024, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 250);

            // Header
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(33, 37, 41)
            };
            var lblTitle = new Label
            {
                Text = "📚  Sistem de Evidență — Bibliotecă",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            header.Controls.Add(lblTitle);

            // MenuStrip
            var menu = new MenuStrip
            {
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                Renderer = new ToolStripProfessionalRenderer(new DarkMenuColorTable())
            };
            menu.Font = new Font("Segoe UI", 10);

            var miCititori = CreateMenu("👤 Cititori", () => ShowPanel(new CititoriPanel()));
            var miCarti = CreateMenu("📖 Cărți", () => ShowPanel(new CartiPanel()));
            var miImprumuturi = CreateMenu("🔄 Împrumuturi", () => ShowPanel(new ImprumuturiPanel()));
            var miRaport = CreateMenu("📊 Raport", OpenReport);
            var miExit = CreateMenu("❌ Ieșire", () => Application.Exit());

            menu.Items.AddRange(new[] { miCititori, miCarti, miImprumuturi, miRaport, miExit });

            // Content panel
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            this.Controls.Add(_contentPanel);
            this.Controls.Add(menu);
            this.Controls.Add(header);
            this.MainMenuStrip = menu;

            // Afișează primul modul
            ShowPanel(new CititoriPanel());
        }

        private static ToolStripMenuItem CreateMenu(string text, Action onClick)
        {
            var item = new ToolStripMenuItem(text)
            {
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            item.Click += (_, _) => onClick();
            return item;
        }

        private void ShowPanel(UserControl panel)
        {
            _contentPanel.Controls.Clear();
            panel.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(panel);
        }

        private void OpenReport()
        {
            using var f = new ReportForm();
            f.ShowDialog(this);
        }
    }


    internal class DarkMenuColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Color.FromArgb(73, 80, 87);
        public override Color MenuItemBorder => Color.FromArgb(73, 80, 87);
        public override Color MenuBorder => Color.FromArgb(52, 58, 64);
        public override Color ToolStripDropDownBackground => Color.FromArgb(52, 58, 64);
        public override Color ImageMarginGradientBegin => Color.FromArgb(52, 58, 64);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(52, 58, 64);
        public override Color ImageMarginGradientEnd => Color.FromArgb(52, 58, 64);
    }
}