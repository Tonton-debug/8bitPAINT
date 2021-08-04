using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using _8bitPaint.PaintServiceLib;
namespace _8bitPaint
{
    enum StateThread
    {
        Start,
        StartNew,
        StartLoop,
        Close,
      
    }
   static class DataBase
    {
       
        public static MainWindow mainWindow { get; set; }
        public static PaintServiceClient client { get;private set; }
        public static string active_category { get; private set; }
        private static Thread CheckUpdateBDThread;
        private static List<string> namePictures = new List<string>();
        private static string[] idPictures = new string[0];
        private static string[] allFilesName = new string[0];
        public static void SetClient(string ip)
        {
            client = new PaintServiceClient("BasicHttpBinding_IPaintService", ip);
        }
        public static void ClearClient()
        {
            client = null;
        }
        public static void SelectCategory(string category)
        {
            active_category = category;
        }
        private static void SetStateThread(object get)
        {
            SetStateThread((StateThread)get);
        }
       private static void CheckDelete()
        {
            for (int i = 0; i < allFilesName.Length; i++)
            {
                if (client.IsDeletePictures(allFilesName[i].Replace(".info",""), active_category))
                {
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.DeleteBDPictureInID(i);
                    });
                  
                     List<string> get_string=allFilesName.ToList();
                    List<string> get_long = idPictures.ToList();
                    get_string.RemoveAt(i);
                    get_long.RemoveAt(i);
                    idPictures = get_long.ToArray();
                    allFilesName = get_string.ToArray();
                }
            }
        }
        public static void CheckUpdate()
        {
            for (int i = 0; i < idPictures.Length; i++)
            {
                List<byte[]> get_bytes = client.GetUpdatePicture(idPictures[i], allFilesName[i].Replace(".info", ""), active_category).ToList();

                if (get_bytes[0].Length!=0)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    using (Stream stream2 = new MemoryStream(get_bytes[0]))
                    {
                        bitmap.StreamSource = stream2;

                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.UpdateBDPicture(i, Encoding.UTF8.GetString(get_bytes[1]), Encoding.UTF8.GetString(get_bytes[2]), Encoding.UTF8.GetString(get_bytes[3]), Encoding.UTF8.GetString(get_bytes[4]), bitmap);
                    });

                   
                }
            }
        }
        public static void SetStateThread(StateThread getState)
        {
            switch (getState)
            {
                case StateThread.StartLoop:
                    CheckUpdateBDThread?.Abort();
                    CheckUpdateBDThread = new Thread(new ParameterizedThreadStart(CheckUpdateBD));
                    CheckUpdateBDThread.Start(true);
                    break;
                case StateThread.Close:
                    CheckUpdateBDThread?.Abort();
                    allFilesName = new string[0];
                    idPictures = new string[0];
                    break;
                case StateThread.Start:
                    CheckUpdateBDThread?.Abort();
                   
                    CheckUpdateBDThread = new Thread(new ParameterizedThreadStart(CheckUpdateBD));
                    CheckUpdateBDThread.Start(null);
                    break;
                case StateThread.StartNew:
                    allFilesName = new string[0];
                    idPictures = new string[0];
                    CheckUpdateBDThread?.Abort();
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.ClearPanelsInBD();
                    });
                    
                    CheckUpdateBDThread = new Thread(new ParameterizedThreadStart(CheckUpdateBD));
                    CheckUpdateBDThread.Start(null);
                    break;
            }
            
        }
        public static void CheckUpdateBD(object obj)
        {
            
            try
            {
               
                do
                {
              
                    string[] removesFiles = new string[0];
                   
                    List<byte[]> get_all_bytes = client.GetNewPictures(ref allFilesName,ref idPictures, active_category).ToList();
                    if( get_all_bytes[0].Length == 0)
                    {
                        mainWindow.Dispatcher.Invoke(() =>
                        {
                            mainWindow.SetActivePanelsInBD(true);
                        });
                    }
                    if (get_all_bytes[0].Length == 0)
                    {
                        CheckDelete();
                        CheckUpdate();
                    }
                    if (obj!=null&&get_all_bytes[0].Length == 0)
                    {

                       
                        Thread.Sleep(mainWindow.settingsProgram.GetValueInListTimes(0)[0] * 1000);
                        continue;
                    }
                    if (obj != null)
                    {
                        mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.SetActivePanelsInBD(false);
                    });
                    }
                    BinaryFormatter binaryFormatter = new BinaryFormatter();

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    using (Stream stream2 = new MemoryStream(get_all_bytes[0]))
                    {
                        bitmap.StreamSource = stream2;

                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                    
                   
                   mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.VisualBDPicture(Encoding.UTF8.GetString(get_all_bytes[1]), Encoding.UTF8.GetString(get_all_bytes[2]), Encoding.UTF8.GetString(get_all_bytes[3]), Encoding.UTF8.GetString(get_all_bytes[4]), bitmap);
                    });
                    
                } while (obj != null);
                new Thread(new ParameterizedThreadStart(SetStateThread)).Start(StateThread.StartLoop);
            }

            catch (Exception get_ex)
            {
                // MessageBox.Show(get_ex.Message);
            }
        }
        public static void PublishFile(int id,string name, string author,string decription, Frame frame,FileHru get_fileHru)
        {
            
           
            string path = (Application.Current.MainWindow as MainWindow).myPathFolder;
            mainWindow.Dispatcher.Invoke(() =>
            {
                mainWindow.SetActivePanelsInBD(false);
            });
            BitmapSource source = (BitmapSource)frame.layers[0].mainSource;
            int count = 0;
            bool send_state = true;

            string stateWriting = "";
            for (int i = 0; i < 2; i++)
            {
                if (send_state)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    using (FileStream file2 = File.Create(path + @"test.bin"))
                    {
                        binaryFormatter.Serialize(file2, get_fileHru);
                    }
                    using (FileStream file2 = File.OpenRead(path + @"test.bin"))
                    {

                        using (FileStream fileStream = File.Create(path + @"test.gz"))
                        {
                            using (GZipStream zip = new GZipStream(fileStream, CompressionMode.Compress))
                            {
                                file2.CopyTo(zip);
                              
                            }
                        }
                    }

                    client.WritePicture(File.ReadAllBytes(path + @"\test.gz"), name, "", "", active_category, send_state, mainWindow.settingsProgram.MyIDInBD, id, ref stateWriting);
                    File.Delete(path + @"\test.gz");
                    File.Delete(path + @"\test.bin");
                }
                else
                {
                    WriteableBitmap wb = new WriteableBitmap(Frame.frames[0].FrameImageSource);
                    var encoder = new GifBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(wb));
                    byte[] data;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        data = ms.ToArray();
                    }
                    client.WritePicture(data, name, author, decription, active_category, false, mainWindow.settingsProgram.MyIDInBD, id, ref stateWriting);
               }
                send_state = false;
              
            }
            mainWindow.Dispatcher.Invoke(() =>
            {
                mainWindow.SetActivePanelsInBD(true);
            });
            SetStateThread(StateThread.Start);
            
            MessageBox.Show(stateWriting);

        }
    }
}
