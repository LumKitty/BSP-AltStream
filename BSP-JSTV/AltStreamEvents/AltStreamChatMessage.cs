using System;
using CP_SDK.Chat.Interfaces;

namespace BSP_AltStream;

internal class AltStreamChatMessage : IChatMessage {
    public string Id { get; internal set; } = Guid.NewGuid().ToString();
    public bool IsSystemMessage => false;
    public bool IsActionMessage => false;
    public bool IsHighlighted {get; internal set;} = false;
    public bool IsGiganticEmote => false;
    public bool IsPing => false;
    public string Message {get; internal set;} = "";
    public IChatUser Sender {get; internal set;}
    public IChatChannel Channel {get; internal set;}
    public IChatEmote[] Emotes { get; internal set;} = [];
}