using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jampacked.ProjectInca
{
    public class Objective : MonoBehaviour
    {
        [SerializeField] 
        GameObject nextObjectiveBlocker = null;

        [SerializeField]
        GameObject objectiveUIRoot;

        protected ObjectiveManager m_objectiveManager;

        void Start()
        {
            m_objectiveManager = FindObjectOfType<ObjectiveManager>();
            if (!m_objectiveManager)
            {
                throw new ArgumentNullException("OBJECTIVE MANAGER MUST EXIST IN THE TUTORIAL SCENE!");
            }
        }

        private void OnEnable()
        {
            if (objectiveUIRoot)
                objectiveUIRoot.SetActive(true);
        }

        private void OnDisable()
        {
            if (objectiveUIRoot)
                objectiveUIRoot.SetActive(false);
        }

        protected void OnCompleted()
        {
            if (objectiveUIRoot)
                objectiveUIRoot.SetActive(false);

            if (nextObjectiveBlocker)
            {
                Destroy(nextObjectiveBlocker);
            }

            m_objectiveManager.UnregisterObjective(this);
            Destroy(gameObject);
        }
    }
}
