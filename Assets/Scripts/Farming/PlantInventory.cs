using UnityEngine;
<<<<<<< HEAD
<<<<<<< HEAD
using System;

namespace Farming
{
    public class PlantInventory : MonoBehaviour
    {
        [Header("Plant Inventory Settings")]
        [SerializeField] private int maxPlants = 999;
        [SerializeField] private int currentPlants = 0;

        /// <summary>Fires whenever plant count changes (currentPlants)</summary>
        public static event Action<int> OnPlantsChanged;

        public int CurrentPlants => currentPlants;
        public int MaxPlants => maxPlants;
        public bool HasPlants => currentPlants > 0;

        void Start()
        {
            // Notify UI of initial state
            OnPlantsChanged?.Invoke(currentPlants);
        }

        /// <summary>
        /// Add plants to the inventory (from harvesting).
        /// </summary>
        public void AddPlants(int count)
        {
            int before = currentPlants;
            currentPlants = Mathf.Min(currentPlants + count, maxPlants);
            
            if (currentPlants != before)
            {
                Debug.Log($"[PlantInventory] Added {count} plants. Total: {currentPlants}");
                OnPlantsChanged?.Invoke(currentPlants);
            }
        }

        /// <summary>
        /// Try to consume one plant (for selling).
        /// Returns true if successful.
        /// </summary>
        public bool TryConsumePlant()
        {
            if (currentPlants <= 0) return false;

            currentPlants--;
            OnPlantsChanged?.Invoke(currentPlants);
            return true;
        }

        /// <summary>
        /// Sell all plants. Returns the number of plants sold.
        /// </summary>
        public int SellAll()
        {
            int soldCount = currentPlants;
            currentPlants = 0;
            
            if (soldCount > 0)
            {
                Debug.Log($"[PlantInventory] Sold {soldCount} plants!");
                OnPlantsChanged?.Invoke(currentPlants);
            }
            
            return soldCount;
        }
    }
}
=======

namespace Farming
{
    public class PlantInventory : MonoBehaviour
{
    [SerializeField] private int plants = 0;

    public int PlantCount => plants;   // ✅ FIX

    public void AddPlant(int amount = 1)
    {
        plants += Mathf.Max(1, amount);
    }

    public bool TryRemovePlants(int amount)
    {
        if (plants < amount) return false;
        plants -= amount;
        return true;
    }
}
}
>>>>>>> d01161f (added the sell desk to the shop scene and edited the ShopPodium.cs script to allow selling plants based off plant inventory count which is gathered from PlantInventory.cs which should be attached to player. PlantCount is increased from FarmTile.cs)
