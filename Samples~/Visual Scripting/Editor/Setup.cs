using System;
using System.Collections.Generic;
using ToolkitEngine.AI;
using UnityEditor;

namespace ToolkitEditor.AI.VisualScripting
{
	[InitializeOnLoad]
	public static class Setup
	{
		static Setup()
		{
			var types = new List<Type>()
			{
				typeof(IFaction),
				typeof(Faction),
				typeof(FactionManagerConfig),
				typeof(Destination),
				typeof(Activity),
				typeof(ActivityAgent),
				typeof(PatrolPath),
				typeof(PatrolPathAgent),
				typeof(NavMeshAgentControl),
			};

			ToolkitEditor.VisualScripting.Setup.Initialize("ToolkitEngine.AI", types);
		}
	}
}