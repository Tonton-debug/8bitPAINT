using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using PaintServiceLibrary;
namespace HostPaintService
{
    class Program
    {
        private static readonly int version = 24;
        static void Main(string[] args)
        {
          
            bool end = false;
            Console.WriteLine(Directory.GetCurrentDirectory() + "/");
          
            Console.WriteLine("WCF Host!\n version:"+version+"\nend\nban\nunban\nlist_ban\nlist_ip");
            
           
            ServiceHost host = new ServiceHost(typeof(PaintService));

            host.Open();
            while (!end)
            {
                switch (Console.ReadLine())
                {
                    case "end":
                        host.Close();
                        end = true;
                        break;
                    case "ban":
                        Console.WriteLine("Write ip:");
                        File.AppendAllText("/root/Debug/black_list.txt", Console.ReadLine());
                        break;
                    case "unban":
                        Console.WriteLine("Write ip:");
                        File.WriteAllText("/root/Debug/black_list.txt", File.ReadAllText("/root/Debug/black_list.txt").Replace(Console.ReadLine(),""));
                        break;
                    case "list_ban":
                        foreach (var item in File.ReadAllLines("/root/Debug/black_list.txt"))
                        {
                            Console.WriteLine(item);
                        }
                        break;
                    case "list_ip":
                        foreach (var item in File.ReadAllLines("/root/Debug/ip_users.txt"))
                        {
                            Console.WriteLine(item);
                        }
                       
                        break;
                }
                
            }



        }
       
        
    }
}
