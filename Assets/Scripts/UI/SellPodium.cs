using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Character;
using Farming;

/// <summary>
/// Individual sell podium that allows selling plants for money.
/// Player walks up and presses E to sell all plants.
/// </summary>
public class SellPodium : MonoBehaviour
{
    [Header("Sell Configuration")]
    [SerializeField] private string itemName = "Plants";
    [SerializeField] private int pricePerPlant = 10;
    [SerializeField] private bool sellAll = true; // If true, sells all plants at once. If false, sells one at a time.
    
    [Header("Interaction")]
    [SerializeField] private Key interactKey = Key.F;
    [SerializeField] private Vector3 triggerOffset = new Vector3(0f, 0f, 0f); // Offset in front of podium
    [SerializeField] private float triggerRadius = 1.5f;
    
    [Header("UI (Optional - auto-creates if null)")]
    [SerializeField] private Canvas podiumCanvas;
    [SerializeField] private float uiHeight = 2f; // Height above podium
    
    private GameObject promptUI;
    private TextMeshProUGUI promptText;
    private PlantInventory playerPlants;
    private PlayerEconomy playerEconomy;
    private bool playerInRange;

    void Start()
    {
        CreatePromptUI();
        HidePrompt();
        
        // Ensure we have a trigger collider
        if (GetComponent<Collider>() == null)
        {
            SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = triggerRadius;
            trigger.center = triggerOffset; // Position in front of podium
            Debug.Log("[SellPodium] Auto-created trigger collider");
        }
        else
        {
            // Update existing collider settings
            SphereCollider existingTrigger = GetComponent<SphereCollider>();
            if (existingTrigger != null)
            {
                existingTrigger.isTrigger = true;
                existingTrigger.radius = triggerRadius;
                existingTrigger.center = triggerOffset;
            }
        }
    }

    void Update()
    {
        // Handle sell input when player is in range
        if (playerInRange && Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
        {
            TrySell();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[SellPodium] Trigger entered by: {other.gameObject.name}, Tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            // Try to get components from the player
            playerPlants = other.GetComponent<PlantInventory>();
            playerEconomy = other.GetComponent<PlayerEconomy>();
            
            // If PlayerEconomy is on a separate GameObject, find it in the scene
            if (playerEconomy == null)
            {
                playerEconomy = FindFirstObjectByType<PlayerEconomy>();
                Debug.Log($"[SellPodium] PlayerEconomy found in scene: {playerEconomy != null}");
            }
            
            playerInRange = true;
            ShowPrompt();
            Debug.Log($"[SellPodium] Player in range - showing sell prompt, Economy: {playerEconomy != null}, Plants: {playerPlants != null}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
            Debug.Log("[SellPodium] Player left range - hiding prompt");
        }
    }

    void CreatePromptUI()
    {
        // Create canvas if needed
        if (podiumCanvas == null)
        {
            GameObject canvasObj = new GameObject("Sell Podium Canvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = Vector3.up * uiHeight;
            
            podiumCanvas = canvasObj.AddComponent<Canvas>();
            podiumCanvas.renderMode = RenderMode.WorldSpace;
            
            // Size the canvas
            RectTransform canvasRect = podiumCanvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(200f, 100f);
            canvasRect.localScale = Vector3.one * 0.01f; // Scale down for world space
            
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create prompt panel
        promptUI = new GameObject("Prompt");
        promptUI.transform.SetParent(podiumCanvas.transform, false);

        Image bg = promptUI.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.8f);

        RectTransform bgRect = bg.rectTransform;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(promptUI.transform, false);

        promptText = textObj.AddComponent<TextMeshProUGUI>();
        promptText.text = GetPromptText();
        promptText.fontSize = 14f;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = Color.white;

        RectTransform textRect = promptText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-10f, -10f);
        textRect.anchoredPosition = Vector2.zero;

        // Make canvas face camera
        if (Camera.main != null)
        {
            podiumCanvas.transform.LookAt(Camera.main.transform);
            podiumCanvas.transform.Rotate(0f, 180f, 0f);
        }
    }

    string GetPromptText()
    {
        string keyName = interactKey.ToString().ToUpper();
        
        if (playerPlants == null || playerPlants.CurrentPlants == 0)
        {
            return $"No {itemName} to sell";
        }
        
        if (sellAll)
        {
            int totalValue = playerPlants.CurrentPlants * pricePerPlant;
            return $"[{keyName}] Sell All {itemName}\n{playerPlants.CurrentPlants}x for ${totalValue}";
        }
        else
        {
            return $"[{keyName}] Sell 1 {itemName}\n${pricePerPlant}";
        }
    }

    void ShowPrompt()
    {
        if (promptUI != null)
        {
            // Refresh the prompt text to reflect current inventory
            if (promptText != null)
            {
                promptText.text = GetPromptText();
            }
            promptUI.SetActive(true);
            UpdatePromptText();
        }
    }

    void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void UpdatePromptText()
    {
        if (promptText == null) return;

        // Check if player has plants to sell
        bool hasPlants = playerPlants != null && playerPlants.CurrentPlants > 0;
        promptText.color = hasPlants ? Color.green : Color.gray;
        promptText.text = GetPromptText();
    }

    void TrySell()
    {
        if (playerEconomy == null)
        {
            Debug.LogWarning("[SellPodium] No PlayerEconomy found!");
            return;
        }

        if (playerPlants == null)
        {
            Debug.LogWarning("[SellPodium] No PlantInventory found!");
            return;
        }

        // Check if player has plants to sell
        if (playerPlants.CurrentPlants <= 0)
        {
            Debug.Log("[SellPodium] No plants to sell!");
            return;
        }

        // Sell plants
        if (sellAll)
        {
            // Sell all plants at once
            int plantCount = playerPlants.CurrentPlants;
            int totalRevenue = plantCount * pricePerPlant;
            int soldCount = playerPlants.SellAll();
            playerEconomy.AddMoney(totalRevenue);
            
            Debug.Log($"[SellPodium] Sold {soldCount} plants for ${totalRevenue}!");
        }
        else
        {
            // Sell one plant at a time
            if (playerPlants.TryConsumePlant())
            {
                playerEconomy.AddMoney(pricePerPlant);
                Debug.Log($"[SellPodium] Sold 1 plant for ${pricePerPlant}");
            }
        }
        
        UpdatePromptText();
    }

    void LateUpdate()
    {
        // Keep canvas facing camera and update text
        if (podiumCanvas != null && Camera.main != null)
        {
            podiumCanvas.transform.LookAt(Camera.main.transform);
            podiumCanvas.transform.Rotate(0f, 180f, 0f);
        }
        
        // Update prompt text while player is in range (to show current plant count)
        if (playerInRange && promptText != null)
        {
            UpdatePromptText();
        }
    }

    // Visualize trigger range in editor
    void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = Color.cyan; // Different color to distinguish from shop podiums
            if (col is SphereCollider sphere)
            {
                // Show sphere at its actual world position (accounting for center offset)
                Vector3 worldCenter = transform.TransformPoint(sphere.center);
                Gizmos.DrawWireSphere(worldCenter, sphere.radius);
            }
            else if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
        else
        {
            // Preview where the trigger will be created
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f); // Cyan preview
            Vector3 worldCenter = transform.TransformPoint(triggerOffset);
            Gizmos.DrawWireSphere(worldCenter, triggerRadius);
        }
    }
}
