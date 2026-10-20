using CP_SDK.Chat.Interfaces;

namespace BSP_AltStream;

internal class AltStreamChatChannel : IChatChannel {
    private string _Name = "NULLCHANNEL";
    public string Id => "lkjstv_"+Name;
    public string Name { get { return _Name; } set { _Name = value; } }
    public bool IsTemp => false;
    public string Prefix => "";
    public bool CanSendMessages => true;
    public bool Live => true;
    public int ViewerCount { get; internal set; } = 0;
    public AltStreamChatChannel(string __Name) {
        _Name = __Name;
    }
}