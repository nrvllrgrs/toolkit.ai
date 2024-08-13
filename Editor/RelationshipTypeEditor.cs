using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ToolkitEngine.AI;

namespace ToolkitEditor.AI
{
    [CustomEditor(typeof(RelationshipType))]
    public class RelationshipTypeEditor : Editor
    {
        #region Fields

        protected RelationshipType m_relationshipType;

        protected SerializedProperty m_factions;
        private ReorderableList m_factionsList;

        protected SerializedProperty m_neutralFactor;
        protected SerializedProperty m_friendFactor;
        protected SerializedProperty m_enemyFactor;

        protected SerializedProperty m_relationships;

        private GUIStyle m_matrixTextStyle;
        private GUIStyle m_matrixCellStyle;

        private IEnumerable<FactionType> m_cachedFactions = null;
        private Texture2D m_neutralIcon, m_friendIcon, m_enemyIcon;

        #endregion

        #region Properties

        private Texture2D NeutralIcon => GetIcon(ref m_neutralIcon, "3efe90d39faeff44d93fb896c729150c");
        private Texture2D FriendIcon => GetIcon(ref m_friendIcon, "600d76d93f62fcb439573b6e3914d279");
        private Texture2D EnemyIcon => GetIcon(ref m_enemyIcon, "9351585e46d48dd48a581451df8511de");

        #endregion

        #region Methods

        private void OnEnable()
        {
            m_cachedFactions = AssetDatabase.FindAssets("t:FactionType")
                .Select(x => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(x), typeof(FactionType)))
                .Cast<FactionType>();

            m_relationshipType = (RelationshipType)target;
            m_factions = serializedObject.FindProperty(nameof(m_factions));

            m_neutralFactor = serializedObject.FindProperty(nameof(m_neutralFactor));
            m_friendFactor = serializedObject.FindProperty(nameof(m_friendFactor));
            m_enemyFactor = serializedObject.FindProperty(nameof(m_enemyFactor));

            m_relationships = serializedObject.FindProperty(nameof(m_relationships));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (m_factionsList == null)
            {
                m_factionsList = new ReorderableList(m_relationshipType.Factions.ToArray(), typeof(FactionType), true, true, true, true);
                m_factionsList.drawHeaderCallback += (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Factions");
                };
                m_factionsList.drawElementCallback += OnDrawElementCallback;
                m_factionsList.onCanAddCallback += OnCanAddCallback;
                m_factionsList.onAddDropdownCallback += OnAddDropdownCallback;
                m_factionsList.onReorderCallback += OnReorderCallback;
                m_factionsList.onCanRemoveCallback += OnCanRemoveCallback;
                m_factionsList.onRemoveCallback += OnRemoveCallback;
            }

            // Draw faction list
            m_factionsList.DoLayoutList();
            
            if (m_relationshipType.Factions.Any())
            {
                EditorGUILayout.LabelField("Relationship Matrix", EditorStyles.boldLabel);

                var validFactions = m_relationshipType.Factions.Where(x => x != null).ToArray();
                GUILayout.Space(EditorGUIUtility.labelWidth + EditorGUIUtility.singleLineHeight * validFactions.Length);

                var lastRect = GUILayoutUtility.GetLastRect();

                if (m_matrixTextStyle == null)
                {
                    m_matrixTextStyle = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleRight,
                        fixedHeight = EditorGUIUtility.singleLineHeight,
                    };
                }

                if (m_matrixCellStyle == null)
                {
                    m_matrixCellStyle = new GUIStyle(GUIStyle.none)
                    {
                    };
                }

                int i = 0;
                foreach (var factionType in m_relationshipType.Factions)
                {
                    if (factionType == null)
                        continue;

                    // Draw column header
                    var x = lastRect.x + EditorGUIUtility.labelWidth + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (validFactions.Length - i);
                    var pivot = new Vector2(x, lastRect.y);

                    GUIUtility.RotateAroundPivot(90f, pivot);
                    EditorGUI.LabelField(new Rect(x, lastRect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
                        factionType.name,
                        m_matrixTextStyle);
                    GUIUtility.RotateAroundPivot(-90f, pivot);

                    // Draw row header
                    var y = lastRect.y + EditorGUIUtility.labelWidth + EditorGUIUtility.singleLineHeight * i;
                    EditorGUI.LabelField(
                        new Rect(lastRect.x, y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
                        factionType.name,
                        m_matrixTextStyle);

                    // Draw row buttons
                    int lastIndex = validFactions.Length - i - 1;
                    for (int j = 0; j < validFactions.Length - i; ++j)
                    {
                        if (!m_relationshipType.TryGetRelationship(validFactions[i], validFactions[^(j + 1)], out RelationshipType.Relationship relationship))
                            continue;

                        x = lastRect.x + EditorGUIUtility.labelWidth + EditorGUIUtility.singleLineHeight * j + EditorGUIUtility.standardVerticalSpacing * (j + 1);
                        var cellRect = new Rect(x, y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

                        EditorGUI.BeginDisabledGroup(j == lastIndex);

                        Texture2D icon;
                        switch (relationship)
                        {
                            case RelationshipType.Relationship.Friend:
                                icon = FriendIcon;
                                break;

                            case RelationshipType.Relationship.Enemy:
                                icon = EnemyIcon;
                                break;

                            default:
                                icon = NeutralIcon;
                                break;
                        }

                        if (EditorGUI.DropdownButton(cellRect, new GUIContent(icon), FocusType.Passive, m_matrixCellStyle))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Neutral"), false, HandleItemClicked, new FactionMenuEventArgs()
                            {
                                faction1 = validFactions[i],
                                faction2 = validFactions[^(j + 1)],
                                relationship = RelationshipType.Relationship.Neutral
                            });
                            menu.AddItem(new GUIContent("Friend"), false, HandleItemClicked, new FactionMenuEventArgs()
                            {
                                faction1 = validFactions[i],
                                faction2 = validFactions[^(j + 1)],
                                relationship = RelationshipType.Relationship.Friend
                            });
                            menu.AddItem(new GUIContent("Enemey"), false, HandleItemClicked, new FactionMenuEventArgs()
                            {
                                faction1 = validFactions[i],
                                faction2 = validFactions[^(j + 1)],
                                relationship = RelationshipType.Relationship.Enemy
                            });
                            menu.DropDown(cellRect);
                        }

                        EditorGUI.EndDisabledGroup();
                    }

                    ++i;
                }
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Damage Factors", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_neutralFactor, new GUIContent("Neutral"));
            EditorGUILayout.PropertyField(m_friendFactor, new GUIContent("Friend"));
            EditorGUILayout.PropertyField(m_enemyFactor, new GUIContent("Enemy"));

            serializedObject.ApplyModifiedProperties();
        }

        private void HandleItemClicked(object parameter)
        {
            var e = (FactionMenuEventArgs)parameter;
            m_relationshipType.SetRelationship(e.faction1, e.faction2, e.relationship);

            Save();
        }

        #endregion

        #region Helper Methods

        private IEnumerable<Object> GetAvailableFactions()
        {
            var knownFactionGUIDs = m_relationshipType.Factions.Where(x => x != null)
                .Select(x => AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(x)).ToString());

            return m_cachedFactions.Except(m_relationshipType.Factions);
        }

        private Texture2D GetIcon(ref Texture2D texture, string guid)
        {
            if (texture == null)
            {
                texture = EditorGUIUtility.Load(AssetDatabase.GUIDToAssetPath(guid)) as Texture2D;
            }
            return texture;
        }

        private void Save()
        {
            EditorUtility.SetDirty(m_relationshipType);
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region ReorderableList Methods

        private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.PropertyField(rect, m_factions.GetArrayElementAtIndex(index), GUIContent.none);
        }

        private bool OnCanAddCallback(ReorderableList list)
        {
            return GetAvailableFactions().Any();
        }

        private void OnAddDropdownCallback(Rect buttonRect, ReorderableList list)
        {
            var factions = GetAvailableFactions();

            var addMenu = new GenericMenu();
            foreach (var faction in factions.OrderBy(x => x.name))
            {
                addMenu.AddItem(new GUIContent(faction.name), false, OnAddFaction, faction);
            }

            addMenu.ShowAsContext();
        }

        private void OnAddFaction(object parameter)
        {
            if (parameter == null)
                return;

            var faction = parameter as FactionType;

            m_relationshipType.Factions.Add(faction);
            m_factionsList.list = m_relationshipType.Factions.ToArray();

            // Add relationships with other factions
            foreach (var other in m_relationshipType.Factions)
            {
                m_relationshipType.SetRelationship(
                    faction,
                    other,
                    faction.Equals(other) ? RelationshipType.Relationship.Friend : RelationshipType.Relationship.Neutral);
            }

            Save();
        }

        private void OnReorderCallback(ReorderableList list)
        {
            m_relationshipType.Factions = list.list as IList<FactionType>;
            Save();
        }

        private bool OnCanRemoveCallback(ReorderableList list)
        {
            return m_relationshipType.Factions != null && m_relationshipType.Factions.Count > 0;
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            // Faction to be removed
            var faction = m_relationshipType.Factions[list.index];

            // Remove relationships with other factions
            foreach (var other in m_relationshipType.Factions)
            {
                m_relationshipType.RemoveRelationship(faction, other);
            }

            // Remove faction from list
            m_relationshipType.Factions.RemoveAt(list.index);
            list.list = m_relationshipType.Factions.ToArray();

            Save();
        }

        #endregion

        #region Structures

        public struct FactionMenuEventArgs
        {
            public FactionType faction1;
            public FactionType faction2;
            public RelationshipType.Relationship relationship;
        }

        #endregion
    }
}
