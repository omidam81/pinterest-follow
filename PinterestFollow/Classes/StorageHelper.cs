using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PinterestFollow.Classes
{
    public class StorageHelper
    {
        public static List<Models.User> LoadUsers()
        {
            string str = LoadUserFileContent();
            if (string.IsNullOrWhiteSpace(str)) return new List<Models.User>();
            return new List<Models.User>(Newtonsoft.Json.JsonConvert.DeserializeObject<Models.User[]>(str));

        }



        public static void SaveUsers(List<Models.User> list)
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            SaveUserFileContent(str);
        }


        private static string LoadUserFileContent()
        {
            try
            {
                string filePath = Application.ExecutablePath;
                filePath = new System.IO.FileInfo(filePath).DirectoryName;
                filePath = string.Join("/", filePath, AppSetting.DirectoryPath);
                filePath = string.Join("/", filePath, "Users.data");
                return System.IO.File.ReadAllText(filePath);
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static void SaveUserFileContent(string str)
        {
            try
            {
                string filePath = Application.ExecutablePath;
                
                filePath = new System.IO.FileInfo(filePath).DirectoryName;

                filePath = string.Join("/", filePath, AppSetting.DirectoryPath);

                if (!System.IO.Directory.Exists(filePath))
                    System.IO.Directory.CreateDirectory(filePath);

                filePath = string.Join("/", filePath, "Users.data");

                System.IO.File.WriteAllText(filePath, str);
            }
            catch (Exception)
            {
            }
        }
    }
}
