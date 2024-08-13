using UnityEngine;

namespace ToolkitEngine.AI
{
    public class ActivityAgent : MonoBehaviour, IPoolItemRecyclable
    {
        #region Fields

        private Animator m_animator;
        private RuntimeAnimatorController m_animatorController;

		#endregion

		#region Properties

		public Activity activity { get; set; }

		#endregion

		#region Methods

        public void Recycle()
        {
            if (m_animator != null)
            {
                m_animator.runtimeAnimatorController = m_animatorController;
            }
        }

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            if (m_animator != null)
            {
                m_animatorController = m_animator.runtimeAnimatorController;
            }
        }

        public void Perform()
        {
            if (activity == null)
                return;

            if (activity.actType == Activity.ActType.Custom)
            {
                activity.Perform();
            }
        }

        public void Cancel()
        {
            activity?.Cancel();
        }

		#endregion
	}
}