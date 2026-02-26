using UnityEngine;
using TMPro;

namespace Environment
{
    /// <summary>
    /// Displays well water storage as a hovering UI above the well.
    /// Shows when player is in range, hides when out of range.
    /// </summary>
    public class WellUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Well well;
        [SerializeField] private Canvas canvas;
        [SerializeField] private TMP_Text waterText;
        [SerializeField] private TMP_Text promptText;
        
        [Header("Positioning")]
        [SerializeField] private Vector3 offset = new Vector3(0, 0, 4f);
        [SerializeField] private bool faceCamera = true;
        [SerializeField] private float textSpacing = 40f;
        
        private Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
            
            // Auto-find well if not assigned
            if (well == null)
                well = GetComponentInParent<Well>();
            
            // Auto-find canvas if not assigned
            if (canvas == null)
                canvas = GetComponentInChildren<Canvas>();
            
            if (canvas != null)
                canvas.worldCamera = mainCamera;
            
            // Set initial visibility
            if (canvas != null)
                canvas.enabled = false;
            
            // Position texts with spacing
            PositionTexts();
        }
        
        void PositionTexts()
        {
            if (waterText != null && promptText != null)
            {
                RectTransform waterRect = waterText.GetComponent<RectTransform>();
                RectTransform promptRect = promptText.GetComponent<RectTransform>();
                
                if (waterRect != null && promptRect != null)
                {
                    // Position water text at top
                    waterRect.anchoredPosition = new Vector2(0, textSpacing / 2);
                    // Position prompt text at bottom
                    promptRect.anchoredPosition = new Vector2(0, -textSpacing / 2);
                }
            }
        }

        void Update()
        {
            if (well == null || canvas == null) return;
            
            // Show/hide based on player proximity
            canvas.enabled = well.PlayerInRange;
            
            if (well.PlayerInRange)
            {
                UpdateText();
                
                // Face camera (better method for UI)
                if (faceCamera && mainCamera != null)
                {
                    transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                                   mainCamera.transform.rotation * Vector3.up);
                }
            }
        }

        void UpdateText()
        {
            if (waterText != null)
            {
                waterText.SetText("Well: {0}/{1}", well.CurrentWaterStored, well.MaxWaterStorage);
                
                // Optional: Color based on water level
                if (well.WaterPercentage < 0.25f)
                    waterText.color = Color.red;
                else if (well.WaterPercentage < 0.5f)
                    waterText.color = Color.yellow;
                else
                    waterText.color = Color.cyan;
            }
            
            if (promptText != null)
            {
                if (well.CurrentWaterStored > 0)
                    promptText.SetText("Press [F] to refill");
                else
                    promptText.SetText("Well is empty");
            }
        }
    }
}
