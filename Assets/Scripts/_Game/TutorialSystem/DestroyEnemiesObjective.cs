using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jampacked.ProjectInca.Events;
using TMPro;

namespace Jampacked.ProjectInca
{
    public class DestroyEnemiesObjective : Objective
    {
        [SerializeField]
        int numEnemiesToDestroy;

        [SerializeField]
        TMP_Text objectiveProgress;

        EventDispatcher m_dispatcher;

        int m_enemiesDestroyed = 0;

        private void Awake()
        {
            m_dispatcher = FindObjectOfType<EventDispatcher>();

            m_dispatcher.AddListener<EnemyDestroyedEvent>(OnEnemyDestroyed);

            objectiveProgress.text = "0 / " + numEnemiesToDestroy;
        }

        void OnEnemyDestroyed(in Events.Event a_evt)
        {
            if (!this || !gameObject || !isActiveAndEnabled)
            {
                return;
            }

            m_enemiesDestroyed++;

            objectiveProgress.text = m_enemiesDestroyed + " / " + numEnemiesToDestroy;

            if (m_enemiesDestroyed >= numEnemiesToDestroy)
            {
                OnCompleted();
            }
        }
    }
}
