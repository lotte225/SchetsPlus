﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;
        public List<string> objectenLijst;
        public Bitmap Bitmap//zorgt ervoor dat de bitmap bereikt kan worden vanuit andere klassen
        {
            get
            {
                return bitmap;
            }
        }
        public List<string> Lijst//maak nieuwe lijst
        {
            get
            {
                return objectenLijst;
            }
        }

        public Schets() : this(new Bitmap(1, 1)) { }
        public Schets(Bitmap bitje)//zorgt ervoor dat de bitmap opgeslagen kan worden
        {
            bitmap = bitje;
            objectenLijst = new List<string>();
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
            gr.Clear(Color.White);//witte achtergrond
            gr.DrawImage(bitmap, 0, 0);
            foreach(string s in objectenLijst)
            {   //wat te tekenen bij welke button
                string[] paras = s.Split();
                if (paras[0] == "Volrechthoek")
                {
                    gr.FillRectangle(new SolidBrush (Color.FromArgb(int.Parse(paras[5]), int.Parse(paras[6]), int.Parse(paras[7]))), TweepuntTool.Punten2Rechthoek (new Point(int.Parse(paras[1]), int.Parse(paras[2])), new Point(int.Parse(paras[3]), int.Parse (paras[4]) )));//tekent volrechthoek uit lijst
                }
                else if (paras[0] == "Kader")
                {
                    gr.DrawRectangle(new Pen(Color.FromArgb(int.Parse(paras[5]), int.Parse(paras[6]), int.Parse(paras[7]))), TweepuntTool.Punten2Rechthoek(new Point(int.Parse(paras[1]), int.Parse(paras[2])), new Point(int.Parse(paras[3]), int.Parse(paras[4]))));//tekent volrechthoek uit lijst
                }
                else if (paras[0] == "Volcirkel")
                {
                    gr.FillEllipse(new SolidBrush(Color.FromArgb(int.Parse(paras[5]), int.Parse(paras[6]), int.Parse(paras[7]))), RechthoekTool.Punten2Rechthoek(new Point(int.Parse(paras[1]), int.Parse(paras[2])), new Point(int.Parse(paras[3]), int.Parse(paras[4]))));//tekent volrechthoek uit lijst
                }
                else if (paras[0] == "Cirkel")
                {
                    gr.DrawEllipse(new Pen(Color.FromArgb(int.Parse(paras[5]), int.Parse(paras[6]), int.Parse(paras[7]))), TweepuntTool.Punten2Rechthoek(new Point(int.Parse(paras[1]), int.Parse(paras[2])), new Point(int.Parse(paras[3]), int.Parse(paras[4]))));//tekent volrechthoek uit lijst
                }
                else if (paras[0] == "Lijn")
                {
                    gr.DrawLine(new Pen(Color.FromArgb(int.Parse(paras[5]), int.Parse(paras[6]), int.Parse(paras[7]))), (new Point(int.Parse(paras[1]), int.Parse(paras[2]))), new Point(int.Parse(paras[3]), int.Parse(paras[4])));
                }
                else if (paras[0] == "pen")
                {
                    gr.DrawLine(new Pen(Color.FromArgb(int.Parse(paras[5]), int.Parse(paras[6]), int.Parse(paras[7]))), (new Point(int.Parse(paras[1]), int.Parse(paras[2]))), new Point(int.Parse(paras[3]), int.Parse(paras[4])));
                }
                else if (paras[0] == "tekst")
                {
                    gr.DrawString(paras[1], new Font("Tahoma", 40), new SolidBrush(Color.FromArgb(int.Parse(paras[2]),int.Parse(paras[3]), int.Parse(paras[4]))), int.Parse(paras[5]), int.Parse(paras[6]),StringFormat.GenericTypographic);

                }
            }

        }

        public void Schoon()
        {
            objectenLijst.Clear();//wis alles
        }
        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);//werkt enkel bij bitmap
            

        }
        public void Wissel()
        {
            objectenLijst.Reverse();//breng achterste objecten naar voren, voorste objecten naar achter.
        }
    }
}
