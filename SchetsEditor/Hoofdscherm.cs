using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;

        public Hoofdscherm()
        {   this.ClientSize = new Size(1200, 800);//grotere client zodat alle knoppen zichtbaar zijn
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = "Schets Plus";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }
        private void maakFileMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("File");
            menu.DropDownItems.Add("Nieuw", null, this.nieuw);
            menu.DropDownItems.Add("Exit", null, this.afsluiten);
            menu.DropDownItems.Add("Open", null, this.open);//maakt open knop
            menuStrip.Items.Add(menu);
        }
        private void maakHelpMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Help");
            menu.DropDownItems.Add("Over \"Schets\"", null, this.about);
            menuStrip.Items.Add(menu);
        }
        private void about(object o, EventArgs ea)
        {   MessageBox.Show("SchetsPlus versie 2.0\n(c) UU Informatica 2010- Edit door Lotte van Horssen en Esmee Dekker, 2016"
                           , "Over \"Schets\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }

        private void nieuw(object sender, EventArgs e)
        {   SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            s.Show();
            s.BackColor = Color.WhiteSmoke;
        }
        private void afsluiten(object sender, EventArgs e)
        {   this.Close();
        }

        private void open(object sender, EventArgs e)//open event
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Plaatjes|*.PNG|Bitmaps|*.BMP|Fotobestanden|*.JPG|Schetsfiles|*.le";//type files
            dialog.Title = "Open file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.FileName.EndsWith(".le"))//indien schetsfile
                {
                    SchetsWin s = new SchetsWin();
                    s.MdiParent = this;
                    s.ReadFromSchetsFile(dialog.FileName);
                    s.Show();
                }
                else
                {
                    SchetsWin s = new SchetsWin();//"normale" afbeeldingen
                    s.MdiParent = this;
                    s.ReadFromFile(dialog.FileName);
                    s.Show();
                }
            }

        }
    }
}
