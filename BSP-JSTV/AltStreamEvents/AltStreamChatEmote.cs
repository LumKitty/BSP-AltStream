using CP_SDK.Animation;
using CP_SDK.Chat.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_AltStream {
    internal class AltStreamChatEmote : IChatEmote {
        public string Id => "lkalts_" + Name;
        public string Name { get; set; }
        public string Uri { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public EAnimationType Animation { get; set; } = EAnimationType.AUTODETECT;

        public AltStreamChatEmote(string _Name, string _Uri, int _StartIndex, int _EndIndex) {
            Name = _Name;
            Uri = _Uri;
            StartIndex = _StartIndex;
            EndIndex = _EndIndex;
        }
    }
}
