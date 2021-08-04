using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _8bitPaint
{
    [Serializable]
    public class Frame
    {
        [NonSerialized]
        public Image main;
        public byte[] imageSourceMain;
        [NonSerialized]
        WriteableBitmap wb;
        [NonSerialized]
        public BitmapSource FrameImageSource;
        [NonSerialized]
        public List<WriteableBitmap> writeableBitmaps = new List<WriteableBitmap>();
        public static Image endImage;
        public static int LastActiveFrameId = 0;
        public static int ActiveFrameId = 0;
        public List<Layer> layers = new List<Layer>();
        public static List<Frame> frames = new List<Frame>();
        public string frameName { get; set; }
        public Frame(int id = -1)
        {
            LastActiveFrameId = ActiveFrameId;
            frames.Add(this);
            ActiveFrameId = id == -1 ? frames.Count - 1 : id;
          
        }
        public static void UpdatebitmapSources()
        {

            foreach (Frame frame in Frame.frames)
            {
                int q = 0;
                foreach (Layer layer in frame.layers)
                {
                    if (frame.writeableBitmaps.Count <= q)
                    {
                        frame.writeableBitmaps.Add(new WriteableBitmap((BitmapSource)layer.main.Source));
                    }
                    else
                    {
                      frame.writeableBitmaps[q] = new WriteableBitmap((BitmapSource)layer.main.Source);
                    }
                    q++;
                  
                }
            }

        }
        public void RemoveLayers(Canvas getPanel, UIElementCollection get, WrapPanel wrapPanel, UIElementCollection get2, ref int layers_count, bool isRestart = false)
        {
            int count = wrapPanel.Children.Count;
            for (int i = 0; i < count; i++)
            {
               
                try { layers[i].mainSource = layers[i].main.Source; } catch { };
                try { wrapPanel.Children.Remove(get2[0]); } catch { };
                try { getPanel.Children.Remove(get[0]); } catch { };
               
             
                //      layers[i].main = null;


            }
            if (isRestart)
            {
                layers = null;
                frames.RemoveAt(ActiveFrameId);
                ActiveFrameId = LastActiveFrameId;
            }
            layers_count = 0;
        }


        public static void SaveToPng(string get, string fither)
        {
            int id = 1;

            int X = 0;
            int Y = 0;
            BitmapSource EditorBitMap = (BitmapSource)frames[0].layers[0].main.Source;
            WriteableBitmap wb = new WriteableBitmap(EditorBitMap.PixelWidth, EditorBitMap.PixelHeight, 96, 96, PixelFormats.Pbgra32, null);
            List<byte[]> bytes = new List<byte[]>();
            int count_layer = 0;
            BitmapEncoder encoder = new PngBitmapEncoder();
            int stride;
            int pixelOffset;
            WriteableBitmap writeable;
            Int32Rect rect;
            Color color;
            byte[] pixels;

            pixels = new byte[(int)EditorBitMap.PixelWidth * (int)EditorBitMap.PixelHeight * EditorBitMap.Format.BitsPerPixel / 8];
          
            foreach (Frame frame in frames)
            {
                foreach (Layer layer in frame.layers)
                {
                    int x = 0;
                    int y = 0;
                    encoder = new PngBitmapEncoder();
                    //   layer.main.IsHitTestVisible = true;
                    //   layer.main.Opacity = 1;
                    byte[] pixels_check = new byte[(int)EditorBitMap.PixelWidth * (int)EditorBitMap.PixelHeight * EditorBitMap.Format.BitsPerPixel / 8];
                    pixels = layer.imageSourceMain;
                    writeable = new WriteableBitmap(EditorBitMap);
                    rect = new Int32Rect(0, 0, (int)EditorBitMap.Width, (int)EditorBitMap.Height);
                    stride = (EditorBitMap.PixelWidth * EditorBitMap.Format.BitsPerPixel) / 8;
                    wb.CopyPixels(rect, pixels_check, stride, 0);
                    for (int i = 3; i < pixels.Length; i += 4)
                    {


                        //pixelOffset = (x + y * EditorBitMap.PixelWidth) * EditorBitMap.Format.BitsPerPixel / 8;
                        color = Color.FromArgb(pixels[i], pixels[i-1], pixels[i-2], pixels[i-3]);

                        if (count_layer > 0)
                        {
                            if (color.A != 0)
                            {

                                pixels[i - 3] = color.B;
                                pixels[i - 2] = color.G;
                                pixels[i - 1] = color.R;
                                pixels[i] = color.A;
                            }
                            else
                            {
                                pixels[i - 3] = pixels_check[i - 3];
                                pixels[i - 2] = pixels_check[i - 2];
                                pixels[i - 1] = pixels_check[i - 1];
                                pixels[i] = pixels_check[i];
                            }


                        }
                        else if (count_layer == 0)
                        {
                            pixels[i - 3] = color.B;
                            pixels[i - 2] = color.G;
                            pixels[i - 1] = color.R;
                            pixels[i] = color.A;

                        }
                       // y += x == EditorBitMap.PixelWidth ? y++ : 0;
                       // x = x == EditorBitMap.PixelWidth ? x = 0 : x+=1;
                    }
                        
                    
                    stride = (EditorBitMap.PixelWidth * EditorBitMap.Format.BitsPerPixel) / 8;
                    wb.WritePixels(rect, pixels, stride, 0);
                    count_layer++;
                    X = 0;
                    Y = 0;
                }
                encoder.Frames.Add(BitmapFrame.Create(wb));
                using (var fileStream = new FileStream(get, FileMode.Create))
                {
                    encoder.Save(fileStream);

                }
                FileInfo fileInfo = new FileInfo(get);
             
                get = fileInfo.DirectoryName + @"\" + fileInfo.Name.Replace(fileInfo.Extension, "").Replace("_" + (id - 1), "") + "_" + id + fileInfo.Extension;
                wb = new WriteableBitmap(EditorBitMap.PixelWidth, EditorBitMap.PixelHeight, 96, 96, PixelFormats.Pbgra32, null);
                id++;

            }

        }
       
        public static BitmapSource FrameToSource(int[] id,int id_layer)
        {

            int X = 0;
            int Y = 0;
            BitmapSource EditorBitMap = (BitmapSource)frames[0].layers[0].main.Source;
            WriteableBitmap wb = new WriteableBitmap(EditorBitMap.PixelWidth, EditorBitMap.PixelHeight, 96, 96, PixelFormats.Pbgra32, null);
            List<byte[]> bytes = new List<byte[]>();
            int count_layer = 0;
            BitmapEncoder encoder = new PngBitmapEncoder();
            int stride;
            int pixelOffset;
            WriteableBitmap writeable;
            Int32Rect rect;
            Color color;
            byte[] pixels;


            foreach (int get in id)
            {
                GC.Collect();
                Layer layer = frames[id_layer].layers[get];
                encoder = new PngBitmapEncoder();
                //   layer.main.IsHitTestVisible = true;
                //   layer.main.Opacity = 1;
                byte[] pixels_check = new byte[(int)EditorBitMap.PixelWidth * (int)EditorBitMap.PixelHeight * EditorBitMap.Format.BitsPerPixel / 8];
                pixels = Frame.frames[ActiveFrameId].layers[get].imageSourceMain;
                writeable = new WriteableBitmap(EditorBitMap);
                rect = new Int32Rect(0, 0, (int)EditorBitMap.Width, (int)EditorBitMap.Height);
                stride = (EditorBitMap.PixelWidth * EditorBitMap.Format.BitsPerPixel) / 8;

                wb.CopyPixels(rect, pixels_check, stride, 0);
                for (int i = 3; i < pixels.Length; i += 4)
                {


                    //pixelOffset = (x + y * EditorBitMap.PixelWidth) * EditorBitMap.Format.BitsPerPixel / 8;
                    color = Color.FromArgb(pixels[i], pixels[i - 1], pixels[i - 2], pixels[i - 3]);

                    if (count_layer > 0)
                    {
                        if (color.A != 0)
                        {

                            pixels[i - 3] = color.B;
                            pixels[i - 2] = color.G;
                            pixels[i - 1] = color.R;
                            pixels[i] = color.A;
                        }
                        else
                        {
                            pixels[i - 3] = pixels_check[i - 3];
                            pixels[i - 2] = pixels_check[i - 2];
                            pixels[i - 1] = pixels_check[i - 1];
                            pixels[i] = pixels_check[i];
                        }


                    }
                    else if (count_layer == 0)
                    {
                        pixels[i - 3] = color.B;
                        pixels[i - 2] = color.G;
                        pixels[i - 1] = color.R;
                        pixels[i] = color.A;

                    }
                    // y += x == EditorBitMap.PixelWidth ? y++ : 0;
                    // x = x == EditorBitMap.PixelWidth ? x = 0 : x+=1;
                }


                stride = (EditorBitMap.PixelWidth * EditorBitMap.Format.BitsPerPixel) / 8;
                wb.WritePixels(rect, pixels, stride, 0);
                count_layer++;
                X = 0;
                Y = 0;
            }
        
            encoder.Frames.Add(BitmapFrame.Create(wb));
            BitmapSource a = encoder.Frames[0];
            return a;

        }
        public static BitmapSource FrameToSource(BitmapSource get_source, int id = 0)
        {
            UpdatebitmapSources();
            int X = 0;
            int Y = 0;
            BitmapSource EditorBitMap = (BitmapSource)frames[0].layers[0].main.Source;
            frames[id].wb = frames[id].wb==null?new WriteableBitmap(EditorBitMap.PixelWidth, EditorBitMap.PixelHeight, 96, 96, PixelFormats.Pbgra32, null) : frames[id].wb;
            List<byte[]> bytes = new List<byte[]>();
            int count_layer = 0;
            BitmapEncoder encoder = new PngBitmapEncoder();
            int stride;
            int pixelOffset;
            WriteableBitmap writeable;
            WriteableBitmap writeable_end=new WriteableBitmap(frames[id].writeableBitmaps[0]);
            Int32Rect rect;
            Color color;
            Color color_check;
            byte[] pixels;
            byte[] pixels_end;
            byte[] pixels_check = new byte[(int)EditorBitMap.PixelWidth * (int)EditorBitMap.PixelHeight * EditorBitMap.Format.BitsPerPixel / 8];
         
            pixels = new byte[(int)EditorBitMap.PixelWidth * (int)EditorBitMap.PixelHeight * EditorBitMap.Format.BitsPerPixel / 8];
            pixels_end = new byte[(int)EditorBitMap.PixelWidth * (int)EditorBitMap.PixelHeight * EditorBitMap.Format.BitsPerPixel / 8];
            stride = (EditorBitMap.PixelWidth * EditorBitMap.Format.BitsPerPixel) / 8;
            rect = new Int32Rect(0, 0, (int)EditorBitMap.Width, (int)EditorBitMap.Height);
            writeable_end.CopyPixels(rect, pixels, stride, 0);
         
            foreach (Layer layer in frames[id].layers)
            {

               
                if (layer.isActive)
                {
                    
                    writeable = frames[id].writeableBitmaps[count_layer];
                   
                    writeable.CopyPixels(rect, pixels_check, stride, 0);
                   
                   
                    if (count_layer != 0)
                    {
                        for (int i = 3; i < pixels_check.Length; i += 4)
                        {
                            if (pixels_check[i] == 0)
                            {
                                pixels_check[i] = pixels[i];
                                pixels_check[i-1] = pixels[i-1];
                                pixels_check[i-2] = pixels[i-2];
                                pixels_check[i-3] = pixels[i-3];
                            }
                        }
                    }
                    writeable_end.WritePixels(rect, pixels_check, stride, 0);
                    writeable_end.CopyPixels(rect, pixels, stride, 0);
                    get_source = writeable_end;     
                    count_layer++;
                    X = 0;
                    Y = 0;
                 
                }
               
            }
           UpdatebitmapSources();
            return get_source;

        }
        
    }
          
}

