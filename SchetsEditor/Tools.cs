﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected SolidBrush kwast;

        public virtual void MuisVast(SchetsControl s, Point p)
        {   startpunt = p;
        }
        public virtual void MuisLos(SchetsControl s, Point p)
        {   kwast = new SolidBrush(s.PenKleur);
        }
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
        
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {  
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = 
                gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
                gr.DrawString   (tekst, font, kwast, this.startpunt, StringFormat.GenericTypographic);
                if (c == 32) sz.Width = 20; // zorgt dat de spatie getekend wordt!
                else s.geefLijst().Add($"tekst {tekst} {kwast.Color.R} {kwast.Color.G} {kwast.Color.B} {startpunt.X} {startpunt.Y}"); 
                //gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                startpunt.X += (int)sz.Width;
                s.Invalidate();
            }
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                                , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                                );
        }
        public static Pen MaakPen(Brush b, int dikte)
        {   Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {   base.MuisVast(s, p);
            kwast = new SolidBrush(Color.Gray);
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {   s.Refresh();
            this.Bezig(s.CreateGraphics(), this.startpunt, p);
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   base.MuisLos(s, p);
            this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
        }
        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(Graphics g, Point p1, Point p2);
        
        public virtual void Compleet(Graphics g, Point p1, Point p2)
        {   this.Bezig(g, p1, p2);
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "kader"; }

        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            s.geefLijst().Add($"Kader {startpunt.X} {startpunt.Y} {p.X} {p.Y} {kwast.Color.R} {kwast.Color.G} {kwast.Color.B}");//voegt kader toe aan lijst
            s.Invalidate();
        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }
    
    public class VolRechthoekTool : TweepuntTool
    {
        public override string ToString() { return "vlak"; }
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            s.geefLijst().Add($"Volrechthoek {startpunt.X} {startpunt.Y} {p.X} {p.Y} {kwast.Color.R} {kwast.Color.G} {kwast.Color.B}");//voegt volrechthoek toe aan lijst
            s.Invalidate();
        }
        public override void Compleet(Graphics g, Point p1, Point p2)
        {   g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class EllipseTool: TweepuntTool // teken ovaal figuur
    {
        public override string ToString() { return "ovaal"; }
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            s.geefLijst().Add($"Cirkel {startpunt.X} {startpunt.Y} {p.X} {p.Y} {kwast.Color.R} {kwast.Color.G} {kwast.Color.B}");//voegt ovaal toe aan lijst
            s.Invalidate();

        }
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }
    public class FillEllipseTool: EllipseTool // teken gevulde ovaal
    {
        public override string ToString() { return "cirkel"; }
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            s.geefLijst().Add($"Volcirkel {startpunt.X} {startpunt.Y} {p.X} {p.Y} {kwast.Color.R} {kwast.Color.G} {kwast.Color.B}");//voegt volcirkel toe aan lijst
            s.Invalidate();
        }
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            s.geefLijst().Add($"Lijn {startpunt.X} {startpunt.Y} {p.X} {p.Y} {kwast.Color.R} {kwast.Color.G} {kwast.Color.B}");//voegt lijn toe aan lijst
            s.Invalidate();

        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   g.DrawLine(MaakPen(this.kwast,3), p1, p2);
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            s.geefLijst().Add($"pen {startpunt.X} {startpunt.Y} {p.X} {p.Y} {kwast.Color.R} {kwast.Color.G} {kwast.Color.B}");//voegt lijn toe aan lijst
            s.Invalidate();
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {   this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }
    
    public class GumTool : PenTool
    {
        public override string ToString() { return "gum"; }
        public override void MuisLos(SchetsControl s, Point p)
        {
            for (int i = s.geefLijst().Count - 1; i >= 0; i--)
            {
                string[] paras = s.geefLijst()[i].Split();//kijkt op positie i van lijst.
                if (paras[0] == "Volrechthoek" || paras[0] == "kader")
                {
                    int x1 = int.Parse(paras[1]);
                    int x2 = int.Parse(paras[3]);
                    int y1 = int.Parse(paras[2]);
                    int y2 = int.Parse(paras[4]);



                    if (paras[0] == "Volrechthoek")
                    {
                        if (((x1 < p.X && x2 > p.X) || (x1 > p.X && x2 < p.X)) && ((y1 < p.Y && y2 > p.Y) || (y1 > p.Y && y2 < p.Y)))
                        {
                            s.geefLijst().RemoveAt(i);
                            break;
                        }
                    }
                    if (paras[0] == "lijn")
                        for (int ongeveerX= p.X - 5; ongeveerX <= p.X + 5; ongeveerX++)
                        {
                            for (int ongeveerY = p.Y - 5; ongeveerY <= p.Y + 5; ongeveerY++)
                            {
                                {
                                    if (((x1 <= ongeveerX && x2 >= ongeveerX) || (x1 >= ongeveerX && x2 >= ongeveerX)) && (ongeveerY == x1 + (x2 - x1) / (y2 - y1)) || ongeveerY == x2 + (x1 - x2) / (x1 - x2))
                                    {
                                        s.geefLijst().RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                        }
                    if (paras[0] == "kader")
                       {
                       if (((((x1 - 5 <= p.X) && (x1 + 5 >= p.X))||((x2-5<=p.X) && (x2+5 >=p.X)))
                            && ((y1<=p.Y &&y2>=p.Y)||(y1>=p.Y && y2 <= p.Y))))
                       {
                            s.geefLijst().RemoveAt(i);
                            break;
                       }
                    }
                    if (paras[0] == "Cirkel")
                    {

                    }

                }
                s.Invalidate();
            }
        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   //g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        }
    }
}
