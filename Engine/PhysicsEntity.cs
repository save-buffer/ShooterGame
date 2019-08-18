using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Engine
{
    class PhysicsEntity : GameObject
    {
        public bool Gravity;
        public float Mass = 1;
        public float g;
        public PointF Velocity; //X and Y Velocities in Pixels/Second
        public float Friction;
        public float AngleOfMotion
        {
            get
            {
                return (float)(180 * Math.Atan2(Velocity.Y, Velocity.X) / (Math.PI));
            }
        }
        public float VelocityMagnitude
        {
            get
            {
                return (float)Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);
            }
        }
        public float Momentum
        {
            get
            {
                return Mass * VelocityMagnitude;
            }
        }
        public float KineticEnergy
        {
            get
            {
                return 0.5f * Mass * VelocityMagnitude * VelocityMagnitude;
            }
        }
        public float PotentialEnergy
        {
            get
            {
                return Mass * g * Position.Y;
            }
        }

        public PhysicsEntity(string Name, string Texture, uint Width, uint Height) : base(Name, Texture, Width, Height)
        {
            Gravity = true;
            g = -9.8f;
            Velocity = new PointF(0, 0);
        }
        public override void Update()
        {
            base.Update();
            if (Velocity.X != 0 && Velocity.Y != 0)
            {
                Position.X += Velocity.X / Game.FrameRate;
                Position.Y += Velocity.Y / Game.FrameRate;
            }

            if (Gravity)
                Velocity.Y += g;
        }
    }
}
