using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Engine;
using XInputWrapper;
namespace ShooterGame
{
    class Player2 : Engine.LivingEntity
    {
        public bool Shielding;

        Gun gun;
        Shield shield;
        XboxController controller;
        XInputVibration vibration;

        public Player2() : base("Player2", null, 32, 32, "Player2", 100)
        {
            this.Gravity = false;
            this.Collision = CollisionTypes.Circular;
            this.CollisionRadius = 32;
            this.ScreenWrapping = true;

            this.ClearFrames();
            this.AddFrame("Player2.png", 0);
            GoToFrame(0);

            gun = new Gun(this);
            shield = new Shield(ref this.Position, 2);
            controller = XboxController.RetrieveController(1);

            Position.X = 200;

            Velocity.X = 0;
            Velocity.Y = 0;
        }

        public void Halt()
        {
            Velocity.X = 0;
            Velocity.Y = 0;
        }

        public void Boost()
        {

            float magnitude = (float)Math.Sqrt(controller.LeftThumbStick.X * controller.LeftThumbStick.X +
                            controller.LeftThumbStick.Y * controller.LeftThumbStick.Y);

            if (magnitude > 32767)
                magnitude = 32767;
            if (magnitude > XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE)
            {
                magnitude -= XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE;
                var NormalizedX = (controller.LeftThumbStick.X) / magnitude;
                var NormalizedY = (controller.LeftThumbStick.Y) / magnitude;
                this.Velocity.X += NormalizedX * 300.0f;
                this.Velocity.Y += NormalizedY * 300.0f;
            }
            #region old_boost
            /*
            float scale = Math.Min(Math.Min(Math.Abs(MAX_VELOCITY /Velocity.X), 
                Math.Abs(MAX_VELOCITY/Velocity.Y)), 10);
            Velocity.X *= scale;
            Velocity.Y *= scale;*/
            #endregion
        }

        bool can_boost = true;
        int boost_cd = 240;
        public const float MAX_VELOCITY = 1000;
        public override void Update()
        {
            base.Update();
            float vel_mag = (float)Math.Sqrt(Velocity.X * Velocity.X +
                Velocity.Y * Velocity.Y);
            float magnitude = (float)Math.Sqrt(controller.LeftThumbStick.X * controller.LeftThumbStick.X +
                            controller.LeftThumbStick.Y * controller.LeftThumbStick.Y);

            if (magnitude > 32767)
                magnitude = 32767;
            if (magnitude > XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE)
            {
                magnitude -= XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE;
                var NormalizedX = (controller.LeftThumbStick.X) / magnitude;
                var NormalizedY = (controller.LeftThumbStick.Y) / magnitude;
                this.Velocity.X += NormalizedX * 6.0f;
                this.Velocity.Y += NormalizedY * 6.0f;
            }

            if (controller.IsRightShoulderPressed)
                Halt();
            if (controller.IsLeftShoulderPressed && can_boost)
            {
                Boost();
                boost_cd = 240;
                can_boost = false;
            }
            if (!controller.IsLeftShoulderPressed && boost_cd == 0)
            {
                can_boost = true;
            }
            if (boost_cd > 0)
                boost_cd--;
            if (vel_mag > MAX_VELOCITY)
            {
                Velocity.X = MAX_VELOCITY * (float)Math.Cos(AngleOfMotion * Math.PI / 180);
                Velocity.Y = MAX_VELOCITY * (float)Math.Sin(AngleOfMotion * Math.PI / 180);
            }
            Shielding = controller.LeftTrigger > XInputConstants.XINPUT_GAMEPAD_TRIGGER_THRESHOLD;


            if (Game.IsKeyPressed(System.Windows.Forms.Keys.W))
                Velocity.Y += 6;
            if (Game.IsKeyPressed(System.Windows.Forms.Keys.S))
                Velocity.Y -= 6;
            if (Game.IsKeyPressed(System.Windows.Forms.Keys.A))
                Velocity.X -= 6;
            if (Game.IsKeyPressed(System.Windows.Forms.Keys.D))
                Velocity.X += 6;

        }


        public override void OnDeath()
        {
            base.OnDeath();
            this.gun.IsDead = true;
            this.shield.IsDead = true;
            TextObject t = new TextObject("victory", "Player 1 wins!");
            t.ZOrder = 20;
            t.F = new Font("Bauhaus 93", 20);
            t.Position.Y = 200;
            Game.AddElement(t);
            Game.RemoveElement("p2h");

            ParticleSystem p = new ParticleSystem("pink", Color.HotPink, 400, 300, 3);
            ParticleSystem p1 = new ParticleSystem("red", Color.Red, 400, 300, 3);
            ParticleSystem p2 = new ParticleSystem("blue", Color.Blue, 400, 300, 3);
            ParticleSystem p3 = new ParticleSystem("aquamarine", Color.Aquamarine, 400, 300, 3);
            p.ParticleSize = 6;
            p2.ParticleSize = 6;
            p2.ParticleSize = 6;
            p3.ParticleSize = 6;
            Game.AddElement(p);
            Game.AddElement(p1);
            Game.AddElement(p2);
            Game.AddElement(p3);
        }

    }
}
