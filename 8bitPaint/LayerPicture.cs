using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8bitPaint
{
   public class LayerPicture
    {
        private int activePixelsInList=-1;
       private List<byte[]> pixels_list = new List<byte[]>();
        public void AddPixelsList( byte[] get)
        {
                ChangeList(); 
            byte[] fill_bytes = new byte[get.Length];
            get.CopyTo(fill_bytes,0);
           
            pixels_list.Add(fill_bytes);
            activePixelsInList=pixels_list.Count-1;

        }
        public void ClearPixels()
        {
            pixels_list = new List<byte[]>();
            activePixelsInList = -1;
        }
        public byte[] ReturnPixels(bool isAdd)
        {
            
                activePixelsInList += isAdd ? 1 : -1;
            activePixelsInList =activePixelsInList== -1 ? 0 : activePixelsInList;
            return pixels_list.ElementAt(activePixelsInList);
        }
        private void ChangeList()
        {
            while (activePixelsInList != pixels_list.Count - 1)
            {
                pixels_list.RemoveAt(pixels_list.Count - 1);
            }
        }
        public bool ReturnState(bool isBack, ref double opasty)
        {
            if (isBack)
            {
                opasty = activePixelsInList != 0?1:0.5;
                return activePixelsInList != 0;
            }
            else
            {
                opasty = activePixelsInList != pixels_list.Count - 1 ? 1 : 0.5;
                return activePixelsInList != pixels_list.Count - 1;
            }
        }
    }
}
