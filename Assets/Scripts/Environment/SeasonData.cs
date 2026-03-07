using UnityEngine;

[CreateAssetMenu(fileName = "SeasonData", menuName = "Scriptable Objects/SeasonData")]
public class SeasonData : ScriptableObject
{
    [Range(-20f, 140f)]
    [Tooltip("Temperature in Fahrenheit")]
    public float avgTemp;
    [Range(0f, 24f)]
    [Tooltip("Daylight in Hours")]
    public float dayLength;

    public Color sunColor;
}