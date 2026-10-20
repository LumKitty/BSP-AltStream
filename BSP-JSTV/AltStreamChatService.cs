using CP_SDK.Chat.Interfaces;
using CP_SDK.Chat.Services;
using CP_SDK.UI.Modals;
using CP_SDK_WebSocketSharp.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using UnityEngine;

namespace BSP_AltStream;

internal class AltStreamChatService : ChatServiceBase, IChatService {
    //WebSocketServer server;
    
    public string DisplayName {get;}= "AltStream";
    public Color AccentColor { get; } = new Color((254/256f), (73/265f), (4/256f));
    internal static AltStreamChatChannel channel = new AltStreamChatChannel(PluginConfig.Instance.UserName);
    internal (IChatService,IChatChannel)[] chans = [];
    public ReadOnlyCollection<(IChatService, IChatChannel)> Channels => chans.ToList().AsReadOnly();
    private Thread EventHandler;
    internal static AltStream objAltStream = new AltStream();

    public bool IsConnectedAndLive() {
        return AltStream.Connected;
    }
    public bool IsInTempChannel(string p_ChannelName) {
        return false;
    }

    public void JoinTempChannel(string p_GroupIdentifier, string p_ChannelName, string p_Prefix, bool p_CanSendMessage) {
        return;
    }

    public void LeaveAllTempChannel(string p_GroupIdentifier) {
        return;
    }

    public void LeaveTempChannel(string p_ChannelName) {
        return;
    }

    public string PrimaryChannelName() {
        return PluginConfig.Instance.UserName;
    }

    public void RecacheEmotes() {
        return;
    }

    public void SendTextMessage(IChatChannel p_Channel, string p_Message) {
        objAltStream.SendChatMessage(p_Message);
    }

    public void Start() {
        try {
            EventHandler = new Thread(new ThreadStart(objAltStream.ConnectWrapper));
            EventHandler.Name = "AltStream_ConnectBot";
            EventHandler.Start();
            
        } catch (Exception ex) {
            Plugin.Log.Error(ex.ToString());
        }
    }

    public void Stop() {
        objAltStream.Disconnect();
        //server.Stop();
    }

    public string WebPageHTML() {
        return "";
    }

    public string WebPageHTMLForm() {
        return "";
    }

    public string WebPageJS() {
        return "";
    }

    public string WebPageJSValidate() {
        return "";
    }

    public void WebPageOnPost(Dictionary<string, string> p_PostData) {
        return;
    }
}
