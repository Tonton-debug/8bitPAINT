using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PaintServiceLibrary
{
   
    [ServiceContract]
    public interface IPaintService
    {
       
        [OperationContract]
        string ReturnPath();
        [OperationContract]
        bool HasUpdate(int version);
        [OperationContract]
        int GetAllCountPicture(string category, bool isPng);
        [OperationContract]
        byte[] Download8BP();
        [OperationContract]
        List<string> GetInfoMyFile(string category, string name);
        [OperationContract]
        List<string> GetAllMyFiles(string category, string uniqueId);
        [OperationContract]
        List<byte[]> GetNewPictures(ref List<string> namePictures, ref List<string> idPictures, string category);
        [OperationContract]
        bool IsDeletePictures(string namePictures, string category);
        [OperationContract]
        string[] GetInfoUpdatesText();
        [OperationContract]
        string[] GetInfoNewsText();
        [OperationContract]
        List<string> GetAllCategories();
        [OperationContract]
        byte[] DownloadFile(string name, string category);
        [OperationContract]
        List<byte[]> GetUpdatePicture(string version, string name, string category);
        [OperationContract]
        string GenerateUniqeID();
        [OperationContract]
        void WritePicture(byte[] getFile, string filename, string author, string dicruption, string category, bool isBin, string uniqueId, int state, ref string info_get);
        [OperationContract]
        bool HasFile(string category, string name);

    }
   
}
