using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.AI.VisualScripting
{
    [AddComponentMenu("")]
    public class OnActivityPerformingMessageListener : MessageListener
    {
        private void Start() => GetComponent<Activity>()?.onPerforming.AddListener((value) =>
        {
            EventBus.Trigger(nameof(OnActivityPerforming), gameObject, value);
        });
    }
}