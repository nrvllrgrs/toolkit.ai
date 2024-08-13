using ToolkitEngine.AI;
using UnityEditor;

namespace ToolkitEditor.AI
{
	[CustomEditor(typeof(Faction), true)]
	public class FactionEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_factionType;
		protected SerializedProperty m_onChanged;

		#endregion

		#region Methods

		protected virtual void OnEnable()
		{
			m_factionType = serializedObject.FindProperty(nameof(m_factionType));
			m_onChanged = serializedObject.FindProperty(nameof(m_onChanged));
		}

		protected override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.PropertyField(m_factionType);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onChanged, "Events"))
			{
				EditorGUILayout.PropertyField(m_onChanged);
				DrawNestedEvents();
			}

		}

		#endregion
	}
}