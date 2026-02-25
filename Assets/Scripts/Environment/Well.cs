using UnityEngine;
using UnityEngine.InputSystem;
using Character;
using Environment;

namespace Environment
{
    /// <summary>
    /// Interactive well that stores water and restores it passively throughout the day.
    /// Player must press F to retrieve water when in range.
    /// </summary>
    public class Well : MonoBehaviour
    {
        [Header("Water Storage")]
        [SerializeField] private int maxWaterStorage = 100;
        [SerializeField] private int currentWaterStored = 100;
        
        [Header("Passive Restoration")]
        [Tooltip("Water restored per day (5 means 1 water every 1/5 of day)")]
        [SerializeField] private int waterRestoredPerDay = 5;
        private float lastDayProgress = 0f;
        private DayController dayController;
        
        [Header("Interaction")]
        [SerializeField] private float interactionRange = 3f;
        [SerializeField] private int waterGivenPerInteraction = 10;
        
        [Header("Visual Feedback")]
        [SerializeField] private ParticleSystem refillEffect;
        [SerializeField] private AudioSource refillSound;
        
        // State tracking
        private Transform playerTransform;
        private WaterResource playerWaterResource;
        private bool playerInRange;
        
        // Public properties
        public int CurrentWaterStored => currentWaterStored;
        public int MaxWaterStorage => maxWaterStorage;
        public bool PlayerInRange => playerInRange;
        public float WaterPercentage => (float)currentWaterStored / maxWaterStorage;

        void Start()
        {
            // Find day controller for passive restoration
            dayController = FindFirstObjectByType<DayController>();
            if (dayController == null)
            {
                Debug.LogWarning("[Well] No DayController found - passive restoration disabled");
            }
            else
            {
                // Subscribe to day change events
                dayController.dayPassedEvent.AddListener(OnDayPassed);
            }
            
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerWaterResource = player.GetComponent<WaterResource>();
            }
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            if (dayController != null)
            {
                dayController.dayPassedEvent.RemoveListener(OnDayPassed);
            }
        }

        /// <summary>
        /// Reset day progress tracking when a new day starts
        /// </summary>
        void OnDayPassed()
        {
            lastDayProgress = 0f;
            Debug.Log("[Well] New day - reset passive restoration tracking");
        }

        void Update()
        {
            UpdatePassiveRestoration();
            CheckPlayerProximity();
            CheckInteractionInput();
        }

        /// <summary>
        /// Restore water passively throughout the day.
        /// 1 water every 1/5 of a day (for 5 water per day).
        /// </summary>
        void UpdatePassiveRestoration()
        {
            if (dayController == null || currentWaterStored >= maxWaterStorage) return;
            
            float currentProgress = dayController.DayProgressPercent;
            float progressThreshold = 1f / waterRestoredPerDay;
            
            // Check if we've crossed a restoration threshold
            int lastWaterCount = Mathf.FloorToInt(lastDayProgress / progressThreshold);
            int currentWaterCount = Mathf.FloorToInt(currentProgress / progressThreshold);
            
            if (currentWaterCount > lastWaterCount)
            {
                int waterToAdd = currentWaterCount - lastWaterCount;
                currentWaterStored = Mathf.Min(currentWaterStored + waterToAdd, maxWaterStorage);
                Debug.Log($"[Well] Passively restored {waterToAdd} water. Now: {currentWaterStored}/{maxWaterStorage}");
            }
            
            lastDayProgress = currentProgress;
        }

        /// <summary>
        /// Check if player is within interaction range
        /// </summary>
        void CheckPlayerProximity()
        {
            if (playerTransform == null) return;
            
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            playerInRange = distance <= interactionRange;
        }

        /// <summary>
        /// Check for F key press when player is in range
        /// </summary>
        void CheckInteractionInput()
        {
            if (!playerInRange || playerWaterResource == null) return;
            
            // Check for F key press (using new Input System)
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                TryRefillPlayer();
            }
        }

        /// <summary>
        /// Try to refill player's water from the well
        /// </summary>
        public void TryRefillPlayer()
        {
            if (playerWaterResource == null)
            {
                Debug.LogWarning("[Well] No player water resource found");
                return;
            }

            // Check if player is already full
            if (playerWaterResource.CurrentWater >= playerWaterResource.MaxWater)
            {
                Debug.Log("[Well] Player water is already full");
                return;
            }

            // Check if well has water
            if (currentWaterStored <= 0)
            {
                Debug.Log("[Well] Well is empty!");
                return;
            }

            // Calculate how much water to transfer
            int playerNeeds = playerWaterResource.MaxWater - playerWaterResource.CurrentWater;
            int waterToGive = Mathf.Min(waterGivenPerInteraction, currentWaterStored, playerNeeds);

            // Transfer water
            currentWaterStored -= waterToGive;
            playerWaterResource.AddWater(waterToGive);

            Debug.Log($"[Well] Gave {waterToGive} water to player. Well now has {currentWaterStored}/{maxWaterStorage}");

            // Play effects
            if (refillEffect)
                refillEffect.Play();
            
            if (refillSound)
                refillSound.Play();
        }

        /// <summary>
        /// Manually add water to the well (for debugging or special events)
        /// </summary>
        public void AddWater(int amount)
        {
            currentWaterStored = Mathf.Min(currentWaterStored + amount, maxWaterStorage);
        }

        // Visualize interaction range in editor
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
