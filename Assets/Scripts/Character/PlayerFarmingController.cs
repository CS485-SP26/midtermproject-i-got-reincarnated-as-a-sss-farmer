using UnityEngine;
using UnityEngine.InputSystem;
using Farming;

namespace Character
{
    public class PlayerFarmingController : MonoBehaviour
    {
        [SerializeField] private SelectorManager selectorManager;

        AnimatedController animatedController;
        WaterResource waterResource;
        SeedInventory seedInventory;

        bool isWateringActive; // prevents multi-drain per animation
        float wateringStartTime;
        const float WATERING_ANIMATION_DURATION = 5.6f;
        const float MAX_WATERING_DURATION = 8f; // Safety timeout (extra buffer beyond animation)

        void Start()
        {
            animatedController = GetComponent<AnimatedController>();
            waterResource = GetComponent<WaterResource>();
            seedInventory = GetComponent<SeedInventory>();

            if (selectorManager == null)
                selectorManager = GetComponent<SelectorManager>();

            Debug.Assert(selectorManager, "PlayerFarmingController requires a SelectorManager.");

            if (!animatedController)
                Debug.LogWarning("No AnimatedController found.");

            if (!waterResource)
                Debug.LogError("No WaterResource found on Player.");

            if (!seedInventory)
                Debug.LogWarning("No SeedInventory found on Player - planting will be unavailable.");
        }

        void OnEnable()
        {
            // Reset watering state when re-enabled (e.g., after scene transition)
            if (isWateringActive)
            {
                Debug.Log("[PlayerFarmingController] OnEnable: Resetting stuck watering state");
                isWateringActive = false;
                if (animatedController)
                    animatedController.SetWatering(false);
            }
            CancelInvoke(nameof(StopWatering));
        }

        void Update()
        {
            // Safety: force-reset watering state if stuck for too long
            if (isWateringActive && Time.time - wateringStartTime > MAX_WATERING_DURATION)
            {
                Debug.LogWarning("[PlayerFarmingController] Watering stuck for too long - forcing reset!");
                StopWatering();
            }
            
            // Check for tool switches to ensure animation state is clean
            CheckToolSwitch();
        }

        // Track last selected tool to detect switches
        private HotbarUI.ToolType lastSelectedTool = HotbarUI.ToolType.WateringCan;
        
        void CheckToolSwitch()
        {
            if (HotbarUI.Instance == null) return;
            
            HotbarUI.ToolType currentTool = HotbarUI.Instance.SelectedTool;
            
            // If tool changed, ensure we're not stuck in watering animation
            if (currentTool != lastSelectedTool)
            {
                if (isWateringActive)
                {
                    Debug.Log($"[PlayerFarmingController] Tool switched from {lastSelectedTool} to {currentTool} - stopping watering animation");
                    StopWatering();
                }
                lastSelectedTool = currentTool;
            }
        }

        // =============================
        // SWITCH SELECTOR
        // =============================
        public void OnSwitchSelector(InputValue inputValue)
        {
            if (!inputValue.isPressed) return;
            selectorManager?.OnSelectorSwitchInput();
        }

        // =============================
        // TILE INTERACT
        // =============================
        public void OnInteract(InputValue value)
        {    
            if (selectorManager == null)
            {
                Debug.LogError("[PlayerFarmingController] SelectorManager is null!");
                return;
            }
            
            FarmTile tile = selectorManager.GetSelectedTile();
            if (tile == null)
            {
                return;
            }

            // Check which tool is selected in the hotbar
            HotbarUI.ToolType selectedTool = HotbarUI.ToolType.WateringCan;
            if (HotbarUI.Instance != null)
            {
                selectedTool = HotbarUI.Instance.SelectedTool;
            }

            // SEEDS: Plant on watered tiles
            if (selectedTool == HotbarUI.ToolType.Seeds)
            {
                if (tile.GetCondition == FarmTile.Condition.Watered && seedInventory != null && seedInventory.HasSeeds)
                {
                    tile.Plant(seedInventory);
                }
                else if (tile.GetCondition != FarmTile.Condition.Watered)
                {
                    Debug.Log("[PlayerFarmingController] Seeds can only be planted on watered tiles!");
                }
                return;
            }

            // WATERING CAN: Till grass, water tilled land, harvest planted tiles
            if (selectedTool == HotbarUI.ToolType.WateringCan)
            {
                // Handle harvesting planted tiles
                if (tile.GetCondition == FarmTile.Condition.Planted)
                {
                    tile.Harvest();
                    Debug.Log("[PlayerFarmingController] Harvested planted tile");
                    return;
                }
                
                bool success = tile.InteractWithWater(waterResource);
                Debug.Log($"[PlayerFarmingController] Watering can interaction - Success: {success}, Tile condition: {tile.GetCondition}");

                if (success && tile.GetCondition == FarmTile.Condition.Watered)
                {
                    TryWater();
                }
            }
        }

        // =============================
        // CORE WATER LOGIC (animation only - water consumption handled by FarmTile.InteractWithWater)
        // =============================
        void TryWater()
        {
            if (!animatedController)
                return;

            // Cancel any pending StopWatering calls (safety)
            CancelInvoke(nameof(StopWatering));

            // If already watering, restart the animation smoothly
            if (isWateringActive)
            {
                // Restart animation from beginning without transitioning out
                animatedController.RestartWateringAnimation();
            }
            else
            {
                // Start fresh animation
                animatedController.SetWatering(true);
            }

            // Lock until animation ends
            isWateringActive = true;
            wateringStartTime = Time.time;

            // Schedule stop after animation completes
            Invoke(nameof(StopWatering), WATERING_ANIMATION_DURATION);
        }

        void StopWatering()
        {
            Debug.Log("[PlayerFarmingController] StopWatering called - resetting animation state");
            isWateringActive = false;

            if (animatedController)
            {
                animatedController.SetWatering(false);
                Debug.Log("[PlayerFarmingController] Called SetWatering(false)");
            }
        }

        void OnDisable()
        {
            // Safety: cancel any pending invokes when disabled
            CancelInvoke(nameof(StopWatering));
            
            // Reset state
            if (isWateringActive && animatedController)
            {
                animatedController.SetWatering(false);
                isWateringActive = false;
            }
        }
    }
}
