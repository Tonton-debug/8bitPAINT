using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _8bitPaint
{
    public enum StatePicture
    {
        Coppy,
        Cut,
        None
    }
    public class Block
    {
        //  public static bool selectedBlocks { get; set; }
        
        public static byte[] souceBlockBytes = Tools.pixels;
        public static StatePicture statePicture { get; set; }
        public static bool selectedBlocks { get; set; }
        public static BitmapSource endBitmapSource = Tools.EditorBitMap;
        public static byte[] bytes_to_coppy { get; set; }
        public Color myColor;
        private Color lastColor = Color.FromArgb(0, 0, 0, 0);
        private int PosX;
        private int PosY;
        private static int width;
        private static int height;
        private static int per;

        public Block(int x, int y, Color color)
        {
            PosX = x;
            PosY = y;
            
            myColor = color;
           
            width = Tools.EditorBitMap.PixelWidth;
       height = Tools.EditorBitMap.PixelHeight;
      per = Tools.EditorBitMap.Format.BitsPerPixel;
            int pixelOffset = (PosX + PosY * width) * per / 8;
            Tools.pixels[pixelOffset] = myColor.B;
            Tools.pixels[pixelOffset + 1] = myColor.G;
            Tools.pixels[pixelOffset + 2] = myColor.R;
            Tools.pixels[pixelOffset + 3] = myColor.A;
            if (bytes_to_coppy != null)
            {
                bytes_to_coppy[pixelOffset] = myColor.B;
                bytes_to_coppy[pixelOffset + 1] = (byte)(myColor.G - 100);
                bytes_to_coppy[pixelOffset + 2] = myColor.R;
                bytes_to_coppy[pixelOffset + 3] = (byte)(myColor.A + 50);
            }
        }
        public void ClearPosAndMove()
        {

            int pixelOffset = (PosX + PosY * width) * per / 8;

            Tools.pixels[pixelOffset] = lastColor.B;
            Tools.pixels[pixelOffset + 1] = lastColor.G;
            Tools.pixels[pixelOffset + 2] = lastColor.R;
            Tools.pixels[pixelOffset + 3] = lastColor.A;
           
        }
        public void ClearPosAndMove(int xMove, int yMove)
        {
          
            int pixelOffset = (PosX + PosY * width) * per / 8;

            Tools.pixels[pixelOffset] = lastColor.B;
            Tools.pixels[pixelOffset + 1] = lastColor.G;
            Tools.pixels[pixelOffset + 2] = lastColor.R;
            Tools.pixels[pixelOffset + 3] = lastColor.A;
            PosY = PosY + yMove == height ? 0 : PosY + yMove;
            PosY = PosY < 0 ? height - 1 : PosY;
            PosX = PosX + xMove == width ? 0 : PosX + xMove;
            PosX = PosX < 0 ? width - 1 : PosX;
        }
        public  void Move()
        {
           
            int pixelOffset = (PosX + PosY * width) * per / 8;
            
            lastColor = Color.FromArgb(Tools.pixels[pixelOffset + 3], Tools.pixels[pixelOffset + 2], Tools.pixels[pixelOffset + 1], Tools.pixels[pixelOffset]);
            Tools.pixels[pixelOffset] = myColor.B;
            Tools.pixels[pixelOffset + 1] = myColor.G;
            Tools.pixels[pixelOffset + 2] = myColor.R;
            Tools.pixels[pixelOffset + 3] = myColor.A;
           




        }
        public void ClearColor()
        {
            int pixelOffset = (PosX + PosY * width) * per / 8;
           
         //   lastColor = Color.FromArgb(Tools.pixels[pixelOffset + 3], Tools.pixels[pixelOffset + 2], Tools.pixels[pixelOffset + 1], Tools.pixels[pixelOffset]);
            myColor.A += 50;
            myColor.G -= 100;
            Tools.pixels[pixelOffset] = myColor.B;
            Tools.pixels[pixelOffset + 1] = myColor.G;
            Tools.pixels[pixelOffset + 2] = myColor.R;
            Tools.pixels[pixelOffset + 3] = myColor.A;
        }
    }
}
