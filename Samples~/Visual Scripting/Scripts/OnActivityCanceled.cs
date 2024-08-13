using Unity.VisualScripting;
using System;

namespace ToolkitEngine.AI.VisualScripting
{
    [UnitTitle("On Canceled"), UnitSurtitle("Activity")]
    public class OnActivityCanceled : BaseActivityEventUnit
    {
        public override Type MessageListenerType => typeof(OnActivityCanceledMessageListener);
    }
}