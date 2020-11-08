using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jampacked.ProjectInca
{
    public class Objective : MonoBehaviour
    {
        public GameObject nextObjectiveBlocker = null;

        protected ObjectiveManager m_objectiveManager;

        void Start()
        {
            m_objectiveManager = FindObjectOfType<ObjectiveManager>();
            if (!m_objectiveManager)
            {
                throw new ArgumentNullException("OBJECTIVE MANAGER MUST EXIST IN THE TUTORIAL SCENE!");
            }

            m_objectiveManager.RegisterObjective(this);
        }

        protected void OnCompleted()
        {
            if (nextObjectiveBlocker)
            {
                Destroy(nextObjectiveBlocker);
            }

            m_objectiveManager.UnregisterObjective(this);
            Destroy(gameObject);
        }
    }
}
