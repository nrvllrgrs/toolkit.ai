using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.AI.Scoring
{
	[EvaluableCategory("AI")]
	public class ReserverFilter : BaseFilter
	{
		#region Fields

		[SerializeField]
		private List<GameObject> m_validReservers = new();

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (!target.TryGetComponent(out Destination destination))
				return false;

			if (m_validReservers.Count == 0)
				return destination.reserved;

			if (!destination.reserved)
				return false;

			return m_validReservers.Contains(destination.reserver);
		}

		#endregion
	}
}