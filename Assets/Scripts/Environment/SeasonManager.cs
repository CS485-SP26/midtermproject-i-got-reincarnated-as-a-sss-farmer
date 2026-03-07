using UnityEngine;
using TMPro;
//using Unity.VisualScripting;

namespace Environment
{
    public class SeasonManager : MonoBehaviour
    {
        public enum Season { Spring, Summer, Fall, Winter, Count }
        public enum DayOfWeek { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }

        [Header("Season Data")]
        [SerializeField] private SeasonData[] seasons = new SeasonData[(int)Season.Count];

        [Header("Calendar Settings")]
        [SerializeField] private int weeksPerSeason = 8;

        [Header("UI")]
        [SerializeField] private TMP_Text dayLabel;
        [SerializeField] private TMP_Text seasonLabel;
        [SerializeField] private TMP_Text calendarLabel;

        [Header("Environment References")]
        [SerializeField] private Light sunLight;
        [SerializeField] private float dayDurationInSeconds = 120f;


        private Season currentSeason = Season.Summer;
        private DayOfWeek currentDay = DayOfWeek.Monday;

        private float currentTime = 0f;

        [SerializeField] private int dayNumber = 1;
        private int weekNumber = 1;

        private SeasonData scratchData;

        // from January to December, amount of days per month
        private readonly int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }; // Jan → Dec
        private readonly string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        private int startMonthIndex = 5; // June is index 5
        private int startDayOffset = 6;   // June 7

        // Safe to edit this data without destroying asset
        public SeasonData RuntimeData
        {
            get
            {
                return scratchData;
            }

            set
            {
                // create a safe clone of the data
                scratchData = Instantiate(value);
            }
        }
        void Start()
        {
            currentSeason = Season.Summer;   // Start in Summer

            SetSeason(currentSeason);
            UpdateCalendarUI();
        }


        public void AdvanceDay()
        {
            dayNumber++;

            // Advance day of week
            currentDay = (DayOfWeek)(((int)currentDay + 1) % 7);

            // Calculate current season: each season = 60 days, start Summer
            int seasonOffset = 1; // Summer = 1
            int seasonIndex = (seasonOffset + (dayNumber - 1) / 60) % (int)Season.Count;
            currentSeason = (Season)seasonIndex;
            SetSeason(currentSeason);

            // Calculate week in current season
            weekNumber = ((dayNumber - 1) % 60) / 7 + 1;

            UpdateCalendarUI();
        }

        void NextSeason()
        {
            currentSeason = (Season)(((int)currentSeason + 1) % (int)Season.Count);
            SetSeason(currentSeason);
        }

        public void SetSeason(Season value)
        {
            currentSeason = value;
            RuntimeData = seasons[(int)value];
        }

        void UpdateCalendarUI()
        {
            // Total day label
            if (dayLabel != null)
                dayLabel.SetText("Day: {0}", dayNumber);

            // Season label
            if (seasonLabel != null)
                seasonLabel.SetText(currentSeason.ToString());

            // Calendar label (Day of Week + Month + Day)
            if (calendarLabel != null)
            {
                int totalDays = dayNumber - 1 + startDayOffset;
                int monthIndex = startMonthIndex;
                int dayOfMonth = 0;

                while (true)
                {
                    int daysInCurrentMonth = daysInMonth[monthIndex];
                    if (totalDays < daysInCurrentMonth)
                    {
                        dayOfMonth = totalDays + 1;
                        break;
                    }
                    else
                    {
                        totalDays -= daysInCurrentMonth;
                        monthIndex = (monthIndex + 1) % daysInMonth.Length;
                    }
                }

                string monthName = monthNames[monthIndex];
                calendarLabel.SetText(string.Format("{0}, {1} {2}", currentDay.ToString(), monthName, dayOfMonth));
            }
        }

        // Useful getters for other systems (plants, weather, etc)
        public Season CurrentSeason => currentSeason;
        public DayOfWeek CurrentDay => currentDay;
        public int CurrentWeek => weekNumber;
        public int CurrentDayNumber => dayNumber;

        // Update is called once per frame
        void Update()
        {
            UpdateTime();
            UpdateSunPosition();
        }

        void UpdateTime()
        {
            currentTime += Time.deltaTime;

            if (currentTime >= dayDurationInSeconds)
            {
                currentTime = 0;
                AdvanceDay();
            }
        }



        void UpdateSunPosition()
        {
            // Challenge: Map currentTime (0 to dayDuration) to a 360 degree rotation
            float timePercent = currentTime / dayDurationInSeconds;
            float sunAngle = timePercent * 360f;

            // Offset by -90 or 90 to make "noon" be directly overhead
            if (sunLight != null)
                sunLight.transform.rotation = Quaternion.Euler(sunAngle - 90f, 170f, 0f);
        }

        public float GetSeasonalGrowthMultiplier()
        {
            // Return a multiplier depending on season
            switch (currentSeason)
            {
                case Season.Spring:
                    return 1.2f; // Faster growth in Spring
                case Season.Summer:
                    return 1.5f; // Fastest growth in Summer
                case Season.Fall:
                    return 0.8f; // Slower growth in Fall
                case Season.Winter:
                    return 0.5f; // Slowest growth in Winter
                default:
                    return 1f;
            }
        }
    }

}