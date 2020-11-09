using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jampacked.ProjectInca
{
    [RequireComponent(typeof(Collider))]
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
