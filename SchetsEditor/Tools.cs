using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
    public class FillEllipseTool: TweepuntTool // teken gevulde ovaal
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
            s.geefLijst().Add($"Pen {startpunt.X} {startpunt.Y} {p.X} {p.Y} {kwast.Color.R} {kwast.Color.G} {kwast.Color.B}");//voegt lijn toe aan lijst
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
                if (paras[0] == "Volrechthoek" || paras[0] == "Kader" || paras[0] == "Lijn" || paras[0] == "Pen"|| paras[0]=="Volcirkel"|| paras[0]== "Cirkel")
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
                    if (paras[0] == "Lijn" || paras[0]=="Pen")//pen bestaat uit lijnen, dus dit kan gewoon
                    {

                        if (x1 == x2)
                        {
                            float dx = p.X - x1;
                            if (Math.Abs(dx) <= 5)
                            {
                                s.geefLijst().RemoveAt(i);
                                break;
                            }
                        }
                        else if (y1 == y2)
                        {
                            float dy = p.Y - y1;
                            if (Math.Abs(dy) <= 5)
                            {
                                s.geefLijst().RemoveAt(i);
                                break;
                            }
                        }
                        else
                        {
                            float a = (float)(y2 - y1) / (x2 - x1);
                            float a2 = -1 / a;
                            float b = y1 - (x1 * a);
                            float b2 = p.Y - (p.X * a2);
                            float x3 = (b2 - b) / (a - a2);
                            float y3 = a * x3 + b;
                            float d = (float)Math.Sqrt((p.Y - y3) * (p.Y - y3) + (p.X - x3) * (p.X - x3));//bepaalt afstand tot lijn

                            if (d <= 5)
                            {
                                s.geefLijst().RemoveAt(i);
                                break;
                            }
                        }

                    }
                    if (paras[0] == "Kader")//gummen van een kader, kliknauwkeurigheid 5px tot lijn
                    {
                        if( (   (x1 - 5 <= p.X && p.X <= x1 + 5 )       &&         ((y1 <= p.Y && p.Y <= y2) || (y1>= p.Y && p.Y >=y2)) )       ||
                            (   (x2 - 5 <= p.X && p.X <= x2 + 5 )       &&         ((y1 <= p.Y && p.Y <= y2) || (y1>= p.Y && p.Y >=y2)) )       ||
                            (   (y1 - 5 <= p.Y && p.Y <= y1 + 5 )       &&         ((x1 <= p.X && p.X <= x2) || (x1>= p.X && p.X >=x2)) )       ||
                            (   (y2 - 5 <= p.Y && p.Y <= y2 + 5 )       &&         ((x1 <= p.X && p.X <= x2) || (x1>= p.X && p.X >=x2)) )       )
                        {
                            s.geefLijst().RemoveAt(i);
                            break;
                        }
                       
                    }
                    if (paras[0] == "Volcirkel"|| paras[0] == "Cirkel")
                    {
                        float mx = (x1 + x2) / 2;
                        float my = (y1 + y2) / 2;
                        float a = Math.Abs((x2-x1)/2);
                        float b = Math.Abs((y2-y1)/2) ;
                        float x = Math.Abs(p.X - mx);
                        float y = Math.Abs(p.Y - my);
                        if (((x * x) / (a * a)) + ((y * y) / (b * b)) <= 1 && paras[0] == "Volcirkel")
                        {
                            s.geefLijst().RemoveAt(i);
                            break;
                        }
                        if (paras[0] == "Cirkel")
                        {
                            //binnencirkel
                            float a1 = a - 5;
                            float b1 = b - 5;
                            //buitencirkel
                            float a2 = a + 5;
                            float b2 = b + 5;
                            if (((x * x) / (a1*a1)) + ((y * y) / (b1 * b1)) >= 1 && ((x * x) / (a2 * a2)) + ((y * y) / (b2 * b2)) <= 1)
                            {
                                s.geefLijst().RemoveAt(i);
                                break;
                            }
                        }

                    }

                }

            }
            s.Invalidate();
        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   //g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        }
    }
}
