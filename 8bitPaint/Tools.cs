using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Draw = System.DrawingCore;
namespace _8bitPaint
{
  public  enum Instruments
    {
      Draw,
      Erase,
      Fill,
      Move,
      Dropper,
      Selected
    }

    public  class Tools
    {
 
        private object obj=new object();
        private static Image end;
        
        private static bool isFirstImage = true;
        public static byte[] pixels;
        public static BitmapSource EditorBitMap;
        public const int size = 1;
        public static Instruments instrumentsTool = Instruments.Draw;
        private static RenderTargetBitmap bitmap = new RenderTargetBitmap(size, size, 0, 0, PixelFormats.Pbgra32);
        public delegate void Draw(Image gett, int xx, int yy, Color draww,byte[] get_bytes,int size);
        public static Draw Draw_delegate = new Draw(ClickBlock);
        public static void ColorToHSV(System.Drawing.Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = (byte)Convert.ToInt32(value);
            byte p = (byte)Convert.ToInt32(value * (1 - saturation));
            byte q = (byte)Convert.ToInt32(value * (1 - f * saturation));
            byte t = (byte)Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
        public static Color GetPixel(BitmapSource bitmap, int x, int y)
        {
            
            
                CroppedBitmap cb = new CroppedBitmap(bitmap, new Int32Rect(x, y, 1, 1));
                byte[] pixel = new byte[bitmap.Format.BitsPerPixel / 8];
                //   MessageBox.Show(bitmap.Format.BitsPerPixel.ToString());
                cb.CopyPixels(pixel, bitmap.Format.BitsPerPixel / 8, 0);
                return Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
           

        }
        

        private static  void Fill(Point pt,Color replacementColor)
        {
 
            Stack<Point> _pixels = new Stack<Point>();
            List<Point> coordinate = new List<Point>();
            int pixelOffset;
            Color  targetColor = GetPixel(EditorBitMap, (int)pt.X, (int)pt.Y);
            _pixels.Push(pt);
            int timer =(int)(EditorBitMap.PixelWidth * EditorBitMap.PixelHeight/ 100);
            if (targetColor == replacementColor)
            {
                return;
            }
            while (_pixels.Count > 0)
            {
               
                Point a = _pixels.Pop();
                if ((int)a.X < EditorBitMap.PixelWidth && (int)a.X > -1 &&
                        (int)a.Y < EditorBitMap.PixelHeight && (int)a.Y > -1)
                {
                    pixelOffset = ((int)a.X + (int)a.Y * EditorBitMap.PixelWidth) * EditorBitMap.Format.BitsPerPixel / 8;
                    Color check = Color.FromArgb(pixels[pixelOffset + 3], pixels[pixelOffset + 2], pixels[pixelOffset + 1], pixels[pixelOffset]);
                    if (check == targetColor)
                    {
                        pixels[pixelOffset] = replacementColor.B;
                        pixels[pixelOffset + 1] = replacementColor.G;
                        pixels[pixelOffset + 2] = replacementColor.R;
                        pixels[pixelOffset + 3] = replacementColor.A;
                        _pixels.Push(new Point(a.X - 1, a.Y));
                        _pixels.Push(new Point(a.X + 1, a.Y));
                        _pixels.Push(new Point(a.X, a.Y - 1));
                        _pixels.Push(new Point(a.X, a.Y + 1));
                      
                        

                    }
                }
               
            }
            
        //    MessageBox.Show(stopwatch.ElapsedMilliseconds.ToString());
         //   end.Source = EditorBitMap;
            // pictureBox1.Refresh(); //refresh our main picture box
            //return;
        }
        public static BitmapSource ConvertLayersToPicture(List<byte[]> get,int sizeX,int sizeY,int perl)
        {
            int X = 0;
            int Y = 0;
         
            WriteableBitmap wb = new WriteableBitmap(sizeX, sizeY, 96, 96, PixelFormats.Pbgra32, null);
        
            List<byte[]> bytes = new List<byte[]>();
            int count_layer = 0;
            BitmapEncoder encoder = new PngBitmapEncoder();
            int stride= (sizeX * perl) / 8; ;
            int pixelOffset;
            WriteableBitmap writeable;
            Int32Rect rect = new Int32Rect(0, 0, sizeX, sizeY); ;
            Color color;
            byte[] pixels;
            wb.WritePixels(rect, get[0], stride, 0);
            foreach (byte[] get_bytes in get)
            {

                WriteableBitmap writeableBitmap = new WriteableBitmap(sizeX, sizeY, 96, 96, PixelFormats.Pbgra32, null);
                encoder = new PngBitmapEncoder();
                //   layer.main.IsHitTestVisible = true;
                //   layer.main.Opacity = 1;
                byte[] pixels_check = new byte[sizeX * sizeY * perl / 8];
                pixels = new byte[(int)sizeX * sizeY * perl / 8];
             
                rect = new Int32Rect(0, 0, sizeX, sizeY);
                stride = (sizeX * perl) / 8;
                writeableBitmap.WritePixels(rect, get_bytes, stride, 0);
                BitmapSource source = ConvertWriteableBitmapToBitmapImage(writeableBitmap);
          
                wb.CopyPixels(rect, pixels_check, stride, 0);
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        pixelOffset = (x + y * sizeX) * perl/ 8;
                        color = Tools.GetPixel(source, x, y);

                        if (count_layer > 0)
                        {
                            if (color.A != 0)
                            {

                                pixels[pixelOffset] = color.B;
                                pixels[pixelOffset + 1] = color.G;
                                pixels[pixelOffset + 2] = color.R;
                                pixels[pixelOffset + 3] = color.A;
                            }
                            else
                            {
                                pixels[pixelOffset] = pixels_check[pixelOffset];
                                pixels[pixelOffset + 1] = pixels_check[pixelOffset + 1];
                                pixels[pixelOffset + 2] = pixels_check[pixelOffset + 2];
                                pixels[pixelOffset + 3] = pixels_check[pixelOffset + 3];
                            }


                        }
                        else if (count_layer == 0)
                        {
                            pixels[pixelOffset] = color.B;
                            pixels[pixelOffset + 1] = color.G;
                            pixels[pixelOffset + 2] = color.R;
                            pixels[pixelOffset + 3] = color.A;

                        }
                    }
                }
                stride = (sizeX * perl) / 8;
                wb.WritePixels(rect, pixels, stride, 0);
                count_layer++;
                X = 0;
                Y = 0;
            }
            encoder.Frames.Add(BitmapFrame.Create(wb));
            BitmapSource a = encoder.Frames[0];
            return a;
        }
        public static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        } 
        public static BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        public static byte[] SetPicture(BitmapSource get, bool isNewFile = true)
        {
            EditorBitMap = get;
            int stride;
            byte[] pixels_local;
            int pixelOffset;
            WriteableBitmap writeable = new WriteableBitmap(EditorBitMap);
            Int32Rect rect = new Int32Rect(0, 0, (int)EditorBitMap.Width, (int)EditorBitMap.Height);
            Color color;
            pixels_local = new byte[(int)EditorBitMap.PixelWidth * (int)EditorBitMap.PixelHeight * EditorBitMap.Format.BitsPerPixel / 8];
            stride = (EditorBitMap.PixelWidth * EditorBitMap.Format.BitsPerPixel) / 8;
            writeable.CopyPixels(rect, pixels_local, stride, 0);
           
           // writeable.WritePixels(rect, pixels_local, stride, 0);
            return pixels_local;
        }
        public static void SetPixel(int get_x, int get_y, Color set, ref BitmapSource bitmap, int size, byte[] pixels_get = null)
        {
            bitmap = bitmap == null ? EditorBitMap : bitmap;
            int stride;
            int pixelOffset;
            WriteableBitmap writeable = new WriteableBitmap(bitmap);
            Int32Rect rect = new Int32Rect(0, 0, (int)bitmap.Width, (int)bitmap.Height);
            int sum = size % 2 == 0 ? size / 2 : (size + 1) / 2;


            for (int x = get_x+size-sum; x != get_x-sum; x--)
            {
                if (x >= bitmap.PixelWidth || x < 0)
                {
                    continue;
                }
                for (int y = get_y+size - sum; y != get_y - sum; y--)
                {
                    pixelOffset = (x + y * bitmap.PixelWidth) * bitmap.Format.BitsPerPixel / 8;
                    if (y >= bitmap.PixelHeight || y < 0)
                    {
                        continue;
                    }
                    if (pixels_get == null)
                    {
                        pixels[pixelOffset] = set.B;
                        pixels[pixelOffset + 1] = set.G;
                        pixels[pixelOffset + 2] = set.R;
                        pixels[pixelOffset + 3] = set.A;
                    }
                    else
                    {
                        pixels_get[pixelOffset] = set.B;
                        pixels_get[pixelOffset + 1] = set.G;
                        pixels_get[pixelOffset + 2] = set.R;
                        pixels_get[pixelOffset + 3] = set.A;
                    }
                }
            }



            stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel) / 8;
            writeable.WritePixels(rect, pixels_get == null ? pixels : pixels_get, stride, 0);

            isFirstImage = false;

            BitmapEncoder enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeable));


            bitmap = (BitmapSource)enc.Frames[0];
            // get.Source = EditorBitMap;
            // end.Source = EditorBitMap;

            if (bitmap == EditorBitMap)
            {
                //  SetPointColor(get_x, get_y, set.ToString());
            }


        }
        private static void SetPointColor(int get_x,int get_y,string get_color)
        {
            PointColor pointColor = new PointColor();
            pointColor.color = get_color.ToString();
            pointColor.X = get_x;
            pointColor.Y = get_y;
            Frame.frames[Frame.ActiveFrameId].layers[(Application.Current.MainWindow as MainWindow).selected_layer].GetPointColors.Add(pointColor);
        } 
        public static  void SetPixel(List<Point> get_pixels, Color set,bool isEnd=false)
        {
            int stride;
            int pixelOffset;

           
            foreach (Point gett in get_pixels)
            {
                pixelOffset = (int)((gett.X + gett.Y * EditorBitMap.PixelWidth) * EditorBitMap.Format.BitsPerPixel / 8);
                pixels[pixelOffset] = set.B;
                pixels[pixelOffset + 1] = set.G;
                pixels[pixelOffset + 2] = set.R;
                pixels[pixelOffset + 3] = set.A;
             //   SetPointColor((int)gett.X, (int)gett.Y, set.ToString());
            }
           
     
          

            //  isFirstImage = false;
            if (isEnd)
            {
             //   WriteableBitmap writeable = new WriteableBitmap(EditorBitMap);
                //Int32Rect rect = new Int32Rect(0, 0, (int)EditorBitMap.Width, (int)EditorBitMap.Height);
                //stride = (EditorBitMap.PixelWidth * EditorBitMap.Format.BitsPerPixel) / 8;
                //writeable.WritePixels(rect, pixels, stride, 0);
                //BitmapEncoder enc = new PngBitmapEncoder();
                //enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeable));


                //EditorBitMap = (BitmapSource)enc.Frames[0];
            }
           
           
           
            // get.Source = EditorBitMap;

        }
        public static void ReplaceColor(Brush get, Image img, string tag)
        {

           
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(get, null, new Rect(0, 0, size, size));
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap(size, size, 0, 0, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);
            img.Source = bitmap;
            img.Tag = tag == null ? img.Tag.ToString() : tag;
         

        }
        public static void ClickBlock(Image get,int x,int y, Color draw,byte[] get_byte,int _size)
        {
            pixels = get_byte;
            EditorBitMap =(BitmapSource)get.Source;
          
           Tools tools=new Tools();
            switch (instrumentsTool)
            {
                case Instruments.Draw:
                    SetPixel(x, y, draw, ref EditorBitMap,_size);
                   
                    break;
                case Instruments.Erase:
                    SetPixel(x, y, Color.FromArgb(0,0,0,0), ref EditorBitMap,_size);
                  
                    break;
                case Instruments.Fill:
                
                    Fill(new Point(x, y), draw);
                    SetPixel(x, y, draw, ref EditorBitMap,1);
                    break;
                case Instruments.Dropper:
                    (Application.Current.MainWindow as MainWindow).Palytre1.VisualSelectedColorImage(GetPixel(EditorBitMap, x, y));
                    break;

            }
            get.Source = EditorBitMap;
        }
    }
}
