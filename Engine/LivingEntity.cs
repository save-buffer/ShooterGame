using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class LivingEntity : PhysicsEntity
    {
        public int Health;
        public int MaxHealth;
        public string ViewName;

        public float JumpHeight;
        public float JumpVelocity
        {
            get
            {
                return (float)Math.Sqrt(2 * Math.Abs(g) * JumpHeight * Game.FrameRate);
            }
        }

        public LivingEntity(string Name, string Texture, uint Width, uint Height, string ViewName, int MaxHealth) :
            base(Name, Texture, Width, Height)
        {
            Health = MaxHealth;
            this.MaxHealth = MaxHealth;
            this.ViewName = ViewName;
            this.JumpHeight = 0;
        }

        public override void Update()
        {
            base.Update();
            if (Health <= 0)
                IsDead = true;
        }
    }
}
