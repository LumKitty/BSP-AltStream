using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_AltStream {
    public class PluginConfig {
        public static PluginConfig Instance { get; set; }

        public string UserName { get; set; }

        public virtual void Changed() { }
        public virtual void OnReload() { }
    }
}
