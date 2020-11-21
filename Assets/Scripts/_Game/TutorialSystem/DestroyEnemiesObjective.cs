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

        int m_enemiesDestroyed = 0;

        private void Start()
        {
            m_dispatcher.AddListener<TutorialEnemyKilledEvent>(OnEnemyDestroyed);

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
