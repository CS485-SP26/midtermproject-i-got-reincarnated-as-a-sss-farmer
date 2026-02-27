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
            if (seeds == null || !seeds.TryConsumeSeed()) return false;

            Condition previousCondition = tileCondition;
            tileCondition = Condition.Planted;
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
                    tileRenderer.material = plantedMaterial != null ? plantedMaterial : grassMaterial;
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
        /// Harvest a planted tile. Resets to grass and fires harvest event.
        /// Currently disabled - planted state commented out.
        /// </summary>
        public void Harvest()
        {
            if (tileCondition != Condition.Planted) return;
            Debug.Log($"[FarmTile] Harvested {gameObject.name}!");
            Condition previousCondition = tileCondition;
            tileCondition = Condition.Grass;
            daysSinceLastInteraction = 0;

            // removing the plant object after harvesting
            if(currentPlant)
            {
                Destroy(currentPlant.gameObject);
                currentPlant = null;
            }

            UpdateVisual();
            FarmingEvents.TileHarvested(this);
        }
    }
}