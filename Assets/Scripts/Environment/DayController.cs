using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Farming;

namespace Environment 
{
    public class DayController : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private Light sunLight;
        [SerializeField] private TMP_Text dayLabel;
        [SerializeField] private PlayerEconomy playerEconomy;
        [SerializeField] private SeasonManager seasonManager;

        [Header("Economy")]
        [SerializeField] private int moneyPerDay = 10;

        [Header("Time Settings")]
        [SerializeField] private float dayLengthSeconds = 60f;
        [SerializeField] private float dayProgressSeconds = 0f; // Debugging from editor
        [SerializeField] private int currentDay = 1;

        // Properties
        public float DayProgressPercent => Mathf.Clamp01(dayProgressSeconds / dayLengthSeconds);
        public int CurrentDay => currentDay;

        public UnityEvent dayPassedEvent = new UnityEvent(); // Invoked at end of day

        public void AdvanceDay()
        {
            // Reset day timer
            dayProgressSeconds = 0f;
            currentDay++;

            // Update day label
            if (dayLabel != null)
                dayLabel.SetText("Day: {0}", currentDay);

            // Award daily money
            if (playerEconomy != null)
            {
                playerEconomy.EarnMoney(moneyPerDay);
                Debug.Log($"[DayController] Day {currentDay} complete! Earned ${moneyPerDay}");
            }

            // Notify SeasonManager
            if (seasonManager != null)
            {
                seasonManager.AdvanceDay(); // Updates season, week, and labels
            }

            // Notify listeners
            dayPassedEvent.Invoke();
        }

        public void UpdateVisuals()
        {
            if (sunLight != null)
            {
                // Sun rotation: 0 = sunrise, 180 = sunset, 360 = next sunrise
                float sunRotationX = Mathf.Lerp(0f, 360f, DayProgressPercent);
                sunLight.transform.rotation = Quaternion.Euler(sunRotationX, 0f, 0f);
            }
        }

        void Update()
        {
            dayProgressSeconds += Time.deltaTime;

            if (dayProgressSeconds >= dayLengthSeconds)
            {
                AdvanceDay();
            }

            UpdateVisuals();
        }
    }
}