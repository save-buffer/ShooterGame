using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace ShooterGame
{
    class Level : Engine.State
    {
        public override void Initialize()
        {
            base.Initialize();
            p1 = new Player1();
            p2 = new Player2();

            p1h = new Lifebar("p1h", "Lifebar1.png", true, 4, 6);
            p2h = new Lifebar("p2h", "Lifebar2.png", false, 4, 6);

            p1n = new TextObject("p1n", "Player 1");
            p2n = new TextObject("p2n", "Player 2");

            p1n.F = new System.Drawing.Font("Bauhaus 93", p1n.F.Size + 2);
            p2n.F = new System.Drawing.Font("Bauhaus 93", p2n.F.Size + 2);

            p1n.Position.X = -450;
            p1n.Position.Y = 280;
            p2n.Position.X = 480;
            p2n.Position.Y = 280;

            Game.AddElement(p1n);
            Game.AddElement(p2n);

            Countdown = new TextObject("Countdown", "3");

            p1h.Position.X = -300;
            p2h.Position.X = 300;
            p1h.Position.Y = 300;
            p2h.Position.Y = 300;
            GameObject background = new GameObject("background", "slitherbackground.png", 1280, 720);
            background.ZOrder = -100;
            background.Visible = true;
            ImageHelper.GenerateBackground();
            Game.AddElement(background);

            Game.AddElement(p1);
            Game.AddElement(p2);
            Game.AddElement(p1h);
            Game.AddElement(p2h);
        }

        Player1 p1;
        Player2 p2;
        Lifebar p1h;
        Lifebar p2h;
        TextObject p1n;
        TextObject p2n;
        TextObject Countdown;

        public override void Update()
        {
                base.Update();

            p1h.Health = p1.Health;
            p2h.Health = p2.Health;
            if (Game.IsKeyPressed(Keys.Escape))
                Game.Quit();
        }
    }
}
