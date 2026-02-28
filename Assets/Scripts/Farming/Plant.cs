using UnityEngine;

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
        public float timeToNextStage = 5.0f; // Seconds between growth stages
        private float timer;

        void Start()
        {
            timer = timeToNextStage;
            UpdatePlantVisuals();
        }

        void Update()
        {
            // changed so the plants can now wither (will be needed as withered plants shouldn't increment the plantInventory counter)
            if (currentState == PlantState.Planted || currentState == PlantState.Growing || currentState == PlantState.Mature)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    Grow();
                    timer = timeToNextStage; // Reset timer for the next stage
                }
            }
        }

        void Grow()
        {
            if (currentState == PlantState.Planted)
            {
                ChangeState(PlantState.Growing);
            }
            else if (currentState == PlantState.Growing)
            {
                ChangeState(PlantState.Mature);
            }
            else if(currentState == PlantState.Mature)
            {
                ChangeState(PlantState.Withered);
            }
        }

        public void ChangeState(PlantState newState)
        {
            currentState = newState;
            UpdatePlantVisuals();
        }

        void UpdatePlantVisuals()
        {
            plantedModel.SetActive(currentState == PlantState.Planted);
            growingModel.SetActive(currentState == PlantState.Growing);
            matureModel.SetActive(currentState == PlantState.Mature);
            witheredModel.SetActive(currentState == PlantState.Withered);
        }
    }
}
