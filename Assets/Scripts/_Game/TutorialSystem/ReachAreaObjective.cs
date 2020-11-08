using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jampacked.ProjectInca
{
    public class ReachAreaObjective : Objective
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerReferences>(out var playerRefs))
            {
                OnCompleted();
            }
        }
    }
}
