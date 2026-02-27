using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Character;
using Farming;

/// <summary>
/// Hotbar UI that shows the watering can icon with remaining water count above it,
/// and the player's current money.
/// </summary>
public class HotbarUI : MonoBehaviour
{
    public enum ToolType { WateringCan = 0, Seeds = 1, HarvestTool = 2 }

    private static HotbarUI instance;
    public static HotbarUI Instance => instance;
    [Header("References (auto-created if null)")]
    [SerializeField] private Canvas hotbarCanvas;
    [SerializeField] private Texture2D wateringCanTexture; // Art/UI/Adament_Watering_can.webp
    [SerializeField] private Texture2D seedTexture;        // Art/UI/seeds.png
    [SerializeField] private Texture2D plantTexture;       // For harvested plants icon
    
    [Header("Sizing")]
    [SerializeField] private float iconSize = 64f;
    [SerializeField] private float bottomMargin = 20f;
    [SerializeField] private float fontSize = 24f;

    private Image wateringCanImage;
    private TextMeshProUGUI waterCountText;
    private TextMeshProUGUI moneyText;
    private Image seedImage;
    private TextMeshProUGUI seedCountText;
    private Image plantImage;
    private TextMeshProUGUI plantCountText;

    // Tool selection
    private int selectedSlot = 0; // 0 = watering can, 1 = seeds, 2 = harvest tool
    private GameObject wateringCanSlot;
    private GameObject seedSlot;
    private GameObject plantSlot;
    private Image wateringCanBorder;
    private Image seedBorder;
    private Image plantBorder;

    public ToolType SelectedTool => (ToolType)selectedSlot;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupCanvas();
        CreateHotbar();
        SelectSlot(0); // Start with watering can selected
    }

    void Update()
    {
        // Number key input for slot selection
        if (Keyboard.current != null)
        {
            if (Keyboard.current[Key.Digit1].wasPressedThisFrame || Keyboard.current[Key.Numpad1].wasPressedThisFrame)
            {
                SelectSlot(0);
            }
            else if (Keyboard.current[Key.Digit2].wasPressedThisFrame || Keyboard.current[Key.Numpad2].wasPressedThisFrame)
            {
                SelectSlot(1);
            }
            else if (Keyboard.current[Key.Digit3].wasPressedThisFrame || Keyboard.current[Key.Numpad3].wasPressedThisFrame)
            {
                SelectSlot(2);
            }
        }
    }

    void OnEnable()
    {
        WaterResource.OnWaterChanged += UpdateWaterDisplay;
        PlayerEconomy.OnMoneyChanged += UpdateMoneyDisplay;
        SeedInventory.OnSeedsChanged += UpdateSeedDisplay;
        PlantInventory.OnPlantsChanged += UpdatePlantDisplay;
        
        // Request initial values after subscribing
        RefreshDisplays();
    }

    void OnDisable()
    {
        WaterResource.OnWaterChanged -= UpdateWaterDisplay;
        PlayerEconomy.OnMoneyChanged -= UpdateMoneyDisplay;
        SeedInventory.OnSeedsChanged -= UpdateSeedDisplay;
        PlantInventory.OnPlantsChanged -= UpdatePlantDisplay;
    }

    void SetupCanvas()
    {
        if (hotbarCanvas != null) return;

        GameObject canvasObj = new GameObject("Hotbar Canvas");
        hotbarCanvas = canvasObj.AddComponent<Canvas>();
        hotbarCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        hotbarCanvas.sortingOrder = 90;

        DontDestroyOnLoad(canvasObj);

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
    }

    void CreateHotbar()
    {
        float slotWidth  = iconSize + 40f;
        float slotHeight = iconSize + 50f;
        float slotSpacing = 8f;
        int   slotCount  = 3; // Updated to 3 slots (watering can, seeds, harvested plants)
        float totalWidth = slotWidth * slotCount + slotSpacing * (slotCount - 1);

        // =============================
        // HOTBAR CONTAINER (bottom center)
        // =============================
        GameObject hotbarObj = new GameObject("Hotbar");
        hotbarObj.transform.SetParent(hotbarCanvas.transform, false);

        RectTransform hotbarRect = hotbarObj.AddComponent<RectTransform>();
        hotbarRect.anchorMin = new Vector2(0.5f, 0f);
        hotbarRect.anchorMax = new Vector2(0.5f, 0f);
        hotbarRect.pivot     = new Vector2(0.5f, 0f);
        hotbarRect.anchoredPosition = new Vector2(0, bottomMargin);
        hotbarRect.sizeDelta = new Vector2(totalWidth, slotHeight);

        Image hotbarBg = hotbarObj.AddComponent<Image>();
        hotbarBg.color = new Color(0f, 0f, 0f, 0.4f);

        // =============================
        // SLOT 1 — WATERING CAN (left)
        // =============================
        float leftX = -(slotWidth + slotSpacing);
        
        wateringCanSlot = CreateSlotContainer(hotbarObj.transform, "Watering Can Slot", new Vector2(leftX, 0f), slotWidth, slotHeight, out wateringCanBorder);
        
        wateringCanImage = CreateSlotIcon(wateringCanSlot.transform, "Watering Can Icon",
            wateringCanTexture, new Color(0.3f, 0.6f, 1f, 1f), new Vector2(0f, -5f));

        waterCountText = CreateSlotLabel(wateringCanSlot.transform, "Water Count",
            new Color(0.4f, 0.8f, 1f), new Vector2(0f, 25f));
        waterCountText.text = "10";

        // =============================
        // SLOT 2 — SEEDS (middle)
        // =============================
        
        seedSlot = CreateSlotContainer(hotbarObj.transform, "Seed Slot", new Vector2(0f, 0f), slotWidth, slotHeight, out seedBorder);
        
        seedImage = CreateSlotIcon(seedSlot.transform, "Seed Icon",
            seedTexture, new Color(0.4f, 0.8f, 0.2f, 1f), new Vector2(0f, -5f));

        seedCountText = CreateSlotLabel(seedSlot.transform, "Seed Count",
            new Color(0.6f, 0.9f, 0.3f), new Vector2(0f, 25f));
        seedCountText.text = "5";

        // =============================
        // SLOT 3 — HARVESTED PLANTS (right)
        // =============================
        float rightX = slotWidth + slotSpacing;
        
        plantSlot = CreateSlotContainer(hotbarObj.transform, "Plant Slot", new Vector2(rightX, 0f), slotWidth, slotHeight, out plantBorder);
        
        plantImage = CreateSlotIcon(plantSlot.transform, "Plant Icon",
            plantTexture, new Color(0.9f, 0.5f, 0.2f, 1f), new Vector2(0f, -5f));

        plantCountText = CreateSlotLabel(plantSlot.transform, "Plant Count",
            new Color(0.9f, 0.7f, 0.3f), new Vector2(0f, 25f));
        plantCountText.text = "0";

        // =============================
        // MONEY DISPLAY (top right)
        // =============================
        GameObject moneyObj = new GameObject("Money Display");
        moneyObj.transform.SetParent(hotbarCanvas.transform, false);

        Image moneyBg = moneyObj.AddComponent<Image>();
        moneyBg.color = new Color(0f, 0f, 0f, 0.4f);

        RectTransform moneyRect = moneyBg.rectTransform;
        moneyRect.anchorMin = new Vector2(1f, 1f);
        moneyRect.anchorMax = new Vector2(1f, 1f);
        moneyRect.pivot     = new Vector2(1f, 1f);
        moneyRect.sizeDelta = new Vector2(160f, 40f);
        moneyRect.anchoredPosition = new Vector2(-20f, -20f);

        GameObject moneyTextObj = new GameObject("Money Text");
        moneyTextObj.transform.SetParent(moneyObj.transform, false);

        moneyText = moneyTextObj.AddComponent<TextMeshProUGUI>();
        moneyText.text      = "$0";
        moneyText.fontSize  = fontSize;
        moneyText.alignment = TextAlignmentOptions.Center;
        moneyText.color     = new Color(1f, 0.9f, 0.3f);

        RectTransform moneyTextRect = moneyText.rectTransform;
        moneyTextRect.anchorMin       = Vector2.zero;
        moneyTextRect.anchorMax       = Vector2.one;
        moneyTextRect.sizeDelta       = Vector2.zero;
        moneyTextRect.anchoredPosition = Vector2.zero;
    }

    // -------------------------------------------------- helpers

    GameObject CreateSlotContainer(Transform parent, string name, Vector2 pos, float width, float height, out Image border)
    {
        GameObject container = new GameObject(name);
        container.transform.SetParent(parent, false);

        RectTransform rt = container.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = pos;

        // Selection border (initially hidden)
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(container.transform, false);

        border = borderObj.AddComponent<Image>();
        border.color = new Color(1f, 1f, 0f, 0.8f); // Yellow highlight

        RectTransform borderRt = border.rectTransform;
        borderRt.anchorMin = Vector2.zero;
        borderRt.anchorMax = Vector2.one;
        borderRt.sizeDelta = new Vector2(4f, 4f); // Slightly larger for border effect
        borderRt.anchoredPosition = Vector2.zero;
        border.enabled = false;

        return container;
    }

    Image CreateSlotIcon(Transform parent, string name, Texture2D tex, Color fallbackColor, Vector2 pos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        Image img = obj.AddComponent<Image>();
        if (tex != null)
        {
            img.sprite = Sprite.Create(tex,
                new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }
        else
        {
            img.color = fallbackColor;
        }
        img.preserveAspect = true;

        RectTransform rt = img.rectTransform;
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.sizeDelta        = new Vector2(iconSize, iconSize);
        rt.anchoredPosition = pos;
        return img;
    }

    TextMeshProUGUI CreateSlotLabel(Transform parent, string name, Color color, Vector2 anchorPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        TextMeshProUGUI label = obj.AddComponent<TextMeshProUGUI>();
        label.fontSize  = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color     = color;

        RectTransform rt = label.rectTransform;
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.sizeDelta        = new Vector2(100f, 30f);
        rt.anchoredPosition = anchorPos;
        return label;
    }

    void SelectSlot(int slotIndex)
    {
        selectedSlot = slotIndex;

        // Update visual indicators
        if (wateringCanBorder != null)
            wateringCanBorder.enabled = (slotIndex == 0);
        if (seedBorder != null)
            seedBorder.enabled = (slotIndex == 1);
        if (plantBorder != null)
            plantBorder.enabled = (slotIndex == 2);

        Debug.Log($"[HotbarUI] Selected tool: {(ToolType)slotIndex}");
    }

    void UpdateWaterDisplay(int current, int max)
    {
        if (waterCountText != null)
        {
            waterCountText.text = current.ToString();

            float ratio = (float)current / max;
            if (ratio <= 0f)
                waterCountText.color = Color.red;
            else if (ratio <= 0.3f)
                waterCountText.color = new Color(1f, 0.5f, 0.2f);
            else
                waterCountText.color = new Color(0.4f, 0.8f, 1f);
        }
    }

    void UpdateMoneyDisplay(int amount)
    {
        if (moneyText != null)
            moneyText.text = $"${amount}";
    }

    void UpdateSeedDisplay(int count)
    {
        if (seedCountText != null)
        {
            seedCountText.text = count.ToString();
            seedCountText.color = count > 0
                ? new Color(0.6f, 0.9f, 0.3f)   // green — seeds available
                : Color.red;                      // red — out of seeds
        }
    }
    
    void UpdatePlantDisplay(int count)
    {
        if (plantCountText != null)
        {
            plantCountText.text = count.ToString();
            plantCountText.color = count > 0
                ? new Color(0.9f, 0.7f, 0.3f)   // orange — plants available
                : new Color(0.5f, 0.5f, 0.5f);  // gray — no plants
        }
    }
    
    void RefreshDisplays()
    {
        // Find and request current values from all systems
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var waterResource = player.GetComponent<Character.WaterResource>();
            if (waterResource != null)
            {
                UpdateWaterDisplay(waterResource.CurrentWater, waterResource.MaxWater);
            }
            
            var seedInventory = player.GetComponent<Farming.SeedInventory>();
            if (seedInventory != null)
            {
                UpdateSeedDisplay(seedInventory.CurrentSeeds);
            }
            
            var plantInventory = player.GetComponent<Farming.PlantInventory>();
            if (plantInventory != null)
            {
                UpdatePlantDisplay(plantInventory.CurrentPlants);
            }
        }
        
        var economy = FindFirstObjectByType<PlayerEconomy>();
        if (economy != null)
        {
            UpdateMoneyDisplay(economy.CurrentMoney);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
