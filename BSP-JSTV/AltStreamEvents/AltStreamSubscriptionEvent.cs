using CP_SDK.Chat.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_AltStream.AltStreamEvents {
    internal class AltStreamSubscriptionEvent : IChatSubscriptionEvent {
        public string DisplayName { get; set; }
        public string SubPlan { get; set; }
        public bool IsGift { get; set; }
    }
}
