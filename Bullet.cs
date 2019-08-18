using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;

namespace ShooterGame
{
    class Bullet : GameObject
    {
        static int c = 0;
        float angle;
        float velocity;
        int player;
        public Bullet(float x, float y, float angle, float velocity, int player) : base("Bullet" + c++, "Bullet" + player + ".png", 8, 8)
        {
            this.Collision = CollisionTypes.Circular;
            this.CollisionRadius = 8;
            this.angle = angle;
            this.velocity = velocity;
            this.Position.X = x;
            this.Position.Y = y;
            this.player = player;
        }
        public override void Update()
        {
            base.Update();
            bool collided = false;
            if (player == 2)
            {
                foreach (object player in Game.CurrentState.Elements.Values)
                    if (player is Player1)
                        if (this.IsCollidedWithGameObject((Player1)player))
                        {
                            Player1 p = (Player1)player;
                            if(!p.Shielding)
                                p.Health -= 1;
                            collided = true;
                        }
            }
            else
            {
                foreach (object player in Game.CurrentState.Elements.Values)
                    if (player is Player2)
                        if (this.IsCollidedWithGameObject((Player2)player))
                        {
                            Player2 p = (Player2)player;
                            if (!p.Shielding)
                                p.Health -= 1;
                            collided = true;
                        }
            }
            if (collided || !IsInViewport)
                this.IsDead = true;

            this.Position.X += (float)(Math.Cos(Math.PI / 180.0 * angle) * velocity);
            this.Position.Y += (float)(Math.Sin(Math.PI / 180.0 * angle) * velocity);
        }
    }
}
