using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.AI.VisualScripting
{
    [AddComponentMenu("")]
    public class OnActivityCanceledMessageListener : MessageListener
    {
        private void Start() => GetComponent<Activity>()?.onCanceled.AddListener((value) =>
        {
            EventBus.Trigger(nameof(OnActivityCanceled), gameObject, value);
        });
    }
}