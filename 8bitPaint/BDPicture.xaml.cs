using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Compression;
using System.IO;
namespace _8bitPaint
{
    /// <summary>
    /// Логика взаимодействия для BDPicture.xaml
    /// </summary>
    public partial class BDPicture : System.Windows.Controls.UserControl
    {
        public BDPicture()
        {
            InitializeComponent();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var FileDialog = new SaveFileDialog();
            FileDialog.Filter = "hru|*.hru";
            if (FileDialog.ShowDialog()==true)
            {
                FileInfo info = new FileInfo(FileDialog.FileName);
                byte[] downloads_bytes=DataBase.client.DownloadFile(TextBlockFileName.Text, DataBase.active_category);
              //  File.Create(FileDialog.FileName);
              using(FileStream fileCreate = File.Create(FileDialog.FileName.Replace(info.Extension, ".gz")))
                {
                    fileCreate.Write(downloads_bytes, 0, downloads_bytes.Length);
                }
                using(FileStream file=new FileStream(FileDialog.FileName.Replace(info.Extension, ".gz"), FileMode.Open))
                {
                 
                    using (GZipStream stream = new GZipStream(file,CompressionMode.Decompress))
                    {
                       
                        using(FileStream file2 = new FileStream(FileDialog.FileName.Replace(info.Extension,".hru"), FileMode.OpenOrCreate))
                        {
                            stream.CopyTo(file2);
                        }
                       
                    }
                }
                File.Delete(FileDialog.FileName.Replace(info.Extension, ".gz"));
               
            }
        }
    }
}
