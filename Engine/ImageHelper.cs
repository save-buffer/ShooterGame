using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Engine
{
    class ImageHelper
    {
        //Supports the resizing of an image with no smoothing and nearest neighbor interpolation.
        public static Image ResizeImage(Image Image, int Width, int Height)
        {
            Bitmap DestImage = new Bitmap(Width, Height);
            Rectangle DestRect = new Rectangle(0, 0, Width, Height);
            Graphics G = Graphics.FromImage(DestImage);

            G.InterpolationMode = InterpolationMode.NearestNeighbor;
            G.SmoothingMode = SmoothingMode.None;
            G.DrawImage(Image, DestRect, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);

            return DestImage;
        }

        public static Image[] GenerateCharacter(int Player)
        {
            const int NumberOfFrames = 4;
            Image[] i = new Image[NumberOfFrames * 2];
            for (int j = 0; j < i.Length; j++)
            {
                i[j] = Image.FromFile("Assets\\Player" + Player + "_" + j % 4 + ".png");
                if (j >= i.Length / 2)
                    i[j].RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            return i;
        }
        public static Image GenerateBackground()
        {
            Image bitmap = new Bitmap(1280, 720);
            using (TextureBrush brush = new TextureBrush(Image.FromFile("Assets\\background2.png"), WrapMode.Tile))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // Do your painting in here
                    g.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
                }
            }
            bitmap.Save("Background.png");
            return bitmap;
        }
}
}
