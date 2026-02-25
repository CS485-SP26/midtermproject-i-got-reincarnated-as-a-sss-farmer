using UnityEngine;
using System;

namespace Character
{
    /// <summary>
    /// Manages player energy with Halo-style shield cooldown mechanics.
    /// Energy is consumed by actions (digging, tilling) and regenerates after idling.
    /// </summary>
    public class EnergyResource : MonoBehaviour
    {
        [Header("Energy Settings")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float currentEnergy = 100f;
        
        [Header("Consumption")]
        [Tooltip("Energy cost for digging/tilling actions")]
        [SerializeField] private float diggingCost = 10f;
        [Tooltip("Energy drain per second while sprinting")]
        [SerializeField] private float sprintDrainRate = 5f;
        
        [Header("Regeneration (Halo-style Shield Cooldown)")]
        [Tooltip("Time in seconds of idling before energy starts regenerating")]
        [SerializeField] private float regenerationDelay = 3f;
        [Tooltip("Energy restored per second after cooldown")]
        [SerializeField] private float regenerationRate = 15f;

        // Internal state
        private float lastEnergyUseTime;
        private bool isRegenerating;
        private bool isSprinting;
        private bool isMoving;

        /// <summary>Fires whenever energy changes (currentEnergy, maxEnergy)</summary>
        public static event Action<float, float> OnEnergyChanged;
        
        /// <summary>Fires when energy regeneration starts</summary>
        public static event Action OnRegenerationStarted;
        
        /// <summary>Fires when energy regeneration stops</summary>
        public static event Action OnRegenerationStopped;

        public float CurrentEnergy => currentEnergy;
        public float MaxEnergy => maxEnergy;
        public float Normalized => currentEnergy / maxEnergy;
        public bool HasEnergy => currentEnergy > 0;
        public bool IsRegenerating => isRegenerating;

        void Start()
        {
            currentEnergy = maxEnergy;
            lastEnergyUseTime = -regenerationDelay; // Start with energy available
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        }

        void Update()
        {
            HandleSprintDrain();
            HandleRegeneration();
        }

        /// <summary>
        /// Drain energy while sprinting with movement.
        /// </summary>
        private void HandleSprintDrain()
        {
            if (isSprinting && isMoving && currentEnergy > 0)
            {
                float drain = sprintDrainRate * Time.deltaTime;
                currentEnergy = Mathf.Max(0, currentEnergy - drain);
                lastEnergyUseTime = Time.time; // Reset cooldown timer
                
                // Stop regeneration if active
                if (isRegenerating)
                {
                    isRegenerating = false;
                    OnRegenerationStopped?.Invoke();
                }
                
                OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            }
        }

        /// <summary>
        /// Handle energy regeneration with Halo-style cooldown.
        /// After idling for regenerationDelay seconds, energy starts regenerating.
        /// </summary>
        private void HandleRegeneration()
        {
            // Check if we're past the cooldown period
            float timeSinceLastUse = Time.time - lastEnergyUseTime;
            bool shouldRegenerate = timeSinceLastUse >= regenerationDelay && currentEnergy < maxEnergy;

            // Start regeneration
            if (shouldRegenerate && !isRegenerating)
            {
                isRegenerating = true;
                OnRegenerationStarted?.Invoke();
                Debug.Log("[Energy] Regeneration started");
            }

            // Stop regeneration
            if (!shouldRegenerate && isRegenerating)
            {
                isRegenerating = false;
                OnRegenerationStopped?.Invoke();
            }

            // Apply regeneration
            if (isRegenerating)
            {
                currentEnergy = Mathf.Min(currentEnergy + regenerationRate * Time.deltaTime, maxEnergy);
                OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            }
        }

        /// <summary>
        /// Try to consume energy for digging/tilling. Returns true if successful.
        /// Resets the regeneration cooldown timer.
        /// </summary>
        public bool TryConsumeEnergy()
        {
            if (currentEnergy < diggingCost)
            {
                Debug.Log($"[Energy] Not enough energy! Need {diggingCost}, have {currentEnergy:F1}");
                return false;
            }

            currentEnergy = Mathf.Max(0, currentEnergy - diggingCost);
            lastEnergyUseTime = Time.time; // Reset cooldown timer
            
            // Stop regeneration immediately
            if (isRegenerating)
            {
                isRegenerating = false;
                OnRegenerationStopped?.Invoke();
            }

            Debug.Log($"[Energy] Used {diggingCost} energy. Remaining: {currentEnergy:F1}/{maxEnergy}");
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            return true;
        }

        /// <summary>
        /// Instantly restore energy to maximum (e.g., from food or rest).
        /// </summary>
        public void RestoreInstant()
        {
            currentEnergy = maxEnergy;
            lastEnergyUseTime = -regenerationDelay;
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            Debug.Log("[Energy] Restored to full");
        }

        /// <summary>
        /// Add a specific amount of energy (e.g., from consumables).
        /// </summary>
        public void AddEnergy(float amount)
        {
            currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            Debug.Log($"[Energy] Added {amount} energy. Now: {currentEnergy:F1}/{maxEnergy}");
        }

        /// <summary>
        /// Upgrade max energy capacity.
        /// </summary>
        public void UpgradeCapacity(float newMax)
        {
            maxEnergy = newMax;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            Debug.Log($"[Energy] Upgraded capacity to {maxEnergy}");
        }

        /// <summary>
        /// Get the time remaining until regeneration starts.
        /// </summary>
        public float GetTimeUntilRegeneration()
        {
            float timeSinceLastUse = Time.time - lastEnergyUseTime;
            return Mathf.Max(0, regenerationDelay - timeSinceLastUse);
        }

        /// <summary>
        /// Set sprinting state for energy drain.
        /// </summary>
        public void SetSprinting(bool sprinting, bool moving)
        {
            isSprinting = sprinting;
            isMoving = moving;
        }
    }
}
