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
using System.Windows.Shapes;

namespace _8bitPaint
{
    /// <summary>
    /// Логика взаимодействия для SelectedSize.xaml
    /// </summary>
    public partial class SelectedSize : Window
    {
        public int SizeX { get; set; }
        public string NameFile { get; set; }
        public int SizeY { get; set; }
      
        public SelectedSize()
        {
            InitializeComponent();
        }

        private void Okey_Button_Click(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(xPixels.Text,out int x))
            {
                SizeX = x;
            }else
            {
                MessageBox.Show("Не удалось конвертировать " + xPixels.Text + " в число");
                return;
            }
            if (int.TryParse(yPixels.Text, out int y))
            {
                SizeY = y;
            }
            else
            {
                MessageBox.Show("Не удалось конвертировать " + yPixels.Text + " в число");
                return;
            }
            if (FileName.Text.Length<10&&FileName.Text.Length>3)
            {
                NameFile = FileName.Text;
            }
            else
            {
                MessageBox.Show("Название файла не должно превышать 10 символов и не должно быть меньше 4 символов");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void RandomName_Button_Click(object sender, RoutedEventArgs e)
        {
            FileName.Text = new Random().Next(1000, 100000000).ToString();
            NameFile = FileName.Text;
        }
    }
}
