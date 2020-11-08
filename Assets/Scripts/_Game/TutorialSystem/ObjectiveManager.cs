using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jampacked.ProjectInca
{
    public class ObjectiveManager : MonoBehaviour
    {
        List<Objective> m_objectiveList = new List<Objective>();

        void Start()
        {

        }

        void Update()
        {
            //Debug.Log(m_objectiveList.Count);
        }

        public void RegisterObjective(Objective a_objective)
        {
            if (!m_objectiveList.Contains(a_objective))
            {
                m_objectiveList.Add(a_objective);
            }
        }

        public void UnregisterObjective(Objective a_objective)
        {
            m_objectiveList.Remove(a_objective);
        }
    }
}
