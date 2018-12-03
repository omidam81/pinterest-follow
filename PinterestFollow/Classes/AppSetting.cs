using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinterestFollow.Classes
{
    public class AppSetting
    {
        public static string DirectoryPath { get { return GetKeyValue("FilePath"); } set { SetKayValue("FilePath", value); } }

        public static int MinDelay
        {
            get { return Convert.ToInt32(GetKeyValue("minTime")); }
            set { SetKayValue("minTime", value.ToString()); }
        }

        public static int TotalHours
        {
            get { return Convert.ToInt32(GetKeyValue("TotalHours")); }
            set { SetKayValue("TotalHours", value.ToString()); }
        }

        public static int MaxDelay
        {
            get { return Convert.ToInt32(GetKeyValue("maxTime")); }
            set { SetKayValue("maxTime", value.ToString()); }
        }
        public static bool AutoSatrt
        {
            get { return Convert.ToBoolean(GetKeyValue("autostart")); }
            set { SetKayValue("autostart", value.ToString()); }
        }
        private static string GetKeyValue(string name)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return configuration.AppSettings.Settings[name].Value;
        }
        private static void SetKayValue(string name, string val)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[name].Value = val;
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");

        }

        public static int ThreadCount
        {
            get { return Convert.ToInt32(GetKeyValue("ThreadCount")); }
            set { SetKayValue("ThreadCount", value.ToString()); }
        }



        public static int MinF
        {
            get { return Convert.ToInt32(GetKeyValue("MinF")); }
            set { SetKayValue("MinF", value.ToString()); }
        }

        public static int MaxF
        {
            get { return Convert.ToInt32(GetKeyValue("MaxF")); }
            set { SetKayValue("MaxF", value.ToString()); }
        }
        public static int MinU
        {
            get { return Convert.ToInt32(GetKeyValue("MinU")); }
            set { SetKayValue("MinU", value.ToString()); }
        }

        public static int MaxU
        {
            get { return Convert.ToInt32(GetKeyValue("MaxU")); }
            set { SetKayValue("MaxU", value.ToString()); }
        }
    }
}
