using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.AI
{
    [RequireComponent(typeof(Activity))]
    public class ActivityAnimatorOverride : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private AnimatorOverrideController[] m_animatorOverrides;

		[SerializeField]
		private string m_triggerParameter = "Perform";

		[SerializeField]
		private string m_stateName = "Activity";

		[SerializeField, Min(0)]
		private int m_layerIndex = 0;

		private Activity m_activity;

		private Animator m_animator;
		private RuntimeAnimatorController m_runtimeAnimatorController;
		private Dictionary<RuntimeAnimatorController, AnimatorOverrideController> m_map = new();
		private Coroutine m_animationThread = null;

#if !UNITY_EDITOR
		private int m_triggerParameterId;
#endif
		#endregion

		#region Methods

		private void Awake()
		{
			m_activity = GetComponent<Activity>();

			foreach (var o in m_animatorOverrides)
			{
				if (o?.runtimeAnimatorController == null)
					continue;

				m_map.Add(o.runtimeAnimatorController, o);
			}

#if !UNITY_EDITOR
			m_triggerParameterId = Animator.StringToHash(m_triggerParameter);
#endif
		}

		private void OnEnable()
		{
			m_activity.onPerforming.AddListener(Performing);
			m_activity.onPerformed.AddListener(Performed);
			m_activity.onCanceled.AddListener(Completed);
		}

		private void OnDisable()
		{
			m_activity.onPerforming.RemoveListener(Performing);
			m_activity.onPerformed.RemoveListener(Performed);
			m_activity.onCanceled.RemoveListener(Completed);
		}

		private void Performing(ActivityEventArgs e)
		{
			if (!e.agent.TryGetComponent(out Animator animator)
				|| !m_map.TryGetValue(animator.runtimeAnimatorController, out AnimatorOverrideController overrideController))
			{
				e.agent.Perform();
				return;
			}

			// Remember controller so it can be restored
			m_animator = animator;
			m_runtimeAnimatorController = animator.runtimeAnimatorController;

			// Override animator
			animator.runtimeAnimatorController = overrideController;

			if (!string.IsNullOrWhiteSpace(m_stateName))
			{
				m_animationThread = StartCoroutine(AsyncWaitAnimation(e));
			}

#if !UNITY_EDITOR
			animator.SetTrigger(m_triggerParameterId);
#else
			animator.SetTrigger(m_triggerParameter);
#endif
		}

		private void Performed(ActivityEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(m_stateName))
				return;

			Completed(e);
		}

		private void Completed(ActivityEventArgs e)
		{
			if (!e.agent.TryGetComponent(out Animator animator) || !ReferenceEquals(animator, m_animator))
				return;

			this.CancelCoroutine(ref m_animationThread);

			// Restore controller
			animator.runtimeAnimatorController = m_runtimeAnimatorController;
		}

		private IEnumerator AsyncWaitAnimation(ActivityEventArgs e)
		{
			yield return new WaitUntil(IsActivityPlaying);
			yield return new WaitWhile(IsActivityPlaying);
			Completed(e);
		}

		private bool IsActivityPlaying()
		{
			if (m_animator == null)
				return false;

			return m_animator.GetCurrentAnimatorStateInfo(m_layerIndex).IsName(m_stateName);
		}

#endregion
	}
}