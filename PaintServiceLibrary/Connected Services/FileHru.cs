using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace _8bitPaint
{
    [Serializable]
    class FileHru
    {
        public List<List<byte[]>> infoFramesImage = new List<List<byte[]>>();
        public List<string> colors = new List<string>();
        public int sizeX;
        public int sizeY;
        public int per;
        public List<PointColor> GetPointColors = new List<PointColor>();
        public Dictionary<int, List<string>> layersNames = new Dictionary<int, List<string>>();
        public List<string> framesNames =new List<string>();
        public string fileName;
        public FileHru(List<Frame> get_frame,int x,int y,int _per,List<string> get_colors,string name)
        {
            PointColor pointColor = new PointColor();
            fileName = name;
            GetPointColors.Add(pointColor);
            int i = 0;
            sizeX = x;
            sizeY = y;
            per = _per;
            colors = get_colors;
           
            foreach(Frame frame in get_frame)
            {
                List<string> fileNames = new List<string>();
                infoFramesImage.Add(new List<byte[]>());
                framesNames.Add(frame.frameName);
                foreach (Layer layer in frame.layers)
                {
                    fileNames.Add(layer.layerName);
                    infoFramesImage[i].Add(layer.imageSourceMain);
                }
                layersNames.Add(i, fileNames);
                i++;
            }
        }
        public static void SaveFile(object objs,string path)
        {
           BinaryFormatter binaryFormatter = new BinaryFormatter();
            using(FileStream file=new FileStream(path, FileMode.Create, FileAccess.Write))
            {
              
                binaryFormatter.Serialize(file, objs);
               
            }
        }
       
    }
}
