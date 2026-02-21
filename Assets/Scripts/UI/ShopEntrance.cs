using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Core;

/// <summary>
/// Place this on a GameObject at the shop entrance. 
/// Shows an interact prompt when player is near, allows entering the shop scene.
/// </summary>
public class ShopEntrance : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private string shopSceneName = "ShopScene";
    [SerializeField] private string spawnPointName = "ShopSpawn";
    
    [Header("Interaction")]
    [SerializeField] private Key interactKey = Key.F;
    [SerializeField] private string promptMessage = "Enter Shop";
    
    [Header("UI (Optional - auto-creates if null)")]
    [SerializeField] private Canvas promptCanvas;
    [SerializeField] private float uiHeight = 2f; // Height above entrance
    
    private GameObject promptUI;
    private TextMeshProUGUI promptText;
    private bool playerInRange;

    void Start()
    {
        CreatePromptUI();
        HidePrompt();
        
        // Ensure we have a trigger collider
        if (GetComponent<Collider>() == null)
        {
            BoxCollider trigger = gameObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(3f, 3f, 3f);
            Debug.Log("[ShopEntrance] Auto-created trigger collider");
        }
    }

    void Update()
    {
        // Handle enter input when player is in range
        if (playerInRange && Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
        {
            EnterShop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowPrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
        }
    }

    void CreatePromptUI()
    {
        // Create canvas if needed
        if (promptCanvas == null)
        {
            GameObject canvasObj = new GameObject("Shop Entrance Canvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = Vector3.up * uiHeight;
            
            promptCanvas = canvasObj.AddComponent<Canvas>();
            promptCanvas.renderMode = RenderMode.WorldSpace;
            
            // Size the canvas
            RectTransform canvasRect = promptCanvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(200f, 80f);
            canvasRect.localScale = Vector3.one * 0.01f; // Scale down for world space
            
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create prompt panel
        promptUI = new GameObject("Prompt");
        promptUI.transform.SetParent(promptCanvas.transform, false);

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
    }

    string GetPromptText()
    {
        string keyName = interactKey.ToString().ToUpper();
        return $"[{keyName}] {promptMessage}";
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
        }
    }

    void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void EnterShop()
    {
        Debug.Log($"[ShopEntrance] Entering shop: {shopSceneName}");
        
        // Set spawn point so player appears at the right location in the shop
        GameManager.Instance.pendingSpawnPoint = spawnPointName;
        
        // Load the shop scene
        GameManager.Instance.LoadScenebyName(shopSceneName);
    }

    void LateUpdate()
    {
        // Keep canvas facing camera
        if (promptCanvas != null && Camera.main != null)
        {
            promptCanvas.transform.LookAt(Camera.main.transform);
            promptCanvas.transform.Rotate(0f, 180f, 0f);
        }
    }

    // Visualize trigger range in editor
    void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = Color.cyan;
            if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(transform.position, sphere.radius);
            }
            else if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
    }
}
