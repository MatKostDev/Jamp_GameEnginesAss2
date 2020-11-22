using Jampacked.ProjectInca.Events;
using TMPro;
using UnityEngine;

namespace Jampacked.ProjectInca
{
    public class TutorialMetricsUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text timeCompletedDisplay;

        [SerializeField]
        TMP_Text numWallRunsDisplay;

        [SerializeField]
        TMP_Text weaponAccuracyDisplay;

        [SerializeField]
        TMP_Text damageDealtDisplay;

        EventDispatcher m_dispatcher;

        void Start()
        {
            m_dispatcher = GameObject.Find("GlobalEventDispatcher").GetComponent<EventDispatcher>();

            m_dispatcher.AddListener<MetricsPreparedEvent>(OnMetricsPrepared);

            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            m_dispatcher.RemoveListener<MetricsPreparedEvent>(OnMetricsPrepared);
        }

        void OnMetricsPrepared(in Events.Event a_evt)
        {
            if (!(a_evt is MetricsPreparedEvent metricsEvent))
            {
                return;
            }

            float completedMinutes = Mathf.Floor(TutorialManager.GetTutorialTimeCompleted() / 60);
            float completedSeconds = Mathf.RoundToInt(TutorialManager.GetTutorialTimeCompleted() % 60);

            if (completedSeconds < 10f)
            {
                timeCompletedDisplay.text = completedMinutes.ToString() + ":0" + completedSeconds.ToString();
            }
            else
            {
                timeCompletedDisplay.text = completedMinutes.ToString() + ":" + completedSeconds.ToString();
            }

            numWallRunsDisplay.text = TutorialManager.GetTutorialNumWallRuns().ToString();

            //convert accuracy fraction to percentage
            weaponAccuracyDisplay.text = Mathf.RoundToInt((TutorialManager.GetTutorialWeaponAccuracy() * 100f)).ToString() + "%";

            damageDealtDisplay.text = Mathf.RoundToInt(TutorialManager.GetTutorialDamageDealt()).ToString();

            gameObject.SetActive(true);
        }
    }
}
