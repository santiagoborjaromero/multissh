using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTSSH
{
    [Serializable]
    class Settings
    {
        public string host; /*{ get; set; }*/
        public string port; /*{ get; set; }*/
        public string user; /*{ get; set; }*/
        public string pass; /*{ get; set; }*/

        /*
                public Settings() { }

                public Settings(string ohost, string oport, string ouser, string opass)
                {
                    host = ohost;
                    port = oport;
                    user = ouser;
                    pass = opass;
                }
          */
    }
}
