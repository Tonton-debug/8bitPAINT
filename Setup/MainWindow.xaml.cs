using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Setup.PaintServer;
using System.IO;
using System.IO.Compression;
using CSharpLib;
using System.Security.Principal;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Forms;
using Shortcut = CSharpLib.Shortcut;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Setup
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public enum UpdateState
    {
        None,
        AutoUpdate,
        Update
    }
    public partial class MainWindow : Window
    {
        private string path_file=Directory.GetCurrentDirectory();
        private PaintServiceClient _paintSever;
        private UpdateState Update = UpdateState.None;
        private bool windowsClosing = false;
        public MainWindow()
        {
          
            InitializeComponent();
          string[] get=Environment.GetCommandLineArgs();

              if (get.Length>1&&get[1] == "update")
                 {
                Update = UpdateState.AutoUpdate;
            
                UpdatePaintClick(null, null);
            }
            
        }
        private void WriteToInfoText(string get)
        {
            if (!windowsClosing)
            {
                Dispatcher.Invoke(() => { InfoTextBlock.Text =get ; });
            }
        }
        private void ConnectToServer()
        {
            Thread.Sleep(3000);

            WriteToInfoText("Пытаемся подключиться к серверу");
            try
            {
                _paintSever = new PaintServiceClient("BasicHttpBinding_IPaintService");

                DownloadPaint();
                }
            catch (Exception e)
            {
                if (!windowsClosing)
                {
                    Dispatcher.Invoke(() => { InfoTextBlock.Text = e.Message; StackPanelButtons.IsEnabled = true; });
                }
            }
           
            
        }
        private void DownloadPaint()
        {
            if (Directory.Exists(path_file))
            {
                Dispatcher.Invoke(() =>
                {
                    DeletePaintClick(null, null);
                });
            }


           
                WriteToInfoText("Скачиваем файл");
           
            string path = Directory.GetCurrentDirectory() + "/paint.zip";
            byte[] file = _paintSever.Download8BP();

            File.WriteAllBytes(path, file);
           
                WriteToInfoText("Извлекаем данные");
            
            ZipFile.ExtractToDirectory(path, path_file);
            switch (Update)
            {
                case UpdateState.None:
                    if (!windowsClosing)
                    {
                        Dispatcher.Invoke(() => { StackPanelButtons.IsEnabled = true;  });
                    }
                    CreateIcon();
                    if (!File.Exists(path_file + "Setup.exe"))
                    {
                        File.Move(Directory.GetCurrentDirectory() + @"\Setup.exe", path_file + "Setup.exe");
                        File.Move(Directory.GetCurrentDirectory() + @"\Setup.pdb", path_file + "Setup.pdb");
                        File.Move(Directory.GetCurrentDirectory() + @"\Setup.exe.config", path_file + "Setup.exe.config");
                        File.Move(Directory.GetCurrentDirectory() + @"\CSharpLib.xml", path_file + "CSharpLib.xml");
                        File.Move(Directory.GetCurrentDirectory() + @"\CSharpLib.dll", path_file + "CSharpLib.dll");
                    }

                    
                    break;
                case UpdateState.Update:
                
                    break;
            }
            if (!windowsClosing)
            {
                WriteToInfoText("Файл установлен");
            }
            else
            {
                MessageBox.Show("Файл установлен");
            }


            if (!windowsClosing)
            {
                Dispatcher.Invoke(() => KillApp());
            }
        }

        private void DownloadPaintClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Вы уверены,что хотите установить рисовалку в "+ fbd.SelectedPath+@"\8BP\"+"?", "Папка", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return;
                }
                path_file = fbd.SelectedPath+@"\8BP\";
                Update = UpdateState.None;
                StackPanelButtons.IsEnabled = false;
                Thread thread = new Thread(new ThreadStart(ConnectToServer));
                thread.Start();
            }
          


        }
        private void KillApp()
        {
          
            Close();
        }
        private void CreateIcon()
        {
            using (Stream stream = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\8bitPaint.LNK"))
            {

            }
             
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            Shortcut shortcut = new Shortcut();
            shortcut.CreateShortcutToFile(path_file+"8bitPaint.exe", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+@"\8bitPaint.LNK",IconLocation: path_file+"paintIco.ico");
           
        }

        private void DeletePaintClick(object sender, RoutedEventArgs e)
        {
            StackPanelButtons.IsEnabled = false;
            if (Directory.Exists(path_file))
            {
                foreach(string files in Directory.GetFiles(path_file))
                {
                    if(new FileInfo(files).Name != "Setup.exe"&& new FileInfo(files).Name != "Setup.pdb" && new FileInfo(files).Name != "settings.dat" && new FileInfo(files).Name != "Setup.exe.config"&& new FileInfo(files).Name != "CSharpLib.xml" && new FileInfo(files).Name != "CSharpLib.dll")
                    {
                        File.Delete(files);
                    }
             
                }
            
            }
            InfoTextBlock.Text = "Рисовалка удалена";
            if (sender != null)
            {
                StackPanelButtons.IsEnabled = true;
            }
        
        }

        private void UpdatePaintClick(object sender, RoutedEventArgs e)
        {
            Update = UpdateState.Update;
            StackPanelButtons.IsEnabled = false;
            Thread thread = new Thread(new ThreadStart(ConnectToServer));
            thread.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            windowsClosing = true;
        }
    }
}
