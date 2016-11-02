using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;

        public Hoofdscherm()
        {   this.ClientSize = new Size(800, 600);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = "Schets editor";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }
        private void maakFileMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("File");
            menu.DropDownItems.Add("Nieuw", null, this.nieuw);
            menu.DropDownItems.Add("Exit", null, this.afsluiten);
            menu.DropDownItems.Add("Opslaan als", null, this.opslaanAls);//maakt opslaan als knop
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
        {   MessageBox.Show("Schets versie 1.0\n(c) UU Informatica 2010"
                           , "Over \"Schets\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }

        private void nieuw(object sender, EventArgs e)
        {   SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            s.Show();
        }
        private void afsluiten(object sender, EventArgs e)
        {   this.Close();
        }

        private void opslaanAls(object sender, EventArgs e)//save as event
        {

        }

        private void open(object sender, EventArgs e)//open event
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Schetsfiles|*.PNG|Bitmaps|*.BMP|Fotobestanden|*.JPG";
            dialog.Title = "Open file";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SchetsWin s = new SchetsWin();
                s.MdiParent = this;
                s.ReadFromFile(dialog.FileName);
                s.Show();
            }

        }
    }
}
