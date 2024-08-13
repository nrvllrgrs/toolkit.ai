using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.AI.VisualScripting
{
    [AddComponentMenu("")]
    public class OnActivityPerformedMessageListener : MessageListener
    {
        private void Start() => GetComponent<Activity>()?.onPerformed.AddListener((value) =>
        {
            EventBus.Trigger(nameof(OnActivityPerformed), gameObject, value);
        });
    }
}