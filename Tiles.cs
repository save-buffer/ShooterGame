using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterGame
{
    class Tiles
    {
        //Constants for the tile width and height in pixels.
        public const int TileWidth = 32;
        public const int TileHeight = 32;

        //List of tiles.
        public static Tile TileDefault = new Tile("tileDefault");
    }
}
