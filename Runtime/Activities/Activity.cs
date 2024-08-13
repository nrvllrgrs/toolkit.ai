using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using ToolkitEngine.Sensors;

namespace ToolkitEngine.AI
{
	public class ActivityEventArgs : System.EventArgs
	{
		#region Properties
		
		public ActivityAgent agent { get; private set; }
		public Activity activity { get; private set; }

		#endregion

		#region Constructors

		public ActivityEventArgs(ActivityAgent agent, Activity activity)
		{
			this.agent = agent;
			this.activity = activity;
		}

		#endregion
	}

	[RequireComponent(typeof(Markup))]
	public class Activity : MonoBehaviour
	{
		#region Enumerators

		public enum ActType
		{
			Custom,
			Instantaneous,
			Timed,
		}

		#endregion

		#region Fields

		[SerializeField]
		private UnityCondition m_predicate = new UnityCondition();

		[SerializeField]
		private ActType m_actType = ActType.Instantaneous;

		[SerializeField, Min(0f)]
		private float m_duration = 1f;

		[SerializeField]
		private int m_uses = 0;

		[SerializeField]
		private bool m_autoDepart = true;

		[SerializeField, Tooltip("Evaluators to calculate activity utility.")]
		private UnityEvaluator m_score = new();

		private Markup m_markup;
		private ActivityAgent m_agent;
		private int m_remainingUses;
		private Coroutine m_performanceThread = null;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<ActivityEventArgs> m_onPerforming;

		[SerializeField]
		private UnityEvent<ActivityEventArgs> m_onPerformed;

		[SerializeField]
		private UnityEvent<ActivityEventArgs> m_onCanceled;

		#endregion

		#region Properties

		public ActType actType => m_actType;
		public bool infiniteUses => m_uses == 0;
		public bool running => m_agent != null;

		public UnityEvent<ActivityEventArgs> onPerforming => m_onPerforming;

		public UnityEvent<ActivityEventArgs> onPerformed => m_onPerformed;

		public UnityEvent<ActivityEventArgs> onCanceled => m_onCanceled;

		#endregion

		#region Methods

		private void Awake()
		{
			m_markup = GetComponent<Markup>();
			m_remainingUses = m_uses;
		}

		private void OnEnable()
		{
			m_markup.onArrival.AddListener(Markup_Arrival);
			m_markup.onDeparture.AddListener(Markup_Departure);
		}

		private void OnDisable()
		{
			m_markup.onArrival.RemoveListener(Markup_Arrival);
			m_markup.onDeparture.RemoveListener(Markup_Departure);
		}

		private void Markup_Arrival(MarkupEventArgs e)
		{
			if (CanPerformBy(e.actor))
			{
				BeginPerform(e.actor);
			}
		}

		private void Markup_Departure(MarkupEventArgs e)
		{
			if (running)
			{
				Cancel();
			}
		}

		protected void BeginPerform(GameObject obj)
		{
			if (!obj.TryGetComponent(out ActivityAgent agent))
				return;

			m_agent = agent;
			m_agent.activity = this;

			m_onPerforming?.Invoke(new ActivityEventArgs(agent, this));

			switch (m_actType)
			{
				case ActType.Instantaneous:
					Perform();
					break;

				case ActType.Timed:
					m_performanceThread = StartCoroutine(AsyncPerform());
					break;
			}
		}

		private IEnumerator AsyncPerform()
		{
			yield return new WaitForSeconds(m_duration);
			Perform();
		}

		public void Perform()
		{
			// Not running, skip
			if (!running)
				return;

			m_remainingUses = Mathf.Max(m_remainingUses - 1, 0);
			m_onPerformed?.Invoke(new ActivityEventArgs(m_agent, this));

			Clean();
		}

		public void Cancel()
		{
			// Not running, skip
			if (!running)
				return;

			this.CancelCoroutine(ref m_performanceThread);
			m_onCanceled?.Invoke(new ActivityEventArgs(m_agent, this));

			Clean();
		}

		private void Clean()
		{
			GameObject actor = m_agent?.gameObject;

			// Activity is performed (or canceled), clear actor
			m_agent.activity = null;
			m_agent = null;

			// Depart after actiity has been performed (or canceled)
			if (m_autoDepart)
			{
				m_markup.Depart(actor);
			}
		}

		public bool CanPerformBy(GameObject actor)
		{
			return (infiniteUses || m_remainingUses > 0) && m_predicate.isTrueAndEnabled;
		}

		public float GetUtility(GameObject actor)
		{
			// Currently in use (or reserved), skip
			if (running || m_markup.reserved)
				return 0f;

			// Cannot be performed by actor, skip
			if (!CanPerformBy(actor))
				return 0f;

			// Actor is not an agent, skip
			if (!actor.TryGetComponent(out ActivityAgent agent))
				return 0f;

			return m_score.Evaluate(actor, gameObject);
		}

		#endregion
	}
}