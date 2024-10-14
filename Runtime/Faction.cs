using UnityEngine;
using ToolkitEngine.Health;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace ToolkitEngine.AI
{
	public class Faction : MonoBehaviour, IHealthModifier
	{
		#region Fields

		[SerializeField]
		private FactionType m_factionType;

		private FactionType m_overrideFactionType;
		private IEnumerable<IHealth> m_storedHealths = null;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<Faction> m_onChanged;

		#endregion

		#region Properties

		public FactionType factionType
		{
			get => m_overrideFactionType ?? m_factionType;
			set
			{
				// No change, skip
				if (factionType == value)
					return;

				// No need for overriding faction to match default faction 
				if (value == m_factionType)
				{
					value = null;
				}

				m_factionType = value;
				m_onChanged?.Invoke(this);
			}
		}

		public UnityEvent<Faction> onChanged => m_onChanged;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_storedHealths = GetComponentsInChildren<IHealth>()
				.Where(x => Equals(x.transform.GetComponentInParent<IFaction>(), this));

			foreach (var health in m_storedHealths)
			{
				health.onValueChanging.AddListener(Health_ValueChanging);
			}
		}

		private void OnDisable()
		{
			if (m_storedHealths == null)
				return;

			foreach (var health in m_storedHealths)
			{
				if (ToolkitUtil.IsNull(health))
					continue;

				health.onValueChanging.RemoveListener(Health_ValueChanging);
			}
		}

		private void Health_ValueChanging(HealthEventArgs e)
		{
			if (TryGetFactor(e.hit.damageType, e.hit.source, out float factor))
			{
				e.preDamageFactor += factor - 1f;
			}
		}

		public bool TryGetFactor(DamageType damageType, GameObject target, out float value)
		{
			var otherFaction = target.GetComponentInParent<IFaction>();
			if (otherFaction == null || !otherFaction.enabled)
			{
				value = 0f;
				return false;
			}

			return FactionManager.CastInstance.Config.TryGetFactor(m_factionType, otherFaction.factionType, out value);
		}

		public bool TryModifyFactor(DamageType damageType, GameObject target, float delta)
		{
			// Cannot modify config factor
			// Intentionally blank
			return false;
		}

		public bool TrySetFactor(DamageType damageType, GameObject target, float value)
		{
			// Cannot set config factor
			// Intentionally blank
			return false;
		}

		#endregion
	}
}