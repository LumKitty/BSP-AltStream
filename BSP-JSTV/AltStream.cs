using AltStreamLib;
using AltStreamLib.Api;
using AltStreamLib.Api.v1.Models;
using AltStreamLib.EventSub;
using AltStreamLib.EventSub.Chat.Models;
using AltStreamLib.Logging;
using BSP_AltStream.AltStreamEvents;
using CP_SDK.Chat;
using CP_SDK.UI.Modals;
using CP_SDK_WebSocketSharp;
using IPA.Config.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
                AltEventClient.OnFollowReceived += AltEventClient_OnFollowReceived;
                AltEventClient.OnRedemptionReceived += AltEventClient_OnRedemptionReceived;
                AltEventClient.OnUnhandledReceived += AltEventClient_OnUnhandledReceived;
                AltEventClient.OnSubscriptionReceived += AltEventClient_OnSubscriptionReceived;
                AltEventClient.OnTipReceived += AltEventClient_OnTipReceived;

                AltEventClient.OnConnected += AltEventClient_OnConnected;
                AltEventClient.OnDisconnected += AltEventClient_OnDisconnected;
            }

            Log("Connecting to AltStream");
            Plugin.service.m_OnSystemMessageCallbacks?.InvokeAll(Plugin.service, "Connecting to AltStream");
            AltEventClient.ConnectAsync();
        }

        private void AltEventClient_OnTipReceived(Data obj) {
            AltStreamChatUser usr = new AltStreamChatUser(obj.Author);
            //Plugin.service.m_OnChannelBitsCallbacks?.InvokeAll(Plugin.service, chan, usr, obj.Kofi.Amount);
            Plugin.service.m_OnSystemMessageCallbacks.InvokeAll(Plugin.service, $"{obj.FollowedBy.DisplayName} donated {obj.Kofi.CurrencyCode}{obj.Kofi.Amount}");
        }

        private void AltEventClient_OnSubscriptionReceived(Data obj) {
            AltStreamChatUser usr = new AltStreamChatUser(obj.Author);
            AltStreamSubscriptionEvent sub = new AltStreamSubscriptionEvent();
            sub.DisplayName = obj.Author.DisplayName;
            sub.SubPlan = obj.Kofi.TierName;
            Plugin.service.m_OnChannelSubscriptionCallbacks.InvokeAll(Plugin.service, chan, usr, sub);
        }

        private void AltEventClient_OnFollowReceived(Data obj) {
            Log($"* {obj.FollowedBy.DisplayName} followed");
            Plugin.service.m_OnSystemMessageCallbacks.InvokeAll(Plugin.service, $"{obj.FollowedBy.DisplayName} followed");
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
            
            Plugin.service.m_OnSystemMessageCallbacks.InvokeAll(Plugin.service, $"{RaidEvent.From.DisplayName} raided with {RaidEvent.Viewers} viewers");
            //AltStreamChatUser usr = new AltStreamChatUser(obj.Raid.From);
            //Plugin.service.m_OnChannelRaidCallbacks?.InvokeAll(Plugin.service, chan, usr, RaidEvent.Viewers); // IDK why the OnChannelRaidCallback doesn't work!
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
        private void AltEventClient_OnMessageReceived(Data message)       { AltMessageReceived(message); }
        private void AltEventClient_OnHighlightReceived(Data message)     { AltMessageReceived(message, true); }
        private void AltEventClient_OnSystemMessageReceived(Data message) { AltMessageReceived(message, false, true); }

        private void AltEventClient_OnConnected() {
            Connected = true;
            Log($"Connected to channel {ChannelName}");
            Plugin.service.m_OnJoinRoomCallbacks?.InvokeAll(Plugin.service, chan);
        }
        private void AltEventClient_OnDisconnected() {
            Connected = false;
            Log("Disconnected.");
            Plugin.service.m_OnLeaveRoomCallbacks?.InvokeAll(Plugin.service, chan);
        }

        internal static async void SendChatMessage(string Message) {
            try {
                //await AltAPI.SendChatMessageAsync(PluginConfig.Instance.UserName, Message); // AltAPI depends on System.Memory.dll and Beat Saber hates that! >.<
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
