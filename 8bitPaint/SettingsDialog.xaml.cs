using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Forms=System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace _8bitPaint
{
    /// <summary>
    /// Логика взаимодействия для SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        private Button activeButton;
        private SettingsMyProgram settingsProgram;
        public SettingsDialog(bool isFirst, SettingsMyProgram get, string version)
        {

            InitializeComponent();
            VersionText.Text += version;
            settingsProgram = get;
          
            FillSettings();
        }
        private void FillButtons()
        {
            int count = 0;
            List<string[]> keys = settingsProgram.GetValuesKeysMetod();
            foreach (var get_panel in PanelKeys.Children)
            {
                StackPanel panel = get_panel as StackPanel;
                Button button = panel.Children[0] as Button;
                Button button2 = panel.Children[2] as Button;
                button.Content = keys[count][0];
                button2.Content = keys[count][1];
                count++;
            }
        }
        private void FillColorsInPicture()
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {

                Shape shape = BabaPicture.Children[i] as Shape;
                PositionPanel position = (PositionPanel)int.Parse(shape.Tag.ToString());
                shape.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(position)));
                count++;
            }
            for (int i = 0; i < 4; i++)
            {

                Shape shape = BDPicture.Children[i] as Shape;
                PositionPanel position = (PositionPanel)int.Parse(shape.Tag.ToString());
                shape.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settingsProgram.GetValueInDictionaryWindowColor(position)));
                count++;
            }

        }
        private int DateConverter( int get,int dateID)
        {
            switch (dateID)
            {
               
                case 1:
                    get = get / 60;
                    break;
                case 2:
                    get = get / 3600;
                    break;
               
               
            }
            return get;
        }
        private void FillPalytre()
        {
            List<string> get_colors = settingsProgram.ReturnColorsInPalytre();
            if (get_colors != null)
            {
                Palytre2.AddToListColorPalytre(get_colors);
                Palytre2.VisualListColorsToPalytre();
            }

        }
        private void FillEveryOneElements()
        {
            AutoUpdateBDTime.SetInfo(settingsProgram.GetValueInListTimes(0)[1], DateConverter(settingsProgram.GetValueInListTimes(0)[0], settingsProgram.GetValueInListTimes(0)[1]), "Обновлять бд ");
            AutoSaveFileTime.SetInfo(settingsProgram.GetValueInListTimes(1)[1], DateConverter(settingsProgram.GetValueInListTimes(1)[0], settingsProgram.GetValueInListTimes(1)[1]), "Автосохранение файла ");
            
        }
        private void FillSettings()
        {
            FillPalytre();
            FillColorsInPicture();
            FillButtons();
            FillEveryOneElements();
            CheckBoxFullSceen.IsChecked = settingsProgram.isFullScreen;
            CheckBoxOpenLastFile.IsChecked = settingsProgram.isOpenLastFile;
            CheckBoxCellActive.IsChecked= settingsProgram.isVisibleCells;
            TextBoxSizeCells.Text = settingsProgram.sizeCells.ToString();
            TextBoxStrokeThickness.Text = settingsProgram.StrokeThickness.ToString();
           
            SelectedPathBackup.Text= settingsProgram.BackupPath;
            if (DataBase.client != null)
            {
                foreach (string item in DataBase.client.GetInfoUpdatesText())
                {
                    VersionsInfo.Text += "\n" + item;
                }
            }
        }
        private string[] ReturnSendString(object sender)
        {
            string[] sendKeys = new string[2];
            activeButton = sender as Button;
            StackPanel stackPanel = activeButton.Parent as StackPanel;
            Button button2 = activeButton == stackPanel.Children[0] as Button ? stackPanel.Children[2] as Button : stackPanel.Children[0] as Button;
       //     activeButton.Content = "...";
            sendKeys[0] = activeButton == stackPanel.Children[0] as Button ? activeButton.Content.ToString() : button2.Content.ToString();
            sendKeys[1] = activeButton == stackPanel.Children[2] as Button ? activeButton.Content.ToString() : button2.Content.ToString();
            return sendKeys;
        }
        private void ClickKeyButton(object sender, RoutedEventArgs e)
        {
            activeButton = sender as Button;
            activeButton.Content = "...";
            settingsProgram.ReplaceKeyMetodToDictionary(activeButton.Tag.ToString(),ReturnSendString(sender));
        }
        private Button FindButton(string key)
        {
            foreach (var get_panel in PanelKeys.Children)
            {
                StackPanel panel = get_panel as StackPanel;
                Button button = panel.Children[0] as Button;
                if (button.Content.ToString() == key && button != activeButton)
                {
                    return button;
                }
            }
            return null;
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

            if (activeButton != null)
            {
                activeButton.Content = e.Key.ToString();
                settingsProgram.ReplaceKeyMetodToDictionary(activeButton.Tag.ToString(), ReturnSendString(activeButton));
                if (settingsProgram.HasCloneKey(ReturnSendString(activeButton)))
                {

                    Button button = FindButton(e.Key.ToString());
                    button.Content = "...";
                    settingsProgram.ReplaceKeyMetodToDictionary(button.Tag.ToString(), ReturnSendString(activeButton));
                    activeButton = null;
                    return;
                }
              

               

                activeButton = null;

            }
        }

        private void ReloadButtons(object sender, RoutedEventArgs e)
        {
            settingsProgram.ClearKeyDictionary();
            settingsProgram.InitializeDictionaryKeysMetod();
            FillButtons();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("G");
        }


        private void SaveChangeEveryOneElements()
        {
            int count = 0;
            foreach (var item in WrapPanelOtherSettings.Children)
            {
                
                if(item is Everyone)
                {
                    int time = 0;
                    ulong time2 = 2;
                    Everyone everyone = item as Everyone;
                    if(int.TryParse(everyone.TextBoxDate.Text,out time))
                    {
                        settingsProgram.ReplaceValueInListTimes(count, new int[] { time, everyone.TimeCombox.SelectedIndex });
                    }
                    else if (ulong.TryParse(everyone.TextBoxDate.Text, out time2))
                    {
                        MessageBox.Show(time2 + " - слишком много цифарок. Не умею цифарки такие считать.");
                    }
                    count++;
                }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveChangeEveryOneElements();
            int size = -1;
            if (int.TryParse(TextBoxSizeCells.Text, out size))
            {
                settingsProgram.sizeCells = size;
            }
            float strick = -1;
            if (float.TryParse(TextBoxStrokeThickness.Text, out strick))
            {
                settingsProgram.StrokeThickness = strick;
            }
            settingsProgram.FillColorsInPalytre(Palytre2.ColorsInPalyte);
            if (Directory.Exists(SelectedPathBackup.Text))
            {
                settingsProgram.BackupPath = SelectedPathBackup.Text;
            }
            DialogResult = true;
        }

     

        private void Fill_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Shape shape = (Shape)sender;
            shape.Fill = new SolidColorBrush(Palytre2.Draw);
            settingsProgram.ReplaceWindowColorInDictionary((PositionPanel)int.Parse(shape.Tag.ToString()), Palytre2.Draw.ToString());
        }

        private void ReloadColorButton_Click(object sender, RoutedEventArgs e)
        {
            settingsProgram.ClearWindowColorDictionary();
            settingsProgram.InitializeDictionaryWindowColor();
            FillColorsInPicture();
        }

        private void CheckBoxCellActive_Click(object sender, RoutedEventArgs e)
        {
            settingsProgram.isVisibleCells = (bool)CheckBoxCellActive.IsChecked;
        }

        private void BackupPathButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new Forms.FolderBrowserDialog();
            folderBrowser.ShowDialog();
            SelectedPathBackup.Text=folderBrowser.SelectedPath;
           
        }

        private void CheckBoxOpenLastFile_Click(object sender, RoutedEventArgs e)
        {
            settingsProgram.isOpenLastFile = (bool)CheckBoxOpenLastFile.IsChecked;
        }

        private void CheckBoxFullSceen_Click(object sender, RoutedEventArgs e)
        {
            settingsProgram.isFullScreen = (bool)CheckBoxFullSceen.IsChecked;
        }
    }
}
