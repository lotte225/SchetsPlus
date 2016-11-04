using System;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;
        public List<string[]> objectenLijst;
        public Bitmap Bitmap//zorgt ervoor dat de bitmap bereikt kan worden vanuit andere klassen
        {
            get
            {
                return bitmap;
            }
        }
        public List<string[]> Lijst//maak nieuwe lijst
        {
            get
            {
                string woorden = "kader x y x y kleur";
                string[] items = woorden.Split(' ');

                return objectenLijst;
            }
        }
        
        public Schets()
        {
            bitmap = new Bitmap(1, 1);
            objectenLijst = new List<string[]>();//iets met objecten
        }
        public Schets(List<string[]> lijstje)//?? geen idee hoe dit te beschrijven, iets met de objecten
        {
            objectenLijst = lijstje;
        }
        public Schets(Bitmap bitje)//zorgt ervoor dat de bitmap opgeslagen kan worden
        {
            bitmap = bitje;
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }
        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;
            }
        }
        public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);
        }
        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }
        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }
    }
}
