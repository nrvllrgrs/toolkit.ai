using UnityEngine;
using UnityEngine.AI;

namespace ToolkitEngine.AI
{
	[RequireComponent(typeof(NavMeshAgentControl))]
	public class PatrolPathAgent : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private PatrolPath m_path;

		[SerializeField]
		private bool m_playOnAwake;

		private bool m_traveling;
		private int m_index = 0;
		private bool m_isReverse = false;
		private NavMeshAgentControl m_navMeshAgentControl;

		#endregion

		#region Properties

		public bool hasPath => m_path != null;

		public PatrolPath path
		{
			get => m_path;
			private set
			{
				// No change, skip
				if (m_path == value)
					return;

				if (value == null)
				{
					traveling = false;
				}

				m_path = value;
			}
		}

		public bool traveling
		{
			get => m_traveling;
			set
			{
				// No change, skip
				if (m_traveling == value)
					return;

				m_traveling = value;

				if (hasPath)
				{
					if (value)
					{
						m_navMeshAgentControl.onArrival.AddListener(Arrival);
					}
					else
					{
						m_navMeshAgentControl.onArrival.RemoveListener(Arrival);
					}
				}
				else
				{
					Debug.LogWarningFormat("PatrolPathAgent {0} has undefined path! Cannot travel!", name);
				}
			}
		}

		#endregion

		#region Methods

		private void Awake()
		{
			m_navMeshAgentControl = GetComponent<NavMeshAgentControl>();

			if (!hasPath)
				return;

			m_index = m_path.GetClosestIndex(transform);

			if (m_playOnAwake)
			{
				Next();
			}
		}

		public void SetPath(PatrolPath path, bool autoTravel = true)
		{
			this.path = path;
			traveling = autoTravel;
		}

		public void Next()
		{
			if (!hasPath)
				return;

			switch (m_path.wrapMode)
			{
				case WrapMode.Loop:
					m_index = (m_index + 1).Mod(m_path.count);
					break;

				case WrapMode.PingPong:
					if (!m_isReverse)
					{
						++m_index;
						if (m_index > m_path.count - 1)
						{
							m_isReverse = true;
							m_index = m_path.count - 2;
						}
					}
					else
					{
						--m_index;
						if (m_index < 0)
						{
							m_isReverse = false;
							m_index = 1;
						}
					}
					break;

				default:
					++m_index;
					if (m_index > m_path.count)
						return;

					break;
			}

			if (m_path.TryGetPosition(m_index, out Vector3 destination))
			{
				traveling = true;
				m_navMeshAgentControl.navMeshAgent.SetDestination(destination);
			}
		}

		private void Arrival(NavMeshAgent navMeshAgent)
		{
			Next();
		}

		#endregion
	}
}