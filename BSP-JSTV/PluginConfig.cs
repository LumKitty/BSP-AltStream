namespace BSP_AltStream {
    public class PluginConfig {
        public static PluginConfig Instance { get; set; }

        public string UserName;
        public string BotToken; 

        public virtual void Changed() { }
        public virtual void OnReload() { }
    }
}
