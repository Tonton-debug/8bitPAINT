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

namespace _8bitPaint
{
    /// <summary>
    /// Логика взаимодействия для Everyone.xaml
    /// </summary>
    public partial class Everyone : UserControl
    {
        public Everyone()
        {
            InitializeComponent();
         
           
        }
        public void SetInfo(int selectID,int second_time,string info)
        {
            TextBoxDate.Text = second_time.ToString();
            TimeCombox.SelectedIndex = selectID;
            InfoTextBlock.Text = info;
            ChangeWorlds();
        }
        private void TimeCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeWorlds();
        }

        private void TextBoxDate_KeyUp(object sender, KeyEventArgs e)
        {

            ChangeWorlds();
        }
        private string GetDeclension(int number, string nominativ, string genetiv, string plural,int timeID)
        {
            number = number % 100;
            if (number >= 11 && number <= 19)
            {
                return plural;
            }
            var i = number % 10;
            switch (i)
            {
                case 1:
                    switch (timeID)
                    {
                        case 0:
                        case 1:
                        case 4:
                            return nominativ;
                        case 2:
                        case 3:
                        case 5:
                        case 6:
                            return nominativ.Replace("ую", "ый");
                        default:
                            return nominativ;
                    }
                case 2:
                case 3:
                case 4:
                            return genetiv;
                default:
                    return plural;
            }
        }
        private string GetDeclension(int number, string nominativ, string genetiv, string plural)
        {
            number = number % 100;
            if (number >= 11 && number <= 19)
            {
                return plural;
            }
            var i = number % 10;
            switch (i)
            {
                case 1:
                    return nominativ;
                case 2:
                case 3:
                case 4:
                    return genetiv;
                default:
                    return plural;
            }
        }
        private void ChangeWorlds()
        {
            int time = 0;
            if (int.TryParse(TextBoxDate.Text, out time))
            {
                EveryoneTextBox.Text = GetDeclension(time, "каждую", "каждые", "каждые", TimeCombox.SelectedIndex);
                TimeComboxItemSecond.Text = GetDeclension(time, "секунду", "секунды", "секунд");
                TimeComboxItemMinute.Text = GetDeclension(time, "минуту", "минуты", "минут");
                TimeComboxItemHour.Text = GetDeclension(time, "час", "часа", "часов");
              

            }
        }
       

        
    }
   

}
