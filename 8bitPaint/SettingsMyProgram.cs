using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _8bitPaint
{
    public enum PositionPanel
    {
        Left,
        Right,
        Up,
        Center,
        Frame,
        Layer,
        ActiveLayer,
        ActiveFrame,
        LeftBD,
        UpBD,
        CenterBD,
        PictureBD
    }
    [Serializable]
   public class SettingsMyProgram
    {
        public string MyIDInBD { get; set; }
        public string BackupPath { get; set; }
        public string lastFilePath { get; set; }
        public bool isVisibleCells { get; set; }
        public int sizeCells { get; set; }
        public bool isOpenLastFile { get; set; }
        public bool isFullScreen { get; set; }
        public float StrokeThickness { get; set; }
        private Dictionary<PositionPanel, string> DictionaryWindowColor = new Dictionary<PositionPanel, string>();
        private  Dictionary<string, string[]> DictionaryKeysMetod = new Dictionary<string, string[]>();
        private List<int[]> ListTimes = new List<int[]>();
        private List<string> ColorsInPalytre = new List<string>();
        private List<DateTime> datesUpdates = new List<DateTime>();
        
        [NonSerialized]
        private MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        public SettingsMyProgram(string standartPath)
        {
            InitializeDictionaryKeysMetod();
            InitializeDictionaryWindowColor();
            InitializeListTime();
            sizeCells = 1;
            StrokeThickness = 0.05f;
            lastFilePath = "";
            BackupPath= standartPath+@"Backup\";
            isOpenLastFile = true;
        }
       public  DateTime GetDate(int id,bool relplace=false)
        {
            datesUpdates[id] = relplace? DateTime.Now.AddSeconds(ListTimes[id][0]): datesUpdates[id];
            return datesUpdates[id];
        }
        public void FillColorsInPalytre(List<string> colors)
        {
            ColorsInPalytre = new List<string>();
            colors.CopyTo(ColorsInPalytre.ToArray());
           
        }
        public void FillColorsInPalytre(List<Color> colors)
        {
            ColorsInPalytre = new List<string>();
            foreach (Color color in colors)
            {
                ColorsInPalytre.Add(color.ToString());
            }
          

        }
        public List<string> ReturnColorsInPalytre()
        {
            return ColorsInPalytre;
        }
      
        public int[] GetValueInListTimes(int position)
        {
            return ListTimes[position];
        }
        private int[] ConvertSecondsToTime(int[] get)
        {
            switch (get[1])
            {

                case 0:
                    get[0] = get[0];
                    break;
                case 1:
                    get[0] = get[0] * 60;
                    break;
                case 2:
                    get[0] = get[0] * 3600;
                    break;
            }
            return get;
        }
        public void ReplaceValueInListTimes(int position, int[] get)
        {
            ListTimes[position] = ConvertSecondsToTime(get);
            datesUpdates[position] = DateTime.Now.AddSeconds(ListTimes[position][0]);

           
          
        }
       
            public void AdddToListTimes(int[] get)
        {
          ListTimes.Add(ConvertSecondsToTime(get));
            datesUpdates.Add(DateTime.Now.AddSeconds(get[0]));
        }
        public string GetValueInDictionaryWindowColor(PositionPanel position)
        {
            return DictionaryWindowColor[position];
        }
        public void AdddMetodToDictionaryWindowColor(PositionPanel positionPanel,string color)
        {
            DictionaryWindowColor.Add(positionPanel, color);
        }
        public void InitializeListTime()
        {
            AdddToListTimes(new int[] { 1, 0 });
            AdddToListTimes(new int[] { 1, 1 });
          
        }
        public void InitializeDictionaryWindowColor()
        {
            AdddMetodToDictionaryWindowColor(PositionPanel.Frame, "#70F0D7C0");
            AdddMetodToDictionaryWindowColor(PositionPanel.Right, "#FFFAEBD7");
            AdddMetodToDictionaryWindowColor(PositionPanel.Left, "#FFFAEBD7");
            AdddMetodToDictionaryWindowColor(PositionPanel.Layer, "#70F0D7C0");
            AdddMetodToDictionaryWindowColor(PositionPanel.Up, "#FFFAEBD7");
            AdddMetodToDictionaryWindowColor(PositionPanel.Center, "#FFA9A9A9");
            AdddMetodToDictionaryWindowColor(PositionPanel.ActiveFrame, "#80FFF7C8");
            AdddMetodToDictionaryWindowColor(PositionPanel.ActiveLayer, "#80FFF7C8");
            AdddMetodToDictionaryWindowColor(PositionPanel.UpBD, "#FFFAEBD7");
            AdddMetodToDictionaryWindowColor(PositionPanel.CenterBD, "#FFFFF4E5");
            AdddMetodToDictionaryWindowColor(PositionPanel.LeftBD, "#FFFAEBD7");
            AdddMetodToDictionaryWindowColor(PositionPanel.PictureBD, "#FFFBDEB9");

        }
        public void InitializeDictionaryKeysMetod()
        {
            AddMetodToDictionaryKeysMetod("movePixelsUp",new string[] { Key.Up.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("movePixelsDown", new string[] { Key.Down.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("movePixelsLeft", new string[] { Key.Left.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("movePixelsRight", new string[] { Key.Right.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("select0", new string[] { Key.D1.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("select1", new string[] { Key.D2.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("select2", new string[] { Key.D3.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("select3", new string[] { Key.D4.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("select4", new string[] { Key.D5.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("select5", new string[] { Key.D6.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("delete", new string[] { Key.D.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("clear", new string[] { Key.R.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("selectObl", new string[] { Key.LeftShift.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("save", new string[] { Key.S.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("load", new string[] { Key.L.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("create", new string[] { Key.C.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("undo", new string[] { Key.LeftCtrl.ToString(),Key.Z.ToString() });
            AddMetodToDictionaryKeysMetod("apply", new string[] { Key.LeftCtrl.ToString(),Key.Y.ToString() });
            AddMetodToDictionaryKeysMetod("coppy", new string[] { Key.LeftCtrl.ToString(), Key.C.ToString() });
            AddMetodToDictionaryKeysMetod("cut", new string[] { Key.LeftCtrl.ToString(), Key.X.ToString() });
            AddMetodToDictionaryKeysMetod("paste", new string[] { Key.LeftCtrl.ToString(), Key.V.ToString() });
            AddMetodToDictionaryKeysMetod("selectFrame", new string[] { Key.M.ToString(), "..." });
            AddMetodToDictionaryKeysMetod("selectLayer", new string[] { Key.N.ToString(), "..." });


        }
        public void ReplaceWindowColorInDictionary(PositionPanel positionPanel, string color)
        {
            DictionaryWindowColor[positionPanel] = color;
        }
       
        public void AddMetodToDictionaryKeysMetod(string method, string[] keys)
        {
            DictionaryKeysMetod.Add(method,keys);
        }
        public void ClearKeyDictionary()
        {
            DictionaryKeysMetod.Clear();
        }
        public void ClearWindowColorDictionary()
        {
            DictionaryWindowColor.Clear();
        }
        public void ReplaceKeyMetodToDictionary(string method,string[] keys)
        {        
                    DictionaryKeysMetod[method] = keys;
        }
        public List<string[]> GetValuesKeysMetod()
        {
            return DictionaryKeysMetod.Values.ToList();
        }
        public bool isNull()
        {
            return DictionaryKeysMetod.Count == 0;
        }
        public bool HasCloneKey(string[] keys)
        {
            int id = 0;
           foreach(var get_key in DictionaryKeysMetod.Values)
            {
                if ((get_key[0]==keys[0]&& get_key[1] == keys[1])|| (get_key[0] == keys[1] && get_key[1] == keys[0]) || (get_key[1] == keys[0] && get_key[0] == keys[1]))
                {
                    if (id == 0)
                    {
                        id = 1;
                        continue;
                    }
                    return true;
                }
                
                
            }
          
            return false;
        }
        private void Run1KeyMetod()
        {

        }
        public void RunMetod()
        {
            KeyConverter keyConverter = new KeyConverter();
            int count = 0;
            KeyValuePair<string, string[]> GetMethod = new KeyValuePair<string, string[]>();
            foreach (var item in DictionaryKeysMetod.Values)
            {

                if (item[0] == "...")
                {
                    if (Keyboard.IsKeyDown((Key)keyConverter.ConvertFromString(item[1])))
                    {
                        GetMethod = DictionaryKeysMetod.ElementAt(count);
                        
                    }

                }
                else if (item[1] == "...")
                {
                    if (Keyboard.IsKeyDown((Key)keyConverter.ConvertFromString(item[0])))
                    {
                        GetMethod = DictionaryKeysMetod.ElementAt(count);
                       
                    }
                }
                else
                {
                    if (Keyboard.IsKeyDown((Key)keyConverter.ConvertFromString(item[0])) && Keyboard.IsKeyDown((Key)keyConverter.ConvertFromString(item[1])))
                    {
                        GetMethod = DictionaryKeysMetod.ElementAt(count);
                        break;
                       
                    }
                }
                count++;

            }
            mainWindow = mainWindow == null ? Application.Current.MainWindow as MainWindow : mainWindow;
            switch (GetMethod.Key)
            {
                case "movePixelsUp":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.MovePixels(0,-1));
                    break;
                case "movePixelsLeft":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.MovePixels(-1, 0));
                    break;
                case "movePixelsRight":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.MovePixels(1, 0));
                    break;
                case "movePixelsDown":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.MovePixels(0, 1));
                    break;
                case "save":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventSaveFile());
                    break;
                case "delete":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventDeleteFile());
                    break;
                case "clear":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventClearActiveLayer());
                    break;
                case "create":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventCreateFile());
                    break;
                case "undo":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventUndo());
                    break;
                case "apply":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventUp());
                    break;
                case "coppy":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventCoppy());
                    break;
                case "paste":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventPaste());
                    break;
                case "cut":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventCut());
                    break;
                case "selectFrame":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventActiveVisibleLayerOrFrame(true));
                    break;
                case "selectLayer":
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SendEventActiveVisibleLayerOrFrame(false));
                    break;
                case "select0":
                case "select1":
                case "select2":
                case "select3":
                case "select4":
                case "select5":
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                     
                        int tag =int.Parse(GetMethod.Key.Replace("select", ""));
                        switch (tag)
                        {
                            case 0:
                                mainWindow.ClickInstruments(mainWindow.pencil, null);
                                break;
                            case 1:
                                mainWindow.ClickInstruments(mainWindow.erarce, null);
                                break;
                            case 2:
                                mainWindow.ClickInstruments(mainWindow.fill, null);
                                break;
                            case 3:
                                mainWindow.ClickInstruments(mainWindow.hand, null);
                                break;
                            case 4:
                                mainWindow.ClickInstruments(mainWindow.dropper, null);
                                break;
                            case 5:
                                mainWindow.ClickInstruments(mainWindow.selected, null);
                                break;
                        }
                       
                    }
                    );
                    break;
            }

        }
    }
}
