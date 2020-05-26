using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MTSSH
{
    class Util
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Log(string area, string txt)
        {
            string text = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " " + area + " - " + txt + "\n";
            System.IO.File.AppendAllText(@"" + InternalConfig.path + @"\" + InternalConfig.file_log, text);
        }

        public static string ReadLog()
        {
            StreamReader r;
            string text = "";
            string file = @"" + InternalConfig.path + @"\" + InternalConfig.file_log;
            if (File.Exists(file))
            {
                //text = System.IO.File.ReadAllText(file);
                using (r = new StreamReader(file)) { text = r.ReadToEnd(); }
            }
            return text;

        }

        public static string ReadConfig()
        {
            StreamReader r;
            string text = "";
            string file = @"" + InternalConfig.path + @"\" + InternalConfig.file_config;
            if (File.Exists(file))
            {
                using (r = new StreamReader(file)) { text = r.ReadToEnd(); }
            }
            return text;

        }
    }
}
