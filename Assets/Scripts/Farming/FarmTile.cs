using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Farming 
{
    public class FarmTile : MonoBehaviour
    {
        public enum Condition { Grass, Tilled, Watered, Planted }

        [SerializeField] private Condition tileCondition = Condition.Grass; 

        [Header("Visuals")]
        [SerializeField] private Material grassMaterial;
        [SerializeField] private Material tilledMaterial;
        [SerializeField] private Material wateredMaterial;
        [SerializeField] private Material plantedMaterial; // Optional - falls back to grassMaterial
        MeshRenderer tileRenderer;
        
        [Header("Plant Settings")]
        [SerializeField] private GameObject plantPrefab; // Prefab of the Plant object
        private Plant currentPlant; // Reference to the spawned plant
        private bool isPlantedSoilWatered = false; // Tracks if planted soil is wet or dry



        [Header("Audio")]
        [SerializeField] private AudioSource stepAudio;
        [SerializeField] private AudioSource tillAudio;
        [SerializeField] private AudioSource waterAudio;

        List<Material> materials = new List<Material>();

        private int daysSinceLastInteraction = 0;
        public FarmTile.Condition GetCondition { get { return tileCondition; } }

        void Start()
        {
            tileRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(tileRenderer, "FarmTile requires a MeshRenderer");

            foreach (Transform edge in transform)
            {
                materials.Add(edge.gameObject.GetComponent<MeshRenderer>().material);
            }
        }


        /// <summary>
        /// Interact without water check (tilling only).
        /// For watering, use InteractWithWater() instead.
        /// </summary>
        public void Interact()
        {
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: Till(); break;
                case FarmTile.Condition.Tilled:
                    // Need water to irrigate tilled land
                    break;
                case FarmTile.Condition.Watered:
                    // Already watered - plants are growing
                    break;
                // case FarmTile.Condition.Planted:
                //     Harvest();
                //     break;
            }
            daysSinceLastInteraction = 0;
        }

        /// <summary>
        /// Interact with water resource check. Consumes water only when watering tilled land.
        /// </summary>
        public bool InteractWithWater(Character.WaterResource waterResource)
        {
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass:
                    Till(); // Tilling doesn't require water
                    daysSinceLastInteraction = 0;
                    return true;
                    
                case FarmTile.Condition.Tilled:
                    if (waterResource != null && waterResource.TryConsumeWater())
                    {
                        Water();
                        daysSinceLastInteraction = 0;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
                case FarmTile.Condition.Watered:
                    return false;

                case FarmTile.Condition.Planted:
                    // Water the plant to start/continue growth
                    if (waterResource != null && waterResource.TryConsumeWater())
                    {
                        WaterPlant();
                        daysSinceLastInteraction = 0;
                        return true;
                    }
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Plant seeds on tilled or watered soil. Consumes one seed from the inventory.
        /// If planted on watered soil, plant starts growing immediately.
        /// If planted on dry soil, plant needs to be watered to start growing.
        /// Returns true if planting succeeded.
        /// </summary>
        public bool Plant(SeedInventory seeds)
        {
            // Can plant on both tilled and watered soil
            if (tileCondition != Condition.Tilled && tileCondition != Condition.Watered) return false;
            if (seeds == null || !seeds.TryConsumeSeed()) return false;

            bool wasWatered = (tileCondition == Condition.Watered);
            Condition previousCondition = tileCondition;
            tileCondition = Condition.Planted;
            isPlantedSoilWatered = wasWatered; // Remember if soil is wet
            UpdateVisual();
            tillAudio?.Play(); // reuse till audio for planting sound
            daysSinceLastInteraction = 0;

            // Spawn the plant object
            SpawnPlant(wasWatered);

            FarmingEvents.TileFarmed(this, previousCondition, tileCondition);
            return true;
        }
        
        /// <summary>
        /// Spawns a plant object on this tile. Called when planting seeds.
        /// </summary>
        /// <param name="startWatered">If true, plant starts watered and will grow immediately</param>
        void SpawnPlant(bool startWatered = false)
        {
            if (plantPrefab == null)
            {
                Debug.LogWarning("[FarmTile] No plant prefab assigned!");
                return;
            }
            
            // Clean up any existing plant
            if (currentPlant != null)
            {
                Destroy(currentPlant.gameObject);
            }
            
            // Instantiate the plant at world position to avoid scale inheritance
            Vector3 worldSpawnPos = transform.position + Vector3.up * 0.01f;
            GameObject plantObj = Instantiate(plantPrefab, worldSpawnPos, Quaternion.identity);
            plantObj.transform.SetParent(transform, true); // Parent it but keep world scale
            currentPlant = plantObj.GetComponent<Plant>();
            
            if (currentPlant != null)
            {
                currentPlant.Initialize();
                currentPlant.SetWatered(startWatered); // Start watered if planted on wet soil
            }
            else
            {
                Debug.LogError($"[FarmTile] Plant prefab does not have Plant component!");
            }
        }
        
        /// <summary>
        /// Water the planted crop to start/continue growth.
        /// </summary>
        void WaterPlant()
        {
            if (currentPlant == null)
            {
                Debug.LogWarning("[FarmTile] Trying to water plant but no plant exists!");
                return;
            }
            
            currentPlant.SetWatered(true);
            isPlantedSoilWatered = true; // Mark soil as watered
            UpdateVisual(); // Update to show wet soil
            waterAudio?.Play();
        }

        public void Till()
        {
            Condition previousCondition = tileCondition;
            tileCondition = FarmTile.Condition.Tilled;
            UpdateVisual();
            tillAudio?.Play();
            
            // Fire farming event for progress tracking
            FarmingEvents.TileFarmed(this, previousCondition, tileCondition);
        }

        public void Water()
        {
            Condition previousCondition = tileCondition;
            tileCondition = FarmTile.Condition.Watered;
            UpdateVisual();
            waterAudio?.Play();
            
            // Fire farming event for progress tracking
            FarmingEvents.TileFarmed(this, previousCondition, tileCondition);
        }


        /// <summary>
        /// Called automatically when the growth timer completes.
        /// Watered tile becomes a planted tile with crops.
        /// Currently disabled - planted state uses grass material.
        /// </summary>
        // void GrowPlant()
        // {
        //     Condition previousCondition = tileCondition;
        //     tileCondition = Condition.Planted;
        //     isGrowing = false;
        //     waterTimer = 0f;
        //     UpdateVisual();
        //     Debug.Log($"[FarmTile] {gameObject.name} has grown into a plant!");
        //     FarmingEvents.TilePlanted(this);
        // }

        private void UpdateVisual()
        {
            if(tileRenderer == null) return;
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: tileRenderer.material = grassMaterial; break;
                case FarmTile.Condition.Tilled: tileRenderer.material = tilledMaterial; break;
                case FarmTile.Condition.Watered: tileRenderer.material = wateredMaterial; break;
                case FarmTile.Condition.Planted:
                    // Show wet or dry soil based on watered state
                    if (plantedMaterial != null)
                    {
                        tileRenderer.material = plantedMaterial;
                    }
                    else
                    {
                        tileRenderer.material = isPlantedSoilWatered ? wateredMaterial : tilledMaterial;
                    }
                    break;
            }
        }

        /// <summary>
        /// Restore tile to a saved condition state (used by save/load system)
        /// </summary>
        public void SetCondition(Condition newCondition)
        {
            tileCondition = newCondition;
            UpdateVisual();
        }

        public void SetHighlight(bool active)
        {
            foreach (Material m in materials)
            {
                if (active)
                {
                    m.EnableKeyword("_EMISSION");
                } 
                else 
                {
                    m.DisableKeyword("_EMISSION");
                }
            }
            if (active) stepAudio.Play();
        }

        public void OnDayPassed()
        {
            daysSinceLastInteraction++;
            if(daysSinceLastInteraction >= 2)
            {
                if(tileCondition == FarmTile.Condition.Planted)
                {
                    // Destroy the plant if tile is reverting
                    if (currentPlant != null)
                    {
                        Destroy(currentPlant.gameObject);
                        currentPlant = null;
                    }
                    isPlantedSoilWatered = false;
                    tileCondition = FarmTile.Condition.Grass;
                }
                else if(tileCondition == FarmTile.Condition.Watered) tileCondition = FarmTile.Condition.Tilled;
                else if(tileCondition == FarmTile.Condition.Tilled) tileCondition = FarmTile.Condition.Grass;
            }
            UpdateVisual();
        }

        /// <summary>
        /// Harvest a planted tile if the plant is mature. Returns true if harvest succeeded.
        /// Resets to grass and fires harvest event.
        /// </summary>
        public bool Harvest(PlantInventory plantInventory)
        {
            if (tileCondition != Condition.Planted) return false;
            if (currentPlant == null) return false;
            
            // Only harvest if plant is mature
            if (currentPlant.currentState != PlantState.Mature)
            {
                Debug.Log($"[FarmTile] Plant is not mature yet! Current state: {currentPlant.currentState}");
                return false;
            }
            
            Debug.Log($"[FarmTile] Harvested mature plant on {gameObject.name}!");
            
            // Add harvested plant to inventory
            if (plantInventory != null)
            {
                plantInventory.AddPlants(1);
            }
            
            // Destroy the plant object
            if (currentPlant != null)
            {
                Destroy(currentPlant.gameObject);
                currentPlant = null;
            }
            
            // Reset tile to tilled (dirt) instead of grass
            Condition previousCondition = tileCondition;
            tileCondition = Condition.Tilled;
            isPlantedSoilWatered = false;
            daysSinceLastInteraction = 0;
            UpdateVisual();
            FarmingEvents.TileHarvested(this);
            
            return true;
        }
        
        /// <summary>
        /// Get the current plant's state. Returns null if no plant exists.
        /// </summary>
        public PlantState? GetPlantState()
        {
            return currentPlant?.currentState;
        }
    }
}