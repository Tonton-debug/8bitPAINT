using System;
using System.Collections.Generic;
using Draw= System.DrawingCore;
using System.Linq;
using System.Reflection;
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
using System.IO;
using Microsoft.Win32;
using Forms=System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.DrawingCore.Imaging;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.Entity;
using _8bitPaint.PaintServiceLib;
using System.IO.Compression;
namespace _8bitPaint
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SettingsMyProgram settingsProgram { get; set; }
        public bool wasReverse = false;
        public int selected_layer = 0;
        private bool thisUpdate = false;
        private bool isChek = true;
        private readonly int _version =11;
        private CancellationTokenSource stopAnimRunToken= new CancellationTokenSource();
        private Thread AutoSaveFileThread;
   
        private string PathBackup;
       
        private bool isDeativeLayers = false;
        private bool IsDeleteColor = false;
        private ScaleTransform st = new ScaleTransform();
        private int countPixelX = 16;
        private int countPixelY = 16;
        private byte[] coppyPixels;
        private int count_colors_in_panel;
        private float add_shades_color = 0.01f;
        private Layer layer;
        private List<Image> pixels = new List<Image>();
        private int count_layer = 0;
        public string myPathFolder { get; private set; }
        private Point lastPoint = new Point(135, 99);
        private float OpacityLayers = 0;
        private float OpacityPalytre = 0;
        private Frame ActiveFrame;
        private int currentAnimFrame = 0;
        private bool isAnimRunning = false;
        private int tick = 5;
        private float OpacityFrame = 0.5f;
        private Point p;
        private bool canMove = false;
        private bool isFrame = false;
       
        private Point selected_point1;
        private Point selected_point2;
        private List<Block> blocks = new List<Block>();
        private bool openBackup = true;
        private int sizeDraw = 1;
        private bool isActiveSettingsKeys = true;
        private LayerPicture layerPicture = new LayerPicture();
        private string endSavePathFile = "";
       
        public MainWindow()
        {
          
            InitializeComponent();
            myPathFolder = AppDomain.CurrentDomain.BaseDirectory;
            ClearColorPanel();
            Random random = new Random();
           
            LoadSettings();
            PathBackup = settingsProgram.BackupPath;
            //CentralTabContol.SelectionChanged += CentralTabContol_SelectionChanged;
           
            VisualBackup();
            ReloadPixelsPicture();
            //VisualBG();
        
            AppDomain currentDomain = AppDomain.CurrentDomain;
            DataBase.mainWindow = Application.Current.MainWindow as MainWindow;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            string[] get = Environment.GetCommandLineArgs();

            if (get.Length > 1)
            {
                OpenFile(true, get[1]);
            }
            CellBG.ChangeCell(settingsProgram.sizeCells,settingsProgram.StrokeThickness);
            CellBG.Visibility = settingsProgram.isVisibleCells ? Visibility.Visible : Visibility.Collapsed;
           
            Block.statePicture = StatePicture.None;
          
            if (File.Exists(settingsProgram.lastFilePath)&&settingsProgram.isOpenLastFile&&get.Length<=1)
            {
                FileInfo file = new FileInfo(settingsProgram.lastFilePath);
                OpenFile(file.Name.Contains(".hru"), settingsProgram.lastFilePath);
            }
            ConnectButton_Click(null, null);
            UpdateProgramFromSettings();
            VisualColorProgram();
           
        }

        //private void CentralTabContol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    switch (CentralTabContol.SelectedIndex)
        //    {
        //        case 0:
        //            if (DataBase.client != null)
        //            {
        //                DataBase.SetStateThread(StateThread.Close);
        //            }
        //            break;
        //        case 1:
        //            if (DataBase.client == null)
        //            {
        //                ConnectButton_Click(null, null);
        //            }
        //            else
        //            {
        //                DataBase.SetStateThread(StateThread.Start);
        //            }
        //            break;
        //    }
        //}

        private void UpdateProgramFromSettings()
        {

          
            VisualBackup();
            OtherSettings();
            VisualColorProgram();
        }
        private void OtherSettings()
        {
            AutoSaveFileThread?.Abort();
            AutoSaveFileThread = new Thread(new ParameterizedThreadStart(AutoSaveFile));
            AutoSaveFileThread.Start(true);
            
            //  ClearFramesAndLayers(false);
            CellBG.ChangeCell(settingsProgram.sizeCells, settingsProgram.StrokeThickness);
            CellBG.Visibility = settingsProgram.isVisibleCells ? Visibility.Visible : Visibility.Collapsed;
            if (DataBase.client != null)
            {
                DataBase.SetStateThread(StateThread.StartLoop);
            }
            if (settingsProgram.isFullScreen)
            {
                myWindow.WindowStyle = WindowStyle.None;
            }
            else
            {
                myWindow.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }
        private void ClearColorPanel()
        {
            UIElementCollection uIElementCollection = Palytre1.ColorPanel.Children;
            while (uIElementCollection.Count != 0)
            {
                Palytre1.ColorPanel.Children.Remove(uIElementCollection[0]);
            }
        }
        public void DeleteBDPictureInID(int id)
        {
             WrapPanelPuctiresList.Children.RemoveAt(id);
        }
        private void VisualBackup()
        {
            if (!Directory.Exists( PathBackup))
            {
                return;
            }
            while (BackupWrapPanel.Items.Count != 0)
            {
                BackupWrapPanel.Items.RemoveAt(0);
            }
            foreach (string path in Directory.GetFiles( PathBackup))
            {
               
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Extension != ".hru")
                {
                    continue;
                }
                bool isCreate = false;
                
                DateTime time = File.GetCreationTime(path);

                foreach (TreeViewItem treeView in BackupWrapPanel.Items)
                {

                    if (treeView.Header.ToString() == time.ToShortDateString())
                    {

                        TreeViewItem treeViewItem = new TreeViewItem();
                        treeViewItem.Header = fileInfo.Name;
                        Button OpenFile = new Button();
                        Button DeleteFile = new Button();
                        OpenFile.Content = "Открыть";
                        DeleteFile.Content = "Удалить";
                        OpenFile.Tag = path;
                        DeleteFile.Tag = path;
                        DeleteFile.Click += DeleteBackupClick_Button;
                        OpenFile.Click += OpenBackupClick_Button;
                        treeViewItem.Items.Add(OpenFile);
                        treeViewItem.Items.Add(DeleteFile);
                        treeView.Items.Add(treeViewItem);
                        isCreate = true;
                    }

                }
                if (!isCreate)
                {
                    TreeViewItem treeViewItemMain = new TreeViewItem();
                    treeViewItemMain.Header = time.ToShortDateString();
                    TreeViewItem treeViewItemChildren = new TreeViewItem();
                    Button OpenFile = new Button();
                    Button DeleteFile = new Button();
                    OpenFile.Content = "Открыть";
                    DeleteFile.Content = "Удалить";
                    OpenFile.Tag = path;
                    DeleteFile.Tag = path;
                    DeleteFile.Click += DeleteBackupClick_Button;
                    OpenFile.Click += OpenBackupClick_Button;
                    treeViewItemChildren.Items.Add(OpenFile);
                    treeViewItemChildren.Items.Add(DeleteFile);
                    treeViewItemChildren.Header = fileInfo.Name;
                    treeViewItemMain.Items.Add(treeViewItemChildren);
                    BackupWrapPanel.Items.Add(treeViewItemMain);
                }
            }
        }
       
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            DateTime time = DateTime.Now;

            string path = (Application.Current.MainWindow as MainWindow).myPathFolder +"Debug_" + time.Year.ToString() + "." + time.Month.ToString() + "." + time.Day.ToString() + "_" + time.Hour.ToString() + "." + time.Minute.ToString() + "." + time.Second.ToString() + ".txt";


            File.AppendAllText(path, e.Message);
            File.AppendAllText(path, e.StackTrace);
            File.AppendAllText(path, e.Source);
            File.AppendAllText(path, e.TargetSite.Name);
            MessageBox.Show("Произошла ошибка! Подробнее смотри в " + path + "\nПожалуйста сохраните вашу работу (желательно в формате .hru)");
            (Application.Current.MainWindow as MainWindow).SaveFile(null, null);
        }
        private void ActivateBytes()
        {
            Instruments instruments_save = Tools.instrumentsTool;
            Tools.instrumentsTool = Instruments.Draw;
            Tools.Draw_delegate(ActiveFrame.layers[selected_layer].main, 0, 0, Tools.GetPixel((BitmapSource)Tools.EditorBitMap, 0, 0), ActiveFrame.layers[selected_layer].imageSourceMain, sizeDraw);
            Tools.instrumentsTool = instruments_save;
            VisualPictureFrame();
        }
        public void VisualCell(BitmapSource bitmapSource_get = null, int q = -1, bool isOpenFile = false)
        {
            GC.Collect();
           // Cell.Width = countPixelX*4;
          //  Cell.Height = countPixelY*4;
           
            //   DrawCanvasPanel.Width = countPixelX;
            //   DrawCanvasPanel.Height = countPixelY;
            DrawingVisual drawingVisual = new DrawingVisual();
            Shape shape = new Ellipse();

            PixelsScrool.Maximum = countPixelX > countPixelY ? countPixelX : countPixelY;
            shape.Width = countPixelX;
            shape.Height = countPixelY;
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                Brush brush1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));

                drawingContext.DrawRectangle(brush1, null, new Rect(0, 0, (int)shape.Width, (int)shape.Height));
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)shape.Width, (int)shape.Height, 0, 0, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);
            BitmapSource bitmapSource = bitmap;
            pixels = new List<Image>();
            List<Draw.Bitmap> bitmaps = new List<Draw.Bitmap>();
            
            Image MainImage = new Image();
            MainPanel.Children.Add(MainImage);

            if (st == new ScaleTransform())
            {
                st = new ScaleTransform(10, 10);

            }
                CellBG.Width = countPixelX;
            CellBG.Height = countPixelY;
            MainDrawCanvas.LayoutTransform = st;
          //  MainPanel.RenderTransform = st;
          //   Background.LayoutTransform = st;
          //  SelectedPole.LayoutTransform = st;
          //   SelectedPole.RenderTransform = st;
            Background.Width = countPixelX;
            Background.Height = countPixelY;
     //       PrevFrame.LayoutTransform = st;

            PrevFrame.Width = countPixelX;
            PrevFrame.Height = countPixelY;
            MainImage.Width = countPixelX;
            MainImage.Height = countPixelY;
            MainImage.Source = bitmapSource_get == null ? bitmap : bitmapSource_get;


            RenderOptions.SetBitmapScalingMode(MainImage, BitmapScalingMode.NearestNeighbor);
            //   MainImage.MouseEnter += Img_MouseEnter;
            //    MainImage.MouseLeave += Img_MouseLeave;
            MainImage.MouseLeftButtonUp += Img_MouseLeftButtonUp;
            MainImage.MouseLeftButtonDown += Img_MouseLeftButtonDown;
            MainImage.MouseMove += Img_MouseLeftButtonMove;
            MainImage.MouseRightButtonDown += Img_MouseRightButtonDown;
            count_layer = q == -1 ? ActiveFrame.layers.Count : q;
            MainImage.Name = "name_" + Frame.ActiveFrameId + "_" + count_layer;
           
            //  MessageBox.Show(MainImage.Name);
            if (bitmapSource_get == null || isOpenFile)
            {
                ActiveFrame.layers.Add(new Layer(MainImage, MainImage.Source, !isOpenFile));


            }
            else
            {
              

                ActiveFrame.layers[count_layer].main = MainImage;

            }
            //Cell.Source = VisualBGCells(Cell);
            AddLayer();

        }
        
        private bool AddBlocks(Point one, Point two)
        {
           
            int x = (int)one.X;
            int y = (int)one.Y;
            int x_end = (int)two.X;
            int y_end = (int)two.Y;
            int subX = x > x_end ? -1 : 1;
            int subY = y > y_end ? -1 : 1;
            int per = Tools.EditorBitMap.Format.BitsPerPixel;
           
            for (; x != x_end + subX; x += subX)
            {
                for (; y != y_end + subY; y += subY)
                {
                    int pixelOffset = (int)((x + y * countPixelX) * per / 8);
                    byte[] source_byte = Tools.pixels;
                    try
                    {
                        if (source_byte[pixelOffset + 3] != 0)
                        {


                            blocks.Add(new Block(x, y, Color.FromArgb((byte)(source_byte[pixelOffset + 3] - 50), source_byte[pixelOffset + 2], (byte)(source_byte[pixelOffset + 1] + 100), source_byte[pixelOffset])));
                        }
                    }
                    catch
                    {
                        return false;
                    }

                }
                y = (int)one.Y;
            }
           
            Block.selectedBlocks = true;
            return true;
        }


        private void CheckSelectedBlocks()
        {
            if (Block.selectedBlocks)
            {
                ClearColor();
            }
        }

        private void Img_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
          
        //    Keyboard.Focus(WrapPanelBackup2);
        //    MessageBox.Show(Keyboard.FocusedElement.ToString());
            Image image = (Image)sender;
            Point get_point = e.GetPosition(image);
            selected_point1 = get_point;
            ClearBlocks();
            if (Tools.instrumentsTool == Instruments.Selected)
            {




                Block.bytes_to_coppy = Block.bytes_to_coppy == null || Block.statePicture == StatePicture.Cut || Block.statePicture == StatePicture.Coppy ? new byte[Tools.pixels.Length] : Block.bytes_to_coppy;
                
                CheckSelectedBlocks();

                if (!AddBlocks(selected_point1, selected_point2))
                {
                   
                    ClearColor();
                    ClearBlocks();
                  
                }

               // Block.statePicture = StatePicture.None;
                ActivateBytes();
            }
            else if (Tools.instrumentsTool == Instruments.Draw || Tools.instrumentsTool == Instruments.Erase || Tools.instrumentsTool == Instruments.Fill)
            {
                layerPicture.AddPixelsList(ActiveFrame.layers[selected_layer].imageSourceMain);
                ChangeReturnPixelsButtons();
            }
            VisualPictureFrame();
        }

        private void Img_MouseRightButtonDown(object sender, MouseEventArgs e)

        {
            if (Tools.instrumentsTool == Instruments.Selected)
            {
                ClearColor();
                ActivateBytes();
                ClearBlocks();
            }

        }
        #region Рисование и холст
        private void Img_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            CheckSelectedBlocks();
            ClearBlocks();
            Image image = (Image)sender;
            Point get_point = e.GetPosition(image);
            selected_point2 = get_point;
            Tools.Draw_delegate(image, (int)get_point.X, (int)get_point.Y, Palytre1.Draw, ActiveFrame.layers[selected_layer].imageSourceMain, sizeDraw);

        }
        private void Img_MouseLeftButtonMove(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;
            Point get_point = e.GetPosition(image);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Tools.Draw_delegate(image, (int)get_point.X, (int)get_point.Y, Palytre1.Draw, ActiveFrame.layers[selected_layer].imageSourceMain, sizeDraw);
            }
        }
        private void ClearBlocks() {
            Title = "CLEARRR" + new Random().Next(0, 1000);
            blocks.Clear();
        }
        private void DrawCanvasPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (e.Delta > 0)
            {
                st.ScaleX *= 1.1f;
                st.ScaleY *= 1.1f;
            }
            else if (st.ScaleX > 1)
            {
                st.ScaleX /= 1.1f;
                st.ScaleY /= 1.1f;
            }
            MainDrawCanvas.LayoutTransform = st;
        }
        private void StartScaleCanvas()
        {
            st.ScaleX = 64;
            st.ScaleY = 64;
            MainPanel.RenderTransform = st;
            Background.LayoutTransform = st;
            PrevFrame.LayoutTransform = st;
        }
        public void ClickInstruments(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            erarce.Opacity = 1;
            pencil.Opacity = 1;
            hand.Opacity = 1;
            fill.Opacity = 1;
            selected.Opacity = 1;
            dropper.Opacity = 1;
            img.Opacity = 0.3;
            Instruments lastInstrument = Tools.instrumentsTool;
            Tools.instrumentsTool = (Instruments)int.Parse(img.Tag.ToString());
            if (lastInstrument == Instruments.Selected && Tools.instrumentsTool != lastInstrument)
            {
                ClearColor();
                ActivateBytes();
                ClearBlocks();
            }
        }



        #endregion

        #region Палитра
       

        
        

        private void ScrollBar_ScrollOpacityPalytre(object sender, ScrollEventArgs e)
        {
           
        }
        private void ReplaceColorButton()
        {
           // DeleteColorButton.Background = IsDeleteColor ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red")) : new SolidColorBrush(Color.FromArgb(221, 221, 221, 221));
        }
        private void DeleteColorClick(object sender, RoutedEventArgs e)
        {

            IsDeleteColor = IsDeleteColor ? false : true;
            ReplaceColorButton();
        }
        private void AddColorClick(object sender, RoutedEventArgs e)
        {
           
        }





        #endregion


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


        
      
        private void SaveFile(bool isPng, string path)
        {

           
            if (!openBackup)
            {
                CheckSelectedBlocks();
                if (!isPng)
                {
                    BitmapSource source = (BitmapSource)ActiveFrame.layers[0].mainSource;
                    List<string> send_color = new List<string>();
                    foreach (UIElement image in Palytre1.ColorPanel.Children)
                    {
                        Image image1 = (Image)image;
                        send_color.Add(image1.Tag.ToString());
                    }
                    FileInfo info = new FileInfo(path);
                    
                    FileHru.SaveFile(new FileHru(Frame.frames, countPixelX, countPixelY, source.Format.BitsPerPixel, send_color, path), path);
                   
                    return;
                }
                Frame.SaveToPng(path, isPng ? ".png" : ".hru");
            }
        }
        private void SaveFile(object sender, RoutedEventArgs e)
        {

            var FileDialog = new SaveFileDialog();
            FileDialog.Filter = "png|*.png|hru|*.hru";
            if (FileDialog.ShowDialog() == true)
            {

                SaveFile(FileDialog.FilterIndex == 2 ? false : true, FileDialog.FileName);

            }
        }

        private void CreateNewFile(object sender, RoutedEventArgs e)
        {
            
            ReloadPixelsPicture();
            SelectedSize selected = new SelectedSize();
            if (selected.ShowDialog() == true)
            {
                countPixelX = selected.SizeX;
                countPixelY = selected.SizeY;
               endSavePathFile =  PathBackup+selected.NameFile+".hru";
                ClearFramesAndLayers();
                StartCreateNewFile();
            }
        }

        private void StartCreateNewFile()
        {
            AutoSaveFileThread?.Abort();
           
            AutoSaveFileThread = new Thread(new ParameterizedThreadStart(AutoSaveFile));
            AutoSaveFileThread.Start(true);
             openBackup = false;
            WrapPanelBackup.Visibility = Visibility.Collapsed;
            CenterCanvas.Visibility = Visibility.Visible;
            st.ScaleX = 20;
            st.ScaleY = 20;
            AddFrame();
            layerPicture.AddPixelsList(ActiveFrame.layers[0].imageSourceMain);
            ChangeReturnPixelsButtons();
        }
        private void MainPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);

        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
           
        }
        private void LoadBinFile()
        {

        }
        private void ReloadPixelsPicture()
        {
            ReturnPixelsBack.IsEnabled = false;
            ReturnPixelsUp.IsEnabled = false;
            ReturnPixelsBack.Opacity = 0.5;
            ReturnPixelsUp.Opacity = 0.5;
            layerPicture?.ClearPixels();
        }
        private void ClearFramesAndLayers(bool isRestart=true)
        {
            Frame.ActiveFrameId = 0;
            Frame.LastActiveFrameId = 0;
            while (FramesWrapPanel.Children.Count != 0)
            {
                Frame.frames[0].RemoveLayers(MainPanel, MainPanel.Children, LayerWrapPanel, LayerWrapPanel.Children, ref count_layer, isRestart);
                FramesWrapPanel.Children.RemoveAt(0);
            }

        }
        private void UpdateTextBoxFrames()
        {
            int q = 0;
            foreach(Frame frame in Frame.frames)
            {
             StackPanel get_panel=FramesWrapPanel.Children[q] as StackPanel;
                TextBox textBox = get_panel.Children[2] as TextBox;
                textBox.Text = frame.frameName;
                q++;
            }
        }
        private void OpenFile(bool isHru, string path,bool openIsNewLayer=false)
        {

           
            if (isHru)
            {

                ReloadPixelsPicture();
                       openBackup = false;
                       AutoSaveFile(null);
                       ClearColorPanel();
                       //    
                        WrapPanelBackup.Visibility = Visibility.Collapsed;
                       CenterCanvas.Visibility = Visibility.Visible;
                       int count = 0;
                       st.ScaleX = 20;
                       st.ScaleY = 20;
                       endSavePathFile = path;
                       ClearFramesAndLayers();
                new Thread(new ThreadStart(() =>
                {
                    BinaryFormatter binary = new BinaryFormatter();
                       using (var stream = File.OpenRead(path))
                       {

                           FileHru fileHru = (FileHru)binary.Deserialize(stream);
                           foreach (List<byte[]> _get_bytes in fileHru.infoFramesImage)
                           {
                               List<BitmapSource> bitmaps = new List<BitmapSource>();
                               foreach (byte[] bytes in _get_bytes)
                               {
                                   WriteableBitmap wb = new WriteableBitmap(fileHru.sizeX, fileHru.sizeY, 96, 96, PixelFormats.Pbgra32, null);
                                   wb.WritePixels(new Int32Rect(0, 0, fileHru.sizeX, fileHru.sizeY), bytes, (fileHru.sizeX * fileHru.per) / 8, 0);
                                   bitmaps.Add(Tools.ConvertWriteableBitmapToBitmapImage(wb));

                                   countPixelX = bitmaps[0].PixelWidth;
                                   countPixelY = bitmaps[0].PixelHeight;
                               }
                            Dispatcher.Invoke(() => AddFrame(bitmaps));
                               ActiveFrame.frameName = fileHru.framesNames[count];
                               List<string> list_names_layers = new List<string>();
                               fileHru.layersNames.TryGetValue(count, out list_names_layers);
                               int q = 0;
                               foreach (Layer get in Frame.frames[Frame.frames.Count - 1].layers)
                               {
                                   get.layerName = list_names_layers[q];
                                   q++;

                               }
                               count++;
                           }

                           layerPicture.AddPixelsList(fileHru.infoFramesImage[0][0]);
                        Dispatcher.Invoke(() => ChangeReturnPixelsButtons());
                           foreach (string color in fileHru.colors)
                           {
                            Dispatcher.Invoke(() => Palytre1.AddColorInPanel((Color)ColorConverter.ConvertFromString("#" + color)));
                           }
                           for (int i = 0; i < Frame.frames.Count; i++)
                           {
                            Dispatcher.Invoke(() => VisualPictureFrame(i));
                          
                           }

                       }
                       AutoSaveFileThread?.Abort();

                       AutoSaveFileThread = new Thread(new ParameterizedThreadStart(AutoSaveFile));
                       AutoSaveFileThread.Start(true);
                    Dispatcher.Invoke(() => UpdateTextBoxFrames());
                    Dispatcher.Invoke(() => SelectedFrame(FramesWrapPanel.Children[Frame.frames.Count - 1], null));

                })).Start();
                return;
            }
            Draw.Bitmap image;
            using (var stream = new FileStream(path, FileMode.Open))
            {
              //  ActiveFileName = new FileInfo(path).Name.Replace(".png", "");
                image = new Draw.Bitmap(stream);

               
               
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                countPixelX = image.Width;
               countPixelY = image.Height;
                image = null;
                if (!openIsNewLayer)
                {
                    ReloadPixelsPicture();
                    openBackup = false;
                    AutoSaveFile(null);
                    ClearColorPanel();
                    //    
                    WrapPanelBackup.Visibility = Visibility.Collapsed;
                    CenterCanvas.Visibility = Visibility.Visible;
                    int count = 0;
                    st.ScaleX = 20;
                    st.ScaleY = 20;
                    endSavePathFile = path;
                    ClearFramesAndLayers();
                    if (Frame.ActiveFrameId > 0)
                    {
                        PrevFrame.Source = Frame.FrameToSource((BitmapSource)PrevFrame.Source, Frame.ActiveFrameId - 1);
                        PrevFrame.Opacity = OpacityFrame;
                    }
                    AddFrame(new List<BitmapSource>() { bitmap });
                    layerPicture.AddPixelsList(Tools.SetPicture(bitmap, false));
                    ChangeReturnPixelsButtons();
                    AutoSaveFileThread?.Abort();

                    AutoSaveFileThread = new Thread(new ParameterizedThreadStart(AutoSaveFile));
                    AutoSaveFileThread.Start(true);
                }
                else
                {
                    DeleteLayerClick(DeleteLayer_Button, null);

                    //       AddLayer();
                 //   VisualCell();
                    ActiveFrame.layers.Last().imageSourceMain =Tools.SetPicture(bitmap,false);

                 //   AddLayer();




                }
            }
        }
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "hru|*.hru|png|*.png";
            int count = 0;


            if (openFile.ShowDialog() == true)
            {
                OpenFile(openFile.FilterIndex==1, openFile.FileName);
          //      layerPicture.AddPixelsList(ActiveFrame.layers[selected_layer].imageSourceMain);
            }
        }



        #region Слои
        
        
       
        
        private void SelectedFrame(object sender, RoutedEventArgs e)
        {

            CheckSelectedBlocks();
            ClearBlocks();
            StackPanel button2 = (StackPanel)sender;
            int id = (int)button2.Tag;
            foreach (StackPanel a in FramesWrapPanel.Children)
            {
                a.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Frame)));
            }
            button2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.ActiveFrame)));

            Frame.frames[Frame.LastActiveFrameId].RemoveLayers(MainPanel, MainPanel.Children, LayerWrapPanel, LayerWrapPanel.Children, ref count_layer);
            ActiveFrame = Frame.frames[id];
            Frame.ActiveFrameId = id;
            Frame.LastActiveFrameId = id;
            int count = 0;

            foreach (Layer layer in ActiveFrame.layers)
            {
                VisualCell((BitmapSource)layer.mainSource, count);
                count++;
            }
            if (ActiveFrame.layers.Count == 0)
            {
                VisualCell();
            }
            //LayerWrapPanel.Children.Add(AddLayer_Button);
            if (Frame.ActiveFrameId > 0)
            {
                PrevFrame.Source = Frame.FrameToSource((BitmapSource)PrevFrame.Source, Frame.ActiveFrameId - 1);
                PrevFrame.Opacity = OpacityFrame;
            }
            if (isFrame)
            {
                switch (Layer.stateLayers)
                {

                    case State.Move:
                        Layer.stateLayers = State.None;

                        MoveLayer(Layer.idMake);
                        break;

                }
            }

        }
       
        private void ForeachLayers()
        {

            try
            {
                for (int i = 0; i < ActiveFrame.layers.Count; i++)
                {
                    Layer layer = ActiveFrame.layers[i];
                    StackPanel get_panel = (StackPanel)LayerWrapPanel.Children[i];
                    get_panel.Opacity = !layer.isActive ? 0.1f : 1;
                    //     if (ActiveFrame.layers[i].main == null) continue;
                    if (i != selected_layer)
                    {

                        layer.main.IsHitTestVisible = false;
                        layer.main.Opacity = isDeativeLayers || !layer.isActive ? 0 : OpacityLayers;

                    }
                    else
                    {
                        layer.main.IsHitTestVisible = true;
                        layer.main.Opacity = 1;

                        Tools.SetPicture((BitmapSource)layer.main.Source);

                    }
                }
            }
            catch { }
        }

        private void SelectedLayer(object sender, RoutedEventArgs e)
        {
            ReloadPixelsPicture();
            CheckSelectedBlocks();
            ClearBlocks();
        
            StackPanel button = (StackPanel)sender;

            selected_layer = (int)button.Tag;

            // CheckBoxActiveLayer.IsChecked = ActiveFrame.layers[selected_layer].isActive;

            for (int i = 0; i < LayerWrapPanel.Children.Count; i++)
            {
                try { StackPanel a = (StackPanel)LayerWrapPanel.Children[i]; a.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Layer))); } catch { }

            }
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.ActiveLayer)));
            ForeachLayers();
            if (!isFrame)
            {
                switch (Layer.stateLayers)
                {
                    case State.Unity:
                        Layer.stateLayers = State.None;

                        UniteLayer(Layer.idMake);
                        break;
                    case State.Move:
                        Layer.stateLayers = State.None;

                        MoveLayer(Layer.idMake);
                        break;

                }
            }
            layerPicture.AddPixelsList(ActiveFrame.layers[selected_layer].imageSourceMain);
        }



        private void VisualColorProgram()
        {
            LeftStackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Left)));
            InstrumentsStackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Up)));
            CenterStackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Center)));
            RightStackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Right)));
            OptionLayer.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Left)));
            OptionFrames.Background= new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Right)));
            ConnectionAndPublishPanel.Background= new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.UpBD)));
            LeftPanelBD.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.LeftBD)));
            CenterPanelBD.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.CenterBD)));
            foreach (var item in LayerWrapPanel.Children)
            {
                StackPanel stackPanel = item as StackPanel;
                stackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Layer)));
            }
            foreach (var item in FramesWrapPanel.Children)
            {
                StackPanel stackPanel = item as StackPanel;
                stackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Frame)));
            }
            foreach (var item in WrapPanelPuctiresList.Children)
            {
                BDPicture bDPicture = item as BDPicture;
                bDPicture.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.PictureBD)));
            }
            try
            {
                SelectedFrame(FramesWrapPanel.Children[Frame.ActiveFrameId], null);
                SelectedLayer(LayerWrapPanel.Children[selected_layer], null);
            }
            catch
            {

            }
        }
        private void VisualPictureFrame(int id = -1)
        {
        //    Thread.th
         //   Map<Thread, StackTraceElement[]> threads = Thread.getAllStackTraces(); 
                id = id == -1 ? Frame.ActiveFrameId : id;
            
                Frame.frames[id].FrameImageSource = Frame.FrameToSource(Frame.frames[id].FrameImageSource, id);
                StackPanel wrap = (StackPanel)FramesWrapPanel.Children[id];
                Image get = (Image)wrap.Children[1];
            get.Source = Frame.frames[id].FrameImageSource;
            GC.Collect();
        }

        #endregion




        #region Анимации
        private void ChangeLayerTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            ActiveFrame.layers[(int)textBox.Tag].layerName = textBox.Text;
        }
        private void ChangeFrameTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
           Frame.frames[(int)textBox.Tag].frameName = textBox.Text;
        }
        private void AddLayer()
        {

            //   LayerWrapPanel.Children.Remove(AddLayer_Button);
            
            
            StackPanel stackPanel = new StackPanel();
            
            TextBlock text = new TextBlock();
            TextBox textBox = new TextBox();
            stackPanel.Width = 496;
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Tag = count_layer;
            text.Text = count_layer.ToString();
            text.FontSize = 14;
            stackPanel.Margin = new Thickness(0, 0, 0, 8);

            stackPanel.MouseDown += SelectedLayer;
            stackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Layer)));
            textBox.FontSize = 24;
            textBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            textBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#32323254"));
            textBox.Width = 270;
            //  textBox.Text = "Layer " + (ActiveFrame.layers.Count).ToString();
            stackPanel.Children.Add(text);
            //if (settingsProgram.CanVisualLayers())
            //{
                Image image = new Image();
                image.Height = 64;
                image.Width = 64;
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

                Binding binding = new Binding();
                binding.Source = ActiveFrame.layers[count_layer].main;
                binding.Path = new PropertyPath("Source");
                image.Stretch = Stretch.Uniform;
            
                image.SetBinding(Image.SourceProperty, binding);
            textBox.Tag = count_layer;
            textBox.TextChanged += ChangeLayerTextBox;
          
                textBox.Text = ActiveFrame.layers[count_layer].layerName != null ? ActiveFrame.layers[count_layer].layerName: textBox.Text.ToString();
            
            stackPanel.Children.Add(image);
           // }
           
            stackPanel.Children.Add(textBox);

            LayerWrapPanel.Children.Add(stackPanel);
           
            SelectedLayer((object)stackPanel, null);
          
            Frame.UpdatebitmapSources();
            //   Button button = AddLayer_Button;
            //   LayerWrapPanel.Children.Add(button);
            //  MessageBox.Show(ActiveFrame.layers[count_layer].main.Name);
        }

       

        //анимации


        #endregion



        //   Button button = AddLayer_Button;
        //   LayerWrapPanel.Children.Add(button);
        //  MessageBox.Show(ActiveFrame.layers[count_layer].main.Name);
        
        private void Click_AddLayer_Button(object sender, RoutedEventArgs e)
        {
            if (MainPanel.Children.Count == 0)
            {
                MessageBox.Show("Сначала вам нужно создать файл или открыть его во вкладке файл");
                return;
            }
            VisualCell();

        }





        private void RunAsync()
        {
        
                try
                {
                    while (true)
                    {
                        stopAnimRunToken.Token.ThrowIfCancellationRequested();
                        Thread.Sleep(1000 / tick);

                        Dispatcher.Invoke(() => Title = currentAnimFrame.ToString());
                        Dispatcher.Invoke(() => Animation.Source = Frame.frames[currentAnimFrame].FrameImageSource);



                        currentAnimFrame = currentAnimFrame < Frame.frames.Count - 1 ? currentAnimFrame + 1 : 0;
                    }
                }
                catch(Exception e)
                {
                   
                }
        }
        //анимации
        private void StartAnim(object sender, RoutedEventArgs e)
        {
            tick = int.TryParse(FPS.Text, out int t) ? t : 1;


            StartBtn.Click += StopAnim;
            isAnimRunning = true;
            StartBtn.Content = "Пауза";
            stopAnimRunToken = new CancellationTokenSource();
            Task.Factory.StartNew(() => { RunAsync(); }, stopAnimRunToken.Token);

        }
        private void StopAnim(object sender, RoutedEventArgs e)
        {
            StartBtn.Click += StartAnim;
            StartBtn.Content = "Старт";
         //   isAnimRunning = false;
            Animation.Source = Frame.FrameToSource((BitmapSource)Animation.Source, 0);

            stopAnimRunToken.Cancel();
         
        }
        private void GoToFrame(object sender, RoutedEventArgs e)
        {
            int i = int.TryParse(GotoText.Text, out int t) ? t : 1;
            if (i > 0 && i <= Frame.frames.Count) Animation.Source = Frame.FrameToSource((BitmapSource)Animation.Source, i - 1);
            currentAnimFrame = i - 1;
        }
        private void OpacityFrameScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            OpacityLayers = 0;
            OpacityFrame = (float)OpastyFrameScrollBar.Value;
            ForeachLayers();
            if (Frame.ActiveFrameId > 0)
            {

                PrevFrame.Opacity = OpacityFrame;
            }

        }
        public void SendEventActiveVisibleLayerOrFrame(bool isFrame)
        {
            
                PrevFrame.Opacity = Frame.ActiveFrameId > 0&&isFrame?OpacityFrame:0;
                OpacityLayers =!isFrame?(float)OpastyLayerScrollBar.Value:1;
          
                 ForeachLayers();
        }
        //Конец анимаций
        private void OpacityLayersScrollBar_Scroll(object sender, ScrollEventArgs e)
        {

            OpacityLayers = (float)OpastyLayerScrollBar.Value;
            OpacityFrame = 0;
            ForeachLayers();
            if (Frame.ActiveFrameId > 0)
            {

                PrevFrame.Opacity = OpacityFrame;
            }


        }



        private void AddFrame(List<BitmapSource> get = null)
        {

            new Frame();
            ActiveFrame = Frame.frames[Frame.ActiveFrameId];
            if (get != null)
            {
                foreach (BitmapSource source in get)
                {
                    Image image1 = new Image();
                    image1.Source = source;
                    ActiveFrame.layers.Add(new Layer(image1, source, false));
                }

            }



            StackPanel stackPanel = new StackPanel();
            TextBox textBox = new TextBox();

            TextBlock text = new TextBlock();
            text.Text = (Frame.frames.Count - 1).ToString();
            stackPanel.Width = 496;
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Tag = Frame.frames.Count - 1;
            stackPanel.Margin = new Thickness(0, 0, 0, 8);
            stackPanel.MouseDown += SelectedFrame;
            stackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.Frame)));
            
            textBox.FontSize = 24;
            textBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            textBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#32323254"));
            textBox.Width = 270;
            textBox.Text = "Frame " + (Frame.frames.Count).ToString();
            textBox.Tag = Frame.frames.Count - 1;
            textBox.TextChanged += ChangeFrameTextBox;
            stackPanel.Children.Add(text);
           
                Image image = new Image();
                image.Height = 64;
                image.Width = 64;


                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                image.Stretch = Stretch.Uniform;
                stackPanel.Children.Add(image);   
                stackPanel.Children.Add(textBox);
            SelectedFrame((object)stackPanel, null);
            if (ActiveFrame.frameName != null)
            {
                ActiveFrame.frameName = textBox.Text;
            }
            else
            {
                 textBox.Text= ActiveFrame.frameName;
            }
           
            FramesWrapPanel.Children.Add(stackPanel);




        }
        private void Click_AddFrame_Button(object sender, RoutedEventArgs e)
        {
            if (MainPanel.Children.Count == 0)
            {
                MessageBox.Show("Сначала вам нужно создать файл или открыть его во вкладке файл");
                return;
            }
            AddFrame();
        }

        private void CanvasPalytre_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void SetColorSettingButton(Button get, string color)
        {
            Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            get.Background = brush;
        }
        private void DeleteLayerClick(object sender, RoutedEventArgs e)
        {
            if (MainPanel.Children.Count == 0)
            {
                MessageBox.Show("Сначала вам нужно создать файл или открыть его во вкладке файл");
                return;
            }
            isFrame = isFrameCheck(sender);
            if (isFrame && Frame.frames.Count == 1)
            {

                MessageBox.Show("Невозможно удалить кадр");
                return;
            }
            if (!isFrame && ActiveFrame.layers.Count == 1)
            {
                MessageBox.Show("Невозможно удалить слой");
                return;
            }

            if (!isFrame)
            {
                SetColorSettingButton(DeleteLayer_Button, "#FFDDDDDD");
                ActiveFrame.layers.RemoveAt(selected_layer);
                LayerWrapPanel.Children.RemoveAt(selected_layer);
                MainPanel.Children.RemoveAt(selected_layer);
                selected_layer = selected_layer - 1 == -1 ? 0 : selected_layer - 1;
                int count = 0;
                Frame.frames[Frame.ActiveFrameId].RemoveLayers(MainPanel, MainPanel.Children, LayerWrapPanel, LayerWrapPanel.Children, ref count_layer, isFrame);

                foreach (Layer layer in ActiveFrame.layers)
                {
                    VisualCell((BitmapSource)layer.mainSource, count);
                    count++;
                }
                VisualPictureFrame();
                return;
            }
            else
            {
                SetColorSettingButton(DeleteFrame_Button, "#FFDDDDDD");
                FramesWrapPanel.Children.RemoveAt(Frame.ActiveFrameId);

                Frame.frames.RemoveAt(Frame.ActiveFrameId);
                Frame.ActiveFrameId = Frame.ActiveFrameId - 1 == -1 ? Frame.ActiveFrameId : Frame.ActiveFrameId - 1;
                Frame.LastActiveFrameId = Frame.ActiveFrameId;
                int q = 0;
                foreach (UIElement element in FramesWrapPanel.Children)
                {
                    StackPanel panel = element as StackPanel;
                    TextBlock text = panel.Children[0] as TextBlock;
                    text.Text = q.ToString();
                    panel.Tag = q;
                    q++;
                    if (q == Frame.ActiveFrameId)
                    {

                    }
                }
                SelectedFrame(FramesWrapPanel.Children[Frame.ActiveFrameId], null);

            }






        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            isDeativeLayers = (bool)check.IsChecked;
            SelectedLayer(LayerWrapPanel.Children[selected_layer], null);
        }

        private void ClickisActiveLayer(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            ActiveFrame.layers[selected_layer].isActive = (bool)check.IsChecked;



            SelectedLayer(LayerWrapPanel.Children[selected_layer], null);
            VisualPictureFrame();

        }




        private bool isFrameCheck(object check)
        {
            Button button = check as Button;
            Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
            button.Background = brush;
            return button.Tag.ToString() == "frame";

        }


        private void MoveLayerClicked_Click(object sender, RoutedEventArgs e)
        {
            if (MainPanel.Children.Count == 0)
            {
                MessageBox.Show("Сначала вам нужно создать файл или открыть его во вкладке файл");
                return;
            }
            isFrame = isFrameCheck(sender);
            ActiveFrame.layers[selected_layer].SetMake(State.Move, isFrame ? Frame.ActiveFrameId : selected_layer);
            //  makeText.Text = " Перемещение ( Выберите слой, что бы поменять слои местами )";
        }

        private void UniteLayerClicked_Click(object sender, RoutedEventArgs e)
        {
            if (MainPanel.Children.Count == 0)
            {
                MessageBox.Show("Сначала вам нужно создать файл или открыть его во вкладке файл");
                return;
            }
            isFrame = isFrameCheck(sender);
            ActiveFrame.layers[selected_layer].SetMake(State.Unity, isFrame ? Frame.ActiveFrameId : selected_layer);
            // makeText.Text = " Объединение ( Выберите слой, что бы объединить их в один )";
        }

        private void СoppyLayerClicked_Click(object sender, RoutedEventArgs e)
        {
            if (MainPanel.Children.Count == 0)
            {
                MessageBox.Show("Сначала вам нужно создать файл или открыть его во вкладке файл");
                return;
            }
            isFrame = isFrameCheck(sender);

            CoppyLayer(isFrame ? Frame.ActiveFrameId : selected_layer);
            // makeText.Text = " Копирование ( Выберите слой, что бы копировать его в другой слой )";
        }
        private void UniteLayer(int res)
        {
            if (MainPanel.Children.Count == 0)
            {
                MessageBox.Show("Сначала вам нужно создать файл или открыть его во вкладке файл");
                return;
            }
            SetColorSettingButton(UniteLayerClicked_Button, "#FFDDDDDD");

            // Image clone1 = (Image)MainPanel.Children[selected_layer];
            ActiveFrame.layers[res].mainSource = Frame.FrameToSource(new int[2] { selected_layer, res }, Frame.ActiveFrameId);

            ActiveFrame.layers[res].imageSourceMain = Tools.SetPicture((BitmapSource)ActiveFrame.layers[res].mainSource, false);
            ActiveFrame.layers.RemoveAt(selected_layer);

            //   ActiveFrame.layers[selected_layer].main = clone1;
            int count = 0;

            Frame.frames[Frame.ActiveFrameId].RemoveLayers(MainPanel, MainPanel.Children, LayerWrapPanel, LayerWrapPanel.Children, ref count_layer);
            foreach (Layer layer in ActiveFrame.layers)
            {
                VisualCell((BitmapSource)layer.mainSource, count);
                count++;
            }
            SelectedLayer(LayerWrapPanel.Children[res == LayerWrapPanel.Children.Count ? res - 1 : res], null);
            ActivateBytes();

        }
        private void CoppyLayer(int ID)
        {
            int count = 0;
            if (!isFrame)
            {
                VisualCell();
                ActiveFrame.layers.RemoveAt(ActiveFrame.layers.Count - 1);

                SetColorSettingButton(СoppyLayerClicked_Button, "#FFDDDDDD");
                //      Frame.frames[Frame.ActiveFrameId].RemoveLayers(MainPanel, MainPanel.Children, LayerWrapPanel, LayerWrapPanel.Children, ref count_layer);

                Image image = (Image)MainPanel.Children[MainPanel.Children.Count - 1];
                ActiveFrame.layers.Add(new Layer(image, (BitmapSource)image.Source));

                ActiveFrame.layers[ActiveFrame.layers.Count - 1].imageSourceMain = Tools.SetPicture((BitmapSource)ActiveFrame.layers[ID].main.Source);



                SelectedLayer(LayerWrapPanel.Children[ActiveFrame.layers.Count - 1], null);
                ActivateBytes();

            }
            else
            {
                int last_selected_layer = selected_layer;
               AddFrame();
                SetColorSettingButton(СoppyFrameClicked_Button, "#FFDDDDDD");
                Frame newFrame = Frame.frames[Frame.frames.Count - 1];

                for (int i = 0; i < Frame.frames[ID].layers.Count; i++)
                {

                    Image image = Frame.frames[ID].layers[i].main;
                    Layer add_layer = new Layer(image, (BitmapSource)image.Source);



                    add_layer.imageSourceMain = Tools.SetPicture((BitmapSource)Frame.frames[ID].layers[i].mainSource, false);

                    newFrame.layers.Add(add_layer);


                }






                Frame.frames[Frame.ActiveFrameId].RemoveLayers(MainPanel, MainPanel.Children, LayerWrapPanel, LayerWrapPanel.Children, ref count_layer);
                Frame.frames[Frame.frames.Count - 1].layers.RemoveAt(0);
                foreach (Layer layer in ActiveFrame.layers)
                {
                   
                    VisualCell((BitmapSource)layer.mainSource, count);
                    count++;
                }
               
                StackPanel stack = (StackPanel)FramesWrapPanel.Children[Frame.frames.Count - 1];
                SelectedFrame(stack, null);
                SelectedLayer(LayerWrapPanel.Children[last_selected_layer], null);
                VisualPictureFrame(ID);
                VisualPictureFrame();
            //    selected_layer = selected_layer != 0 ? selected_layer - 1 : 0;
            }
        }
        private void MoveLayer(int res)
        {
            int count = 0;
            if (!isFrame)
            {
                SetColorSettingButton(MoveLayerlicked_Button, "#FFDDDDDD");
                Image clone1 = (Image)MainPanel.Children[selected_layer];
                Image clone2 = (Image)MainPanel.Children[res];
                ActiveFrame.layers[res].mainSource = (BitmapSource)clone1.Source;
                ActiveFrame.layers[selected_layer].mainSource = (BitmapSource)clone2.Source;
                ActiveFrame.layers[res].main = clone1;
                ActiveFrame.layers[selected_layer].main = clone2;
                ActiveFrame.layers[res].imageSourceMain = Tools.SetPicture((BitmapSource)ActiveFrame.layers[res].mainSource, false);
                ActiveFrame.layers[selected_layer].imageSourceMain = Tools.SetPicture((BitmapSource)ActiveFrame.layers[selected_layer].mainSource, false);

                Frame.frames[Frame.ActiveFrameId].RemoveLayers(MainPanel, MainPanel.Children, LayerWrapPanel, LayerWrapPanel.Children, ref count_layer);
                foreach (Layer layer in ActiveFrame.layers)
                {
                    VisualCell((BitmapSource)layer.mainSource, count);
                    count++;
                }
                VisualPictureFrame();
            }
            else
            {
                SetColorSettingButton(MoveFrameClicked_Button, "#FFDDDDDD");
                List<Layer> layers1 = Frame.frames[res].layers;
                List<Layer> layers2 = ActiveFrame.layers;

                Frame.frames[res].layers = layers2;

                ActiveFrame.layers = layers1;
                Frame.frames[res].RemoveLayers(MainPanel, MainPanel.Children, LayerWrapPanel, LayerWrapPanel.Children, ref count_layer);
                Frame.frames[Frame.ActiveFrameId].RemoveLayers(MainPanel, MainPanel.Children, LayerWrapPanel, LayerWrapPanel.Children, ref count_layer);
                foreach (Layer layer in ActiveFrame.layers)
                {
                    VisualCell((BitmapSource)layer.mainSource, count);
                    count++;
                }
                StackPanel stack = (StackPanel)FramesWrapPanel.Children[Frame.ActiveFrameId];
                SelectedFrame(stack, null);

                VisualPictureFrame(res);
                VisualPictureFrame();
                selected_layer = 0;

            }

        }

        private void DrawCanvasPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Tools.instrumentsTool == Instruments.Move)
            {
                Canvas c = (Canvas)sender;
                Mouse.Capture(c);
                p = Mouse.GetPosition(c);
                canMove = true;
            }
        }

        private void DrawCanvasPanel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Tools.instrumentsTool == Instruments.Move && canMove)
            {
                Canvas c = (Canvas)sender;
                c.SetValue(Canvas.LeftProperty, e.GetPosition(null).X - p.X - 450);
                c.SetValue(Canvas.TopProperty, e.GetPosition(null).Y - p.Y - 150);
            }
        }

        private void DrawCanvasPanel_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            canMove = false;
        }





        private void TabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SetVisibilityFrames(Visibility visibility)
        {
            //OptionLayers.Visibility = visibility;
            //   TabControlLeftOptions.Height = visibility==Visibility.Visible?668:996;
        }
        private void AutoSaveFile(object state)
        {
           
            try
            {
                do
                {
                   
                    if (state != null)
                    {
                        
                        Thread.Sleep(settingsProgram.GetValueInListTimes(1)[0]*1000);
                        Dispatcher.Invoke(() => { Title = "E" + new Random().Next(0, 1000).ToString(); });
                       
                    }
                    
                        if (!Directory.Exists( PathBackup))
                        {
                            Directory.CreateDirectory( PathBackup);
                        }
                       
                     
                        FileInfo info = new FileInfo(endSavePathFile);
                    endSavePathFile = info.Extension == ".png" ?  PathBackup + info.Name.Replace(".png","") +".hru": endSavePathFile;
                    if (!Block.selectedBlocks||state==null)
                    {
                        Dispatcher.Invoke(() => { SaveFile(false, endSavePathFile); });
                    }
                    
                   
                } while (state != null);
            }
            catch (Exception e)
            {
              
            }
        }
        private void LoadSettings()
        {
            if (File.Exists(myPathFolder+"settings.dat"))
            {
                using (Stream stream = new FileStream(myPathFolder+"settings.dat", FileMode.Open))
                {

                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    settingsProgram = binaryFormatter.Deserialize(stream) as SettingsMyProgram;

                }
            }
            else
            {
                settingsProgram = new SettingsMyProgram(myPathFolder);
            }
        }
        private void SaveSettings()
        {
            settingsProgram.lastFilePath = endSavePathFile;
            using (Stream stream = new FileStream(myPathFolder+"settings.dat", FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, settingsProgram);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AutoSaveFileThread?.Abort();
            DataBase.SetStateThread(StateThread.Close);
        
            AutoSaveFile(null);
            SaveSettings();
          //  isChek = false;
           
            if (DataBase.client!=null&&DataBase.client.HasUpdate(_version))
            {
                string JavaPath = myPathFolder+"Setup.exe";
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = JavaPath;
                startInfo.Arguments = "update";
                Process.Start(startInfo);
            }

        }

        private void CategoriButton_Click(object sender, RoutedEventArgs e)
        {
            while (FilesComboBox.Items.Count != 0)
            {
                FilesComboBox.Items.RemoveAt(0);
            }
            Button button = sender as Button;
            CategoriesWrapPanel.IsEnabled = false;
            ConnectionAndPublishPanel.IsEnabled = false;
           
            DataBase.SelectCategory(button.Content.ToString());
           DataBase.SetStateThread(StateThread.StartNew);


        }
        private void ConnectCloseButton_Click(object sender, RoutedEventArgs e)
        {
            DataBase.SetStateThread(StateThread.Close);
            while (CategoriesWrapPanel.Children.Count != 0)
            {
                CategoriesWrapPanel.Children.RemoveAt(0);
            }
            while (WrapPanelPuctiresList.Children.Count != 0)
            {
                WrapPanelPuctiresList.Children.RemoveAt(0);
            }
            DataBase.ClearClient();
            ConnectButton.Click += ConnectButton_Click;
            ConnectButton.Click -= ConnectCloseButton_Click;
            ConnectButton.Content = "Подключиться";
        }
            private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //for (int i = 0; i < 1000; i++)
                //{
                //    var get = (Key)i;
                //    char getChar = 's';
                //   if(char.TryParse(get.ToString(),out getChar))
                //    {
                //        MessageBox.Show(getChar.ToString());
                //    }
                  
                //}
               
                DataBase.SetClient(Addres.Text);
                StateConnection.Text = "Подключено";
               
                foreach (string path in DataBase.client.GetAllCategories())
                {
                    Button button = new Button();
                    button.Content = path;
                    button.Tag = path.Replace("/", "");
                    button.Click += CategoriButton_Click;
                    CategoriesWrapPanel.Children.Add(button);
                    DataBase.SelectCategory(path.Replace("/", ""));
                }
                SetActivePanelsInBD(false);

                    settingsProgram.MyIDInBD = settingsProgram.MyIDInBD==null?DataBase.client.GenerateUniqeID(): settingsProgram.MyIDInBD;
                TextBoxMyID.Text = settingsProgram.MyIDInBD;
                TextBlockMyID.Text = settingsProgram.MyIDInBD;
                // Task.Factory.StartNew(() => { CheckUpdateBD(); });
                DataBase.SetStateThread(StateThread.StartLoop);
                ConnectButton.Click -= ConnectButton_Click;
                ConnectButton.Click += ConnectCloseButton_Click;
                ConnectButton.Content = "Отключиться";



            }
            catch (Exception ex)
            {
                StateConnection.Text = ex.Message;
            }
            finally
            {
                SetActivePanelsInBD(true);
            }

        }
        public void OpenBackupClick_Button(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            OpenFile(true, button.Tag.ToString());
        }
        public void DeleteBackupClick_Button(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TreeViewItem viewItem = button.Parent as TreeViewItem;
            File.Delete(button.Tag.ToString());
            TreeViewItem mainViemItem = viewItem.Parent as TreeViewItem;
            mainViemItem.Items.Remove(viewItem);
        }
        public void DownloadClick_Button(object sender, RoutedEventArgs e)
        {
            BinaryFormatter binary = new BinaryFormatter();
            Button button = sender as Button;
            var FileDialog = new SaveFileDialog();
            FileDialog.Filter = "bin|*.hru";
            if (FileDialog.ShowDialog() == true)
            {

            //  Stream stream = new MemoryStream(client.GetPicture(active_path.ToString(), true, button.Tag.ToString())[0]);
          //      FileHru Setting = (FileHru)binary.Deserialize(stream);

         //        FileHru.SaveFile(Setting, FileDialog.FileName);
            }
        }

        
           
        
        
        private void ClearAndNotSave()
        {
            ClearFramesAndLayers();
            WrapPanelBackup.Visibility = Visibility.Visible;
            CenterCanvas.Visibility = Visibility.Collapsed;
            openBackup = true;
        }
        public void SendEventSaveFile()
        {
            SaveFile(null, null);
        }
        public void SendEventClearActiveLayer()
        {
            if (!openBackup)
            {
                int lenght = ActiveFrame.layers[selected_layer].imageSourceMain.Length;
                ActiveFrame.layers[selected_layer].imageSourceMain = new byte[lenght];
                ActivateBytes();
                VisualPictureFrame();
            }
        }
        public void SendEventCreateFile()
        {
            CreateNewFile(null, null);
        }
        public void SendEventCoppy()
        {
           
                
                ClearColor();
                Block.statePicture = StatePicture.Coppy;
            coppyPixels = new byte[Tools.pixels.Length];
            Block.bytes_to_coppy.CopyTo(coppyPixels, 0);


        }
        public void SendEventPaste()
        {
            if (coppyPixels != null&&(Block.statePicture == StatePicture.Coppy || Block.statePicture == StatePicture.Cut))
            {
                VisualCell();
                ActiveFrame.layers[selected_layer].imageSourceMain = coppyPixels;
            //    Tools.pixels = Block.bytes_to_coppy;
             //   Block.bytes_to_coppy.CopyTo(ActiveFrame.layers[selected_layer].imageSourceMain,0);
                ActivateBytes();
                ClearColor();

                Block.statePicture = StatePicture.None;
                Block.bytes_to_coppy = null;

            }
           
        }
        private void RemoveAllPanels()
        {
            while (WrapPanelPuctiresList.Children.Count != 0)
            {
                WrapPanelPuctiresList.Children.RemoveAt(0);
            }
        }
        public void SendEventCut()
        {
          
                //  Block.Can_coppy = true;
                ClearColor();
                
                //     Block.Can_coppy = false;
                Block.statePicture = StatePicture.Cut;
            coppyPixels = new byte[Tools.pixels.Length];
            Block.bytes_to_coppy.CopyTo(coppyPixels, 0);

            foreach (Block block in blocks)
                {
                    block.ClearPosAndMove();
                }

            ActivateBytes();
            

        }
        public void SendEventDeleteFile()
        {
            ClearAndNotSave();
        }
        public void SendEventUndo()
        {
            BackPixels(null, null);
        }
        public void SendEventUp()
        {
            UpPixels(null, null);
        }
        private void Publish(int id)
        {
            BitmapSource source = (BitmapSource)ActiveFrame.layers[0].mainSource;
            List<string> send_color = new List<string>();
            foreach (UIElement image in Palytre1.ColorPanel.Children)
            {
                Image image1 = (Image)image;
                send_color.Add(image1.Tag.ToString());
            }


            DataBase.PublishFile(id, TextBoxFileNamePublish.Text, TextBoxAuthor.Text, TextBoxDescription.Text, Frame.frames[0], new FileHru(Frame.frames, countPixelX, countPixelY, source.Format.BitsPerPixel, send_color, TextBoxFileNamePublish.Text));
        }
        private void PublishFile_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataBase.client == null)
            {
                MessageBox.Show("Наверное сначала стоит подключиться к серверу");
                return;
            }
            Publish(0);

        }
        public void UpdateBDPicture(int idPicture,string name, string author, string descruption, string id, BitmapImage bitmap)
        {
            BDPicture getPicture = WrapPanelPuctiresList.Children[idPicture] as BDPicture;
            Image image = new Image();
            getPicture.Tag = name;
            image.Width = 50;
            image.Height = 50;
            image.Source = bitmap;

            getPicture.TextBlockFileName.Text = name;
            getPicture.TextBlockAuthor.Text = author;
            getPicture.TextBlockInfo.TextWrapping = TextWrapping.Wrap;
            getPicture.TextBlockInfo.Text = descruption;
            getPicture.TextBlockID.Text = id;
            getPicture.MainPicrute.Source = bitmap;
            RenderOptions.SetBitmapScalingMode(getPicture.MainPicrute, BitmapScalingMode.NearestNeighbor);
        }
        public void VisualBDPicture(string name,string author,string descruption,string id,BitmapImage bitmap)
        {
            BDPicture getPicture = new BDPicture();
            getPicture.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(PositionPanel.PictureBD)));
            Image image = new Image();
            getPicture.Tag = name;
            image.Width = 50;
            image.Height = 50;
            image.Source = bitmap;
            
            getPicture.TextBlockFileName.Text = name;
            getPicture.TextBlockAuthor.Text = author;
            getPicture.TextBlockInfo.TextWrapping = TextWrapping.Wrap;
            getPicture.TextBlockInfo.Text = descruption;
            getPicture.TextBlockID.Text = id;
            getPicture.MainPicrute.Source = bitmap;
            RenderOptions.SetBitmapScalingMode(getPicture.MainPicrute, BitmapScalingMode.NearestNeighbor);
            WrapPanelPuctiresList.Children.Add(getPicture);
            
        }
        public void ClearPanelsInBD()
        {
            while (WrapPanelPuctiresList.Children.Count != 0)
            {
                WrapPanelPuctiresList.Children.RemoveAt(0);
            }
        }
        public void SetActivePanelsInBD(bool isActive)
        {
            CategoriesWrapPanel.IsEnabled = isActive;
            ConnectionAndPublishPanel.IsEnabled = isActive;
            ConnectionAndPublishPanel.IsEnabled = isActive;
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            settingsProgram = settingsProgram == null ? new SettingsMyProgram(myPathFolder) : settingsProgram;
            var settingsDialog = new SettingsDialog(settingsProgram.isNull(), settingsProgram, _version.ToString());
            if (settingsDialog.ShowDialog() == true)
            {
               
                UpdateProgramFromSettings();
                SaveSettings();
            }
            Frame.ActiveFrameId = Frame.frames.Count-1;
            Frame.LastActiveFrameId = Frame.frames.Count - 1;
          //  set.Owner = this;
           // set.Show();
        }
        
        private void FillListBlock()
        {
            ActivateBytes();
            byte[] source_byte = Tools.pixels;
            int x = 0;
            int y = 0;
           


            for (int i = 3; i < source_byte.Length - 1; i += 4)
            {
                if (source_byte[i] != 0)
                {

                    blocks.Add(new Block(x, y, Color.FromArgb(source_byte[i], source_byte[i - 1], source_byte[i - 2], source_byte[i - 3])));
                }

                y = x < countPixelX - 1 ? y : y + 1;
                x = x < countPixelX - 1 ? x + 1 : 0;


            }
        }
        private void MoveBlocksAsync(int x, int y)
        {
            foreach (Block block in blocks)
            {
                block.ClearPosAndMove(x, y);
            }
            foreach (Block block in blocks)
            {
                block.Move();
            }
            ActiveFrame.layers[selected_layer].mainSource = Tools.EditorBitMap;
            ActiveFrame.layers[selected_layer].main.Source = ActiveFrame.layers[selected_layer].mainSource;
            ActiveFrame.layers[selected_layer].imageSourceMain = Tools.pixels;
            ActivateBytes();
            //  StackPanel button = new StackPanel();
            //  button.Tag = selected_layer;
            //  SelectedLayer(button, null);

        }
        private void ClearColor()
        {
            foreach (Block block in blocks)
            {
                block.ClearColor();
            }
            Block.selectedBlocks = false;
            ActivateBytes();
        }
        public void MovePixels(int x, int y)
        {

            if (blocks.Count == 0)
            {
                FillListBlock();
            }
           

            //if (!isUp && !wasReverse)
            //{
            //    blocks.Reverse();
            //    wasReverse = true;
            //}
            //else if (isUp && wasReverse)
            //{
            //    blocks.Reverse();
            //    wasReverse = false;
            //}D
            MoveBlocksAsync(x, y);
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {


        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
          
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            //     isActiveSettingsKeys = true;
           
            if (!(Keyboard.FocusedElement is TextBox))
            {
              
                settingsProgram.RunMetod();
            }
            
            //switch (key)
            //{
            //    case Key.Up:

            //        MovePixels(0, -1);
            //        break;
            //    case Key.Down:

            //        MovePixels(0, 1);
            //        break;
            //    case Key.Left:

            //        MovePixels(-1, 0);
            //        break;
            //    case Key.Right:

            //        MovePixels(1, 0);
            //        break;
            //}
        }





        private void CountPixels_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (!int.TryParse(CountPixels.Text, out sizeDraw))
            {
                CountPixels.Text = "1";
                return;
            }
            if (sizeDraw > countPixelX || sizeDraw > countPixelX)
            {
                CountPixels.Text = "1";
                return;
            }

            // CountPixels.Focusable = false;
        }




        private void CountPixels_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            //  isActiveSettingsKeys = false;
            //  myWindow.Focusable = false;
        }

        private void CountPixels_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int get = 0;
            if(int.TryParse(textBox.Text,out get))
            {
                if (get < (countPixelX > countPixelY ? countPixelX : countPixelY))
                {
                    sizeDraw = get;
                    PixelsScrool.Value = sizeDraw;
                }
            }
        }

        private void PixelsScrool_Scroll(object sender, ScrollEventArgs e)
        {

            sizeDraw = (int)PixelsScrool.Value;
            CountPixels.Text = sizeDraw.ToString();
        }

        private void BackPixels(object sender, MouseButtonEventArgs e)
        {
            if (ReturnPixelsBack.IsEnabled)
            {
                layerPicture.ReturnPixels(false).CopyTo(ActiveFrame.layers[selected_layer].imageSourceMain, 0);


                ChangeReturnPixelsButtons();
                ActivateBytes();
            }
        }
        private void ChangeReturnPixelsButtons()
        {
            double opasyti = 0;
            ReturnPixelsBack.IsEnabled = layerPicture.ReturnState(true, ref opasyti);
            ReturnPixelsBack.Opacity = opasyti;
            ReturnPixelsUp.IsEnabled = layerPicture.ReturnState(false, ref opasyti);
            ReturnPixelsUp.Opacity = opasyti;
        }
        private void UpPixels(object sender, MouseButtonEventArgs e)
        {
            if (ReturnPixelsUp.IsEnabled)
            {
                layerPicture.ReturnPixels(true).CopyTo(ActiveFrame.layers[selected_layer].imageSourceMain, 0);
                ChangeReturnPixelsButtons();
                ActivateBytes();
            }
        }

        
        private void CountPixels_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int check = 0;
           //   isActiveSettingsKeys = true;
            if (int.TryParse(CountPixels.Text, out check))
            {
                if (check < (countPixelX > countPixelY ? countPixelX : countPixelY))
                {
                    sizeDraw = check;
                    PixelsScrool.Value = sizeDraw;
                }

            }
            CountPixels.Text = sizeDraw.ToString();
        }

       

        private void myWindow_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UIElement uIElement = (UIElement)e.KeyboardDevice.FocusedElement;
            isActiveSettingsKeys=!(uIElement is TextBox);
        }

        private void pencil_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Up &&Keyboard.IsKeyDown(Key.LeftShift))
            {
             
            }
        }

        private void myWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {


            WrapPanelBackup.Height = e.NewSize.Height - 235;
            WrapPanelCenterCanvas.Height = e.NewSize.Height - 235;
            CenterCanvas.Height = e.NewSize.Height - 235;
            AllPictureinBD.Height = e.NewSize.Height-190;
            CategoriesScrollViewer.Height = e.NewSize.Height-90;
        }

        private void ButtonJoinThisID_Click(object sender, RoutedEventArgs e)
        {
            if (settingsProgram.MyIDInBD != TextBoxMyID.Text)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Вы уверены,что хотите войти под данным айди?\nУчтите, что данный айди не сохранится, поэтому лучше будет, если вы сохраните текущий айди в текстовый файл", "Новый айди", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    TextBoxMyID.Text = settingsProgram.MyIDInBD; 
                    return;
                }
            }
            if (DataBase.client == null)
            {
                MessageBox.Show("Наверное сначала стоит подключиться к серверу");
                return;
            }
            
            settingsProgram.MyIDInBD = TextBoxMyID.Text;
            TextBlockMyID.Text = settingsProgram.MyIDInBD;
        }

        private void ButtonCreateID_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Вы уверены,что хотите сгенерировать новый айди?\nУчтите, что данный айди не сохранится, поэтому лучше будет, если вы сохраните текущий айди в текстовый файл", "Новый айди", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }
            if (DataBase.client == null)
            {
                MessageBox.Show("Наверное сначала стоит подключиться к серверу");
                return;
            }
            settingsProgram.MyIDInBD = DataBase.client.GenerateUniqeID();
            TextBoxMyID.Text = settingsProgram.MyIDInBD;
            TextBlockMyID.Text = settingsProgram.MyIDInBD;
        }

        private void ButtonUpdateFile_Click(object sender, RoutedEventArgs e)
        {
            if (DataBase.client == null)
            {
                MessageBox.Show("Наверное сначала стоит подключиться к серверу");
                return;
            }
            Publish((bool)UpdateAllCheckBox.IsChecked?3:1);
        }

        private void ButtonDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            if (DataBase.client == null)
            {
                MessageBox.Show("Наверное сначала стоит подключиться к серверу");
                return;
            }
            Publish(2);
        }

        private void GetFilesButton_Click(object sender, RoutedEventArgs e)
        {
            while (FilesComboBox.Items.Count!=0)
            {
                FilesComboBox.Items.RemoveAt(0);
            }
            if (DataBase.client == null)
            {
                MessageBox.Show("Наверное сначала стоит подключиться к серверу");
                return;
            }
            foreach (string get in DataBase.client.GetAllMyFiles(DataBase.active_category, settingsProgram.MyIDInBD))
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = get;
                FilesComboBox.Items.Add(comboBoxItem);
            }
        }
     
        private void FilesComboBox_Initialized(object sender, EventArgs e)
        {
            FilesComboBox.SelectionChanged += FilesComboBox_SelectionChanged;
        }

        private void FilesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilesComboBox.SelectedIndex >=0)
            {
                ComboBoxItem comboBoxItem = FilesComboBox.Items[FilesComboBox.SelectedIndex] as ComboBoxItem;
                string[] info = DataBase.client.GetInfoMyFile(DataBase.active_category, comboBoxItem.Content.ToString());
                TextBoxFileNamePublish.Text = comboBoxItem.Content.ToString();
                TextBoxAuthor.Text = info[0];
                TextBoxDescription.Text = info[1];
            }
        }

        private void OpenFileInNewLayer(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "png|*.png";
          


            if (openFile.ShowDialog() == true)
            {
                OpenFile(false, openFile.FileName,true);
            }
        }

        private void UpdateAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            UpdateFileInfo.Visibility =(bool)UpdateAllCheckBox.IsChecked ? Visibility.Collapsed : Visibility.Visible;
        }
    }

}






    


