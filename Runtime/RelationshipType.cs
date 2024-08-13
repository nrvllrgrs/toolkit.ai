using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.AI
{
    [CreateAssetMenu(menuName = "Toolkit/AI/Relationship")]
    public class RelationshipType : ScriptableObject
    {
        #region Enumerators

        public enum Relationship
        {
            Neutral,
            Friend,
            Enemy,
        }

        #endregion

        #region Fields

        [SerializeField]
        private List<FactionType> m_factions = new();

        [SerializeField, Range(0f, 1f)]
        private float m_neutralFactor = 1f;

        [SerializeField, Range(0f, 1f)]
        private float m_friendFactor = 1f;

        [SerializeField, Range(0f, 1f)]
        private float m_enemyFactor = 1f;

        [SerializeField]
        private SerializableFactionRelationship m_relationships = new();

        #endregion

        #region Properties

        public IList<FactionType> Factions { get => m_factions; set => m_factions = value.ToList(); }

        #endregion

        #region Methods

        public float GetFactor(Relationship relationship)
        {
            switch (relationship)
            {
                case Relationship.Neutral:
                    return m_neutralFactor;

                case Relationship.Friend:
                    return m_friendFactor;

                case Relationship.Enemy:
                    return m_enemyFactor;
            }
            return 0f;
        }

        public bool TryGetFactor(FactionType a, FactionType b, out float factor)
        {
            if (!TryGetRelationship(a, b, out Relationship relationship))
            {
                factor = 0f;
                return false;
            }

            factor = GetFactor(relationship);
            return true;
        }

        public bool TryGetRelationship(FactionType a, FactionType b, out Relationship relationship)
        {
            if (a == null || b == null)
            {
                relationship = Relationship.Neutral;
                return false;
            }
            return m_relationships.TryGetValue(new FactionPair(a, b), out relationship);
        }

        public void SetRelationship(FactionType a, FactionType b, Relationship relationship)
        {
            if (a == null || b == null)
                return;

            var pair = new FactionPair(a, b);
            if (m_relationships.ContainsKey(pair))
            {
                m_relationships[pair] = relationship;
            }
            else
            {
                m_relationships.Add(pair, relationship);
            }
        }

        public bool RemoveRelationship(FactionType a, FactionType b)
        {
            if (a == null || b == null)
                return false;

            var pair = new FactionPair(a, b);
            if (m_relationships.ContainsKey(pair))
            {
                m_relationships.Remove(pair);
                return true;
            }
            return false;
        }

        #endregion

        #region Structures

        [Serializable]
        private class SerializableFactionRelationship : SerializableDictionary<FactionPair, Relationship>
        { }

        [Serializable]
        public class FactionPair
        {
            #region Fields

            [SerializeField]
            private FactionType m_faction1, m_faction2;

            #endregion

            #region Constructors

            public FactionPair(FactionType faction1, FactionType faction2)
            {
                m_faction1 = faction1;
                m_faction2 = faction2;
            }

            #endregion

            #region Methods

            public override bool Equals(object obj)
            {
                if (obj is FactionPair && obj != null)
                {
                    return Equals(GetHashCode(), ((FactionPair)obj).GetHashCode());
                }
                return false;
            }

            public override int GetHashCode()
            {
                int a = m_faction1.GetHashCode();
                int b = m_faction2.GetHashCode();
                return a > b
                    ? b * 37 + a
                    : a * 37 + b;

            }

            #endregion
        }

        #endregion
    }
}