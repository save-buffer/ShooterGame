using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using XInputWrapper;

namespace ShooterGame
{
    class Shield : GameObject
    {
        Engine.PointF Following;
        int player;
        XboxController controller;
        public int ShieldDuration = 5; //In seconds
        private int FramesOfShield;
        public Shield(ref Engine.PointF Position, int player) : base("Shield" + player, null, 50, 50)
        {
            this.Following = Position;
            this.player = player;
            this.Visible = false;
            Game.AddElement(this);
            this.ZOrder = 10;
            controller = XboxController.RetrieveController(player - 1);
            FramesOfShield = (int)(Game.FrameRate * ShieldDuration);
        }
        bool prev_visible = false;
        public override void Update()
        {
            base.Update();
            prev_visible = this.Visible;
            this.Visible = FramesOfShield >= 0 && controller.LeftTrigger > XInputConstants.XINPUT_GAMEPAD_TRIGGER_THRESHOLD;
            if (this.Visible != prev_visible)
                FramesOfShield -= 60;
            if (this.Visible)
                FramesOfShield--;
            else if (FramesOfShield > 0 &&  FramesOfShield < (int)(Game.FrameRate * ShieldDuration))
                FramesOfShield++;
            else if (FramesOfShield <= 0)
                FramesOfShield--;
            if (FramesOfShield == -(int)(Game.FrameRate * ShieldDuration))
                FramesOfShield = -FramesOfShield;
        }

        public override void Draw(Graphics G, Engine.PointF Dimensions)
        {
            base.Draw(G, Dimensions);
            var p = Game.GameToScreenCoordinates((Engine.PointF)Following.Clone(), Dimensions, 50, 50);

            float af = (255 * ((float)FramesOfShield / (float)(Game.FrameRate * ShieldDuration)));
            if (af > 255)
                af = 255;
            if (af < 0)
                af = 0;
            int a = (int)af;

            int r = 0;
            int g = 255 * Math.Abs(player - 2);
            int b = 255 * (player - 1);

            G.DrawEllipse(new Pen(Color.FromArgb(a, r, g, b), 3), p.X, p.Y, 50, 50);
            G.FillEllipse(new SolidBrush(Color.FromArgb((int)(af / 255 * 100), r, g, b)), p.X, p.Y, 50, 50);
        }
    }
}
