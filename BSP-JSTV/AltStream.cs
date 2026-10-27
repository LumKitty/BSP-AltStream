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
using WatsonWebsocket;

namespace BSP_AltStream {
    internal class AltStream {
        private const int ConnectionTimeout = 120;
        private string ChannelName;
        internal AltStreamLib.Api.ApiClient AltAPI = new();
        internal static bool Connected = false;
        internal EventSubClient AltEventClient;

        AltStreamChatMessage msg = new AltStreamChatMessage();
        AltStreamChatUser usr = new AltStreamChatUser();
        AltStreamChatChannel chan = new AltStreamChatChannel(PluginConfig.Instance.UserName);
        AltStreamChatEmote FakeEmote = new AltStreamChatEmote("dfdsfadsfdsafdsf", "https://altstream.online/logo_ashe_big.gif", 420, 69);

        // Remove once AltStream has proper chat sending
        static WatsonWsClient wsClient = null;
        static System.Threading.CancellationToken CT = new System.Threading.CancellationToken();

        internal void Connect() {
            ChannelName = PluginConfig.Instance.UserName;

            if (AltEventClient == null) {
                AltEventClient = new EventSubClient(ChannelName);
                AltEventClient.OnMessageReceived += AltEventClient_OnMessageReceived;
                AltEventClient.OnHighlightReceived += AltEventClient_OnHighlightReceived;

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

        private void AltEventClient_OnUnhandledReceived(EventMessage obj) {
            Log("Unrecognised Event:");
            Log(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }

        private void AltEventClient_OnRedemptionReceived(EventMessage obj) {
            AltStreamLib.EventSub.Chat.Models.Events.Redemption RedeemEvent = (AltStreamLib.EventSub.Chat.Models.Events.Redemption)obj.Event;
            Log($"* {RedeemEvent.From} redeemed \"{RedeemEvent.RewardName}\" for {RedeemEvent.Cost} {RedeemEvent.PointsName}");

            if (String.IsNullOrEmpty(RedeemEvent.UserInput)) {
                Plugin.service.m_OnSystemMessageCallbacks.InvokeAll(Plugin.service, $"{RedeemEvent.From} redeemed {RedeemEvent.RewardName}");
            } else {
                Plugin.service.m_OnSystemMessageCallbacks.InvokeAll(Plugin.service, $"{RedeemEvent.From} redeemed {RedeemEvent.RewardName} ({RedeemEvent.UserInput})");
            }
        }

        private void AltEventClient_OnRaidReceived(EventMessage obj) {
            AltStreamLib.EventSub.Chat.Models.Events.RaidIncoming RaidEvent = (AltStreamLib.EventSub.Chat.Models.Events.RaidIncoming)obj.Event;
            Log($"* {RaidEvent.SourceUsername} raided with {RaidEvent.Viewers}");
            usr = new AltStreamChatUser();
            chan = new AltStreamChatChannel(PluginConfig.Instance.UserName);
            usr.UserName = RaidEvent.SourceUsername;
            Plugin.service.m_OnChannelRaidCallbacks?.InvokeAll(Plugin.service, chan, usr, RaidEvent.Viewers);
        }

        private void AltMessageReceived(AltStreamLib.EventSub.Chat.Models.Message Context, bool Highlighted = false) {

            try {
                // Convert to BeatSaber message object

                msg = new AltStreamChatMessage();
                usr = new AltStreamChatUser();

                //Log("Adding Username");
                usr.UserName = Context.AuthorUsername;
                usr.DisplayName = Context.Author;
                usr.IsSubscriber = false;
                usr.IsBroadcaster = false;
                usr.IsModerator = false;

                //Log("Adding Message");
                msg.Message = Context.Text;
                msg.Id = Context.Id;
                //Log("Merging User");
                msg.Sender = usr;
                //Log("Adding Channel");
                msg.Channel = AltStreamChatService.channel;
                msg.IsHighlighted = Highlighted;
                msg.Emotes = [];

                //string TempMessage = " " + msg.Message + " ";
                //Log("Calling ChatPlex");
                Plugin.service.m_OnTextMessageReceivedCallbacks?.InvokeAll(Plugin.service, msg);
            } catch (Exception ex) {
                Log(ex.ToString());
            }
        }
        private void AltEventClient_OnHighlightReceived(EventMessage obj) {
            AltMessageReceived(obj.Context, true);
        }
        private void AltEventClient_OnMessageReceived(AltStreamLib.EventSub.Chat.Models.Message message) {
            AltMessageReceived(message);
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
                const string VNyanURL = "ws://localhost:8112/";
                if (wsClient == null) {
                    Log($"Connecting to websocket {VNyanURL}");
                    wsClient = new WatsonWsClient(new Uri(VNyanURL));
                    wsClient.KeepAliveInterval = 1000;
                    wsClient.Start();
                }
                Log($"Sending chat message to Websocket: {Message}");
                await wsClient.SendAsync(Message, WebSocketMessageType.Text, CT);
            } catch (Exception Ex) {
                Log("ERROR: " + Ex.ToString());
            }
        }

        internal void SendWhisper(string Message, string UserName) {

        }
    }
}
