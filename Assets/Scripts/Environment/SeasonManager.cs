using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public enum DayOfWeek { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }
public enum Season { Spring, Summer, Autumn, Winter }

public class SeasonManager : MonoBehaviour
{
    [Header("Settings")]
    public float dayDurationInSeconds = 120f; //2 minutes per day
    public int daysPerSeason = 28;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI calendarText;

    [Header("Environment References")]
    [SerializeField] private Light sunLight;

    // Internal State
    private float currentTime;
    private int dayNumber = 1;
    private DayOfWeek currentDay = DayOfWeek.Monday;
    private Season currentSeason = Season.Spring;

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

    void AdvanceDay()
    {
        dayNumber++;
        
        // Calculate Day of Week: (dayNumber - 1) % 7 
        currentDay = (DayOfWeek)((dayNumber - 1) % 7);

        // Calculate Season
        int seasonIndex = ((dayNumber - 1) / daysPerSeason) % 4;
        currentSeason = (Season)seasonIndex;

        UpdateCalendarUI();
    }

    void UpdateCalendarUI()
    {
    // We concatenate the strings first, then let SetText handle the number
    string header = currentSeason.ToString() + " - " + currentDay.ToString() + " Day: ";
    calendarText.SetText(header + "{0}", dayNumber);
    }

    void UpdateSunPosition()
    {
        // Challenge: Map currentTime (0 to dayDuration) to a 360 degree rotation
        float timePercent = currentTime / dayDurationInSeconds;
        float sunAngle = timePercent * 360f;
        
        // Offset by -90 or 90 to make "noon" be directly overhead
        sunLight.transform.rotation = Quaternion.Euler(sunAngle - 90f, 170f, 0f);
    }
}
