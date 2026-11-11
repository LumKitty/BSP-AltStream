using AltStreamLib;
using AltStreamLib.Api;
using AltStreamLib.Api.v1.Models;
using AltStreamLib.EventSub;
using AltStreamLib.EventSub.Chat.Models;
using AltStreamLib.Logging;
using CP_SDK.Chat;
using CP_SDK.UI.Modals;
using CP_SDK_WebSocketSharp;
using IPA.Config.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Runtime;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BSP_AltStream.Functions;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace BSP_AltStream {
    internal class AltStream {
        private const int ConnectionTimeout = 120;
        private string ChannelName;
        //internal static AltStreamLib.Api.ApiClient AltAPI = new(PluginConfig.Instance.BotToken);
        internal static bool Connected = false;
        internal EventSubClient AltEventClient;

        AltStreamChatChannel chan = new AltStreamChatChannel(PluginConfig.Instance.UserName);
        AltStreamChatEmote FakeEmote = new AltStreamChatEmote("dfdsfadsfdsafdsf", "https://altstream.online/logo_ashe_big.gif", 420, 69);

        internal void Connect() {
            ChannelName = PluginConfig.Instance.UserName;

            if (AltEventClient == null) {
                AltEventClient = new EventSubClient(PluginConfig.Instance.UserName, PluginConfig.Instance.BotToken);
                AltEventClient.OnMessageReceived += AltEventClient_OnMessageReceived;
                AltEventClient.OnHighlightReceived += AltEventClient_OnHighlightReceived;
                AltEventClient.OnSystemMessageReceived += AltEventClient_OnSystemMessageReceived;

                AltEventClient.OnRaidReceived += AltEventClient_OnRaidReceived;
                AltEventClient.OnRedemptionReceived += AltEventClient_OnRedemptionReceived;
                AltEventClient.OnUnhandledReceived += AltEventClient_OnUnhandledReceived;

                AltEventClient.OnConnected += AltEventClient_OnConnected;
                AltEventClient.OnDisconnected += AltEventClient_OnDisconnected;
            }

            Log("Connecting to AltStream");
            Plugin.service.m_OnSystemMessageCallbacks?.InvokeAll(Plugin.service, "Connecting to AltStream");
            AltEventClient.ConnectAsync();
        }

        internal void Disconnect() {
            AltEventClient.DisconnectAsync();
        }

        private void AltEventClient_OnUnhandledReceived(Data obj) {
            Log("Unrecognised Event:");
            Log(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }

        private void AltEventClient_OnRedemptionReceived(Data obj) {
            //AltStreamLib.EventSub.Chat.Models.Events.Redemption RedeemEvent = (AltStreamLib.EventSub.Chat.Models.Events.Redemption)obj.Event;
            Redemption RedeemEvent = obj.Redemption;
            Log($"* {RedeemEvent.From.ChannelName} redeemed \"{RedeemEvent.RewardName}\" for {RedeemEvent.Cost} {RedeemEvent.PointsName}");

            if (String.IsNullOrEmpty(RedeemEvent.UserInput)) {
                Plugin.service.m_OnSystemMessageCallbacks.InvokeAll(Plugin.service, $"{RedeemEvent.From.DisplayName} redeemed {RedeemEvent.RewardName}");
            } else {
                Plugin.service.m_OnSystemMessageCallbacks.InvokeAll(Plugin.service, $"{RedeemEvent.From.DisplayName} redeemed {RedeemEvent.RewardName} ({RedeemEvent.UserInput})");
            }
        }

        private void AltEventClient_OnRaidReceived(Data obj) {
            //AltStreamLib.EventSub.Chat.Models.Events.RaidIncoming RaidEvent = (AltStreamLib.EventSub.Chat.Models.Events.RaidIncoming)obj.Event;
            Raid RaidEvent = obj.Raid;
            
            Log($"* {RaidEvent.From.DisplayName} raided with {RaidEvent.Viewers}");
            AltStreamChatUser usr = new AltStreamChatUser(obj.Raid.From);
            //chan = new AltStreamChatChannel(PluginConfig.Instance.UserName);
            Plugin.service.m_OnChannelRaidCallbacks?.InvokeAll(Plugin.service, chan, usr, RaidEvent.Viewers);
        }

        private void AltMessageReceived(Data Context, bool Highlighted = false, bool SystemMessage = false) {

            try {
                // Convert to BeatSaber message object

                AltStreamChatMessage msg = new AltStreamChatMessage();
                AltStreamChatUser usr = new AltStreamChatUser(Context.Author);

                //Log("Adding Message");
                msg.Message = Context.Message.Text;
                msg.Id = Context.Message.Id;
                //Log("Merging User");
                msg.Sender = usr;
                //Log("Adding Channel");
                msg.Channel = AltStreamChatService.channel;
                msg.IsHighlighted = Highlighted;
                msg.IsSystemMessage = SystemMessage;
                msg.Emotes = [];

                //string TempMessage = " " + msg.Message + " ";
                //Log("Calling ChatPlex");
                Plugin.service.m_OnTextMessageReceivedCallbacks?.InvokeAll(Plugin.service, msg);
            } catch (Exception ex) {
                Log(ex.ToString());
            }
        }
        private void AltEventClient_OnHighlightReceived(Data message) {
            AltMessageReceived(message, true);
        }
        private void AltEventClient_OnMessageReceived(Data message) {
            AltMessageReceived(message);
        }
        private void AltEventClient_OnSystemMessageReceived(Data message) {
            AltMessageReceived(message, false, true);
        }

        private void AltEventClient_OnConnected() {
            Connected = true;
            Log($"Connected to channel {ChannelName}");
            AltStreamChatChannel chan = new AltStreamChatChannel(PluginConfig.Instance.UserName);
            Plugin.service.m_OnJoinRoomCallbacks?.InvokeAll(Plugin.service, chan);
        }
        private void AltEventClient_OnDisconnected() {
            Connected = false;
            Log("Disconnected.");
            AltStreamChatChannel chan = new AltStreamChatChannel(PluginConfig.Instance.UserName);
            Plugin.service.m_OnLeaveRoomCallbacks?.InvokeAll(Plugin.service, chan);
        }

        internal void ConnectWrapper() {
            Connect();
            //while(true) {
            //    Task.Delay(10000);
            //}
        }

        internal static async void SendChatMessage(string Message) {
            try {
                /*
                const string VNyanURL = "ws://localhost:8112/";
                if (wsClient == null) {
                    Log($"Connecting to websocket {VNyanURL}");
                    wsClient = new WatsonWsClient(new Uri(VNyanURL));
                    wsClient.KeepAliveInterval = 1000;
                    wsClient.Start();
                }
                Log($"Sending chat message to Websocket: {Message}");
                await wsClient.SendAsync(Message, WebSocketMessageType.Text, CT);
                */
                //await AltAPI.SendChatMessageAsync(PluginConfig.Instance.UserName, Message);
                JObject JSONMessage = new JObject(
                    new JProperty("channel", PluginConfig.Instance.UserName),
                    new JProperty("message", Message)
                );
                await httpRequest.HttpRequest("POST", "https://altstream.online/api/v2/chat", JSONMessage.ToString(), PluginConfig.Instance.BotToken);

            } catch (Exception Ex) {
                Log("ERROR: " + Ex.ToString());
            }
        }

        internal void SendWhisper(string Message, string UserName) {

        }
    }
}
