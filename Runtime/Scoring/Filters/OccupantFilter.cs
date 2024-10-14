using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.AI.Scoring
{
	[EvaluableCategory("AI")]
	public class OccupantFilter : BaseFilter
    {
		#region Fields

		[SerializeField]
		private List<GameObject> m_validOccupants = new();

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (!target.TryGetComponent(out Destination destination))
				return false;

			if (m_validOccupants.Count == 0)
				return !destination.vacant;

			if (destination.vacant)
				return false;

			return m_validOccupants.Contains(destination.occupant);
		}

		#endregion
	}
}