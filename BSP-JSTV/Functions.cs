using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_AltStream {
    internal static class Functions {
        internal static void Log(string Message) {
            Plugin.Log.Debug(Message);
        }
        internal static void ErrorHandler(Exception ex) {
            Plugin.Log.Error(ex.ToString());
        }
    }
}
