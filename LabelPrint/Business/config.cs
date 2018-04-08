using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Business
{
    static class Config
    {
        public static string API_URL
        {
            get
            {
                return GetConfig("API_URL");
            }
            set { SaveConfig("API_URL", value); }
        }

        public static string API_KEY
        {
            get
            {
                return GetConfig("API_KEY");
            }
            set { SaveConfig("API_KEY", value); }
        }

        public static string MQTT_SERVER
        {
            get
            {
                return GetConfig("MQTT_SERVER");
            }
            set { SaveConfig("MQTT_SERVER", value); }
        }

        public static string MQTT_TOPIC_INLKNK
        {
            get
            {
                return GetConfig("MQTT_TOPIC_INLKNK");
            }
            set { SaveConfig("MQTT_TOPIC_INLKNK", value); }
        }


        public static string PRINTER_ID
        {
            get
            {
                return GetConfig("PRINTER_ID");
            }
        }


        public static string MQTT_BASE_TOPIC
        {
            get
            {
                return GetConfig("MQTT_BASE_TOPIC");
            }
        }

        public static string LatestDbSyncTime
        {
            get
            {
                return GetConfig("LatestDbSyncTime");
            }
            set { SaveConfig("LatestDbSyncTime", value); }
        }

        public static string DbSyncInterval
        {
            get
            {
                return GetConfig("DbSyncInterval");
            }
            set { SaveConfig("DbSyncInterval", value); }
        }

        public static bool AllowDbSync
        {
            get
            {
                return Convert.ToBoolean(GetConfig("AllowDbSync"));
            }
            set { SaveConfig("AllowDbSync", value.ToString()); }
        }

        public static string PackReserveId
        {
            get { return GetConfig("PackReserveId"); }
            set { SaveConfig("PackReserveId", value); }
        }

        public static string LotReserveId
        {
            get { return GetConfig("LotReserveId"); }
            set { SaveConfig("LotReserveId", value); }
        }

        public class IntemCuon
        {

            public static string FileReport
            {
                get
                {
                    return GetConfig("IntemCuon_FileReport");
                }
            }
        }

        public class IntemThungCuon
        {

            public static string FileReport
            {
                get
                {
                    return GetConfig("IntemThungCuon_FileReport");
                }
            }
        }

        public class InTemMACIMEI
        {
            public static string FileReport
            {
                get
                {
                    return GetConfig("InTemMACIMEI_FileReport");
                }
            }
        }

        public class InTemSeriNo
        {
            public static string FileReport
            {
                get
                {
                    return GetConfig("InTemSeriNo_FileReport");
                }
            }
        }

        public class InTemThanhPham
        {
            public static string FileReport
            {
                get
                {
                    return GetConfig("InTemThanhPham_FileReport");
                }
            }
        }

        public class InTemThung
        {
            public static string FileReport
            {
                get
                {
                    return GetConfig("InTemThung_FileReport");
                }
            }
        }

        public class InTemViTri
        {
            public static string FileReport
            {
                get
                {
                    return GetConfig("InTemViTri_FileReport");
                }
            }
        }

        public class Sync
        {
            public static string Latest_ID
            {
                get
                {
                    return GetConfig("Latest_ID");
                }
                set
                {
                    WriteConfig("Latest_ID", value);
                }
            }

        }

        private static void SaveConfig(string Key, string Value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //make changes
            config.AppSettings.Settings[Key].Value = Value;

            //save to apply changes
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static void WriteConfig(string Key, string Value)
        {
            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string configFile = System.IO.Path.Combine(appPath, GetConfig("AppConfig"));
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            config.AppSettings.Settings[Key].Value = Value;
            config.Save();
        }

        public static string GetConfig(string Key)
        {
            return ConfigurationManager.AppSettings[Key];
        }
    }
}
