using System.Runtime.Remoting.Messaging;
using AltStreamLib.EventSub.Chat.Models;
using CP_SDK.Chat.Interfaces;

namespace BSP_AltStream;

internal class AltStreamChatUser : IChatUser {
    public string Id { get; internal set; }
    public string UserName {get; internal set;}
    public string DisplayName {get; internal set;}
    public string PaintedName => UserName;
    public string Color { get; internal set; }
    public bool IsBroadcaster { get; internal set; } = false;
    public bool IsModerator { get; internal set; } = false;
    public bool IsSubscriber { get; internal set; } = false;
    public bool IsVip { get; internal set; } = false;
    public IChatBadge[] Badges => [];

    internal AltStreamChatUser(Author User) {
        Id = User.Id;
        UserName = User.ChannelName;
        DisplayName = User.DisplayName;
        if (User.Color != null) {
            Color = User.Color;
        } else {
            Color = "#FE4904";
        }
        IsBroadcaster = User.Roles.Broadcaster;
        IsModerator = User.Roles.Moderator;
        IsSubscriber = User.Roles.ChannelSubscriber;
    }
}