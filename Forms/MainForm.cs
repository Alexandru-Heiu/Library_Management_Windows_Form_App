using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagement.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = "Sistem de Evidență Bibliotecă";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
