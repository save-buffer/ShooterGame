using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class SolidObject : GameObject
    {
        public SolidObject(string Name, string Texture, uint Width, uint Height) :
            this(Name, Texture, Width, Height, CollisionTypes.Rectangular)
        {
        }

        public SolidObject(string Name, string Texture, uint Width, uint Height, CollisionTypes CollisionType) :
            base(Name, Texture, Width, Height)
        {
            Collision = CollisionType;
            CollisionBox = new Engine.PointF(Width, Height);
        }
    }
}
