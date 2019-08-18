using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;

namespace ShooterGame
{
    class Lifebar : GameObject
    {
        public int Health = 100;
        bool LeftToRight;
        public Lifebar(string Name, string Texture1, bool LeftToRight, int width, int height) : base(Name, Texture1, (uint)width, (uint)height)
        {
            this.LeftToRight = LeftToRight;
        }

        public override void Draw(Graphics G, Engine.PointF Dimensions)
        {
            var p = Game.GameToScreenCoordinates(this.Position, Dimensions, (int)Width * 100, (int)Height);
            if(LeftToRight)
            {
                for (int i = 0; i < Health; i++)
                {
                    G.DrawImage(this.Models[0].Image, p);
                    p.X += Width;
                }
            }
            else
            {
                p.X += Width * 100;
                for(int i = 0; i < Health; i++)
                {
                    G.DrawImage(this.Models[0].Image, p);
                    p.X -= Width;
                }
            }
        }
    }
}
