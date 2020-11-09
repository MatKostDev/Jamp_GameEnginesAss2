using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jampacked.ProjectInca
{
    public class ObjectiveManager : MonoBehaviour
    {
        [SerializeField] 
        List<Objective> objectiveList = new List<Objective>();

        [SerializeField]
        GameObject allObjectivesDoneUI;

        void Start()
        {
            allObjectivesDoneUI.SetActive(false);

            objectiveList[0].gameObject.SetActive(true);
            for (int i = 1; i < objectiveList.Count; i++)
            {
                objectiveList[i].gameObject.SetActive(false);
            }
        }

        public void UnregisterObjective(Objective a_objective)
        {
            objectiveList.Remove(a_objective);

            if (objectiveList.Count == 0)
            {
                allObjectivesDoneUI.SetActive(true);
            }
            else
            {
                objectiveList[0].gameObject.SetActive(true);
            }
        }
    }
}
