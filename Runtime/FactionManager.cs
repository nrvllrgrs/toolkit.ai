using UnityEngine;

namespace ToolkitEngine.AI
{
    public class FactionManager : Singleton<FactionManager>
    {
        #region Fields

        [SerializeField]
        private RelationshipType m_relationship;

        #endregion

        #region Properties

        public RelationshipType Relationship => m_relationship;

        #endregion
    }
}