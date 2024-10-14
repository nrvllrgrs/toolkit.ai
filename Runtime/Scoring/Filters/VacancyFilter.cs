using UnityEngine;

namespace ToolkitEngine.AI.Scoring
{
	[EvaluableCategory("AI")]
	public class VacancyFilter : BaseFilter
	{
		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (!target.TryGetComponent(out Destination destination))
				return false;

			return destination.vacant;
		}

		#endregion
	}
}