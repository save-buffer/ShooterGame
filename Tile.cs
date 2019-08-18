using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
namespace ShooterGame
{
    class Tile : SolidObject
    {
        public Tile(string Name) :
            this(Name, Name + ".png")
        {
        }

        public Tile(string Name, string Texture) :
            this(Name, Texture, Tiles.TileWidth, Tiles.TileHeight)
        {
        }

        public Tile(string Name, string Texture, uint Width, uint Height) :
            base(Name, Texture, Width, Height, CollisionTypes.Rectangular)
        {
        }
    }
}
