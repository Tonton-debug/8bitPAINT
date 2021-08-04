using System;
using System.Collections.Generic;
using Draw = System.DrawingCore;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.DrawingCore.Imaging;

namespace _8bitPaint
{
  public  enum State
    {
        Move,
        Unity,
        Copy,
       
        None

    }
    [Serializable]
    public struct PointColor
    {
        public int X;
        public int Y;
        public string color;
     
    }
    [Serializable]
    public class Layer
    {
      
        
        public bool isActive = true;
        public bool isNewFile;
        [NonSerialized]
        public Image main;
        [NonSerialized]
        public ImageSource mainSource;
        [NonSerialized]
        public WriteableBitmap mainSourceWB;
        public byte[] imageSourceMain;
        [NonSerialized]
        public static  State stateLayers = State.None;
        public static int idMake = -1;
        [NonSerialized]
        public static Image endImage;
        public string layerName { get; set; }
        public List<PointColor> GetPointColors = new List<PointColor>();
       
            
        public  void SetMake(State get,int id)
        {
            idMake = id;
            stateLayers = get;
           
            

        }
        public Layer(Image _get,ImageSource _get_source,bool newFile=true)
        {
            mainSource = _get_source;
            isNewFile = newFile;
            //  int stride = (source.PixelWidth * source.Format.BitsPerPixel) / 8;
            //  WriteableBitmap writeable = new WriteableBitmap(source);
            mainSourceWB = new WriteableBitmap((BitmapSource)mainSource);
            imageSourceMain = Tools.SetPicture(((BitmapSource)mainSource),newFile);
            main = _get;

         //   imageSourceMain = new byte[(int)source.PixelWidth * (int)source.PixelHeight * source.Format.BitsPerPixel / 8];
         //   writeable.CopyPixels(imageSourceMain, stride, 0);

        }
      
    }
}
