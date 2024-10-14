using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.AI
{
	public class DestinationEventArgs : System.EventArgs
	{
		#region Properties

		public Destination destination { get; private set; }
		public GameObject actor { get; private set; }

		#endregion

		#region Constructors

		public DestinationEventArgs(Destination destination, GameObject actor)
		{
			this.destination = destination;
			this.actor = actor;
		}

		#endregion
	}

    public class Destination : MonoBehaviour
    {
		#region Fields

		private GameObject m_reserver;
		private GameObject m_occupant;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<DestinationEventArgs> m_onArrival;

		[SerializeField]
		private UnityEvent<DestinationEventArgs> m_onDeparture;

		[SerializeField]
		private UnityEvent<DestinationEventArgs> m_onReserved;

		[SerializeField]
		private UnityEvent<DestinationEventArgs> m_onCanceled;

		#endregion

		#region Properties

		/// <summary>
		/// Indicates whether markup is currently occupied
		/// </summary>
		public bool vacant => m_occupant == null;

		/// <summary>
		/// Indicates whether markup is being reserved by actor
		/// </summary>
		public bool reserved => m_reserver != null;

		public GameObject occupant => m_occupant;
		public GameObject reserver => m_reserver;

		public UnityEvent<DestinationEventArgs> onArrival => m_onArrival;
		public UnityEvent<DestinationEventArgs> onDeparture => m_onDeparture;
		public UnityEvent<DestinationEventArgs> onReserved => m_onReserved;
		public UnityEvent<DestinationEventArgs> onCanceled => m_onCanceled;

		#endregion

		#region Methods

		public virtual bool Reserve(GameObject obj)
		{
			// Already occupied, cannot reserve
			if (!vacant)
				return false;

			m_reserver = obj;
			m_onReserved?.Invoke(new DestinationEventArgs(this, obj));
			return true;
		}

		public virtual bool Cancel(GameObject obj)
		{
			if (!ReservedBy(obj))
				return false;

			ForceCancel();
			return true;
		}

		public void ForceCancel()
		{
			var t = m_reserver;
			m_reserver = null;

			m_onReserved?.Invoke(new DestinationEventArgs(this, t));
		}

		public virtual bool Arrive(GameObject obj)
		{
			if (!CanOccupy(obj))
				return false;

			m_reserver = null;
			m_occupant = obj;
			m_onArrival?.Invoke(new DestinationEventArgs(this, obj));
			return true;
		}

		public virtual bool Depart(GameObject obj)
		{
			if (!OccupiedBy(obj))
				return false;

			m_occupant = null;
			m_onDeparture?.Invoke(new DestinationEventArgs(this, obj));
			return true;
		}

		public virtual void Evict()
		{
			m_occupant = null;
		}

		public bool CanOccupy(GameObject obj)
		{
			return vacant && (m_reserver == null || ReservedBy(obj));
		}

		public bool OccupiedBy(GameObject obj)
		{
			return Equals(m_occupant, obj);
		}

		public bool ReservedBy(GameObject obj)
		{
			return Equals(m_reserver, obj);
		}

		#endregion

		#region Editor-Only
#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			GizmosUtil.DrawArrow(transform, Color.blue);
		}

#endif
		#endregion
	}
}