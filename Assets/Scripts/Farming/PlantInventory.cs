using UnityEngine;
using System;

namespace Farming
{
    /// <summary>
    /// Tracks harvested plants that can be sold at the shop.
    /// Fires an event when plant count changes for UI updates.
    /// </summary>
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
