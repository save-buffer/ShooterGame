using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Engine
{
    class TextObject : Element
    {
        public string Text;
        public Color C;
        public Font F;
        private int size;
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                F = new Font(F.OriginalFontName, value);
                size = value;
            }
        }
        public TextObject(string Name, string Text) : base(Name)
        {
            this.Text = Text;
            C = Color.White;
            size = 12;
            F = new Font("Consolia", size);
        }
        public override void Draw(Graphics g, PointF Dimensions)
        {
            base.Draw(g, Dimensions);
            g.DrawString(this.Text, F, new SolidBrush(C), Game.GameToScreenCoordinates(this.Position, Dimensions, size * Text.Length, size));
        }
    }
}
