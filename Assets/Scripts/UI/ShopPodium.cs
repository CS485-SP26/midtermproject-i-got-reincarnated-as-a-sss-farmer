using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Character;
using Farming;

/// <summary>
/// Individual shop podium that displays an item for purchase (or selling).
/// Player walks up and presses a key to buy/sell.
/// </summary>
public class ShopPodium : MonoBehaviour
{
    // Added PlantSell
    public enum ItemType { Water, Seeds, WaterCapacityUpgrade, PlantSell }

    [Header("Item Configuration")]
    [SerializeField] private ItemType itemType = ItemType.Water;
    [SerializeField] private string itemName = "Water";
    [SerializeField] private int itemCost = 5;              // Used for BUY items
    [SerializeField] private int itemAmount = 1;            // Units to buy/sell per key press

    [Header("Plant Sell Settings")]
    [SerializeField] private int plantSellPrice = 15;       // $ per plant

    [Header("Interaction")]
    [SerializeField] private Key interactKey = Key.F;
    [SerializeField] private Vector3 triggerOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float triggerRadius = 1.5f;

    [Header("UI (Optional - auto-creates if null)")]
    [SerializeField] private Canvas podiumCanvas;
    [SerializeField] private float uiHeight = 2f;

    private GameObject promptUI;
    private TextMeshProUGUI promptText;

    private WaterResource playerWater;
    private PlayerEconomy playerEconomy;
    private SeedInventory playerSeeds;

    // NEW: player plant inventory reference (rename to your actual script if needed)
    private PlantInventory playerPlants;

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
            trigger.center = triggerOffset;
            Debug.Log("[ShopPodium] Auto-created trigger collider");
        }
        else
        {
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
        if (playerInRange && Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
        {
            if (itemType == ItemType.PlantSell)
                TrySellPlants();
            else
                TryPurchase();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ShopPodium] Trigger entered by: {other.gameObject.name}, Tag: {other.tag}");
        if (!other.CompareTag("Player")) return;

        // Get components from player
        playerWater = other.GetComponent<WaterResource>();
        playerSeeds = other.GetComponent<SeedInventory>();
        playerEconomy = other.GetComponent<PlayerEconomy>();

        // NEW: plant inventory (rename to match your project)
        playerPlants = other.GetComponent<PlantInventory>();

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

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        HidePrompt();
        Debug.Log("[ShopPodium] Player left range - hiding prompt");
    }

    void CreatePromptUI()
    {
        if (podiumCanvas == null)
        {
            GameObject canvasObj = new GameObject("Podium Canvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = Vector3.up * uiHeight;

            podiumCanvas = canvasObj.AddComponent<Canvas>();
            podiumCanvas.renderMode = RenderMode.WorldSpace;

            RectTransform canvasRect = podiumCanvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(200f, 100f);
            canvasRect.localScale = Vector3.one * 0.01f;

            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        promptUI = new GameObject("Prompt");
        promptUI.transform.SetParent(podiumCanvas.transform, false);

        Image bg = promptUI.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.8f);

        RectTransform bgRect = bg.rectTransform;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

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

        if (Camera.main != null)
        {
            podiumCanvas.transform.LookAt(Camera.main.transform);
            podiumCanvas.transform.Rotate(0f, 180f, 0f);
        }
    }

    string GetPromptText()
    {
        string keyName = interactKey.ToString().ToUpper();

        if (itemType == ItemType.PlantSell)
        {
            // Sell prompt
            int total = plantSellPrice * Mathf.Max(1, itemAmount);
            return $"[{keyName}] Sell {itemAmount} Plant(s)\n+${total}";
        }

        // Buy prompt
        return $"[{keyName}] Buy {itemName}\n${itemCost}";
    }

    void ShowPrompt()
    {
        if (promptUI == null) return;

        if (promptText != null)
            promptText.text = GetPromptText();

        promptUI.SetActive(true);
        UpdatePromptText();
    }

    void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void UpdatePromptText()
    {
        if (promptText == null) return;

        if (itemType == ItemType.PlantSell)
        {
            // Green if player has enough plants to sell
            bool canSell = playerPlants != null && playerPlants.PlantCount >= Mathf.Max(1, itemAmount);
            promptText.color = canSell ? Color.green : Color.red;
            promptText.text = GetPromptText();
            return;
        }

        // Buying logic color
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

        if (playerEconomy.CurrentMoney < itemCost)
        {
            Debug.Log($"[ShopPodium] Not enough money! Need ${itemCost}, have ${playerEconomy.CurrentMoney}");
            return;
        }

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

        if (success)
        {
            playerEconomy.TrySpend(itemCost);
            UpdatePromptText();
        }
    }

    // NEW: Sell plants
    void TrySellPlants()
    {
        if (playerEconomy == null)
        {
            Debug.LogWarning("[ShopPodium] No PlayerEconomy found (needed to add money)!");
            return;
        }

        if (playerPlants == null)
        {
            Debug.LogWarning("[ShopPodium] No PlantInventory found on Player!");
            return;
        }

        int amountToSell = Mathf.Max(1, itemAmount);

        if (playerPlants.PlantCount < amountToSell)
        {
            Debug.Log($"[ShopPodium] Not enough plants to sell. Need {amountToSell}, have {playerPlants.PlantCount}");
            return;
        }

        // Remove plants first
        if (!playerPlants.TryRemovePlants(amountToSell))
        {
            Debug.LogWarning("[ShopPodium] TryRemovePlants failed.");
            return;
        }

        int earned = plantSellPrice * amountToSell;

        // CHANGE IF NEEDED: Use whatever method your economy uses to add money.
        playerEconomy.AddMoney(earned);

        Debug.Log($"[ShopPodium] Sold {amountToSell} plant(s) for +${earned}");
        UpdatePromptText();
    }

    void LateUpdate()
    {
        if (podiumCanvas != null && Camera.main != null)
        {
            podiumCanvas.transform.LookAt(Camera.main.transform);
            podiumCanvas.transform.Rotate(0f, 180f, 0f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = Color.yellow;
            if (col is SphereCollider sphere)
            {
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
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Vector3 worldCenter = transform.TransformPoint(triggerOffset);
            Gizmos.DrawWireSphere(worldCenter, triggerRadius);
        }
    }
}