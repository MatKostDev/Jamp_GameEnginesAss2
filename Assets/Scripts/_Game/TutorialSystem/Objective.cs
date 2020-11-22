using System;
using System.Collections;
using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
    public class Objective : MonoBehaviour
    {
        [SerializeField] 
        GameObject nextObjectiveBlocker = null;

        [SerializeField]
        GameObject objectiveUIRoot;

        protected EventDispatcher m_dispatcher;

        protected ObjectiveManager m_objectiveManager;

        void Awake()
        {
            m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

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

            m_dispatcher.PostEvent(new ObjectiveCompletedEvent());

            Destroy(gameObject);
        }
    }
}
