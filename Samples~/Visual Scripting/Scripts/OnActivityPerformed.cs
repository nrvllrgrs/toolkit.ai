using Unity.VisualScripting;
using System;

namespace ToolkitEngine.AI.VisualScripting
{
    [UnitTitle("On Performed"), UnitSurtitle("Activity")]
    public class OnActivityPerformed : BaseActivityEventUnit
    {
        public override Type MessageListenerType => typeof(OnActivityPerformedMessageListener);
    }
}