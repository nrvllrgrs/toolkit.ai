using Unity.VisualScripting;
using System;

namespace ToolkitEngine.AI.VisualScripting
{
    [UnitTitle("On Performing"), UnitSurtitle("Activity")]
    public class OnActivityPerforming : BaseActivityEventUnit
    {
        public override Type MessageListenerType => typeof(OnActivityPerformingMessageListener);
    }
}