using UnityEngine;
using UnityEngine.UI;
using Character;

/// <summary>
/// UI display for player energy with visual feedback for regeneration state.
/// </summary>
public class EnergyMeterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnergyResource energyResource;
    [SerializeField] private Slider slider;
    
    [Header("Visual Feedback (Optional)")]
    [Tooltip("Optional: Image component to change color during regeneration")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color regeneratingColor = Color.green;
    [SerializeField] private Color lowEnergyColor = Color.red;
    [SerializeField] private float lowEnergyThreshold = 0.25f;

    void Start()
    {
        // Auto-find references if not set
        if (!slider)
            slider = GetComponent<Slider>();

        if (slider && !fillImage)
            fillImage = slider.fillRect?.GetComponent<Image>();

        if (!energyResource)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
                energyResource = player.GetComponent<EnergyResource>();
        }

        if (!energyResource)
        {
            Debug.LogError("EnergyMeterUI: No EnergyResource found.");
            enabled = false;
            return;
        }

        // Configure slider settings
        if (slider)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
        }

        // Subscribe to energy events
        EnergyResource.OnEnergyChanged += UpdateDisplay;
        EnergyResource.OnRegenerationStarted += OnRegenerationStarted;
        EnergyResource.OnRegenerationStopped += OnRegenerationStopped;

        // Initial update
        UpdateDisplay(energyResource.CurrentEnergy, energyResource.MaxEnergy);
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        EnergyResource.OnEnergyChanged -= UpdateDisplay;
        EnergyResource.OnRegenerationStarted -= OnRegenerationStarted;
        EnergyResource.OnRegenerationStopped -= OnRegenerationStopped;
    }

    void Update()
    {
        if (!energyResource || !slider) return;

        // Update slider value
        slider.value = energyResource.Normalized; // 0–1

        // Update color based on state
        UpdateColor();
    }

    private void UpdateDisplay(float current, float max)
    {
        if (!slider) return;
        slider.value = current / max;
    }

    private void UpdateColor()
    {
        if (!fillImage) return;

        // Priority: Low energy warning > Regenerating > Normal
        if (energyResource.Normalized <= lowEnergyThreshold)
        {
            fillImage.color = lowEnergyColor;
        }
        else if (energyResource.IsRegenerating)
        {
            fillImage.color = regeneratingColor;
        }
        else
        {
            fillImage.color = normalColor;
        }
    }

    private void OnRegenerationStarted()
    {
        Debug.Log("[EnergyMeterUI] Energy regeneration started - changing color");
    }

    private void OnRegenerationStopped()
    {
        Debug.Log("[EnergyMeterUI] Energy regeneration stopped");
    }
}
