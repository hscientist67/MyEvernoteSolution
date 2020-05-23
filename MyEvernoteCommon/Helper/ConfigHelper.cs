using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernoteCommon.Helper
{
    public class ConfigHelper
    {
        public static T Get<T>(string key)
        {
            //Burada generic bir metot oluşturduk. Biz ne tipde bu metodu gönderirsek o da bize o tipde geri döndürecek.
            return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
        }
    }
}
