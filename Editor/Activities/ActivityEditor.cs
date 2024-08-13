using UnityEditor;
using ToolkitEngine.AI;

namespace ToolkitEditor.AI
{
	[CustomEditor(typeof(Activity))]
    public class ActivityEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_type;
		protected SerializedProperty m_predicate;
		protected SerializedProperty m_actType;
		protected SerializedProperty m_duration;
		protected SerializedProperty m_uses;
		protected SerializedProperty m_autoDepart;
		protected SerializedProperty m_score;

		// Events
		protected SerializedProperty m_onPerforming;
		protected SerializedProperty m_onPerformed;
		protected SerializedProperty m_onCanceled;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_predicate = serializedObject.FindProperty(nameof(m_predicate));
			m_actType = serializedObject.FindProperty(nameof(m_actType));
			m_duration = serializedObject.FindProperty(nameof(m_duration));
			m_uses = serializedObject.FindProperty(nameof(m_uses));
			m_autoDepart = serializedObject.FindProperty(nameof(m_autoDepart));
			m_score = serializedObject.FindProperty(nameof(m_score));

			// Events
			m_onPerforming = serializedObject.FindProperty(nameof(m_onPerforming));
			m_onPerformed = serializedObject.FindProperty(nameof(m_onPerformed));
			m_onCanceled = serializedObject.FindProperty(nameof(m_onCanceled));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_predicate);
			EditorGUILayout.PropertyField(m_actType);

			if ((int)Activity.ActType.Timed == m_actType.intValue)
			{
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_duration);
				--EditorGUI.indentLevel;
			}

			EditorGUILayout.PropertyField (m_uses);
			EditorGUILayout.PropertyField(m_autoDepart);

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_score);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onPerforming, "Events"))
			{
				EditorGUILayout.PropertyField(m_onPerforming);
				EditorGUILayout.PropertyField(m_onPerformed);
				EditorGUILayout.PropertyField(m_onCanceled);
			}
		}

		#endregion
	}
}