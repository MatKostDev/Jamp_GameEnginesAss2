using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
	public class PlayerExp : MonoBehaviour
	{
		[Header("General")]
		[SerializeField]
		int expRequiredForFirstLevel = 100;
		
		[SerializeField]
		int expRequiredIncreasePerLevel = 40;

		[Header("UI")]
		[SerializeField]
		Image expBarFill = null;

		[SerializeField]
		TMP_Text levelNumDisplay = null;

		EventDispatcher m_dispatcher;
		
		int m_currentLevel = 0;
		int m_currentExp;
		
		int m_expRequiredForLevel;

		void Start()
		{
			m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();
			
			m_dispatcher.AddListener<PickUpExpEvent>(OnPickedUpExp);
			
			expBarFill.fillAmount = 0f;
			levelNumDisplay.text  = "0";

			m_expRequiredForLevel = expRequiredForFirstLevel;
		}

		void OnDestroy()
		{
			m_dispatcher.RemoveListener<PickUpExpEvent>(OnPickedUpExp);
		}

		void OnPickedUpExp(in Events.Event a_evt)
		{
			if (!(a_evt is PickUpExpEvent expEvent))
			{
				return;
			}
            if (expEvent.playerObjectId != gameObject.GetInstanceID())
            {
                return;
            }

            AddExp(expEvent.expAmount);
		}

		void AddExp(int a_amount)
		{
			m_currentExp += a_amount;

			if (m_currentExp >= m_expRequiredForLevel)
			{
				m_currentLevel++;
				
				m_currentExp          -= m_expRequiredForLevel; //allow overflow to add progress to next level
				m_expRequiredForLevel += expRequiredIncreasePerLevel;

				levelNumDisplay.text = m_currentLevel.ToString();
			}

			expBarFill.fillAmount = (float)m_currentExp / m_expRequiredForLevel;
		}
	}
}
