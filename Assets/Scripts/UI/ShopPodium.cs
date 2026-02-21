using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Character;
using Farming;

/// <summary>
/// Individual shop podium that displays an item for purchase.
/// Player walks up and presses E to buy.
/// </summary>
public class ShopPodium : MonoBehaviour
{
    public enum ItemType { Water, Seeds, WaterCapacityUpgrade }

    [Header("Item Configuration")]
    [SerializeField] private ItemType itemType = ItemType.Water;
    [SerializeField] private string itemName = "Water";
    [SerializeField] private int itemCost = 5;
    [SerializeField] private int itemAmount = 1; // How many units to give (e.g., 5 water, 10 seeds)
    
    [Header("Interaction")]
    [SerializeField] private Key interactKey = Key.F;
    [SerializeField] private Vector3 triggerOffset = new Vector3(0f, 0f, 0f); // Offset in front of podium
    [SerializeField] private float triggerRadius = 1.5f;
    
    [Header("UI (Optional - auto-creates if null)")]
    [SerializeField] private Canvas podiumCanvas;
    [SerializeField] private float uiHeight = 2f; // Height above podium
    
    private GameObject promptUI;
    private TextMeshProUGUI promptText;
    private WaterResource playerWater;
    private PlayerEconomy playerEconomy;
    private SeedInventory playerSeeds;
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
            Debug.Log("[ShopPodium] Auto-created trigger collider");
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
        // Handle purchase input when player is in range
        if (playerInRange && Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
        {
            TryPurchase();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ShopPodium] Trigger entered by: {other.gameObject.name}, Tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            // Try to get components from the player
            playerWater = other.GetComponent<WaterResource>();
            playerSeeds = other.GetComponent<SeedInventory>();
            playerEconomy = other.GetComponent<PlayerEconomy>();
            
            // If PlayerEconomy is on a separate GameObject, find it in the scene
            if (playerEconomy == null)
            {
                playerEconomy = FindFirstObjectByType<PlayerEconomy>();
                Debug.Log($"[ShopPodium] PlayerEconomy found in scene: {playerEconomy != null}");
            }
            
            playerInRange = true;
            ShowPrompt();
            Debug.Log($"[ShopPodium] Player in range - showing prompt for {itemName}, Economy: {playerEconomy != null}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
            Debug.Log("[ShopPodium] Player left range - hiding prompt");
        }
    }

    void CreatePromptUI()
    {
        // Create canvas if needed
        if (podiumCanvas == null)
        {
            GameObject canvasObj = new GameObject("Podium Canvas");
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
        return $"[{keyName}] Buy {itemName}\n${itemCost}";
    }

    void ShowPrompt()
    {
        if (promptUI != null)
        {
            // Refresh the prompt text to reflect current key binding
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

        // Check if player can afford
        bool canAfford = playerEconomy != null && playerEconomy.CurrentMoney >= itemCost;
        promptText.color = canAfford ? Color.green : Color.red;
        promptText.text = GetPromptText();
    }

    void TryPurchase()
    {
        if (playerEconomy == null)
        {
            Debug.LogWarning("[ShopPodium] No PlayerEconomy found!");
            return;
        }

        // Check if player has enough money
        if (playerEconomy.CurrentMoney < itemCost)
        {
            Debug.Log($"[ShopPodium] Not enough money! Need ${itemCost}, have ${playerEconomy.CurrentMoney}");
            return;
        }

        // Process purchase based on item type
        bool success = false;
        switch (itemType)
        {
            case ItemType.Water:
                if (playerWater != null)
                {
                    playerWater.AddWater(itemAmount);
                    success = true;
                    Debug.Log($"[ShopPodium] Purchased {itemAmount} water for ${itemCost}");
                }
                break;

            case ItemType.Seeds:
                if (playerSeeds != null)
                {
                    playerSeeds.AddSeeds(itemAmount);
                    success = true;
                    Debug.Log($"[ShopPodium] Purchased {itemAmount} seeds for ${itemCost}");
                }
                break;

            case ItemType.WaterCapacityUpgrade:
                if (playerWater != null)
                {
                    int newMax = playerWater.MaxWater + itemAmount;
                    playerWater.UpgradeCapacity(newMax);
                    success = true;
                    Debug.Log($"[ShopPodium] Upgraded water capacity by {itemAmount} for ${itemCost}");
                }
                break;
        }

        // Deduct money if purchase was successful
        if (success)
        {
            playerEconomy.TrySpend(itemCost);
            UpdatePromptText();
        }
    }

    void LateUpdate()
    {
        // Keep canvas facing camera
        if (podiumCanvas != null && Camera.main != null)
        {
            podiumCanvas.transform.LookAt(Camera.main.transform);
            podiumCanvas.transform.Rotate(0f, 180f, 0f);
        }
    }

    // Visualize trigger range in editor
    void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = Color.yellow;
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
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Vector3 worldCenter = transform.TransformPoint(triggerOffset);
            Gizmos.DrawWireSphere(worldCenter, triggerRadius);
        }
    }
}
