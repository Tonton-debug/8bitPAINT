using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using MoreClases;
using System.ServiceModel.Channels;

namespace PaintServiceLibrary
{
    [ServiceBehavior(InstanceContextMode =InstanceContextMode.PerSession)]
    public class PaintService : IPaintService
    {
        public DateTime waitDateTime;
        private string writeChar = "/";
      private  char[] all = new char[]
            {
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
            };

        private char[] all2 = new char[]
        {
                'а','б','в','г','д','е','ё','ж','з','и','й','к','л','м','н','о','п','р','с','т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я'

        };
        private char[] all3 = new char[]
      {
                '`','/','?','.',',',')','(','*','&','^','%','$','#','@','!','|','+','=','-',';','№','"','<','>'
      };
        public byte[] Download8BP()
        {
            return File.ReadAllBytes(ReturnPath() + "paint.zip");
        }

        public int GetAllCountPicture(string category, bool isPng)
        {
            int count = 0;
            foreach (string path in Directory.GetFiles(ReturnPath() + category))
            {
                FileInfo fileInfo = new FileInfo(path);

                count = fileInfo.Extension == ".png" == isPng ? count + 1 : count;
            }
            return count;
        }
        
        public List<byte[]> GetNewPictures(ref List<string> namePictures, ref List<string> idPictures, string category)
        {

            foreach (string path in Directory.GetFiles(ReturnPath() + category +  writeChar))
            {
                FileInfo fileInfo = new FileInfo(path);
                if (!namePictures.Contains(fileInfo.Name) && fileInfo.Extension == ".info")
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    using (FileStream stream = File.OpenRead(path))
                    {
                        SettingsFile settingsFile = binaryFormatter.Deserialize(stream) as SettingsFile;
                      
                        namePictures.Add(fileInfo.Name);
                        idPictures.Add(File.ReadAllText(ReturnPath() + category + writeChar + fileInfo.Name.Replace(".info","") + ".version"));
                        return new List<byte[]>() { File.ReadAllBytes(path.Replace(".info",".gif")), Encoding.UTF8.GetBytes(settingsFile.name), Encoding.UTF8.GetBytes(settingsFile.author), Encoding.UTF8.GetBytes(settingsFile.description), Encoding.UTF8.GetBytes(File.ReadAllText(ReturnPath() + category + writeChar + fileInfo.Name.Replace(".info", "") + ".version")) };
                    }



                }
               
            }
            
            return new List<byte[]>() { new byte[0], new byte[0], new byte[0], new byte[0] };
        }


        public bool HasUpdate(int version)
        {
            return int.Parse(File.ReadAllText(ReturnPath() + "version.txt")) > version;
        }

        public string ReturnPath()
        {
              return "/root/Debug/";
        //    return @"C:\Users\tatar\source\repos\8bitPaint1D\HostPaintService\bin\Debug\";
         //  return "/root/Debug/";
        }
        private bool isIPBan()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            //получаем IP клиента.
            string ipAddr = endpoint.Address;
            foreach (string item in File.ReadAllLines(ReturnPath() +"black_list.txt"))
            {
                if (item == ipAddr)
                {
                    return true;
                }
            }
            return false;
        }
        private void AddIpAddres(string author)
        {
            OperationContext context = OperationContext.Current;
            
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            //получаем IP клиента.
            string ipAddr = endpoint.Address;
            if(!File.ReadAllText(ReturnPath() + "ip_users.txt").Contains(ipAddr))
            {
                
                File.AppendAllText(ReturnPath() + "ip_users.txt", "----------\n" + ipAddr + "\n" + author+"\n----------\n");
            }
            
        }


        List<string> IPaintService.GetAllCategories()
        {
            List<string> get_all_directories = new List<string>();
            foreach (string info in Directory.GetDirectories(ReturnPath()))
            {
                get_all_directories.Add(info.Replace(ReturnPath(), ""));
            }
            return get_all_directories;
        }
        public string GenerateUniqeID()
        {
          
            string get_str = "";
          
            Random random = new Random();
            
            for (int i = 0; i < 15; i++)
            {
                switch (random.Next(0, 4))
                {
                    case 0:
                        get_str += random.Next(0, 2) == 1 ? all[random.Next(0, all.Length)].ToString().ToUpper() : all[random.Next(0, all.Length)].ToString();

                        break;
                    case 1:
                        get_str += random.Next(0, 2) == 1 ? all2[random.Next(0, all2.Length)].ToString().ToUpper() : all2[random.Next(0, all2.Length)].ToString();

                        break;
                    case 2:
                        get_str +=all3[random.Next(0, all3.Length)].ToString();

                        break;
                    case 3:
                        get_str += random.Next(0, 10).ToString();
                        break;

                }
            }
          
            get_str += Convert.ToBase64String(new byte[] { (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255) });
          

            for (int i = 0; i < 6; i++)
            {
                get_str = get_str.Replace(get_str.ElementAt(random.Next(0, get_str.Length)).ToString(), all2[random.Next(0,all2.Length)].ToString());
            }
            
            get_str += "()" + DateTime.Now.ToString()+":"+DateTime.Now.Millisecond;



            return get_str;
        }
        bool IPaintService.HasFile(string category, string name)
        {
            foreach (string files in Directory.GetFiles(ReturnPath() + category + @"\"))
            {
                if (files.Contains(name + ".info"))
                {
                    return true;
                }
            }
            return false;
        }
        //private string GenerateUniqueID()
        //{

        //}
        private bool CheckFile(string uniqueId, string path,string author)
        {
            foreach(string directory in Directory.GetDirectories(path))
            {
                
                foreach (string file in Directory.GetFiles(directory+ writeChar))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension == ".info")
                    {
                        using (FileStream stream = File.OpenRead(file))
                        {
                            SettingsFile settingsFile = binaryFormatter.Deserialize(stream) as SettingsFile;

                            if (settingsFile.unique_id != uniqueId && settingsFile.author == author)
                            {
                                return false;
                            }

                        }
                    }
                }
            }
            return true;
        }
        private bool CheckFile(string uniqueId, string path)
        {
            if (!File.Exists(path))
            {
                return true;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream stream = File.OpenRead(path))
            {
                SettingsFile settingsFile = binaryFormatter.Deserialize(stream) as SettingsFile;
                
                    return settingsFile.unique_id == uniqueId;

            }

        }
        private bool CheckLenght(string author, string dicruption, string filename)
        {
            return author.Length > 23 || dicruption.Length > 97 || filename.Length > 16;
        }
       
        void IPaintService.WritePicture(byte[] getFile, string filename, string author, string dicruption, string category, bool isBin, string uniqueId, int state, ref string info_get)
        {
           
              info_get = "Успешно";
            string end = isBin ? ".gz" : ".info";
            OperationContext context = OperationContext.Current;

            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            //получаем IP клиента.
                string ipAddr = endpoint.Address;
        //    string ipAddr = "addres";
            string path = ReturnPath() + category +  writeChar + filename + end;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            SettingsFile settingsFile = new SettingsFile();
           

           
            
            



            if (!isBin && CheckLenght(author, dicruption, filename))
            {
                info_get = "Слишком много букав";
                return;
            }
            if (File.Exists(ReturnPath() + category +  writeChar + filename + ".info") && state == 0)
            {
                info_get = "Данный файл уже существует. Возможно вы хотите его обновить или удалить?";
                return;
            }
            if (isIPBan())
            {
                info_get = "Ха!Ха!Ха!Вас забанил админ";
                return;
            }
            if (!File.Exists(ReturnPath() + category + writeChar + filename + ".info") && state == 1)
            {
                info_get = "Не удалось найти указанный файл";
                return;
            }
            try
            {
                FileInfo info = new FileInfo(ReturnPath() + ipAddr);
                DateTime dateTimeNow = DateTime.Now;
                if (info.LastWriteTime.AddSeconds(30).Subtract(dateTimeNow)>TimeSpan.Zero)
                {
                    info_get = "Жди \n" + info.LastWriteTime.AddSeconds(30).Subtract(dateTimeNow);
                    return;
                }
                else
                {

                    info.Delete();

                }
            }
            catch
            {
             
            }
            if (CheckFile(uniqueId, ReturnPath() + category + writeChar + filename + ".info") && CheckFile(uniqueId, ReturnPath(), author))
            {
                switch (state)
                {
                    case 0:

                        if (!isBin)
                        {

                            settingsFile.name = filename;
                            settingsFile.author = author;
                            settingsFile.description = dicruption;
                            settingsFile.unique_id = uniqueId;

                            File.WriteAllText(ReturnPath() + category + writeChar + filename + ".version", DateTime.Now.ToString());
                        }
                        using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            if (isBin)
                            {
                                file.Write(getFile, 0, getFile.Length);
                            }
                            else
                            {

                                using (FileStream file2 = new FileStream(ReturnPath() + category + writeChar + filename + ".gif", FileMode.Create, FileAccess.Write))
                                {
                                   if(getFile.Length> 6250000)
                                    {
                                        info_get = "АААА ЗАЧЕМ ТЫ ПЫТАЕШЬСЯ СЛОМАТЬ БД ААААААА.Я СЛИШКОМ СЛАБАЯ,ЧТО БЫ СОХРАНИТЬ ПНГ С РАЗМЕРОМ БОЛЬШЕ 50 МЕГАБАЙТ!!! ";
                                        return;

                                    }
                                    file2.Write(getFile, 0, getFile.Length);
                                }
                                binaryFormatter.Serialize(file, settingsFile);
                                
                            }

                        }

                        break;
                    case 1:
                    case 3:


                        if (!isBin&&state!=3)
                        {
                            using (FileStream file2 = new FileStream(ReturnPath() + category + writeChar + filename + ".gif", FileMode.Create, FileAccess.Write))
                            {
                                if (getFile.Length > 6250000)
                                {
                                    info_get = "АААА ЗАЧЕМ ТЫ ПЫТАЕШЬСЯ СЛОМАТЬ БД ААААААА.Я СЛИШКОМ СЛАБАЯ,ЧТО БЫ СОХРАНИТЬ ПНГ С РАЗМЕРОМ БОЛЬШЕ 50 МЕГАБАЙТ!!! ";
                                    return;

                                }
                                file2.Write(getFile, 0, getFile.Length);
                            }


                        }
                        using (FileStream file12 = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            if (isBin && state != 3)
                            {
                                file12.Write(getFile, 0, getFile.Length);
                            }
                            else
                            {

                                settingsFile.name = filename;
                                settingsFile.author = author;
                                settingsFile.description = dicruption;
                                settingsFile.unique_id = uniqueId;
                                File.WriteAllText(ReturnPath() + category + writeChar + filename + ".version", DateTime.Now.ToString());
                                binaryFormatter.Serialize(file12, settingsFile);
                                
                            }

                        }


                        break;
                    case 2:
                        FileInfo file3 = new FileInfo(path);
                        if (file3.Extension == ".info")
                        {

                            File.Delete(ReturnPath() + category + writeChar + filename + ".gif");
                            File.Delete(ReturnPath() + category + writeChar + filename + ".version");

                        }
                        File.Delete(path);

                        break;

                }
                if (!isBin)
                {
                    AddIpAddres(author);
                }
                if (!File.Exists(ReturnPath() + ipAddr)&&!isBin&&state!=2)
                {
                    File.Create(ReturnPath() + ipAddr);
                }
            }
            else
            {
                info_get = "Неверный айди";
            }
        }

        public bool IsDeletePictures(string namePicture, string category)
        {
            return !File.Exists(ReturnPath() + category +  writeChar + namePicture+ ".info");

           
        }

        public List<string> GetAllMyFiles(string category, string uniqueId)
        {
            FileInfo fileInfo;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            List<string> get_all_files=new List<string>();
            foreach(string file in Directory.GetFiles(ReturnPath() + category + writeChar))
            {
                 fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".info")
                {
                    using (FileStream stream = File.OpenRead(file))
                    {
                        SettingsFile settingsFile = binaryFormatter.Deserialize(stream) as SettingsFile;

                        if (settingsFile.unique_id == uniqueId)
                        {
                            get_all_files.Add(settingsFile.name);
                        }

                    }
                }
            }
            return get_all_files;
        }

        public List<string> GetInfoMyFile(string category, string nameFile)
        {
            FileInfo fileInfo;
            string path = ReturnPath() + category + writeChar + nameFile + ".info";
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            List<string> get_all_files = new List<string>();

            fileInfo = new FileInfo(path);

            using (FileStream stream = File.OpenRead(path))
            {
                SettingsFile settingsFile = binaryFormatter.Deserialize(stream) as SettingsFile;


                get_all_files.Add(settingsFile.author);
                get_all_files.Add(settingsFile.description);
            }
            return get_all_files;
        }
        public byte[] DownloadFile(string name,string category)
        {
            return File.ReadAllBytes(ReturnPath() + category + writeChar + name + ".gz");
        }

        public List<byte[]> GetUpdatePicture(string version, string name, string category)
        {
            string new_version = File.ReadAllText(ReturnPath() + category + writeChar + name + ".version");
            if (new_version != version)
            {
              
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                string path = ReturnPath() + category + writeChar + name + ".info";
                using (FileStream stream = File.OpenRead(path))
                {
                    SettingsFile settingsFile = binaryFormatter.Deserialize(stream) as SettingsFile;


                    return new List<byte[]>() { File.ReadAllBytes(path.Replace(".info", ".gif")), Encoding.UTF8.GetBytes(settingsFile.name), Encoding.UTF8.GetBytes(settingsFile.author), Encoding.UTF8.GetBytes(settingsFile.description), Encoding.UTF8.GetBytes(new_version) };

                }
            }
            return new List<byte[]>()
            {
                new byte[0]
            };
            }

        public string[] GetInfoUpdatesText()
        {
            return File.ReadAllLines(ReturnPath() + "updates.txt");
        }

        public string[] GetInfoNewsText()
        {
            return File.ReadAllLines(ReturnPath() + "news.txt");
        }
    }
}

