using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace ToolkitEngine.AI
{
	[RequireComponent(typeof(SplineContainer))]
    public class PatrolPath : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private WrapMode m_wrapMode = WrapMode.PingPong;

		private SplineContainer m_splineContainer;

		#endregion

		#region Properties

		public WrapMode wrapMode => m_wrapMode;

		private Spline spline
		{
			get
			{
				if (m_splineContainer == null)
				{
					m_splineContainer = GetComponent<SplineContainer>();
				}
				return m_splineContainer[0];
			}
		}

		/// <summary>
		/// Number of points in path
		/// </summary>
		public int count => spline.Count;

		#endregion

		#region Methods

		public int GetClosestIndex(Transform transform)
		{
			float minSqrDistance = float.MaxValue;
			int index = -1;

			for (int i = 0; i < spline.Count; ++i)
			{
				Vector3 position = GetKnotPosition(i);
				var sqrDistance = (transform.position - position).sqrMagnitude;
				if (sqrDistance < minSqrDistance)
				{
					minSqrDistance = sqrDistance;
					index = i;
				}
			}

			return index;
		}

		public Vector3 GetClosestPosition(Transform transform)
		{
			TryGetPosition(GetClosestIndex(transform), out Vector3 position);
			return position;
		}

		public bool TryGetPosition(int index, out Vector3 position)
		{
			if (index.Between(0, spline.Count - 1))
			{
				position = GetKnotPosition(index);
				return true;
			}

			position = Vector3.zero;
			return false;
		}

		private Vector3 GetKnotPosition(int index)
		{
			return m_splineContainer.transform.position + m_splineContainer.transform.rotation * spline[index].Position;
		}

		#endregion
	}
}
