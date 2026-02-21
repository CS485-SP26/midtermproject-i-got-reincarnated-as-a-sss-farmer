using UnityEngine;
using System;

namespace Farming
{
    /// <summary>
    /// Tracks the player's seed count. Seeds are consumed when planting on Watered tiles.
    /// </summary>
    public class SeedInventory : MonoBehaviour
    {
        [Header("Seed Settings")]
        [SerializeField] private int startingSeeds = 5;

        private int currentSeeds;

        /// <summary>Fires whenever seed count changes (newCount)</summary>
        public static event Action<int> OnSeedsChanged;

        public int CurrentSeeds => currentSeeds;
        public bool HasSeeds => currentSeeds > 0;

        void Awake()
        {
            currentSeeds = startingSeeds;
        }

        void Start()
        {
            OnSeedsChanged?.Invoke(currentSeeds);
        }

        /// <summary>
        /// Try to consume one seed. Returns true if successful.
        /// </summary>
        public bool TryConsumeSeed()
        {
            if (currentSeeds <= 0) return false;

            currentSeeds--;
            Debug.Log($"[Seeds] Used 1 seed. Remaining: {currentSeeds}");
            OnSeedsChanged?.Invoke(currentSeeds);
            return true;
        }

        /// <summary>
        /// Add seeds to the inventory (from shop or pickup).
        /// </summary>
        public void AddSeeds(int amount)
        {
            currentSeeds += amount;
            Debug.Log($"[Seeds] Added {amount} seeds. Total: {currentSeeds}");
            OnSeedsChanged?.Invoke(currentSeeds);
        }
    }
}
