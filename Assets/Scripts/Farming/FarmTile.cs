using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Farming 
{
    public class FarmTile : MonoBehaviour
    {
        public enum Condition { Grass, Tilled, Watered, Planted }

        [SerializeField] private Condition tileCondition = Condition.Grass; 
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

        private PlantInventory counter;

        void Start()
        {
            tileRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(tileRenderer, "FarmTile requires a MeshRenderer");
            foreach (Transform edge in transform)
            {
                MeshRenderer mesh = edge.GetComponent<MeshRenderer>();
                if (mesh != null)
                {
                    materials.Add(mesh.material);
                }
            }
        }

        // to check for the player's PlantInventory
        private void Awake()
        {
            counter = FindFirstObjectByType<PlantInventory>();
            Debug.Assert(counter, "[FarmTile] needs a reference to player's PlantInventory");
        }


        /// <summary>
        /// General interaction with this farm tile using an optional water resource.
        /// Tills grass tiles, waters tilled soil when water is available, and handles
        /// plant watering/harvesting when a plant is present.
        /// </summary>
        public void Interact(Character.WaterResource waterResource)
        public void Interact(Character.WaterResource waterResource)
        {
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: Till(); break;
                case FarmTile.Condition.Tilled:
                if(waterResource != null && waterResource.TryConsumeWater()){ Water(); } break;
                if(waterResource != null && waterResource.TryConsumeWater()){ Water(); } break;
                case FarmTile.Condition.Watered:
                    // already watered
                    // already watered
                    break;
                case FarmTile.Condition.Planted:
                    if (currentPlant != null)
                    {
                    // only waters if plant is not Mature or Withered state
                        if (currentPlant.currentState != PlantState.Mature && currentPlant.currentState != PlantState.Withered)
                        {
                            if (waterResource != null && currentPlant.TryWater() && waterResource.TryConsumeWater())
                            if (waterResource != null && currentPlant.TryWater() && waterResource.TryConsumeWater())
                            {
                                // watered successfully
                                // watered successfully
                            }
                        }

                        // Harvest only if the plant is Mature
                        if (currentPlant.currentState == PlantState.Mature)
                        {
                            Harvest();
                        }
                    }
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
                    if (currentPlant != null)
                    {
                        if (waterResource != null && currentPlant.TryWater() && waterResource.TryConsumeWater())
                        {
                            daysSinceLastInteraction = 0;
                            return true;
                        }
                    }
                    return false; // Could not water planted tile
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

<<<<<<< HEAD
            // Spawn the plant object
            SpawnPlant(wasWatered);
=======
            // creating a Plant object relative to that tile's position (using the tile's plantSpawn)
            // note: this *should* be a child of the respective farm tile, however the model "squishes" when I do & that shouldn't be happening
<<<<<<< HEAD
<<<<<<< HEAD
            if(plantPrefab)
            {
                if (plantSpawn == null)
                {
                    Debug.LogWarning($"FarmTile '{name}' is missing a plantSpawn reference. Cannot instantiate plant prefab.", this);
                }
                else
                {
                    currentPlant = Instantiate(plantPrefab, plantSpawn.position, UnityEngine.Quaternion.identity);
                    //currentPlant.ChangeState(PlantState.Planted);
                }
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
                if(tileCondition == FarmTile.Condition.Planted)
                {
<<<<<<< HEAD
<<<<<<< HEAD
                    // Destroy the plant if tile is reverting
                    if (currentPlant != null)
                    {
                        Destroy(currentPlant.gameObject);
                        currentPlant = null;
                    }
                    isPlantedSoilWatered = false;
                    tileCondition = FarmTile.Condition.Grass;
=======
                    case Condition.Tilled:
                        if(currentPlant == null) {tileCondition = Condition.Grass;}

                        break;
                    
                    case Condition.Watered:
                        if(currentPlant == null) {tileCondition = Condition.Tilled;}
                        break;

                    // "in the event the tile's already planted, if the plant's withered then change the tile to dirt (instead of just grass)"
                    // note: this code runs only when the 
                    case Condition.Planted:
                        // "if the currentPlant still xists, check if it's withered"
                        if(currentPlant != null) {
                            // "if the currentPlant's state is withered, destroy it & set that tile to tilled / dirt"
                            if(currentPlant.currentState == PlantState.Withered)
                            {
                                Debug.Log("[FarmTile] Plant has withered, turning into dirt");
                                tileCondition = Condition.Tilled;
                                Destroy(currentPlant.gameObject);
                                currentPlant = null;
                            }
                        }
                        break;

                    case Condition.Grass:
                        break;  

>>>>>>> 86f62b7 (Modified Planting & Harvesting Logic [RM])
                }
                else if(tileCondition == FarmTile.Condition.Watered) tileCondition = FarmTile.Condition.Tilled;
                else if(tileCondition == FarmTile.Condition.Tilled) tileCondition = FarmTile.Condition.Grass;
            }
            
            UpdateVisual();
            
        }

        /// <summary>
<<<<<<< HEAD
<<<<<<< HEAD
        /// Harvest a planted tile if the plant is mature. Returns true if harvest succeeded.
        /// Resets to grass and fires harvest event.
        /// </summary>
        /// 
        // note: [Ryan] modified / re-structured to handle withered plant cases
        /// 
        // note: [Ryan] modified / re-structured to handle withered plant cases
        public void Harvest()
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
            // should be removed, but just in-case will leave commented out
            // Condition previousCondition = tileCondition;

            // "if our plant exists, check its status to determine that tile's state"
            if(currentPlant != null)
            {
                // "if the tile has a 'fresh' plant (i.e., not withered), add to our PlantInventory & set the tile to grass..."
                if(currentPlant.currentState != PlantState.Withered) {
                    counter.AddPlant(1);
                    tileCondition = Condition.Grass;
                
                }
                // "... else we assume the plant withered, so DON'T add that plant to the PlantInventory & set the tile to tilled / dirt" 
                else
                {
                    Debug.Log("Withered plants don't sell for money at all!");
                    tileCondition = Condition.Tilled;
                }
                // regardless of what kind of plant was harvested, show the player's plant count
                Debug.Log("Plant Count: " + counter.PlantCount);
                
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
