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
        EnergyResource energyResource;
        SeedInventory seedInventory;
        PlantInventory plantInventory;
        FertilizerInventory fertilizerInventory;

        bool isWateringActive; // prevents multi-drain per animation
        float wateringStartTime;
        const float WATERING_ANIMATION_DURATION = 5.6f;
        const float MAX_WATERING_DURATION = 8f; // Safety timeout (extra buffer beyond animation)
        // will be incremented at every harvest, where every 2 will yield 1 new fertilizer for the player
        public int compostCounter = 0;

        void Start()
        {
            animatedController = GetComponent<AnimatedController>();
            waterResource = GetComponent<WaterResource>();
            energyResource = GetComponent<EnergyResource>();
            seedInventory = GetComponent<SeedInventory>();
            plantInventory = GetComponent<PlantInventory>();
            fertilizerInventory = GetComponent<FertilizerInventory>();

            if (selectorManager == null)
                selectorManager = GetComponent<SelectorManager>();

            Debug.Assert(selectorManager, "PlayerFarmingController requires a SelectorManager.");

            if (!animatedController)
                Debug.LogWarning("No AnimatedController found.");

            if (!waterResource)
                Debug.LogError("No WaterResource found on Player.");

            if (!energyResource)
                Debug.LogError("No EnergyResource found on Player.");

            if (!seedInventory)
                Debug.LogWarning("No SeedInventory found on Player - planting will be unavailable.");
                
            if (!plantInventory)
                Debug.LogWarning("No PlantInventory found on Player - harvesting will be unavailable.");
        
            // now considers the player's fertilizer
            if(!fertilizerInventory)
                Debug.LogWarning("No FertilizerInventory found on Player - fertilizing will be unavailable.");
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

            // SEEDS: Plant on tilled or watered tiles only (no harvesting)
            if (selectedTool == HotbarUI.ToolType.Seeds)
            {
                if ((tile.GetCondition == FarmTile.Condition.Tilled || tile.GetCondition == FarmTile.Condition.Watered) 
                    && seedInventory != null && seedInventory.HasSeeds)
                {
                    tile.Plant(seedInventory);
                }
                else if (tile.GetCondition == FarmTile.Condition.Planted)
                {
                    Debug.Log("[PlayerFarmingController] Can't plant on planted tiles! Use harvest tool (key 3) to harvest.");
                }
                else if (tile.GetCondition != FarmTile.Condition.Tilled && tile.GetCondition != FarmTile.Condition.Watered)
                {
                    Debug.Log("[PlayerFarmingController] Seeds can only be planted on tilled or watered tiles!");
                }
                return;
            }

            // HARVEST TOOL: Harvest mature plants
            if (selectedTool == HotbarUI.ToolType.HarvestTool)
            {
                if (tile.GetCondition == FarmTile.Condition.Planted)
                {
                    // Try to harvest if the plant is mature
                    if (tile.Harvest(plantInventory))
                    {
                        Debug.Log("[PlayerFarmingController] Harvested mature plant!");

                        // adding onto our compostCounter (to get more fertilizer)
                        compostCounter++;
                        // "if the player's harvested 2 plants, add 1 fertilizer to their inventory"
                        if(compostCounter % 2 == 0)
                        {
                            fertilizerInventory.AddFertilizer(1);
                            Debug.Log("[PlayerFarmingController] Added 1 fertilizer!");
                        }

                    }
                    else
                    {
                        Debug.Log("[PlayerFarmingController] Plant is not ready to harvest yet!");
                    }
                }
                else
                {
                    Debug.Log("[PlayerFarmingController] No plant to harvest!");
                }
                return;
            }

            // FERTILIZER TOOL: Speed up plant growth
            if (selectedTool == HotbarUI.ToolType.Fertilizer)
            {
                // "if the tile is planted, then check if fertilizer was applied..."
                if (tile.GetCondition == FarmTile.Condition.Planted)
                {
                    // "if fertilizer was applied, write that in the log... "
                    if (tile.ApplyFertilizer(fertilizerInventory))
                    {
                        Debug.Log("[PlayerFarmingController] Fertilizer applied!");
                    }
                    // "... else say you can't"
                    else
                    {
                        Debug.Log("[PlayerFarmingController] Cannot fertilize this plant.");
                    }
                }
                // "... else say fertilizer only works on non-mature plants"
                else
                {
                    Debug.Log("[PlayerFarmingController] Fertilizer can only be used on planted crops.");
                }
                return;

            }
            
            // WATERING CAN: Till grass, water tilled land, water planted tiles
            if (selectedTool == HotbarUI.ToolType.WateringCan)
            {
                // Check if action requires energy (tilling grass)
                bool requiresEnergy = tile.GetCondition == FarmTile.Condition.Grass;
                
                // Check resources before interaction
                if (requiresEnergy && energyResource != null && !energyResource.HasEnergy)
                {
                    Debug.Log("[PlayerFarmingController] Not enough energy to till!");
                    return;
                }
                
                // Try to consume energy for digging/tilling
                if (requiresEnergy && energyResource != null)
                {
                    if (!energyResource.TryConsumeEnergy())
                    {
                        Debug.Log("[PlayerFarmingController] Failed to consume energy for tilling");
                        return;
                    }
                }
                
                bool success = tile.InteractWithWater(waterResource);
                Debug.Log($"[PlayerFarmingController] Watering can interaction - Success: {success}, Tile condition: {tile.GetCondition}");

                if (success && (tile.GetCondition == FarmTile.Condition.Watered || tile.GetCondition == FarmTile.Condition.Planted))
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
