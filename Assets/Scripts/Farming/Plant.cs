using UnityEngine;
using Environment;

namespace Farming
{
    public enum PlantState { Planted, Growing, Mature, Withered }

    public class Plant : MonoBehaviour
    {
        public PlantState currentState;

        [Header("Visual Models")]
        public GameObject plantedModel;
        public GameObject growingModel;
        public GameObject matureModel;
        public GameObject witheredModel;

        [Header("Settings")]
        public float baseTimeToNextStage = 5.0f; // Seconds between growth stages
        private float timer;
        private bool isWatered = false; // Plant needs to be watered to start growing

        [Header("Season Manager Reference")]
        public SeasonManager seasonManager;
        void Start()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            timer = baseTimeToNextStage;
            currentState = PlantState.Planted; // Ensure we start in Planted state
            UpdatePlantVisuals();
        }

        void Update()
        {
            if (!isWatered || currentState == PlantState.Mature || currentState == PlantState.Withered)
                return;

            // Adjust growth timer based on season
            float growthMultiplier = 1f;
            if (seasonManager != null)
            {
                growthMultiplier = GetSeasonGrowthMultiplier(seasonManager.CurrentSeason);
            }

            timer -= Time.deltaTime * growthMultiplier;

            if (timer <= 0f)
            {
                Grow();
                timer = baseTimeToNextStage; // Reset timer
            }
        }
        
        /// <summary>
        /// Set the watered state of the plant. Plant will only grow when watered.
        /// </summary>
        public void SetWatered(bool watered)
        {
            bool wasWatered = isWatered;
            isWatered = watered;
            if (watered && !wasWatered)
            {
                Debug.Log($"[Plant] Plant watered - growth started!");
            }
        }

        void Grow()
        {
            if (currentState == PlantState.Planted)
            {
                Debug.Log("[Plant] Growing from Planted to Growing stage!");
                ChangeState(PlantState.Growing);
            }
            else if (currentState == PlantState.Growing)
            {
                Debug.Log("[Plant] Growing from Growing to Mature stage!");
                ChangeState(PlantState.Mature);
            }
        }

        public void ChangeState(PlantState newState)
        {
            currentState = newState;
            UpdatePlantVisuals();
        }

        void UpdatePlantVisuals()
        {
            if (plantedModel != null) plantedModel.SetActive(currentState == PlantState.Planted);
            if (growingModel != null) growingModel.SetActive(currentState == PlantState.Growing);
            if (matureModel != null) matureModel.SetActive(currentState == PlantState.Mature);
            if (witheredModel != null) witheredModel.SetActive(currentState == PlantState.Withered);
        }

        float GetSeasonGrowthMultiplier(SeasonManager.Season season)
        {
            switch (season)
            {
                case SeasonManager.Season.Spring:
                    return 1.2f;
                case SeasonManager.Season.Summer:
                    return 1.5f;
                case SeasonManager.Season.Fall:
                    return 0.8f;
                case SeasonManager.Season.Winter:
                    return 0.5f;
                default:
                    return 1f;
            }
        }
    }
}
