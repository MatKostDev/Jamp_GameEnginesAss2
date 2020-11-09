using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
    public class DestroyEnemiesObjective : Objective
    {
        [SerializeField]
        int numEnemiesToDestroy;

        EventDispatcher m_dispatcher;

        int m_enemiesDestroyed = 0;

        private void Awake()
        {
            m_dispatcher = FindObjectOfType<EventDispatcher>();

            m_dispatcher.AddListener<EnemyDestroyedEvent>(OnEnemyDestroyed);
        }

        void OnEnemyDestroyed(in Events.Event a_evt)
        {
            if (!this || !gameObject || !isActiveAndEnabled)
            {
                return;
            }

            m_enemiesDestroyed++;

            if (m_enemiesDestroyed >= numEnemiesToDestroy)
            {
                OnCompleted();
            }
        }
    }
}
