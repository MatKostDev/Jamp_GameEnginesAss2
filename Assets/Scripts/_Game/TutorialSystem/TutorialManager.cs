using System;
using System.Runtime.InteropServices;
using Jampacked.ProjectInca.Events;
using UnityEngine;

namespace Jampacked.ProjectInca
{
    public class TutorialManager : MonoBehaviour
    {
        static EventDispatcher m_dispatcher;

        private const string DLL_NAME = "JampGameEnginesAss2DLL";

        [DllImport(DLL_NAME)]
        private static extern void ResetMetrics();

        [DllImport(DLL_NAME)]
        private static extern int GetNumWallRuns();

        [DllImport(DLL_NAME)]
        private static extern float GetDamageDealt();

        [DllImport(DLL_NAME)]
        private static extern float GetCompletedTime();

        [DllImport(DLL_NAME)]
        private static extern float GetWeaponAccuracy();

        [DllImport(DLL_NAME)]
        private static extern void IncrementNumWallRuns();

        [DllImport(DLL_NAME)]
        private static extern void AddDamageDealt(float a_damageToAdd);

        [DllImport(DLL_NAME)]
        private static extern void SetCompletedTime(float a_timeCompleted);

        [DllImport(DLL_NAME)]
        private static extern void ProcessWeaponFired(bool a_wasHit);

        public static int GetTutorialNumWallRuns()
        {
            return GetNumWallRuns();
        }

        public static float GetTutorialDamageDealt()
        {
            return GetDamageDealt();
        }

        public static float GetTutorialTimeCompleted()
        {
            return GetCompletedTime();
        }

        public static float GetTutorialWeaponAccuracy()
        {
            return GetWeaponAccuracy();
        }

        void Start()
        {
            ResetMetrics();

            m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

            m_dispatcher.AddListener<WallRunStartedEvent>(OnWallRunStarted);
            m_dispatcher.AddListener<WeaponFiredEvent>(OnWeaponFired);
            m_dispatcher.AddListener<EnemyDamagedEvent>(OnEnemyDamaged);
            m_dispatcher.AddListener<TutorialCompletedEvent>(OnTutorialCompleted);
        }

        void OnDestroy()
        {
            m_dispatcher.RemoveListener<WallRunStartedEvent>(OnWallRunStarted);
            m_dispatcher.RemoveListener<WeaponFiredEvent>(OnWeaponFired);
            m_dispatcher.RemoveListener<EnemyDamagedEvent>(OnEnemyDamaged);
            m_dispatcher.RemoveListener<TutorialCompletedEvent>(OnTutorialCompleted);
        }

        static void OnWallRunStarted(in Events.Event a_evt)
        {
            if (!(a_evt is WallRunStartedEvent wallRunEvent))
            {
                return;
            }

            IncrementNumWallRuns();
        }

        static void OnWeaponFired(in Events.Event a_evt)
        {
            if (!(a_evt is WeaponFiredEvent firedEvent))
            {
                return;
            }

            ProcessWeaponFired(firedEvent.didHitEnemy);
        }

        static void OnEnemyDamaged(in Events.Event a_evt)
        {
            if (!(a_evt is EnemyDamagedEvent damagedEvent))
            {
                return;
            }

            AddDamageDealt(damagedEvent.damageDealt);
        }

        static void OnTutorialCompleted(in Events.Event a_evt)
        {
            if (!(a_evt is TutorialCompletedEvent completedEvent))
            {
                return;
            }

            SetCompletedTime(completedEvent.timeCompleted);

            m_dispatcher.PostEvent(new MetricsPreparedEvent());
        }
    }
}