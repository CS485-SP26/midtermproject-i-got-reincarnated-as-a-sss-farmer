using System.Collections.Generic;
using UnityEngine;
using Environment;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;
using System;

namespace Farming 
{
    public class FarmTile : MonoBehaviour
    {
        public enum Condition { Grass, Tilled, Watered, Planted }

        [SerializeField] private Condition tileCondition = Condition.Grass; 
        // our plant object
        [SerializeField] private Plant plantPrefab;
        // reference to the actual plant object
        private Plant currentPlant;
        [SerializeField] private Transform plantSpawn;


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

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
        [SerializeField] private PlantInventory counter;
=======
        private PlantInventory counter;
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
        [SerializeField] private PlantInventory counter;
>>>>>>> 30ea09b (Apply PR review feedback to FarmTile.cs)
=======
        private PlantInventory counter;
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
        [SerializeField] private PlantInventory counter;
>>>>>>> 30ea09b (Apply PR review feedback to FarmTile.cs)

        void Start()
        {
            tileRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(tileRenderer, "FarmTile requires a MeshRenderer");
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
            // modified the for-loop so the transform "plantSpawn" doesn't cause errors in detecting mesh renders
=======
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
            // modified the for-loop so the transform "plantSpawn" doesn't cause errors in detecting mesh renders
>>>>>>> 5477fb0 (Confirmation of Merge with Salvador's Branch)
=======
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
            // modified the for-loop so the transform "plantSpawn" doesn't cause errors in detecting mesh renders
>>>>>>> 5477fb0 (Confirmation of Merge with Salvador's Branch)
            foreach (Transform edge in transform)
            {
                MeshRenderer mesh = edge.GetComponent<MeshRenderer>();
                if (mesh != null)
                {
                    materials.Add(mesh.material);
                }
<<<<<<< HEAD
<<<<<<< HEAD
            }
        }

        // to check for the player's PlantInventory
        private void Awake()
        {
            if (counter == null)
            {
                counter = FindFirstObjectByType<PlantInventory>();
                Debug.Assert(counter != null, "[FarmTile] needs a reference to player's PlantInventory");
=======
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
            }
        }

        // to check for the player's PlantInventory
        private void Awake()
        {
            if (counter == null)
            {
                counter = FindFirstObjectByType<PlantInventory>();
                Debug.Assert(counter != null, "[FarmTile] needs a reference to player's PlantInventory");
=======
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
            }
        }

        // to check for the player's PlantInventory
        private void Awake()
        {
            if (counter == null)
            {
                counter = FindFirstObjectByType<PlantInventory>();
                Debug.Assert(counter != null, "[FarmTile] needs a reference to player's PlantInventory");
            }
        }


        /// <summary>
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
        /// Interact with this farm tile using an optional water resource.
        /// May till grass, consume water to water tilled soil or plants, and harvest mature plants.
=======
        /// General interaction with this farm tile using an optional water resource.
        /// Tills grass tiles, waters tilled soil when water is available, and handles
        /// plant watering/harvesting when a plant is present.
>>>>>>> 44c1d0a (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
=======
        /// General interaction with this farm tile using an optional water resource.
        /// Tills grass tiles, waters tilled soil when water is available, and handles
        /// plant watering/harvesting when a plant is present.
=======
        /// General interaction with this farm tile using an optional water resource.
        /// Tills grass tiles, waters tilled soil when water is available, and handles
        /// plant watering/harvesting when a plant is present.
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
=======
        /// Interact with this farm tile using an optional water resource.
        /// May till grass, consume water to water tilled soil or plants, and harvest mature plants.
>>>>>>> 7545cdd (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
<<<<<<< HEAD
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
=======
        /// General interaction with this farm tile using an optional water resource.
        /// Tills grass tiles, waters tilled soil when water is available, and handles
        /// plant watering/harvesting when a plant is present.
>>>>>>> 44c1d0a (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
=======
>>>>>>> 4e4296b (Apply PR review feedback: null safety, operator precedence, billboard class name, seed count init)
        /// </summary>
        public void Interact(Character.WaterResource waterResource)
        {
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: Till(); break;
                case FarmTile.Condition.Tilled:
                if(waterResource != null && waterResource.TryConsumeWater()){ Water(); } break;
                case FarmTile.Condition.Watered:
                    // already watered
                    break;
                case FarmTile.Condition.Planted:
                    Harvest();
                    break;
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
<<<<<<< HEAD
                    // Water the plant to start/continue growth
                    if (waterResource != null && waterResource.TryConsumeWater())
                    {
<<<<<<< HEAD
                        WaterPlant();
                        daysSinceLastInteraction = 0;
                        return true;
                    }
                    return false;
=======
<<<<<<< HEAD
                    if (currentPlant != null)
                    {
                        if (waterResource != null && currentPlant.TryWater() && waterResource.TryConsumeWater())
                        {
                            daysSinceLastInteraction = 0;
                            return true;
                        }
                    }
<<<<<<< HEAD
                    return false; // Already planted
>>>>>>> 553f965 (Small change to Part 10 and additions for Part 11)
=======
                        if (waterResource != null && currentPlant.TryWater() && waterResource.TryConsumeWater())
                        {
                            daysSinceLastInteraction = 0;
                            return true;
                        }
                    }
                    return false; // Could not water planted tile
>>>>>>> 44c1d0a (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
>>>>>>> 1a943be (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
=======
                    return false; // Could not water planted tile
>>>>>>> 44c1d0a (Address PR review comments: fix bugs in Plant, FarmTile, BillboardWaterDropletIcon, HotbarUI)
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
<<<<<<< HEAD
            // Can plant on both tilled and watered soil
            if (tileCondition != Condition.Tilled && tileCondition != Condition.Watered) return false;
            if (seeds == null || !seeds.TryConsumeSeed()) return false;
=======
            if (tileCondition != Condition.Watered) return false;
            if (seeds == null) return false;

            if (plantPrefab == null)
            {
                Debug.LogWarning($"[FarmTile] {gameObject.name} has no plantPrefab assigned; aborting plant.");
                return false;
            }

            if (!seeds.TryConsumeSeed()) return false;
>>>>>>> a1d5ff0 (Apply PR review feedback to FarmTile.cs)

            bool wasWatered = (tileCondition == Condition.Watered);
            Condition previousCondition = tileCondition;
            tileCondition = Condition.Planted;
            isPlantedSoilWatered = wasWatered; // Remember if soil is wet
            UpdateVisual();
            tillAudio?.Play(); // reuse till audio for planting sound
            daysSinceLastInteraction = 0;

            // creating a Plant object relative to that tile's position (using the tile's plantSpawn)
            // note: this *should* be a child of the respective farm tile, however the model "squishes" when I do & that shouldn't be happening
            if(plantPrefab)
            {
                currentPlant = Instantiate(plantPrefab, plantSpawn.position, UnityEngine.Quaternion.identity);
                currentPlant.ChangeState(PlantState.Planted);
            }

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

        private void UpdateVisual()
        {
            if(tileRenderer == null) return;
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: tileRenderer.material = grassMaterial; break;
                case FarmTile.Condition.Tilled: tileRenderer.material = tilledMaterial; break;
                case FarmTile.Condition.Watered: tileRenderer.material = wateredMaterial; break;
                // when planted, change the tile to the tilledMaterial; after harvest it'll change back to grassMaterial
                case FarmTile.Condition.Planted:
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> d3dc40d (Modified Planting & Harvesting Logic [RM])
=======
>>>>>>> 0c82600 (Apply PR review feedback to FarmTile.cs)
=======
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
>>>>>>> 30ea09b (Apply PR review feedback to FarmTile.cs)
                    // Show wet or dry soil based on watered state
                    if (plantedMaterial != null)
                    {
                        tileRenderer.material = plantedMaterial;
                    }
                    else
                    {
                        tileRenderer.material = isPlantedSoilWatered ? wateredMaterial : tilledMaterial;
                    }
=======
                    tileRenderer.material = plantedMaterial != null ? plantedMaterial : wateredMaterial;
>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
                    tileRenderer.material = plantedMaterial != null ? plantedMaterial : tilledMaterial;
>>>>>>> a1d5ff0 (Apply PR review feedback to FarmTile.cs)
=======
>>>>>>> d3dc40d (Modified Planting & Harvesting Logic [RM])
=======
=======
                    tileRenderer.material = plantedMaterial != null ? plantedMaterial : tilledMaterial;
>>>>>>> a1d5ff0 (Apply PR review feedback to FarmTile.cs)
>>>>>>> 0c82600 (Apply PR review feedback to FarmTile.cs)
=======
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
=======
                    tileRenderer.material = plantedMaterial != null ? plantedMaterial : tilledMaterial;
>>>>>>> a1d5ff0 (Apply PR review feedback to FarmTile.cs)
>>>>>>> 30ea09b (Apply PR review feedback to FarmTile.cs)
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

        // [Ryan] rewrote this function to better suit having plant objects inside each farm tile (given the plant growing logic comes from Plant.cs)
        public void OnDayPassed()
        {

            daysSinceLastInteraction++;
            if(daysSinceLastInteraction >= 2)
            {
                // switch-case statement based on the tile's condition
                switch(tileCondition)
                {
                    case Condition.Tilled:
                        if(currentPlant == null) {tileCondition = Condition.Grass;}

                        break;
                    
                    case Condition.Watered:
                        if(currentPlant == null) {tileCondition = Condition.Tilled;}
                        break;

                    // "in the event the tile's already planted, if the plant's witheed then change the tile to dirt (instead of just grass)"
                    case Condition.Planted:
                        if(currentPlant && currentPlant.currentState == PlantState.Withered)
                        {
                            tileCondition = Condition.Tilled;
                            Destroy(currentPlant.gameObject);
                            currentPlant = null;
                        }

                        break;

                    case Condition.Grass:
                        break;  

                }
                // optional for now
                daysSinceLastInteraction = 0;
            }
            
            UpdateVisual();
            
        }

        /// <summary>
<<<<<<< HEAD
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
=======
        /// Harvest a planted tile. Resets to grass (provided plant is not withered) and fires harvest event.
        /// </summary>
        /// 
        // note: [Ryan] modified / re-structured to handle withered plant cases
        public void Harvest()
        {

            if (tileCondition != Condition.Planted) {return;}
            Debug.Log($"[FarmTile] Harvested {gameObject.name}!");

            // "if our plant exists, check its status to determine that tile's state"
            if(currentPlant != null)
>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])
            {
                // "if the tile has a 'fresh' plant (i.e., not withered), add to our PlantInventory & set the tile to grass..."
                if(currentPlant.currentState != PlantState.Withered) {
                    if (counter != null) counter.AddPlant(1);
                    tileCondition = Condition.Grass;
                
                }
                // "... else we assume the plant withered, so DON'T add that plant to the PlantInventory & set the tile to tilled / dirt" 
                else
                {
                    Debug.Log("Withered plants don't sell for money at all!");
                    tileCondition = Condition.Tilled;
                }
                // regardless of what kind of plant was harvested, show the player's plant count
                if (counter != null) Debug.Log("Plant Count: " + counter.PlantCount);
                
                // actually removing the plant & ensuring its reference is null
                Destroy(currentPlant.gameObject);
                currentPlant = null;
            }
<<<<<<< HEAD
<<<<<<< HEAD
            
            // Reset tile to tilled (dirt) instead of grass
            Condition previousCondition = tileCondition;
            tileCondition = Condition.Tilled;
            isPlantedSoilWatered = false;
<<<<<<< HEAD
=======
=======
            else
            {
                // No plant reference (e.g., destroyed externally); revert tile to tilled
                tileCondition = Condition.Tilled;
            }
>>>>>>> a1d5ff0 (Apply PR review feedback to FarmTile.cs)

            // since we interacted with the tile, regardless of outcome, reset our interaction check
>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])
            daysSinceLastInteraction = 0;
<<<<<<< HEAD
=======
=======
        /// Harvest a planted tile. Resets to grass (provided plant is not withered) and fires harvest event.
        /// </summary>
        /// 
        // note: [Ryan] modified / re-structured to handle withered plant cases
        public void Harvest()
        {
>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])

            if (tileCondition != Condition.Planted) {return;}
            Debug.Log($"[FarmTile] Harvested {gameObject.name}!");

            // "if our plant exists, check its status to determine that tile's state"
            if(currentPlant != null)
            {
                // "if the tile has a 'fresh' plant (i.e., not withered), add to our PlantInventory & set the tile to grass..."
                if(currentPlant.currentState != PlantState.Withered) {
                    if (counter != null) counter.AddPlant(1);
=======
            daysSinceLastInteraction = 0;
=======
        /// Harvest a planted tile. Resets to grass (provided plant is not withered) and fires harvest event.
        /// </summary>
        /// 
        // note: [Ryan] modified / re-structured to handle withered plant cases
        public void Harvest()
        {
>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])

            if (tileCondition != Condition.Planted) {return;}
            Debug.Log($"[FarmTile] Harvested {gameObject.name}!");

            // "if our plant exists, check its status to determine that tile's state"
            if(currentPlant != null)
            {
                // "if the tile has a 'fresh' plant (i.e., not withered), add to our PlantInventory & set the tile to grass..."
                if(currentPlant.currentState != PlantState.Withered) {
<<<<<<< HEAD
                    counter.AddPlant(1);
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
                    if (counter != null) counter.AddPlant(1);
>>>>>>> 30ea09b (Apply PR review feedback to FarmTile.cs)
                    tileCondition = Condition.Grass;
                
                }
                // "... else we assume the plant withered, so DON'T add that plant to the PlantInventory & set the tile to tilled / dirt" 
                else
                {
                    Debug.Log("Withered plants don't sell for money at all!");
                    tileCondition = Condition.Tilled;
                }
                // regardless of what kind of plant was harvested, show the player's plant count
<<<<<<< HEAD
<<<<<<< HEAD
                if (counter != null) Debug.Log("Plant Count: " + counter.PlantCount);
=======
                Debug.Log("Plant Count: " + counter.PlantCount);
>>>>>>> 34b0dad (Modified Planting & Harvesting Logic [RM])
=======
                if (counter != null) Debug.Log("Plant Count: " + counter.PlantCount);
>>>>>>> 30ea09b (Apply PR review feedback to FarmTile.cs)
                
                // actually removing the plant & ensuring its reference is null
                Destroy(currentPlant.gameObject);
                currentPlant = null;
            }
            else
            {
                // No plant reference (e.g., destroyed externally); revert tile to tilled
                tileCondition = Condition.Tilled;
            }

            // since we interacted with the tile, regardless of outcome, reset our interaction check
            daysSinceLastInteraction = 0;

            // removing the plant object after harvesting
            if(currentPlant)
            {
                Destroy(currentPlant.gameObject);
                currentPlant = null;
            }

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
