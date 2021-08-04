using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _8bitPaint
{
    /// <summary>
    /// Логика взаимодействия для MyPalytre.xaml
    /// </summary>
    public partial class MyPalytre : UserControl
    {
        private int count_colors_in_panel = 0;
        private Point lastPoint;
        private float add_shades_color = 0.001f;
        private float OpacityPalytre = 1;
        private bool IsDeleteColor = false;
        private double h;
        private double s;
        private double v;
        private Color ARGBColor = Color.FromArgb(0, 0, 0, 0);
        public List<Color> ColorsInPalyte { get; private set; }
        public Color Draw { get; set; }
        public MyPalytre()
        {
            ColorsInPalyte = new List<Color>();
            InitializeComponent();
            Draw = (Color)ColorConverter.ConvertFromString("Black");
            VisualSelectedColorImage(Draw);
            AddColorInPanel(Draw);
            AddColorInPanel((Color)ColorConverter.ConvertFromString("white"));
            SetColorToActiveColorImg(Draw);
        }
        public void VisualListColorsToPalytre()
        {
            foreach (var item in ColorsInPalyte)
            {
                AddColorInPanel(item, false);
            }
        }
        public void SetColorToActiveColorImg(Color color)
        {
            Draw = color;
            Brush brush = new SolidColorBrush(color);
            Tools.ReplaceColor(brush, ActiveColorImage, color.ToString());
        }
        private void CanvasPalytre_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VisualSelectedColorImage(e.GetPosition(Palytre));
        }

        private void PalytreSelected(object sender, MouseButtonEventArgs e)
        {

        }
        public void VisualSelectedColorImage(Color clr, bool isARGB = false)
        {
            Tools.ColorToHSV(System.Drawing.Color.FromArgb(clr.R, clr.G, clr.B), out h, out s, out v);
            Shape shape = (Shape)Palytre;
            Brush brush1 = shape.Fill;

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {

                drawingContext.DrawRectangle(brush1, null, new Rect(0, 0, (int)shape.Width, (int)shape.Height));
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)shape.Width, (int)shape.Height, 0, 0, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);
            BitmapSource bitmapSource = bitmap;



            Brush brush = new SolidColorBrush((Color)Color.FromArgb(isARGB ? clr.A : (byte)ScrollBarPalytre.Value, clr.R, clr.G, clr.B));
            Tools.ReplaceColor(brush, SelectedColorImage, int.Parse(Math.Round(ScrollBarPalytre.Value).ToString()).ToString("X2") + clr.R.ToString("X2") + clr.G.ToString("X2") + clr.B.ToString("X2"));

            CanvasPalytre.Children.Remove(CirclePoint);
            CanvasPalytre.Children.Add(CirclePoint);
            clr.A = (byte)ScrollBarPalytre.Value;
            if (!isARGB)
            {
                VisualARGBAndHex(clr);
            }
            VisualShades();
        }
        private void VisualSelectedColorImage(Point point)
        {

            Shape shape = (Shape)Palytre;
            Brush brush1 = shape.Fill;

            Color clr;
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {

                drawingContext.DrawRectangle(brush1, null, new Rect(0, 0, (int)shape.Width, (int)shape.Height));
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)shape.Width, (int)shape.Height, 0, 0, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);
            BitmapSource bitmapSource = bitmap;

            clr = Tools.ColorFromHSV(h, point.X / CanvasPalytre.Width, (CanvasPalytre.Height - point.Y) / CanvasPalytre.Height);

            Brush brush = new SolidColorBrush((Color)Color.FromArgb((byte)ScrollBarPalytre.Value, clr.R, clr.G, clr.B));
            Tools.ReplaceColor(brush, SelectedColorImage, int.Parse(Math.Round(ScrollBarPalytre.Value).ToString()).ToString("X2") + clr.R.ToString("X2") + clr.G.ToString("X2") + clr.B.ToString("X2"));
            Canvas.SetLeft(CirclePoint, (int)point.X);
            Canvas.SetTop(CirclePoint, (int)point.Y);
            CanvasPalytre.Children.Remove(CirclePoint);
            CanvasPalytre.Children.Add(CirclePoint);
            lastPoint = point;
            clr.A = (byte)ScrollBarPalytre.Value;
            //  MessageBox.Show((point.X-3).ToString()+"\n"+(point.Y-5).ToString());
            VisualARGBAndHex(clr);
            VisualShades();
            //  InfoText.Text += "\nselect color:" + clr.R + "_" + clr.G + "_" + clr.B;
        }
        private void AddShaders()
        {
            foreach (UIElement uIElement in PanelShadeImages.Children)
            {
                
                Image image = uIElement as Image;
                if (image.Visibility == Visibility.Visible)
                    AddColorInPanel(Tools.GetPixel((BitmapSource)image.Source, 0, 0));
            }
        }
        private void VisualARGBAndHex(Color get)
        {
            A_Color_TextBox.Text = get.A.ToString();
            R_Color_TextBox.Text = get.R.ToString();
            G_Color_TextBox.Text = get.G.ToString();
            B_Color_TextBox.Text = get.B.ToString();
            Hex_Color_TextBox.Text = get.A.ToString("X2") + get.R.ToString("X2") + get.G.ToString("X2") + get.B.ToString("X2");
            ARGBColor = get;


        }
        private void VisualHSV(Color get)
        {
            double h;
            double s;
            double v;
            int maxPos = (int)CanvasPalytre.Height;
            System.Drawing.Color color = System.Drawing.Color.FromArgb(get.R, get.G, get.B);
            Tools.ColorToHSV(color, out h, out s, out v);



            GradientBar.Value = h;

            ImageBrush picture = (ImageBrush)GradientBar.Background;
            BitmapSource picture_source = (BitmapSource)picture.ImageSource;
            MyGradientStop.Color = Tools.GetPixel(picture_source, (int)GradientBar.Value, 0);
            //   color = System.Drawing.Color.FromArgb(255, MyGradientStop.Color.R, MyGradientStop.Color.G, MyGradientStop.Color.B);
            //     MessageBox.Show(h+"\n"+ s * maxPos + "\n"+ v * maxPos);


            Canvas.SetTop(CirclePoint, maxPos - (int)(v * maxPos));
            Canvas.SetLeft(CirclePoint, (int)(s * maxPos));
            lastPoint = new Point((int)(s * maxPos), (int)(v * maxPos));
            CanvasPalytre.Children.Remove(CirclePoint);
            CanvasPalytre.Children.Add(CirclePoint);
            Shape shape = (Shape)Palytre;
            Brush brush1 = shape.Fill;
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {

                drawingContext.DrawRectangle(brush1, null, new Rect(0, 0, (int)shape.Width, (int)shape.Height));
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)shape.Width, (int)shape.Height, 0, 0, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);
            BitmapSource bitmapSource = bitmap;
            Color lastPointColor = Tools.GetPixel(bitmapSource, (int)lastPoint.X, (int)lastPoint.Y);
            //  InfoText.Text = GradientBar.Value.ToString();

            // lastPoint = new Point(maxPos - v * maxPos, s * maxPos);
        }
        private void VisualShades()
        {

            Brush brush;
            Color color;
            float add = 0.1f;
            float r;
            float g;
            float b;
            color = Tools.GetPixel((BitmapSource)SelectedColorImage.Source, 0, 0);

            foreach (FrameworkElement get in PanelShadeImages.Children)
            {
                brush = new SolidColorBrush(color);
                Tools.ReplaceColor(brush, (Image)get, color.ToString());
                r = (255 - color.R) * add + color.R;
                g = (255 - color.G) * add + color.G;
                b = (255 - color.B) * add + color.B;
                color.R = byte.Parse(Math.Round(r).ToString());
                color.G = byte.Parse(Math.Round(g).ToString());
                color.B = byte.Parse(Math.Round(b).ToString());


                add += add_shades_color;

            }
        }
        private void Palytre_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                VisualSelectedColorImage(e.GetPosition(Palytre));

            }

        }

        private void AddColorClick(object sender, RoutedEventArgs e)
        {
            AddColorInPanel((Color)ColorConverter.ConvertFromString("#" + SelectedColorImage.Tag.ToString()));
        }

        private void AddShadesClick(object sender, RoutedEventArgs e)
        {
            AddShaders();
        }

        private void DeleteColorClick(object sender, RoutedEventArgs e)
        {
            IsDeleteColor = IsDeleteColor ? false : true;
            DeleteColorButton.Background = new SolidColorBrush(IsDeleteColor ? (Color)ColorConverter.ConvertFromString("Red") : Color.FromArgb(255, 221, 221, 221));
        }
        public void AddColorInPanel(Color color, bool addColor = true)
        {
            Image img;
            Brush brush;
            img = new Image();
            img.Height = 32;
            img.Width = 32;
            img.Margin = new Thickness(2, 0, 0, 0);
            ColorPanel.Children.Add(img);
            if (color.A == 0)
            {

                img.Tag = "00FFFFFF";
                img.MouseLeftButtonDown += ClickColor;
                count_colors_in_panel++;
                // img.Source = Background.Source;
                return;
            }
            brush = new SolidColorBrush(color);
            Tools.ReplaceColor(brush, img, color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"));
            img.MouseLeftButtonDown += ClickColor;
            count_colors_in_panel++;
            if (addColor)
            {
                ColorsInPalyte.Add(color);
            }

        }
        public void AddToListColorPalytre(Color get)
        {
            ColorsInPalyte.Add(get);
        }
        public void AddToListColorPalytre(List<Color> get)
        {
            ColorsInPalyte.Clear();
            get.CopyTo(ColorsInPalyte.ToArray());
        }
        public void AddToListColorPalytre(List<string> get)
        {
            ColorsInPalyte.Clear();
            foreach (var item in get)
            {
                ColorsInPalyte.Add((Color)ColorConverter.ConvertFromString(item));
            }
        }
        public void AddToListColorPalytre(string get)
        {
            ColorsInPalyte.Add((Color)ColorConverter.ConvertFromString(get));
        }
        private void ClickColor(object sender, MouseEventArgs e)
        {
            if (IsDeleteColor)
            {
                int id = ColorPanel.Children.IndexOf((Image)sender);
                Image image1 = (Image)sender;

                ColorsInPalyte.Remove((Color)ColorConverter.ConvertFromString("#" + image1.Tag.ToString()));

                ColorPanel.Children.Remove((Image)sender);
                DeleteColorButton.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                IsDeleteColor = false;
                return;
            }
            Image image = (Image)sender;
            if (image.Tag.ToString() == "00FFFFFF")
            {
                // ActiveColorImage.Source = Background.Source;
                Draw = Color.FromArgb(0, 0, 0, 0);
                return;
            }
            Draw = (Color)ColorConverter.ConvertFromString("#" + image.Tag.ToString());
            Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + image.Tag.ToString()));
            Tools.ReplaceColor(brush, ActiveColorImage, Draw.A.ToString("X2") + Draw.R.ToString("X2") + Draw.G.ToString("X2") + Draw.B.ToString("X2"));

        }
        private void ScrollShadesColor(object sender, ScrollEventArgs e)
        {
            ScrollBar bar = (ScrollBar)sender;

            add_shades_color = (float)bar.Value;
            VisualShades();
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            ImageBrush picture = (ImageBrush)scrollBar.Background;
            BitmapSource picture_source = (BitmapSource)picture.ImageSource;
            //  Tools.GetPixel(picture_source, (int)scrollBar.Value, 2).ToString();
            MyGradientStop.Color = Tools.GetPixel(picture_source, (int)scrollBar.Value, 2);
            h = scrollBar.Value;
            //GradientStopCollection gradients = new GradientStopCollection();
            //gradients.Add(new GradientStop(Color.FromArgb(Tools.GetPixel(picture_source, (int)scrollBar.Value, 2).A, 0, 0, 0), 0.1));
            //gradients.Add(new GradientStop(Color.FromArgb(Tools.GetPixel(picture_source, (int)scrollBar.Value, 2).A, 255, 255, 255), 0.9));
            //gradients.Add(new GradientStop(Tools.GetPixel(picture_source, (int)scrollBar.Value, 2), 0.5));
            //LinearGradientBrush brush = new LinearGradientBrush(gradients);
            //Palytre.Fill = brush;
            VisualSelectedColorImage(lastPoint);
        }

        private void ScrollBar_ScrollOpacityPalytre(object sender, ScrollEventArgs e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            OpacityPalytre = (float)scrollBar.Value;

            VisualSelectedColorImage(lastPoint);
        }

        private void ARGB_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            byte convertInt = 0;
            TextBox textBox = sender as TextBox;
            if (textBox.Tag.ToString() == "HEX")
            {
                try
                {
                    VisualHSV((Color)ColorConverter.ConvertFromString("#" + textBox.Text));
                    VisualSelectedColorImage((Color)ColorConverter.ConvertFromString("#" + textBox.Text));
                }
                catch { }
            }
            if (byte.TryParse(textBox.Text, out convertInt))
            {
                switch (textBox.Tag)
                {
                    case "A":
                        ARGBColor.A = convertInt;
                        ScrollBarPalytre.Value = convertInt;
                        break;
                    case "R":
                        ARGBColor.R = convertInt;
                        break;
                    case "G":
                        ARGBColor.G = convertInt;
                        break;
                    case "B":
                        ARGBColor.B = convertInt;
                        break;

                }


                VisualHSV(ARGBColor);
                VisualSelectedColorImage(ARGBColor, true);
                //  VisualSelectedColorImage(lastPoint);
            }

        }

        private void CountShadersScrolBar_Scroll(object sender, ScrollEventArgs e)
        {
            for (int i = 9; i > e.NewValue; i--)
            {
                Image image = PanelShadeImages.Children[i] as Image;
                image.Visibility = Visibility.Collapsed;


            }
            for (int q = 0; q < e.NewValue+1; q++)
            {
                Image image = PanelShadeImages.Children[q] as Image;
                image.Visibility = Visibility.Visible;
            }
        }
    }
}
        

