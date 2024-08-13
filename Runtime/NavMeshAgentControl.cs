using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace ToolkitEngine.AI
{
	[RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshAgentControl : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private GameObject m_target;

		private NavMeshAgent m_navMeshAgent;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<NavMeshAgent> m_onArrival;

		#endregion

		#region Properties

		public NavMeshAgent navMeshAgent => this.GetComponent(ref m_navMeshAgent);

		public bool hasTarget => target != null;

		public GameObject target
		{
			get => m_target;
			set
			{
				// No change, skip
				if (m_target == value)
					return;

				m_target = value;
				UpdateTarget();
			}
		}

		public UnityEvent<NavMeshAgent> onArrival => m_onArrival;

		#endregion

		#region Methods

		private void Awake()
		{
			m_navMeshAgent = m_navMeshAgent ?? GetComponent<NavMeshAgent>();
			UpdateTarget();
		}

		private void UpdateTarget()
		{
			m_navMeshAgent.updateRotation = hasTarget;
		}

		private void Update()
		{
			if (hasTarget && !Mathf.Approximately(m_navMeshAgent.angularSpeed, 0f))
			{
				// Rotate to face target
				var direction = Quaternion.LookRotation(
					Vector3.ProjectOnPlane(m_target.transform.position - transform.position, Vector3.up),
					Vector3.up);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, direction, m_navMeshAgent.angularSpeed);
			}

			// Notify when arrive at destination
			if (m_navMeshAgent.DestinationReached())
			{
				m_onArrival?.Invoke(m_navMeshAgent);
			}
		}

		#endregion
	}
}