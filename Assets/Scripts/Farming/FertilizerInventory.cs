using UnityEngine;
using System;

namespace Farming
{
    // track's the player's fertilizer
    public class FertilizerInventory : MonoBehaviour
    {
        [Header("Fertilizer Settings")]
        // starting count of fertilizer
        [SerializeField] private int fertilizerCount = 3;
        private int currentFertilizer;
        public static event Action<int> OnFertilizerChanged;

        public int CurrentFertilizer => currentFertilizer;
        public bool HasFertilizer => currentFertilizer > 0;

        void Awake()
        {
            currentFertilizer = fertilizerCount;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            OnFertilizerChanged?.Invoke(currentFertilizer);
        }

        // mimicking the other consumables; function that will try to consume fertilizer
        public bool TryUseFertilizer()
        {
            if(currentFertilizer <= 0) {return false;}

            currentFertilizer--;
            Debug.Log($"[Fertilizer] Used 1 fertilizer. Remaining: {currentFertilizer}");
            OnFertilizerChanged?.Invoke(currentFertilizer);
            return true;
        
        }

        // allows more fertilizer to be added
        // note: as of the current version, there's no way to add extra fertilizer
        public void AddFertilizer(int amount)
        {
            currentFertilizer += amount;
            Debug.Log($"[Fertilizer] Added {amount} fertilizer. Total: {currentFertilizer}");
            OnFertilizerChanged?.Invoke(currentFertilizer);

        }
    }    
}

