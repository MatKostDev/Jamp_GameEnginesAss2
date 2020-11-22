using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jampacked.ProjectInca.Events;

namespace Jampacked.ProjectInca
{
    public class ObjectiveManager : MonoBehaviour
    {
        [SerializeField] 
        List<Objective> objectiveList = new List<Objective>();

        [SerializeField]
        GameObject allObjectivesDoneUI;

        EventDispatcher m_dispatcher;

        void Start()
        {
            m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

            allObjectivesDoneUI.SetActive(false);

            objectiveList[0].gameObject.SetActive(true);
            for (int i = 1; i < objectiveList.Count; i++)
            {
                objectiveList[i].gameObject.SetActive(false);
            }

            m_dispatcher.AddListener<ObjectiveCompletedEvent>(OnObjectiveCompleted);
        }

        void OnDestroy()
        {
            m_dispatcher.RemoveListener<ObjectiveCompletedEvent>(OnObjectiveCompleted);
        }

        void OnObjectiveCompleted(in Events.Event a_evt)
        {
            if (!(a_evt is ObjectiveCompletedEvent completedEvent))
            {
                return;
            }

            UnregisterCurrentObjective();
        }

        void UnregisterCurrentObjective()
        {
            objectiveList.RemoveAt(0);

            if (objectiveList.Count == 0)
            {
                allObjectivesDoneUI.SetActive(true);

                var competedEvent = new TutorialCompletedEvent
                {
                    timeCompleted = Time.time,
                };
                m_dispatcher.PostEvent(competedEvent);
            }
            else
            {
                objectiveList[0].gameObject.SetActive(true);
            }
        }
    }
}
