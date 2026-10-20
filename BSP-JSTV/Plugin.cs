using CP_SDK_WebSocketSharp;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using System.Threading;
using System.Threading.Tasks;
using IpaLogger = IPA.Logging.Logger;


namespace BSP_AltStream;

[Plugin(RuntimeOptions.SingleStartInit)]
internal class Plugin {
    internal static IpaLogger Log { get; private set; } = null!;
    internal static Config cfg { get; private set; }
    internal static AltStreamChatService service;
    internal static Thread AuthoriseUserThread;

    [Init]
    public Plugin(IpaLogger ipaLogger,Config config, PluginMetadata pluginMetadata) {
        Log = ipaLogger;
        PluginConfig.Instance = config.Generated<PluginConfig>();
        Log.Info($"{pluginMetadata.Name} {pluginMetadata.HVersion} initialized.");
    }
        
    [OnStart]
    public void OnApplicationStart() {
        Log.Debug("OnApplicationStart");
        PluginConfig.Instance.UserName = "LumKitty";
        if (!string.IsNullOrEmpty(PluginConfig.Instance.UserName)) {
            Log.Debug("Create chat service");
            service = new AltStreamChatService();
            Log.Debug("Register chat service");
            CP_SDK.Chat.Service.RegisterExternalService(service);
        } else {
            Log.Error("Username not configured in BSP-AltStream.json");
        }
    }

    [OnExit]
    public void OnApplicationQuit() {
        Log.Debug("OnApplicationQuit");
    }
}
