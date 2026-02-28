using System.Collections.Generic;
using UnityEngine;
using Environment;

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

        [Header("Audio")]
        [SerializeField] private AudioSource stepAudio;
        [SerializeField] private AudioSource tillAudio;
        [SerializeField] private AudioSource waterAudio;

        List<Material> materials = new List<Material>();

        private int daysSinceLastInteraction = 0;
        public FarmTile.Condition GetCondition { get { return tileCondition; } }

        [SerializeField] private PlantInventory counter;

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
            if (counter == null)
            {
                counter = FindFirstObjectByType<PlantInventory>();
                Debug.Assert(counter != null, "[FarmTile] needs a reference to player's PlantInventory");
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
                    return false; // Already planted
            }
            return false;
        }

        /// <summary>
        /// Plant seeds on a watered tile. Consumes one seed from the inventory.
        /// Returns true if planting succeeded.
        /// </summary>
        public bool Plant(SeedInventory seeds)
        {
            if (tileCondition != Condition.Watered) return false;
            if (seeds == null) return false;

            if (plantPrefab == null)
            {
                Debug.LogWarning($"[FarmTile] {gameObject.name} has no plantPrefab assigned; aborting plant.");
                return false;
            }

            if (!seeds.TryConsumeSeed()) return false;

            Condition previousCondition = tileCondition;
            tileCondition = Condition.Planted;
            UpdateVisual();
            tillAudio?.Play(); // reuse till audio for planting sound
            daysSinceLastInteraction = 0;

            // creating a Plant object relative to that tile's position (using the tile's plantSpawn)
            // note: this *should* be a child of the respective farm tile, however the model "squishes" when I do & that shouldn't be happening
            Vector3 spawnPosition = plantSpawn != null ? plantSpawn.position : transform.position;
            currentPlant = Instantiate(plantPrefab, spawnPosition, UnityEngine.Quaternion.identity);
            currentPlant.ChangeState(PlantState.Planted);

            FarmingEvents.TileFarmed(this, previousCondition, tileCondition);
            return true;
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
                // when planted, change the tile to the tilledMaterial; after harvest it'll change back to grassMaterial
                case FarmTile.Condition.Planted:
                    tileRenderer.material = plantedMaterial != null ? plantedMaterial : tilledMaterial;
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

                    // "in the event the tile's already planted, if the plant's withered then change the tile to dirt (instead of just grass)"
                    // note: this code runs only when the tile has gone at least two days without interaction
                    case Condition.Planted:
                        // "if the currentPlant still exists, check if it's withered"
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
                        else
                        {
                            // Plant reference lost (e.g., destroyed externally); revert tile to tilled
                            tileCondition = Condition.Tilled;
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
            else
            {
                // No plant reference (e.g., destroyed externally); revert tile to tilled
                tileCondition = Condition.Tilled;
            }

            // since we interacted with the tile, regardless of outcome, reset our interaction check
            daysSinceLastInteraction = 0;
            UpdateVisual();
            FarmingEvents.TileHarvested(this);
        }
    }
}