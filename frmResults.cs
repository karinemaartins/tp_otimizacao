using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Simplex
{
    public partial class frmResults : Form
    {
        public frmResults()
        {
            InitializeComponent();
        }

        public void AdicionarControle(Control controle)
        {
            controle.Dock = DockStyle.Top;
            controle.BringToFront();
            panContent.Controls.Add(controle);            
            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog(this) == DialogResult.OK)
            {
                printDocument1.DocumentName = "Simplex " + DateTime.Now.ToString("HH:mm:ss");
                printDocument1.PrinterSettings = printDialog1.PrinterSettings;
                printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument1_PrintPage);
                printDocument1.Print();
            }
            
        }

        void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap b = new Bitmap(panContent.Width, panContent.Height);
            panContent.DrawToBitmap(b, new Rectangle(0,0,panContent.Width, panContent.Height));
            e.Graphics.DrawImage(b, new Point(0, 0));
            
        }
    }
}
