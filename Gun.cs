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
    class Gun : GameObject
    {
        Player1 Following;
        Player2 Following2;
        XboxController controller;
        XInputVibration vibration;
        int PlayerNumber;
        List<float> angles;
        float velocity = 0;
        public const float VELOCITY_INCREMENT = 0.3f;
        public const float MAX_VELOCITY = 20;

        public Gun(GameObject Player) : base("Gun" + (Player.Name == "Player1" ? 1 : 2),
            null,
            32,
            32)
        {
            angles = new List<float>(40);

            if (Player.Name == "Player1")
            {
                Following = (Player1)Player;
                controller = XboxController.RetrieveController(0);
            }
            else
            {
                Following2 = (Player2)Player;
                controller = XboxController.RetrieveController(1);
            }
            PlayerNumber = Player.Name == "Player1" ? 1 : 2;
            Game.AddElement(this);
            this.AddFrame(new Bitmap(32, 32), 0);
        }

        public void Shoot()
        {
            foreach (float angle in angles)
                Game.AddElement(new Bullet(Position.X, Position.Y, angle, velocity, PlayerNumber));
        }

        public override void Update()
        {
            base.Update();
            if (PlayerNumber == 1)
            {
                this.Position.X = Following.Position.X;
                this.Position.Y = Following.Position.Y;
            }
            else
            {
                this.Position.X = Following2.Position.X;
                this.Position.Y = Following2.Position.Y;
            }

            if (controller.RightTrigger > XInputConstants.XINPUT_GAMEPAD_TRIGGER_THRESHOLD || Game.IsKeyPressed(System.Windows.Forms.Keys.F))
            {
                if (velocity + VELOCITY_INCREMENT < MAX_VELOCITY)
                {
                    velocity += VELOCITY_INCREMENT;
                }
                    vibration.LeftMotorSpeed = vibration.RightMotorSpeed = (ushort)(65535 * (velocity / MAX_VELOCITY));
                    controller.Vibrate(vibration);
                    float magnitude = (float)Math.Sqrt(controller.RightThumbStick.X * controller.RightThumbStick.X +
                        controller.RightThumbStick.Y * controller.RightThumbStick.Y);
                    if (magnitude > XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE)
                    {
                        float angle = (float)(Math.Atan2(controller.RightThumbStick.Y, controller.RightThumbStick.X) * 180.0 / Math.PI);
                        angles.Add(angle);
                }

            }
            else if (angles.Count > 0 || Game.IsKeyReleased(System.Windows.Forms.Keys.F))
            {
                Shoot();
                angles.Clear();
                velocity = 0;
                vibration.LeftMotorSpeed = vibration.RightMotorSpeed = 0;
                controller.Vibrate(vibration);
            }
            if (controller.RightTrigger < XInputConstants.XINPUT_GAMEPAD_TRIGGER_THRESHOLD)
            {
                vibration.LeftMotorSpeed = vibration.RightMotorSpeed = 0;
                controller.Vibrate(vibration);
            }
        }
        public override void Draw(Graphics G, Engine.PointF Dimensions)
        {
            base.Draw(G, Dimensions);
            Engine.PointF pos;
            if (this.PlayerNumber == 1)
                pos = Following.Position;
            else
                pos = Following2.Position;
            var ScreenPosition = Game.GameToScreenCoordinates(pos, Dimensions, 32, 32);
            ScreenPosition.X += 16;
            ScreenPosition.Y += 16;
            foreach (float angle in angles)
            {
                System.Drawing.PointF p1 = new System.Drawing.PointF(ScreenPosition.X, ScreenPosition.Y);
                System.Drawing.PointF p2 = new System.Drawing.PointF(ScreenPosition.X, ScreenPosition.Y);
                p1.X += (float)Math.Cos(-angle * Math.PI / 180.0) * 30;
                p1.Y += (float)Math.Sin(-angle * Math.PI / 180.0) * 30;

                p2.X += (float)Math.Cos(-angle * Math.PI / 180.0) * 40;
                p2.Y += (float)Math.Sin(-angle * Math.PI / 180.0) * 40;
                int r = 100;
                int g, b;
                if (this.PlayerNumber == 1)
                {
                    g = (int)(255 * velocity / MAX_VELOCITY);
                    b = 100;
                }
                else
                {
                    g = 100;
                    b = (int)(255 * velocity / MAX_VELOCITY);
                }

                G.DrawLine(new Pen(Color.FromArgb(255, r, g, b), 10),
                p1, p2);
            }
        }
    }
}
